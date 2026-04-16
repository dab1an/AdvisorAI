using Azure.Identity;
using Microsoft.SemanticKernel;
using Pinecone;
using server.Services;
using Microsoft.AspNetCore.HttpOverrides;

try
{
    var builder = WebApplication.CreateBuilder(args);

    string secretSource = builder.Configuration["Secrets:Source"] ?? "AppSettings";
    Console.WriteLine($"Startup configuration: Secrets source = {secretSource}");

    if (secretSource.Equals("KeyVault", StringComparison.OrdinalIgnoreCase))
    {
        string? vaultUri = builder.Configuration["Secrets:KeyVaultUri"];

        if (string.IsNullOrWhiteSpace(vaultUri))
        {
            throw new InvalidOperationException(
                "Secrets:KeyVaultUri must be configured when Secrets:Source is set to 'KeyVault'.");
        }

        DefaultAzureCredentialOptions credentialOptions = new DefaultAzureCredentialOptions();
        string? managedIdentityClientId = builder.Configuration["Secrets:ManagedIdentityClientId"];

        if (!string.IsNullOrWhiteSpace(managedIdentityClientId))
        {
            credentialOptions.ManagedIdentityClientId = managedIdentityClientId;
        }

        builder.Configuration.AddAzureKeyVault(
            new Uri(vaultUri),
            new DefaultAzureCredential(credentialOptions));
    }

    // normalize secret aliases so existing services can continue using underscore keys.
    Dictionary<string, string?> normalizedSecrets = new()
    {
        ["OPENAI_API_KEY"] = GetFirstDefined(builder.Configuration, "OPENAI_API_KEY", "OPENAI-API-KEY"),
        ["PINECONE_API_KEY"] = GetFirstDefined(builder.Configuration, "PINECONE_API_KEY", "PINECONE-API-KEY"),
        ["REDIS_CONNECTION"] = GetFirstDefined(builder.Configuration, "REDIS_CONNECTION", "REDIS-CONNECTION")
    };

    builder.Configuration.AddInMemoryCollection(normalizedSecrets);

    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddOpenAITextEmbeddingGeneration(
        modelId: "text-embedding-3-small",
        builder.Configuration["OPENAI_API_KEY"]
            ?? throw new InvalidOperationException("OPENAI_API_KEY is not configured."));

    builder.Services.AddSingleton<PineconeClient>(sp =>
        new PineconeClient(
            builder.Configuration["PINECONE_API_KEY"]
                ?? throw new InvalidOperationException("PINECONE_API_KEY is not configured.")));

    builder.Services.AddScoped<IAuditParserService, AuditParserService>();
    builder.Services.AddScoped<IFlowchartParserService, FlowchartParserService>();
    builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
    builder.Services.AddScoped<IChatService, ChatService>();
    builder.Services.AddSingleton<IChatHistoryStore, RedisChatHistoryStore>();
    builder.Services.AddHealthChecks();

    string[] configuredOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
        ?? [];

    string[] envOrigins = (builder.Configuration["CORS_ALLOWED_ORIGINS"] ?? string.Empty)
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    string[] allowedOrigins = configuredOrigins
        .Concat(envOrigins)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    if (allowedOrigins.Length == 0)
    {
        allowedOrigins = ["http://localhost:5173"];
    }

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    builder.Services.AddControllers();

    var app = builder.Build();

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();
    app.UseHttpsRedirection();
    app.MapControllers();
    app.MapHealthChecks("/health");
    app.Run();
}
catch (Exception ex)
{
    Console.Error.WriteLine("Fatal startup error:");
    Console.Error.WriteLine(ex.ToString());
    throw;
}

static string? GetFirstDefined(IConfiguration configuration, params string[] keys)
{
    foreach (string key in keys)
    {
        string? value = configuration[key];

        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
    }

    return null;
}