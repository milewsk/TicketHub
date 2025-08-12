using Presentation.Middlewares;

namespace Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }

    public static WebApplication ConfigureApplication(this WebApplication app,
        IConfiguration configuration)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        // Add request middlewares
        AddMiddlewares(app);

        app.MapControllers();

        return app;
    }


    private static WebApplication AddMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        return app;
    }
}