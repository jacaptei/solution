using ImoviewWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ImoviewWorkerService>();

var host = builder.Build();
host.Run();
