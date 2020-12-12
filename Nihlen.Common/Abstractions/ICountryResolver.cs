using MaxMind.GeoIP2.Responses;

namespace Nihlen.Common.Abstractions
{
    public interface ICountryResolver
    {
        (string Code, string Name) GetCountryResponse(string ipAddress);
    }
}
