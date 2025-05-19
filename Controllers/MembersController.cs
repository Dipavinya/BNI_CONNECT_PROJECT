using BniConnect.Interface;
using BniConnect.Models;
using BniConnect.ViewModel;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using Serilog;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BniConnect.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BniConnect.Controllers
{
    public class MembersController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly IParserService _parserService;
        private readonly ITemplateService _templateService;
        private readonly IMailSettings _mailSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MembersController> _logger;
        private readonly IWhatsAppSettingService _whisatAppSettingService;
        private readonly ICategoryService _categoryService;
        private readonly IMemberRepository memberRepository;

        public MembersController(HttpClient httpClient, IConfiguration configuration, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IAccountService accountService, IParserService parserService, ITemplateService templateService,
            IMailSettings mailSettings, IHttpContextAccessor httpContextAccessor, ILogger<MembersController> logger, IWhatsAppSettingService whisatAppSettingService, ICategoryService categoryService,IMemberRepository memberRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
            _parserService = parserService;
            _templateService = templateService;
            _mailSettings = mailSettings;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _whisatAppSettingService = whisatAppSettingService;
            _categoryService = categoryService;
            this.memberRepository = memberRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var clientId = user.Id;
            var defaultTemplate = await _mailSettings.GetLastUsedMailTemplateByClientId(clientId);
            var availableTemplates = await _templateService.GetTemplatesByClient(clientId);
            var mailSetting = await _mailSettings.GetMailSettingsByClientId(clientId);
            var whatsAppSetting = await _whisatAppSettingService.GetWhatsAppSettingsByClientId(clientId);
            var files = await GetFiles();
            var countrys = await _parserService.GetCountries();
            var category = await _categoryService.Getcategorys();
            var subCategories = await _categoryService.GetSubCategories();
            var viewModel = new ShowTableViewModel
            {
                Members = new List<Member>(),
                LastTemplateId = defaultTemplate,
                MailSettings = mailSetting,
                AvailableTemplates = availableTemplates,
                WhatsAppSetting = whatsAppSetting,
                Files = files,
                SearchParams = new MemberSearchModel(),
                CountryMst = countrys,
                CategoryMst = category,
                SubCategoryMst = new List<SubCategoryMst>()
            };
            return View(viewModel);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SearchWithCookie(MemberSearchModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var clientId = user.Id;


                // Call your DB instead of external API
                //var members = await memberRepository.SearchMembersAsync(model);
                List<Member> members = await memberRepository.SearchMembersAsync(model);
                int totalRecords = members.Count();

               
                //var totalpage = model.TotalPages + model.CurrentPage;

               
                return Json(new { members = members, totalCount = totalRecords });
                //return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while getting Search data from DB");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return RedirectToAction("Login", "Account");
            }
        }

        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> SearchWithCookie(MemberSearchModel model)
        //{
        //    try
        //    {
        //        string Cookiestr = HttpContext.Session.GetString("Cookie");
        //        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        //        if (Cookiestr == null)
        //        {
        //            var data = new ApplicationUser
        //            {
        //                user_id = user.user_id,
        //                Password = user.Password
        //            };

        //            await GetCookie(data);
        //            Cookiestr = HttpContext.Session.GetString("NewCookie");
        //        }

        //        var postUrl = "https://www.bniconnectglobal.com/web/secure/networkAddConnectionsJson";

        //        // Setting the POST data with pagination
        //        var postData = new List<KeyValuePair<string, string>>
        //{
        //    new KeyValuePair<string, string>("searchMembers", "Search Members"),
        //    new KeyValuePair<string, string>("formSubmit", "true"),
        //    new KeyValuePair<string, string>("currentPage", model.CurrentPage.ToString()),
        //    new KeyValuePair<string, string>("perPage", "25"),  // Change perPage as needed
        //    new KeyValuePair<string, string>("memberKeywords", model.MemberKeywords),
        //    new KeyValuePair<string, string>("memberFirstName", model.MemberFirstName),
        //    new KeyValuePair<string, string>("memberLastName", model.MemberLastName),
        //    new KeyValuePair<string, string>("memberCompanyName", model.MemberCompanyName),
        //    new KeyValuePair<string, string>("memberIdCountry", model.MemberCountryId),
        //    new KeyValuePair<string, string>("memberCity", model.MemberCity),
        //    new KeyValuePair<string, string>("memberState", model.MemberState),
        //    new KeyValuePair<string, string>("memberPrimaryCategory", model.MemberPrimaryCategory),
        //    new KeyValuePair<string, string>("memberSecondaryCategory", model.MemberSecondaryCategory)
        //};

        //        var content = new FormUrlEncodedContent(postData);

        //        _httpClient.DefaultRequestHeaders.Clear();
        //        _httpClient.DefaultRequestHeaders.Add("cookie", Cookiestr);
        //        _httpClient.DefaultRequestHeaders.Add("accept", "application/json, text/javascript, */*");
        //        _httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36 Edg/129.0.0.0");
        //        _httpClient.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
        //        _httpClient.DefaultRequestHeaders.Add("origin", "https://www.bniconnectglobal.com");
        //        _httpClient.DefaultRequestHeaders.Add("referer", "https://www.bniconnectglobal.com/web/secure/networkAddConnections");

        //        var response = await _httpClient.PostAsync(postUrl, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string jsonResponse = await response.Content.ReadAsStringAsync();
        //            var jsonDocument = JsonDocument.Parse(jsonResponse);
        //            var aaData = jsonDocument.RootElement.GetProperty("aaData").ToString();

        //            var members = ParseMembers(aaData);
        //            var totalpage = model.TotalPages + model.CurrentPage;
        //            var clientId = user.Id;
        //            var defaultTemplate = await _mailSettings.GetLastUsedMailTemplateByClientId(clientId);
        //            var availableTemplates = await _templateService.GetTemplatesByClient(clientId);
        //            var mailSetting = await _mailSettings.GetMailSettingsByClientId(clientId);
        //            var whatsAppSetting = await _whisatAppSettingService.GetWhatsAppSettingsByClientId(clientId);
        //            var files = await GetFiles();
        //            var countrys = await _parserService.GetCountries();
        //            var category = await _categoryService.Getcategorys();
        //            var subCategories = await _categoryService.GetSubCategoryByCatId(Convert.ToInt32(model.MemberPrimaryCategory));

        //            var searchParams = new MemberSearchModel
        //            {
        //                MemberKeywords = model.MemberKeywords,
        //                MemberFirstName = model.MemberFirstName,
        //                MemberLastName = model.MemberLastName,
        //                MemberCity = model.MemberCity,
        //                MemberCompanyName = model.MemberCompanyName,
        //                MemberCountryId = model.MemberCountryId,
        //                MemberPrimaryCategory = model.MemberPrimaryCategory,
        //                MemberSecondaryCategory = model.MemberSecondaryCategory,
        //                MemberState = model.MemberState,
        //                CurrentPage = model.CurrentPage,
        //                TotalPages = totalpage
        //            };

        //            var viewModel = new ShowTableViewModel
        //            {
        //                Members = members,
        //                LastTemplateId = defaultTemplate,
        //                MailSettings = mailSetting,
        //                AvailableTemplates = availableTemplates,
        //                WhatsAppSetting = whatsAppSetting,
        //                Files = files,
        //                SearchParams = searchParams,
        //                CountryMst = countrys,
        //                CategoryMst = category,
        //                SubCategoryMst = subCategories
        //            };

        //            return View("Index", viewModel);
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Failed to send POST request.");
        //            return View();
        //        }
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        ModelState.AddModelError(string.Empty, "A network error occurred. Please try again later.");
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "Error while getting Search data");
        //        ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
        //        return RedirectToAction("Login", "Account");
        //    }
        //}

        public async Task<IActionResult> HandleCheckBox(string[] userIds)
        {
            if (userIds == null || userIds.Length == 0)
            {
                return BadRequest(new { StatusCode = 400, Message = "User IDs cannot be null or empty." });
            }

            var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var results = new List<object>();

            foreach (var id in userIds)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    results.Add(new { UserId = id, Status = "Invalid", Message = "User ID is null or empty.", StatusCode = 400 });
                    continue;
                }

                var user = await _accountService.GetUserById(id);
                if (user != null)
                {
                    results.Add(new { UserId = id, Status = "Exists", Message = $"{user.DisplayName} already exists.", StatusCode = 200 });
                    continue;
                }

                var apiUrl = $"https://www.bniconnectglobal.com/web/secure/networkProfile?userId={id}&canAddNetwok=true";
                var cookie = HttpContext.Session.GetString("Cookie") ?? HttpContext.Session.GetString("NewCookie");

                try
                {
                    await Task.Delay(2500); // Simulating delay
                    var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    request.Headers.Add("Host", "www.bniconnectglobal.com");
                    request.Headers.Add("Cookie", cookie);

                    var response = await _httpClient.SendAsync(request);
                    var statusCode = (int)response.StatusCode;
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.TooManyRequests) // 429 Too Many Requests
                    {
                        results.Add(new
                        {
                            UserId = id,
                            Status = "RateLimited",
                            Message = "API rate limit exceeded. Try again later.",
                            StatusCode = 429,
                            ApiResponse = responseBody // Capture response from API
                        });
                        continue;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var userProfile = await _parserService.ParseUserProfiles(responseBody, id);
                        userProfile.ClientId = clientId;

                        if (userProfile != null)
                        {
                            bool result = await _accountService.InsertUserProfile(userProfile);
                            results.Add(new
                            {
                                UserId = id,
                                Status = result ? "Inserted" : "Failed",
                                Message = result ? "User profile inserted successfully." : "Failed to insert user profile.",
                                StatusCode = result ? 201 : 500
                            });
                        }
                        else
                        {
                            results.Add(new { UserId = id, Status = "NotFound", Message = "User profile not found.", StatusCode = 404 });
                        }
                    }
                    else
                    {
                        results.Add(new
                        {
                            UserId = id,
                            Status = "ApiError",
                            Message = $"API call failed with status: {statusCode} - {response.ReasonPhrase}",
                            StatusCode = statusCode
                        });
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    results.Add(new { UserId = id, Status = "Error", Message = $"HTTP request error: {httpEx.Message}", StatusCode = 500 });
                }
                catch (Exception ex)
                {
                    results.Add(new { UserId = id, Status = "Error", Message = $"An error occurred: {ex.Message}", StatusCode = 500 });
                }
            }

            return Ok(results);
        }



        public async Task<IActionResult> GetSocialLinks(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var apiUrl = $"https://www.bniconnectglobal.com/web/secure/networkProfile?userId={userId}&canAddNetwok=true";
            var cookie = HttpContext.Session.GetString("Cookie");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("Host", "www.bniconnectglobal.com");
                request.Headers.Add("Cookie", cookie);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var userProfile = await _parserService.ParseSocialProfiles(responseBody, userId);

                    if (userProfile != null)
                    {
                        return Ok(userProfile);
                    }
                    else
                    {
                        return NotFound("User profile not found.");
                    }
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"API call failed with status: {(int)response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while getting GetSocialLinks ");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetUserEmail(string userId)
        {
            try
            {
                var email = await _accountService.GetEmailByUserId(userId);
                if (email != null)
                {
                    return Ok(email);
                }
                return NotFound("Email not found for the provided userId.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching email for userId: " + userId);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetUserNumber(string userId)
        {
            var number = await _accountService.GetNumberByUserId(userId);
            if (number != null)
            {
                return Ok(number);
            }
            return NotFound();
        }

        public async Task<IActionResult> ConnectionHistory()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var clientId = user.Id;
            var defaultTemplate = await _mailSettings.GetLastUsedMailTemplateByClientId(clientId);
            var availableTemplates = await _templateService.GetTemplatesByClient(clientId);
            var mailSetting = await _mailSettings.GetMailSettingsByClientId(clientId);
            var whatsAppSetting = await _whisatAppSettingService.GetWhatsAppSettingsByClientId(clientId);
            var files = await GetFiles();
            var viewModel = new ShowTableViewModel
            {
                Members = new List<Member>(),
                LastTemplateId = defaultTemplate,
                MailSettings = mailSetting,
                AvailableTemplates = availableTemplates,
                WhatsAppSetting = whatsAppSetting,
                Files = files,
                SearchParams = new MemberSearchModel(),
                CountryMst = new List<CountryMst>(),
                CategoryMst =new List<CategoryMst>(),
                SubCategoryMst = new List<SubCategoryMst>()
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<JsonResult> GetConnectionHistoryData()
        {
            var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var conHistory = await _accountService.GetConnectionHistoryByClientId(clientId);

            if (conHistory != null)
            {
                return Json(new { data = conHistory });
            }
            else
            {
                return Json(new { data = "No data available" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteSelectedConnections([FromBody] DeleteSelectedConnectionsRequest request)
        {
            if (request?.ids == null || request.ids.Length == 0)
            {
                return Json(new { success = false, message = "No IDs provided for deletion." });
            }

            try
            {
                bool isDeleted = await _accountService.DeleteConnection(request.ids);

                if (isDeleted)
                {
                    return Json(new { success = true, message = "Connections deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete connections." });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in DeleteSelectedConnections");
                return Json(new { success = false, message = "An error occurred while deleting connections." });
            }
        }
        public class DeleteSelectedConnectionsRequest
        {
            public string[] ids { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SendConnection([FromBody] SendConnection model)
        {
            var url = "https://www.bniconnectglobal.com/web/secure/networkConnectionsAddToMyConnection";
            var cookie = HttpContext.Session.GetString("Cookie");
            var responses = new List<string>();
            var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            foreach (var member in model.Members)
            {
                var personalizedBody = model.AddToMyConnectionsBody;
                var decodedBody = WebUtility.UrlDecode(personalizedBody);
                decodedBody =  decodedBody.Replace("{Your Name}", member.Name).Replace("Send Connection", "");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("isFormSubmit", "true"),
                    new KeyValuePair<string, string>("userId", member.UserId),
                    new KeyValuePair<string, string>("addToMyConnectionsBody", decodedBody)
                });

                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                // Set headers specific to this request
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                request.Headers.Add("Cookie", cookie);
                request.Headers.Add("Origin", "https://www.bniconnectglobal.com");
                request.Headers.Referrer = new Uri("https://www.bniconnectglobal.com/web/secure/networkAddConnections");
                request.Headers.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36 Edg/129.0.0.0");
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");

                var response = await _httpClient.SendAsync(request);
                var userProfile = await _accountService.GetUserById(member.UserId);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    if (userProfile != null)
                    {
                        await _accountService.SaveConnectinHistory(userProfile, true, clientId);
                    }
                    responses.Add(member.Name);
                }
                else
                {
                    if (userProfile != null)
                    {
                        await _accountService.SaveConnectinHistory(userProfile, false, clientId);
                    }
                    responses.Add(member.Name);
                }
            }

            return Ok(responses);
        }


        public static List<Member> ParseMembers(string jsonResponse)
        {
            if (string.IsNullOrEmpty(jsonResponse))
            {
                throw new ArgumentNullException(nameof(jsonResponse), "Input data cannot be null or empty.");
            }

            var members = new List<Member>();
            var data = JArray.Parse(jsonResponse);

            var htmlData = data[0].ToString();

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlData);

            var rows = doc.DocumentNode.SelectNodes("//tr");
            if (rows == null) return members;

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");
                if (cells == null || cells.Count < 6) continue;

                var userIdNode = cells[1].SelectSingleNode(".//a");
                var userId = userIdNode != null ? userIdNode.GetAttributeValue("href", "").Split('=')[1] : string.Empty;

                var member = new Member
                {
                    UserId = userId,
                    Name = userIdNode?.InnerText.Trim(),
                    Chapter = cells[2].InnerText.Trim(),
                    Company = cells[3].InnerText.Trim(),
                    City = cells[4].InnerText.Trim(),
                    IndustryClassification = cells[5].InnerText.Trim(),
                    Location = cells[4].InnerText.Trim(),
                    Category = cells[5].InnerText.Trim(),
                    ConnectionSent = row.SelectSingleNode(".//img[contains(@src, 'add.gif')]") == null
                };

                members.Add(member);
            }

            return members;
        }

        public async Task<List<FileViewModel>> GetFiles()
        {
            try
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsDirectory))
                {
                    return new List<FileViewModel>();
                }

                var files = Directory.GetFiles(uploadsDirectory);

                var fileUrls = files.Select(file => new FileViewModel
                {
                    FileName = Path.GetFileName(file),
                    FileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{Path.GetFileName(file)}"
                }).ToList();

                return fileUrls;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error While getting files");
                return new List<FileViewModel>();
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSubcategoryById(int catCode)
        {
            var subcategories = await _categoryService.GetSubCategoryByCatId(catCode);
            if (subcategories != null)
            {
                return Json(new { status = "Success", data = subcategories});
            }
            return Json(new { status = "Error", message = "No subcategories found" });
        }

        public async Task<IActionResult> GetCookie(ApplicationUser login)
        {
            try
            {

                if (login != null)
                {
                    var apiUrl = _configuration["ApiSettings:LoginUrl"];
                    var authToken = _configuration["ApiSettings:authorization"];
                    var formData = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        client_id = login.client_id,
                        user_id = login.user_id,
                        password = login.Password,
                    });
                    var httpContent = new StringContent(formData, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
                    var response = await _httpClient.PostAsync(apiUrl, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var chromeOptions = new ChromeOptions();
                        //chromeOptions.AddArgument("--headless");
                        chromeOptions.AddArgument("--disable-popup-blocking");
                        chromeOptions.AddArgument("--disable-notifications");
                        chromeOptions.AddArgument("--disable-infobars");
                        chromeOptions.AddArgument("--no-sandbox");
                        chromeOptions.AddArgument("--disable-extensions");
                        chromeOptions.AddArgument("--start-maximized");
                        chromeOptions.AddArgument("--window-size=1920x1080");

                        using (IWebDriver driver = new ChromeDriver(chromeOptions))
                        {
                            // Navigate to the login URL
                            driver.Navigate().GoToUrl("https://www.bniconnectglobal.com/login/?message=");

                            // Create a WebDriverWait with a timeout of 10 seconds
                            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(10));

                            // Wait for the page to be fully loaded
                            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                            await Task.Delay(2000);

                            // Wait for the username field and enter the user ID
                            var username = wait.Until(d => d.FindElement(By.Name("username")));
                            username.SendKeys(login.user_id);

                            // Wait for the password field and enter the password
                            var password = wait.Until(d => d.FindElement(By.Name("password")));
                            password.SendKeys(login.Password);

                            // Wait for the submit button and click it
                            var submitButton = wait.Until(d => d.FindElement(By.XPath("//button[@type='submit']")));
                            submitButton.Click();

                            // Optional: Wait for a short time to ensure that the login process completes
                            await Task.Delay(3000);

                            // Get all cookies after logging in
                            ReadOnlyCollection<OpenQA.Selenium.Cookie> cookies = driver.Manage().Cookies.AllCookies;
                            string cookieStr = string.Join("; ", cookies.Select(cookie => $"{cookie.Name}={cookie.Value}"));
                            HttpContext.Session.SetString("NewCookie", cookieStr);
                            HttpContext.Session.SetString("ClientId", login.user_id.ToString());

                            return RedirectToAction("Index", "Members");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    }

                }
            }
            catch (HttpRequestException httpEx)
            {
                await Response.WriteAsync(httpEx.ToString());
                ModelState.AddModelError(string.Empty, "A network error occurred. Please try again later.");
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                await Response.WriteAsync(ex.ToString());
                return new EmptyResult();
            }

            return View("Index", login);
        }
    }
}
