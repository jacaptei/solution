using AutoMapper;

using JaCaptei.Application.DAL;
using JaCaptei.Application.Integracao;
using JaCaptei.Model;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Polly;
using Polly.Contrib.WaitAndRetry;

using RepoDb;

using ReprocessarImovelVistaSoftFunction;

using System.Reflection;

var builder = new HostBuilder()
.ConfigureFunctionsWebApplication()
.ConfigureServices((ctx, services) =>
{
    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();

    var assembly = Assembly.GetExecutingAssembly();
    var types = assembly.GetTypes()
                        .Where(t => t.Namespace != null && t.Namespace.StartsWith("JaCaptei.Application.Mapper"))
                        .ToArray();

    services.AddAutoMapper(types);
    services.AddScoped<DBcontext>();

    AppSettingsRecord settings = new();
    string EnvironmentSettings = ctx.Configuration.GetSection("Environment").Value;

    services.AddHttpClient("imoview", client =>
    {
        client.BaseAddress = new Uri("");
    })
    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));
    new ConfigureFromConfigurationOptions<AppSettingsRecord>(ctx.Configuration.GetSection(EnvironmentSettings)).Configure(settings);
    settings.CopyToStaticSettings();

    GlobalConfiguration.Setup().UsePostgreSql();
    services.AddScoped<VistaSoftService>(provider =>
    {
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        var context = provider.GetRequiredService<DBcontext>();
        var mapper = provider.GetRequiredService<IMapper>();
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<ReprocessarVistaSoftFn>();
        var vsDAO = new VistaSoftDAO(context.GetConn());
        return new VistaSoftService(httpClientFactory, context, mapper, logger, vsDAO);
    });
});

var host = builder.Build();
host.Run();
