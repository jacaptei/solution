using ImoviewWorker;

using JaCaptei.Model;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);
AppSettingsRecord settings = new ();
string EnvironmentSettings = builder.Configuration.GetSection("Environment").Value ?? "";
new ConfigureFromConfigurationOptions<AppSettingsRecord>(builder.Configuration.GetSection(EnvironmentSettings)).Configure(settings);      //configuration.GetSection(EnvironmentSettings).Bind(settings);
settings.CopyToStaticSettings();
builder.Services.AddHostedService<ImoviewWorkerService>();

var host = builder.Build();
host.Run();
