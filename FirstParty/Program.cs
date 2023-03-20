using System.Net;

var builder = Host.CreateDefaultBuilder(args);

builder.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering(11111, 30000, new IPEndPoint(IPAddress.Loopback, 11111));
    siloBuilder.ConfigureLogging(logging => logging.AddConsole());
});
await builder.RunConsoleAsync();