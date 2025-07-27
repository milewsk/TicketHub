using Infrastructure.Configurations;
using Infrastructure.Constants;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using NpgsqlTypes;

using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace Infrastructure.Extensions;

public static class SerilogExtension
{
    public static void RegisterSerilog(this WebApplicationBuilder builder)
    {
        Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .MinimumLevel.Override("MudBlazor", LogEventLevel.Information)
                .MinimumLevel.Override("Serilog", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.AddOrUpdate",
                    LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.WithUtcTime()
                .Enrich.WithUserInfo()
                .WriteTo.Async(
                    wt => wt.File("./log/log-.txt", rollingInterval: RollingInterval.Day))
                .WriteTo.Async(wt =>
                    wt.Console(
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3} {ClientIp}] {Message:lj}{NewLine}{Exception}"))
                .ApplyConfigPreferences(context.Configuration)
        );
    }

    private static void ApplyConfigPreferences(
        this LoggerConfiguration serilogConfig,
        IConfiguration configuration)
    {
        WriteToDatabase(serilogConfig, configuration);
    }

    private static void WriteToDatabase(
        LoggerConfiguration serilogConfig,
        IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase")) return;

        var dbProvider =
            configuration.GetValue<string>(
                $"{nameof(DatabaseSettings)}:{nameof(DatabaseSettings.DbProvider)}");
        var connectionString =
            configuration.GetValue<string>(
                $"{nameof(DatabaseSettings)}:{nameof(DatabaseSettings.ConnectionString)}");

        switch (dbProvider)
        {
            case DbProviderKeys.SqlServer:
                //WriteToSqlServer(serilogConfig, connectionString);
                break;
            case DbProviderKeys.PostgreSql:
                WriteToNpgsql(serilogConfig, connectionString);
                break;
            case DbProviderKeys.MySql:
                // WriteToSqLite(serilogConfig, "\\BlazorDashboardDb.db");
                break;
        }
    }

    private static void WriteToNpgsql(LoggerConfiguration serilogConfig, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        const string logTableName = "system_logs";

        IDictionary<string, ColumnWriterBase> columnOptions =
            new Dictionary<string, ColumnWriterBase>
            {
                { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                { "time_stamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
                { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                { "properties", new PropertiesColumnWriter(NpgsqlDbType.Varchar) },
                { "log_event", new LogEventSerializedColumnWriter(NpgsqlDbType.Varchar) },
                {
                    "user_name", new SinglePropertyColumnWriter("UserName", PropertyWriteMethod.Raw,
                        NpgsqlDbType.Varchar)
                },
                {
                    "client_ip", new SinglePropertyColumnWriter("ClientIP", PropertyWriteMethod.Raw,
                        NpgsqlDbType.Varchar)
                },
                {
                    "client_agent", new SinglePropertyColumnWriter(
                        "ClientAgent",
                        PropertyWriteMethod.ToString,
                        NpgsqlDbType.Varchar)
                }
            };

        serilogConfig.WriteTo.Async(wt => wt.PostgreSQL(
            connectionString,
            logTableName,
            columnOptions,
            LogEventLevel.Information,
            needAutoCreateTable: false,
            schemaName: "public",
            useCopy: false
        ));
    }

    public static LoggerConfiguration WithUtcTime(
        this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With<UtcTimestampEnricher>();
    }

    public static LoggerConfiguration WithUserInfo(
        this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With<UserInfoEnricher>();
    }
}

internal class UtcTimestampEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
    {
        logEvent.AddOrUpdateProperty(pf.CreateProperty("TimeStamp",
            logEvent.Timestamp.UtcDateTime));
    }
}

internal class UserInfoEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserInfoEnricher()
        : this(new HttpContextAccessor())
    {
    }

    public UserInfoEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "";
        var headers = _httpContextAccessor.HttpContext?.Request?.Headers;
        var clientIp = headers != null && headers.ContainsKey("X-Forwarded-For")
            ? headers["X-Forwarded-For"].ToString().Split(',').First().Trim()
            : _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";
        var clientAgent = headers != null && headers.ContainsKey("User-Agent")
            ? headers["User-Agent"].ToString()
            : "";

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", userName));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientIP", clientIp));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientAgent", clientAgent));
    }
}