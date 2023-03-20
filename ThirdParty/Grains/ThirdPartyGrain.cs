using System.Security.Cryptography;
using Shared;

namespace ThirdParty.Grains;

public class ThirdPartyGrain : Grain, IThirdPartyGrain
{
    private Dictionary<Guid, byte[]> Keys { get; } = new();
    
    /// <inheritdoc />
    public Task<OneTimeKeyResult> GetOneTimeKey()
    {
        var newGuid = Guid.NewGuid();
        var bytes = RandomNumberGenerator.GetBytes(512);
        
        Keys[newGuid] = bytes;
        return Task.FromResult(new OneTimeKeyResult(newGuid, bytes));
    }

    /// <inheritdoc />
    public Task<UnmaskedVotes> UnmaskVotes(Vote[] encryptedVotes)
    {
        // Shuffle votes so receiver cannot know the order of the votes
        var randomOrderedVotes = encryptedVotes.OrderBy(_ => Guid.NewGuid());
        var votes = randomOrderedVotes.Select(DecryptVote).ToArray();
        return Task.FromResult(new UnmaskedVotes(votes));
    }

    private byte[] DecryptVote(Vote vote)
    {
        return vote.EncryptedData.ApplyXorCipher(Keys[vote.ThirdPartyKeyId]);
    }
}