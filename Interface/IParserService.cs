using BniConnect.Models;

namespace BniConnect.Interface
{
    public interface IParserService
    {
        Task<UserProfile> ParseUserProfiles(string htmlResponse, string userId);
        Task<CountryMst> GetCountryByName(string countryName);
        Task<UserProfile> ParseSocialProfiles(string htmlResponse, string userId);
        Task<List<CountryMst>> GetCountryByCode(string code);
        Task<List<CountryMst>> GetCountries();
    }
}
