namespace Shared;

public interface IFirstPartyGrain : IGrainWithGuidKey
{
    Task<byte[]> GetPublicKey();
    Task RegisterVote(RegisterVote registerVote);
    Task<Result> GetVotes();
}


public interface IThirdPartyGrain : IGrainWithGuidKey
{
    Task<OneTimeKeyResult> GetOneTimeKey();
    Task<UnmaskedVotes> UnmaskVotes(ICollection<EncryptedVoteData> encryptedVotes);
}