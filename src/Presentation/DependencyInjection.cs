using MudBlazor;
using MudBlazor.Services;
using Presentation.Components;

namespace Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();

        services.AddMudBlazorServices(configuration);

        return services;
    }

    public static WebApplication ConfigureServer(
        this WebApplication app,
        IConfiguration configuration)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        return app;
    }

    private static IServiceCollection AddMudBlazorServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMudServices(config =>
        {
            MudGlobal.InputDefaults.ShrinkLabel = true;
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 3000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });

        services.AddMudPopoverService();
        services.AddMudBlazorSnackbar();
        services.AddMudBlazorDialog();

        return services;
    }
}