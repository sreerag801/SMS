using Microsoft.AspNetCore.Mvc;

namespace SMS.API.API;

public static class SerilogTest
{
    public static void MapErrorLogEndpoints(this WebApplication app)
    {
        var v1 = app.MapGroup("/api/v1/test").WithTags(nameof(SerilogTest).ToLowerInvariant()).WithOpenApi();

        v1.MapGet("/generate-error-logs",
            async ([FromServices] ILogger<Program> logger, CancellationToken ct) =>
        {
            logger.LogInformation("log information");
            logger.LogWarning("log warning");
            logger.LogError("log error");

            await Task.CompletedTask;
        })
        .WithName("GenerateErrorLogs");
    }
}
