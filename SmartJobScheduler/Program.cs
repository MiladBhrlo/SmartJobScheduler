using Microsoft.Extensions.Hosting;
using SmartJobScheduler.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSmartJobScheduler(builder.Configuration);

var host = builder.Build();

await host.RunAsync();