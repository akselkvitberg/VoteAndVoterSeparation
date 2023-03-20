using System.Text;
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

var clusterClient = host.Services.GetRequiredService<IClusterClient>();
var firstPartyGrain = clusterClient.GetGrain<IFirstPartyGrain>("FirstParty");
var thirdPartyGrain = clusterClient.GetGrain<IThirdPartyGrain>("ThirdParty");

await RegisterVote("Party 1");
await RegisterVote("Party 1");
await RegisterVote("Party 1");
await RegisterVote("Party 2");
await RegisterVote("Party 2");
await RegisterVote("Party 2");
await GetVotes();

async Task RegisterVote(string message)
{
    var publicKey = await firstPartyGrain.GetPublicKey();
    var oneTimeKey = await thirdPartyGrain.GetOneTimeKey();
    
    var encryptedData = 
        Encoding.UTF8.GetBytes(message)
        .EncryptRsa(publicKey)
        .PadToLength(oneTimeKey.Key.Length)
        .ApplyXorCipher(oneTimeKey.Key);
    
    await firstPartyGrain.SendVote(new Vote(oneTimeKey.Id, encryptedData));
}

async Task GetVotes()
{
    var data = await firstPartyGrain.GetVotes();
    foreach (var message in data.Votes)
    {
        Console.WriteLine($"{message.Key}: {message.Value}");
    }
}
