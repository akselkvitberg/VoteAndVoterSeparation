using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Shared;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var builder = new HostBuilder()
    .UseOrleansClient(client =>
    {
        client.UseLocalhostClustering();
    });

var host = builder.Build();
await host.StartAsync();

Console.WriteLine("Client successfully connected to silo host \n");

var clusterClient = host.Services.GetRequiredService<IClusterClient>();
var electionId = Guid.NewGuid();
var firstPartyGrain = clusterClient.GetGrain<IFirstPartyGrain>(electionId);
var thirdPartyGrain = clusterClient.GetGrain<IThirdPartyGrain>(electionId);

await RegisterVote("Voter 1", "Party 1");
await RegisterVote("Voter 2", "Party 1");
await RegisterVote("Voter 3", "Party 1");
await RegisterVote("Voter 4", "Party 2");
await RegisterVote("Voter 5", "Party 2");
await RegisterVote("Voter 6", "Party 2");

// Voter 1 changes their vote
await RegisterVote("Voter 1", "Party 2");

await GetVotes();

await host.StopAsync();

async Task RegisterVote(string voterId, string message)
{
    var firstPartyKey = await firstPartyGrain.GetPublicKey();
    var thirdPartyKey = await thirdPartyGrain.GetOneTimeKey();
    
    Log.Information("Registering vote for {Voter} for {Party}", voterId, message);
    
    var encryptedData = 
        Encoding.UTF8.GetBytes(message)
        .EncryptWithRsaPublicKey(firstPartyKey)
        .ApplyXorCipher(thirdPartyKey.Key);
    
    await firstPartyGrain.RegisterVote(new RegisterVote(voterId, thirdPartyKey.Id, encryptedData));
}

async Task GetVotes()
{
    Log.Information("Tallying results:");
    var data = await firstPartyGrain.GetVotes();
    foreach (var message in data.Votes)
    {
        Log.Information($"{message.Key}: {message.Value}");
    }
}
