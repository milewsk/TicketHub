using Common;

using MediatR;

using Microsoft.Extensions.Logging;

using Serilog.Context;

namespace Application.Abstractions.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Handling request {requestName}", requestName);

        // Handle event
        TResponse response = await next(cancellationToken);

        // Log information about event
        if (response.IsSuccess)
        {
            logger.LogInformation("Successfully handled request {requestName}", requestName);
        }
        else
        {
            using (LogContext.PushProperty("Error", response.Error, true))
            {
                logger.LogInformation("Completed request {requestName} with error", requestName);
            }
        }
        
        return response;
    }
}