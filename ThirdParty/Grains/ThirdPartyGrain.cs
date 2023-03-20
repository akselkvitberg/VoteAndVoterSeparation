using System.Security.Cryptography;
using Serilog;
using Shared;

namespace ThirdParty.Grains;

public class ThirdPartyGrain : Grain, IThirdPartyGrain
{
    private Dictionary<Guid, byte[]> Keys { get; } = new();
    
    /// <inheritdoc />
    public Task<OneTimeKeyResult> GetOneTimeKey()
    {
        var keyId = Guid.NewGuid();
        Log.Information("Received request for one-time-key - issued key {KeyId}", keyId);
        var bytes = RandomNumberGenerator.GetBytes(512);
        
        Keys[keyId] = bytes;
        return Task.FromResult(new OneTimeKeyResult(keyId, bytes));
    }

    /// <inheritdoc />
    public Task<UnmaskedVotes> UnmaskVotes(ICollection<EncryptedVoteData> encryptedVotes)
    {
        Log.Information("Received request to unmask {Count} votes", encryptedVotes.Count);
        
        // Shuffle votes so receiver cannot know the order of the votes
        var unmaskedVotes = 
            encryptedVotes
            .Shuffle()
            .Select(UnmaskVote)
            .ToArray();
        return Task.FromResult(new UnmaskedVotes(unmaskedVotes));
    }

    private byte[] UnmaskVote(EncryptedVoteData registerVote)
    {
        var key = Keys[registerVote.ThirdPartyKeyId];
        var unmaskedData = registerVote.EncryptedData.ApplyXorCipher(key);
        Log.Information("-- Unmasking vote with {KeyId} - Previewing data: {UnmaskedData}", registerVote.ThirdPartyKeyId, Convert.ToBase64String(unmaskedData).Substring(0, 10));
        return unmaskedData;
    }
}