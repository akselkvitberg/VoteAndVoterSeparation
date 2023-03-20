using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

var builder = new HostBuilder()
    .UseOrleansClient(client =>
    {
        client.UseLocalhostClustering();
    })
    .ConfigureLogging(logging => logging.AddConsole());

var host = builder.Build();
await host.StartAsync();

Console.WriteLine("Client successfully connected to silo host \n");

var clusterClient = host.Services.GetService<IClusterClient>();
var firstPartyGrain = clusterClient.GetGrain<IFirstPartyGrain>("FirstParty");
var thirdPartyGrain = clusterClient.GetGrain<IThirdPartyGrain>("ThirdParty");

var firstPartyHello = await firstPartyGrain.SayHello(); 
Console.WriteLine(firstPartyHello);
var thirdPartyHello = await thirdPartyGrain.SayHello();
Console.WriteLine(thirdPartyHello);

await host.StopAsync();