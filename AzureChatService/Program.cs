using AzureChatService.Agents;
using Microsoft.Agents.Authentication.Msal;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.AddAgentApplicationOptions();
builder.Services.AddSingleton<IStorage, MemoryStorage>();
builder.AddAgent<EchoAgent>();
builder.Services.AddDefaultMsalAuth(builder.Configuration);

var app = builder.Build();

// Request diagnostics
app.Use(async (context, next) =>
{
    var logger = context.RequestServices
        .GetRequiredService<ILogger<Program>>();

    logger.LogInformation(
        "Incoming: {Method} {Path}",
        context.Request.Method,
        context.Request.Path);

    await next();

    logger.LogInformation(
        "Completed: {Method} {Path} Status: {StatusCode}",
        context.Request.Method,
        context.Request.Path,
        context.Response.StatusCode);
});

// Authentication middleware (only when authentication is registered)
app.UseAuthentication();
app.UseAuthorization();

// Simple health check
app.MapGet("/", () => Results.Ok("Bot is running"));

// Azure Bot messaging endpoint
app.MapPost("/api/messages", async (
    HttpRequest request,
    HttpResponse response,
    IAgentHttpAdapter adapter,
    IAgent agent,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    logger.LogInformation("BOT MESSAGE HANDLER INVOKED");

    await adapter.ProcessAsync(
        request,
        response,
        agent,
        cancellationToken);
});

app.Run();