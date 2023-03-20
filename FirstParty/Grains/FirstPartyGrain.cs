using System.Security.Cryptography;
using System.Text;
using Serilog;
using Shared;

namespace FirstParty.Grains;

public class FirstPartyGrain : Grain, IFirstPartyGrain
{
    private readonly RSA _rsa;
    private readonly byte[] _xorKey;
    private readonly Dictionary<string, EncryptedVoteData> _votes = new();

    public FirstPartyGrain()
    {
        _rsa = RSA.Create();
        _xorKey = RandomNumberGenerator.GetBytes(512);
    }

    /// <inheritdoc />
    public Task<byte[]> GetPublicKey()
    {
        Log.Information("Received request for Public key");
        return Task.FromResult(_rsa.ExportRSAPublicKey());
    }

    /// <inheritdoc />
    public Task RegisterVote(RegisterVote registerVote)
    {
        Log.Information("Received vote registration with third party key {KeyId} - Previewing data: {Data}", registerVote.ThirdPartyKeyId, Convert.ToBase64String(registerVote.EncryptedData).Substring(0,10));
        
        // vote is encrypted first with public key, then masked with third party xor key, then masked again with first party xor key
        var maskedVote = new EncryptedVoteData(
            registerVote.ThirdPartyKeyId,
            registerVote.EncryptedData.ApplyXorCipher(_xorKey));
        Log.Information("Masking vote data with first party xor key - Previewing data: {Data}", Convert.ToBase64String(maskedVote.EncryptedData).Substring(0, 10));
        
        // Allow the voter to change their vote - old vote is simply discarded
        _votes[registerVote.VoterId] = maskedVote;
        
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<Result> GetVotes()
    {
        Log.Information("Received request to tally votes");
        var thirdPartyGrain = GrainFactory.GetGrain<IThirdPartyGrain>(this.GetPrimaryKey());

        // Shuffle the votes so no correlation can be made from time the third party key was created
        var unmaskedVotes = await thirdPartyGrain.UnmaskVotes(_votes.Values.Shuffle());

        var decryptedVotes = 
            unmaskedVotes.Votes
            .Select(vote => vote.ApplyXorCipher(_xorKey))
            .Select(vote => vote.DecryptWithRsa(_rsa))
            .Select(b => Encoding.UTF8.GetString(b))
            .GroupBy(x=>x)
            .ToDictionary(x=> x.Key, x=>x.Count());

        return new Result(decryptedVotes);
    }
}

