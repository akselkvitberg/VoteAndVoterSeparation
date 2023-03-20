using Shared;

namespace ThirdParty.Grains;

public class ThirdPartyGrain : Grain, IThirdPartyGrain
{
    /// <inheritdoc />
    public async Task<string> SayHello()
    {
        return "Hello from third party";
    }
}