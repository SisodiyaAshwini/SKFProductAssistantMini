using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductAssistant.Services;
using ProductAssistant.Data.RetrievelEngine.KeywordBasedRetriever;
using StackExchange.Redis;
using ProductAssistant.Services.Service;
using ProductAssistant.Services.Interface;

public static class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices(services =>
            {
                services
                    .AddApplicationInsightsTelemetryWorkerService()
                    .ConfigureFunctionsApplicationInsights()
                    .AddScoped<IRAGService, RAGService>()
                    .AddSingleton<IOpenAIService, OpenAIService>()
                    .AddScoped<IProductKeywordRetriever, ProductKeywordRetriever>()
                    .AddSingleton<IConnectionMultiplexer>(sp =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();
                        string? connectionString = config["RedisCache:ConnectionString"] ?? string.Empty;
                        return ConnectionMultiplexer.Connect(connectionString);
                    })
                    .AddSingleton<IRedisCacheService, RedisCacheService>();
            })
            .Build();

        host.Run();
    }
}