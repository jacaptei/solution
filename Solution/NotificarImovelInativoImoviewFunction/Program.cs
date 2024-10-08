using JaCaptei.Application.DAL;
using JaCaptei.Application.Email;
using JaCaptei.Application.Integracao;
using JaCaptei.Model;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NotificarImovelInativoImoviewFunction;

using RepoDb;


var builder = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((ctx, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        //var assembly = Assembly.GetExecutingAssembly();
        //var types = assembly.GetTypes()
        //                    .Where(t => t.Namespace != null && t.Namespace.StartsWith("JaCaptei.Application.Mapper"))
        //                    .ToArray();

        //services.AddAutoMapper(types);
        services.AddScoped<DBcontext>();
        services.AddScoped<EmailService>();
        AppSettingsRecord settings = new();
        string EnvironmentSettings = ctx.Configuration.GetSection("Environment").Value;
        //var imoviewUrl = ctx.Configuration.GetSection($"{EnvironmentSettings}:imoviewUrl").Value;

        //services.AddHttpClient("imoview", client =>
        //{
        //    client.BaseAddress = new Uri(imoviewUrl);
        //})
        //.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));
        new ConfigureFromConfigurationOptions<AppSettingsRecord>(ctx.Configuration.GetSection(EnvironmentSettings)).Configure(settings);
        settings.CopyToStaticSettings();

        GlobalConfiguration.Setup().UsePostgreSql();
        services.AddScoped<ImoviewService>(provider =>
        {
            var context = provider.GetRequiredService<DBcontext>();
            var logger = provider.GetRequiredService<ILogger<NotificarImovelIntativoImoviewFn>>();
            var es = provider.GetRequiredService<EmailService>();
            var imoviewDAO = new ImoviewDAO(context.GetConn());
            return new ImoviewService(context, logger, es);
        });
    });

var host = builder.Build();
host.Run();
