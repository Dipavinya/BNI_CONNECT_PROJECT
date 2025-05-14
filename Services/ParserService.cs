using BniConnect.Data;
using BniConnect.Interface;
using BniConnect.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BniConnect.Services
{
    public class ParserService : IParserService
    {
        private readonly ApplicationDbContext _context;

        public ParserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CountryMst> GetCountryByName(string countryName)
        {
            var country = await _context.CountryMst
                .FirstOrDefaultAsync(c => c.Country.ToLower() == countryName.ToLower());

            return country;
        }

        public async Task<List<CountryMst>> GetCountryByCode(string code)
        {
            var country = await _context.CountryMst.Where(c => c.Code == code).ToListAsync();

            return country;
        }

        public async Task<List<CountryMst>> GetCountries()
        {
            var country = await _context.CountryMst.ToListAsync();
            return country;
        }

        public async Task<UserProfile> ParseUserProfiles(string htmlResponse, string userId)
        {
            if (string.IsNullOrEmpty(htmlResponse))
            {
                throw new ArgumentNullException(nameof(htmlResponse), "Input data cannot be null or empty.");
            }

            try
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlResponse);

                // Extract email
                var emailNode = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='fieldtext']//a[contains(@href, 'mailto:')]");

                var displayNameNode = htmlDocument.DocumentNode.SelectSingleNode("//label[@for='memberDisplayName']/span[@class='fieldtext']");
                var industryNode = htmlDocument.DocumentNode.SelectSingleNode("//label[@for='memberPrimaryCategory']/span[@class='fieldtext']");
                var mobileNode = htmlDocument.DocumentNode.SelectSingleNode("//label[@for='memberMobileNumber']/span[@class='fieldtext']");
                var phoneNode = htmlDocument.DocumentNode.SelectSingleNode("//label[@for='memberPhoneNumber']/span[@class='fieldtext']");
                var countryNode = htmlDocument.DocumentNode.SelectSingleNode("//label[@for='memberCountry']/span[@class='fieldtext']");

                string displayName = displayNameNode != null ? displayNameNode.InnerText.Trim() : string.Empty;
                string industry = industryNode != null ? industryNode.InnerText.Trim() : string.Empty;
                string mobile = mobileNode != null ? mobileNode.InnerText.Trim() : string.Empty;
                string phone = phoneNode != null ? phoneNode.InnerText.Trim() : string.Empty;
                string country = countryNode != null ? countryNode.InnerText.Trim() : string.Empty;


                if (!string.IsNullOrEmpty(country))
                {
                    if (!string.IsNullOrEmpty(mobile) && !mobile.StartsWith('+'))
                    {
                        var countryInfo = await GetCountryByName(country);
                        if (countryInfo != null)
                        {
                            mobile = $"{countryInfo.Code}{mobile}";
                        }
                    }

                    if (!string.IsNullOrEmpty(phone) && !phone.StartsWith('+'))
                    {
                        var countryInfo = await GetCountryByName(country);
                        if (countryInfo != null)
                        {
                            phone = $"{countryInfo.Code}{phone}";
                        }
                    }
                }

                // Create the UserProfile object
                var userProfile = new UserProfile
                {
                    UserId = userId,
                    DisplayName = displayName,
                    Industry = industry,
                    MobileNumber = mobile,
                    PhoneNumber = phone,
                    Email = emailNode != null ? emailNode.InnerText.Trim() : string.Empty
                };

                return userProfile;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while parsing Userprofile");
                throw new Exception("Error parsing user profile data", ex);
            }
        }

        public async Task<UserProfile> ParseSocialProfiles(string htmlResponse, string userId)
        {
            if (string.IsNullOrEmpty(htmlResponse))
            {
                throw new ArgumentNullException(nameof(htmlResponse), "Input data cannot be null or empty.");
            }

            try
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlResponse);

                var socialLinks = new List<string>();

                var socialNetworkingLinksNode = htmlDocument.DocumentNode
                    .SelectSingleNode("//label[@for='memberSocialNetworkingLinks']/span[@class='fieldtext sitefavicons']");
                if (socialNetworkingLinksNode != null)
                {
                    var socialLinksNodes = socialNetworkingLinksNode.Descendants("a");
                    foreach (var linkNode in socialLinksNodes)
                    {
                        var href = linkNode.GetAttributeValue("href", string.Empty);
                        if (!string.IsNullOrEmpty(href))
                        {
                            if (href.Contains("linkedin.com"))
                            {
                                socialLinks.Add($"LinkedIn: {href}");
                            }
                            else if (href.Contains("facebook.com"))
                            {
                                socialLinks.Add($"Facebook: {href}");
                            }
                            else if (href.Contains("instagram.com"))
                            {
                                socialLinks.Add($"Instagram: {href}");
                            }
                            else
                            {
                                socialLinks.Add(href);
                            }
                        }
                    }
                }

                var userProfile = new UserProfile
                {
                    UserId = userId,
                    SocialNetworkLinks = socialLinks,
                };

                return userProfile;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Error while parsing social networks");
                throw new Exception("Error parsing user profile data", ex);
            }
        }


    }

}
