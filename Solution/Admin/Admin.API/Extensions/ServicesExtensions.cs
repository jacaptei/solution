using JaCaptei.Application.DAL;

using Polly;
using Polly.Contrib.WaitAndRetry;

using System.Reflection;

namespace JaCaptei.Admin.API;

internal static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, Model.AppSettingsRecord settings)
    {
        services.AddHttpClient("crm", client =>
        {
            client.BaseAddress = new Uri(settings.crmEndpoint);
        })
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3)));

        services.AddHttpClient("location", client =>
        {
            client.BaseAddress = new Uri("https://brasilaberto.com/api/v1/");
        })
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));

        services.AddHttpClient("imoview", client =>
        {
            client.BaseAddress = new Uri("https://api.imoview.com.br/");
        })
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));

        services.AddHttpClient("vistasoft", client =>
        {
            client.BaseAddress = new Uri("http://sandbox-rest.vistahost.com.br/");
        })
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));


        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes()
                            .Where(t => t.Namespace != null && t.Namespace.StartsWith("JaCaptei.Application.Mapper"))
                            .ToArray();

        services.AddAutoMapper(types);
        services.AddScoped<DBcontext>();
        return services;
    }
}