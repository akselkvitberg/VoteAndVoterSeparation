namespace Shared;

public interface IFirstPartyGrain : IGrainWithStringKey
{
    Task<byte[]> GetPublicKey();
    Task SendVote(Vote vote);
    Task<Result> GetVotes();
}


public interface IThirdPartyGrain : IGrainWithStringKey
{
    Task<OneTimeKeyResult> GetOneTimeKey();
    Task<UnmaskedVotes> UnmaskVotes(Vote[] encryptedVotes);
}