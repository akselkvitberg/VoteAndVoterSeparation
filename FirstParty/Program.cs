using System.Net;
using Serilog;

var builder = Host.CreateDefaultBuilder(args);
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

builder.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering(11111, 30000, new IPEndPoint(IPAddress.Loopback, 11111));
    siloBuilder.ConfigureLogging(l => l.ClearProviders());
    //siloBuilder.ConfigureLogging(logging => logging.AddConsole());
});
await builder.RunConsoleAsync();