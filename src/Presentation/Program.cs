using Application;
using Infrastructure;

namespace Presentation;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddInfrastructure(builder.Configuration)
            .AddPresentationLayer(builder.Configuration);

        var app = builder.Build();

        // Configure web application
        app.ConfigureServer(builder.Configuration);

        // Run web application
        app.Run();
    }
}