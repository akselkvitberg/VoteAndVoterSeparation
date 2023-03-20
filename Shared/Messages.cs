namespace Shared;

[GenerateSerializer]
public record OneTimeKeyResult(
    [property: Id(1)] Guid Id, 
    [property: Id(2)] byte[] Key
);

[GenerateSerializer]
public record RegisterVote(
    [property: Id(1)] string VoterId, 
    [property: Id(2)] Guid ThirdPartyKeyId, 
    [property: Id(3)] byte[] EncryptedData
);

[GenerateSerializer]
public record EncryptedVoteData(
    [property: Id(1)] Guid ThirdPartyKeyId,
    [property: Id(2)] byte[] EncryptedData
);

[GenerateSerializer]
public record UnmaskedVotes(
    [property: Id(1)] IEnumerable<byte[]> Votes
);

[GenerateSerializer]
public record Result(
    [property: Id(1)] Dictionary<string, int> Votes
);