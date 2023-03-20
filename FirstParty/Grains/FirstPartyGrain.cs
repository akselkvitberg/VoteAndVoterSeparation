using Shared;

namespace FirstParty.Grains;

public class FirstPartyGrain : Grain, IFirstPartyGrain
{
    /// <inheritdoc />
    public async Task<string> SayHello()
    {
        return "Hello from first party";
    }
}