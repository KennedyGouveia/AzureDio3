using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBufferSize = 1024 * 1024 * 100; // 100 MB
});

builder.Services.AddSingleton(s =>
{
    string connectionstring = Environment.GetEnvironmentVariable("cosmosDBConnectionString") ?? throw new InvalidOperationException("Cosmos DB connection string is not set.");
    return new CosmosClient(connectionstring);
});

builder.Build().Run();
