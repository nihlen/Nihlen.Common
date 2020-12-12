using System;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Microsoft.Extensions.Caching.Memory;
using Nihlen.Common.Abstractions;

namespace Nihlen.Common
{
    public class CountryResolver : ICountryResolver
    {
        private readonly IMemoryCache _cache;
        private readonly string _geoipDatabasePath;

        public CountryResolver(IMemoryCache cache, string geoipDatabasePath)
        {
            _cache = cache;
            _geoipDatabasePath = geoipDatabasePath;
        }

        public (string Code, string Name) GetCountryResponse(string ipAddress)
        {
            var cached = _cache.Get<CountryResponse>(ipAddress);
            if (cached != null)
                return (cached.Country.IsoCode, cached.Country.Name);

            if (string.IsNullOrWhiteSpace(_geoipDatabasePath))
                throw new Exception("No GeoipDbPath found");

            using (var reader = new DatabaseReader(_geoipDatabasePath))
            {
                if (reader.TryCountry(ipAddress, out var response))
                {
                    _cache.Set(ipAddress, response, DateTime.UtcNow.AddDays(1));
                    return (response?.Country.IsoCode, response?.Country.Name);
                }

                return (null, null);
            }
        }
    }
}
