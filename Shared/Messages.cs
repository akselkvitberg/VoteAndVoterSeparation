namespace Shared;

[GenerateSerializer]
public record Result([property: Id(1)] Dictionary<string, int> Votes);


[GenerateSerializer]
public record UnmaskedVotes(
    [property: Id(1)] IEnumerable<byte[]> Votes
);

[GenerateSerializer]
public record Vote(
    [property: Id(1)]Guid ThirdPartyKeyId, 
    [property: Id(2)] byte[] EncryptedData
);

[GenerateSerializer]
public record OneTimeKeyResult(
    [property: Id(1)] Guid Id, 
    [property: Id(2)] byte[] Key
);