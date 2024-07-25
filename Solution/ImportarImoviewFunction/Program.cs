using JaCaptei.Application.DAL;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Polly;
using Polly.Contrib.WaitAndRetry;

using System.Reflection;


var builder = new HostBuilder()
.ConfigureFunctionsWebApplication()
.ConfigureServices(services =>
{
services.AddApplicationInsightsTelemetryWorkerService();
services.ConfigureFunctionsApplicationInsights();
services.AddHttpClient("imoview", client =>
{
    client.BaseAddress = new Uri("https://mockimoviewincluirfunction20240725002953.azurewebsites.net/api/");
})
.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));
var assembly = Assembly.GetExecutingAssembly();
var types = assembly.GetTypes()
                    .Where(t => t.Namespace != null && t.Namespace.StartsWith("JaCaptei.Application.Mapper"))
                    .ToArray();

services.AddAutoMapper(types);
services.AddScoped<DBcontext>();
});

var host = builder.Build();
host.Run();
