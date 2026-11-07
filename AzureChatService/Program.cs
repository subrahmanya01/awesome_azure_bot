using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Storage;

using AzureChatService.Agents;

var builder = WebApplication.CreateBuilder(args);

// Register ASP.NET Core services
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Register Microsoft Agents SDK
builder.AddAgentApplicationOptions();
builder.AddAgent<EchoAgent>();

// Register storage
builder.Services.AddSingleton<IStorage, MemoryStorage>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine(
        $"Incoming request: {context.Request.Method} " +
        $"{context.Request.Scheme}://" +
        $"{context.Request.Host}" +
        $"{context.Request.Path}"
    );

    await next();
});

app.UseHttpsRedirection();

app.MapControllers();

// Azure Bot messaging endpoint
app.MapPost(
    "/api/messages",
    async (
        HttpRequest request,
        HttpResponse response,
        IAgentHttpAdapter adapter,
        IAgent agent,
        ILogger<Program> logger,
        CancellationToken cancellationToken) =>
    {

        try
        {
            logger.LogInformation("Processing incoming request to /api/messages");
            await adapter.ProcessAsync(
                request,
                response,
                agent,
                cancellationToken
            );
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error processing incoming request to /api/messages");
            response.StatusCode = 500;
            await response.WriteAsync("Internal Server Error", cancellationToken);
        }
        
    }
);

app.Run();