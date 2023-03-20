using System.Security.Cryptography;
using System.Text;
using Shared;

namespace FirstParty.Grains;

public class FirstPartyGrain : Grain, IFirstPartyGrain
{
    private readonly RSA _rsa;
    private readonly byte[] _xorKey;

    public FirstPartyGrain()
    {
        _rsa = RSA.Create();
        _xorKey = RandomNumberGenerator.GetBytes(512);
    }
    

    /// <inheritdoc />
    public Task<byte[]> GetPublicKey()
    {
        return Task.FromResult(_rsa.ExportRSAPublicKey());
    }

    /// <inheritdoc />
    public Task SendVote(Vote vote)
    {
        var maskedVote = vote with { EncryptedData = vote.EncryptedData.ApplyXorCipher(_xorKey) };
        Votes.Add(maskedVote);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<Result> GetVotes()
    {
        var thirdPartyGrain = GrainFactory.GetGrain<IThirdPartyGrain>("ThirdParty");
        
        var unmaskedVotes = await thirdPartyGrain.UnmaskVotes(Votes.OrderBy(_ => Guid.NewGuid()).ToArray());

        var decryptedVotes = 
            unmaskedVotes.Votes
            .Select(vote => vote.ApplyXorCipher(_xorKey))
            .Select(vote => vote.DecryptRsa(_rsa))
            .Select(b => Encoding.UTF8.GetString(b))
            .GroupBy(x=>x)
            .ToDictionary(x=> x.Key, x=>x.Count());

        return new Result(decryptedVotes);
    }

    private List<Vote> Votes { get; } = new();
}