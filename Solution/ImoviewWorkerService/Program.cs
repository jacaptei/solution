using ImoviewWorker;

using JaCaptei.Application.DAL;
using JaCaptei.Model;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using RepoDb;

using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);
AppSettingsRecord settings = new ();
string EnvironmentSettings = builder.Configuration.GetSection("Environment").Value ?? "";
new ConfigureFromConfigurationOptions<AppSettingsRecord>(builder.Configuration.GetSection(EnvironmentSettings)).Configure(settings);      //configuration.GetSection(EnvironmentSettings).Bind(settings);
settings.CopyToStaticSettings();
builder.Services.AddHostedService<ImoviewWorkerService>();
builder.Services.AddHttpClient("imoview", client =>
{
    client.BaseAddress = new Uri("https://api.imoview.com.br/");
});

builder.Services.AddScoped<DBcontext>();
var assembly = Assembly.GetExecutingAssembly();
var types = assembly.GetTypes()
                    .Where(t => t.Namespace != null && t.Namespace.StartsWith("JaCaptei.Application.Mapper"))
                    .ToArray();

builder.Services.AddAutoMapper(types);
GlobalConfiguration.Setup().UsePostgreSql();
var host = builder.Build();
host.Run();
