using BniConnect.Interface;
using BniConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Text;

namespace BniConnect.Controllers
{
    public class AccountController : Controller
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
        private readonly ISubscriptionService _subscriptionService;

        public AccountController(HttpClient httpClient, IConfiguration configuration, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IAccountService accountService, IParserService parserService, ITemplateService templateService,
            IMailSettings mailSettings, IHttpContextAccessor httpContextAccessor, ISubscriptionService subscriptionService)
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
            _subscriptionService = subscriptionService;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(ApplicationUser login)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("Login", login);

                // 1. Check subscription
                var subscription = await _subscriptionService.GetSubscriptionByEmail(login.user_id);
                if (subscription == null || !subscription.Allfeature)
                {
                    ModelState.AddModelError(string.Empty, "Your subscription is not active, please contact admin.");
                    return View("Login", login);
                }

                // 2. Selenium to login to external site
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--disable-popup-blocking");
                chromeOptions.AddArgument("--disable-notifications");
                chromeOptions.AddArgument("--disable-infobars");
                chromeOptions.AddArgument("--no-sandbox");
                chromeOptions.AddArgument("--disable-extensions");
                chromeOptions.AddArgument("--start-maximized");
                chromeOptions.AddArgument("--window-size=1920x1080");

                //using (IWebDriver driver = new ChromeDriver(chromeOptions))
                //{
                //    driver.Navigate().GoToUrl("https://www.bniconnectglobal.com/login/?message=");
                //    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                //    wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                //    await Task.Delay(2000);

                //    var username = wait.Until(d => d.FindElement(By.Name("username")));
                //    username.SendKeys(login.user_id);

                //    var password = wait.Until(d => d.FindElement(By.Name("password")));
                //    password.SendKeys(login.Password);

                //    var submitButton = wait.Until(d => d.FindElement(By.XPath("//button[@type='submit']")));
                //    submitButton.Click();

                //    await Task.Delay(3000); // Wait for login to complete
                //}

                // 3. Local user setup
                var user = await _userManager.FindByNameAsync(login.user_id);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = login.user_id,
                        Email = login.user_id,
                        Password = login.Password,
                        user_id = login.user_id,
                    };

                    var result = await _userManager.CreateAsync(user, login.Password);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View("Index", login);
                    }
                }

                // 4. Sign in locally
                await _signInManager.SignInAsync(user, isPersistent: false);

                // 5. Store secure cookies
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("UserId", user.user_id, cookieOptions);
                Response.Cookies.Append("UserEmail", user.Email, cookieOptions);
                Response.Cookies.Append("UserName", user.UserName, cookieOptions);
                Response.Cookies.Append("Password", user.Password, cookieOptions);

                return RedirectToAction("Index", "Members");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                await Response.WriteAsync(ex.ToString());
                return new EmptyResult();
            }

            return View("Index", login);
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(ApplicationUser login)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            var subscription = await _subscriptionService.GetSubscriptionByEmail(login.user_id);
        //            if (subscription == null || subscription.Allfeature == false)
        //            {
        //                ModelState.AddModelError(string.Empty, "Your subscription is not active, please contact admin.");
        //                return View("Login", login);
        //            }
        //            var apiUrl = _configuration["ApiSettings:LoginUrl"];
        //            var authToken = _configuration["ApiSettings:authorization"];
        //            var formData = System.Text.Json.JsonSerializer.Serialize(new
        //            {
        //                client_id = login.client_id,
        //                user_id = login.user_id,
        //                password = login.Password,
        //            });
        //            var httpContent = new StringContent(formData, Encoding.UTF8, "application/json");
        //            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
        //            var response = await _httpClient.PostAsync(apiUrl, httpContent);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var chromeOptions = new ChromeOptions();
        //                //chromeOptions.AddArgument("--headless");
        //                chromeOptions.AddArgument("--disable-popup-blocking");
        //                chromeOptions.AddArgument("--disable-notifications");
        //                chromeOptions.AddArgument("--disable-infobars");
        //                chromeOptions.AddArgument("--no-sandbox");
        //                chromeOptions.AddArgument("--disable-extensions");
        //                chromeOptions.AddArgument("--start-maximized");
        //                chromeOptions.AddArgument("--window-size=1920x1080");

        //                using (IWebDriver driver = new ChromeDriver(chromeOptions))
        //                {
        //                    // Navigate to the login URL
        //                    driver.Navigate().GoToUrl("https://www.bniconnectglobal.com/login/?message=");

        //                    // Create a WebDriverWait with a timeout of 10 seconds
        //                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(10));

        //                    // Wait for the page to be fully loaded
        //                    wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        //                    await Task.Delay(2000);

        //                    // Wait for the username field and enter the user ID
        //                    var username = wait.Until(d => d.FindElement(By.Name("username")));
        //                    username.SendKeys(login.user_id);

        //                    // Wait for the password field and enter the password
        //                    var password = wait.Until(d => d.FindElement(By.Name("password")));
        //                    password.SendKeys(login.Password);

        //                    // Wait for the submit button and click it
        //                    var submitButton = wait.Until(d => d.FindElement(By.XPath("//button[@type='submit']")));
        //                    submitButton.Click();

        //                    // Optional: Wait for a short time to ensure that the login process completes
        //                    await Task.Delay(3000);

        //                    // Get all cookies after logging in
        //                    ReadOnlyCollection<OpenQA.Selenium.Cookie> cookies = driver.Manage().Cookies.AllCookies;
        //                    string cookieStr = string.Join("; ", cookies.Select(cookie => $"{cookie.Name}={cookie.Value}"));
        //                    HttpContext.Session.SetString("Cookie", cookieStr);
        //                    HttpContext.Session.SetString("ClientId", login.user_id.ToString());
        //                    var user = await _userManager.FindByNameAsync(login.user_id);

        //                    if (user == null)
        //                    {
        //                        user = new ApplicationUser
        //                        {
        //                            UserName = login.user_id,
        //                            Email = login.user_id,
        //                            PasswordHash = login.Password,
        //                            Password = login.Password,
        //                            user_id = login.user_id,
        //                        };

        //                        var result = await _userManager.CreateAsync(user, login.Password);
        //                        if (!result.Succeeded)
        //                        {
        //                            foreach (var error in result.Errors)
        //                            {
        //                                ModelState.AddModelError(string.Empty, error.Description);
        //                            }
        //                            return View("Index", login);
        //                        }
        //                    }

        //                    await _signInManager.SignInAsync(user, isPersistent: false);

        //                    return RedirectToAction("Index", "Members");
        //                }
        //            }
        //            else
        //                    {
        //                ModelState.AddModelError(string.Empty, "Invalid username or password.");
        //            }

        //        }
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        await Response.WriteAsync(httpEx.ToString());
        //        ModelState.AddModelError(string.Empty, "A network error occurred. Please try again later.");
        //        return new EmptyResult();
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
        //        await Response.WriteAsync(ex.ToString());
        //        return new EmptyResult();
        //    }

        //    return View("Index", login);
        //}

        [HttpGet]        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}

