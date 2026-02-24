
using Grafana.OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var otelUri = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!;

Console.WriteLine($"OTEL_EXPORTER_OTLP_ENDPOINT: {otelUri}");

builder.Services.AddOpenTelemetry()
    .WithLogging(logging =>
    {
        logging.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri(otelUri);
            opt.Protocol = OtlpExportProtocol.HttpProtobuf;
        });
    })
    .UseGrafana();

builder.Logging.AddOpenTelemetry(logging => logging.UseGrafana());


var app = builder.Build();


app.MapPost("/loki-log", (LogRequest request, ILogger<Program> logger) =>
{
    switch (request.Level)
    {
        case 0:
            logger.LogTrace(request.Msg);
            break;
        case 1:
            logger.LogDebug(request.Msg);
            break;
        case 2:
            logger.LogInformation(request.Msg);
            break;
        case 3:
            logger.LogWarning(request.Msg);
            break;
        case 4:
            logger.LogError(request.Msg);
            break;
        case 5:
            logger.LogCritical(request.Msg);
            break;
        default:
            logger.LogInformation(request.Msg);
            break;
    }

    return Results.Ok(new { Sent = request.Msg, Level = request.Level });
});

app.Run();

public record LogRequest(int Level, string Msg);


