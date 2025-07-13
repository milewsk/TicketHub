using System.Diagnostics.CodeAnalysis;

namespace Common;

public class Result
{
    // Properties
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    // Constructors
    public Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error != Error.None))
        {
            throw new ArgumentException("Error is not valid", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    // Methods
    public static Result Success() => new Result(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new Result<TValue>(value, true, Error.None);

    public static Result<TValue> Failure<TValue>(Error error) =>
        new Result<TValue>(default, false, error);
}

// Generic Result
public class Result<TValue> : Result
{
    // Properties
    private readonly TValue? _value;

    // Constructors
    public Result(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    // Methods
    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot get value from failure result");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null
            ? Success(value)
            : Failure<TValue>(Error.NullValue);

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}