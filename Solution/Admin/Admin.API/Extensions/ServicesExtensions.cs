﻿using JaCaptei.Admin.API.Consumers;
using JaCaptei.Application.DAL;

using MassTransit;

using Polly;
using Polly.Contrib.WaitAndRetry;

using System.Reflection;

namespace JaCaptei.Admin.API;

internal static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, Model.AppSettingsRecord settings)
    {
        services.AddMassTransit(busConfig =>
        {
            busConfig.AddConsumer<IntegracaoClienteConsumer>(c =>
            {
                c.ConcurrentMessageLimit = 1;
            });
            //busConfig.UsingRabbitMq((context, cfg) =>
            //{
            //    //cfg.Host("rabbitmq", 5672, "/", h =>
            //    //{
            //    //    h.Username("guest");
            //    //    h.Password("guest");
            //    //});
            //    cfg.Host(new Uri("amqp://localhost:5672"), h =>
            //    {
            //        h.Username("guest");
            //        h.Password("guest");
            //    });
            //    cfg.ConfigureEndpoints(context);
            //});
            services.AddMassTransit(x =>
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    var azureMQConn = settings.AzureMQ;
                    cfg.Host(azureMQConn);
                    x.AddConsumer<IntegracaoClienteConsumer>();
                    cfg.ReceiveEndpoint("integracaocliente", e =>
                    {
                        e.ConfigureConsumer<IntegracaoClienteConsumer>(context);
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });


        });
        services.AddMassTransitHostedService();

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

        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes()
                            .Where(t => t.Namespace != null && t.Namespace.StartsWith("JaCaptei.Application.Mapper"))
                            .ToArray();

        services.AddAutoMapper(types);
        services.AddScoped<DBcontext>();
        return services;
    }
}