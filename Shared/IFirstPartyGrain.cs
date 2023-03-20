namespace Shared;

public interface IFirstPartyGrain : IGrainWithStringKey
{
    Task<string> SayHello();
}

public interface IThirdPartyGrain : IGrainWithStringKey
{
    Task<string> SayHello();
}