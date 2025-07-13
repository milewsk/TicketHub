using Common;

namespace Domain.Users;

public class User : AuditableEntity
{
    public string Email { get; init; }

    public string PasswordHash { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public User()
    {
    }

    public User(Guid id, string email, string passwordHash, string firstName,
        string lastName) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
    }
}