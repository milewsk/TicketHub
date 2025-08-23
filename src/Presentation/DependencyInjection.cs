using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

using Presentation.Middlewares;

namespace Presentation;

public static class DependencyInjection
{
    private static readonly string[] ConfigureOptions = ["en-US", "pl-PL", "de-DE"];

    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddLocalization()
            .AddControllers();

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

        app.AddLocalization();

        app.MapControllers();

        return app;
    }

    private static WebApplication AddMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        return app;
    }


    private static IServiceCollection AddLocalization(
        this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = ConfigureOptions;

            options.SetDefaultCulture("en-US")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            options.RequestCultureProviders.Insert(
                0,
                new AcceptLanguageHeaderRequestCultureProvider());
        });

        return services;
    }


    private static WebApplication AddLocalization(this WebApplication app)
    {
        var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
        if (localizationOptions?.Value != null)
        {
            app.UseRequestLocalization(localizationOptions.Value);
        }

        return app;
    }
}