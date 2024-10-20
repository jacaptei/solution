using AutoMapper;

using JaCaptei.Application.DAL;
using JaCaptei.Application.Integracao;
using JaCaptei.Model;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RepoDb;

using ReprocessarIntegracaoImoviewFunction;

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
        });
        new ConfigureFromConfigurationOptions<AppSettingsRecord>(ctx.Configuration.GetSection(EnvironmentSettings)).Configure(settings);
        settings.CopyToStaticSettings();
        var s = ctx.Configuration.GetSection("QueueDelay").Value;
        if (!int.TryParse(s, out int queueDelay))
            queueDelay = 1000;
        GlobalConfiguration.Setup().UsePostgreSql();
        services.AddScoped<ImoviewService>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var context = provider.GetRequiredService<DBcontext>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<ReprocessarIntegracaoImoviewFn>>();
            var imoviewDAO = new ImoviewDAO(context.GetConn());
            return new ImoviewService(httpClientFactory, context, mapper, logger, imoviewDAO, -1, queueDelay);
        });
    });

var host = builder.Build();
host.Run();