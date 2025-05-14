using BniConnect.Interface;
using BniConnect.Models;
using BniConnect.Services;
using BniConnect.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Security.Claims;
using System.Globalization;

namespace BniConnect.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITemplateService _templateService;
        private readonly IMailSettings _mailSettings;
        private readonly IWhatsAppSettingService _whisatAppSettingService;

        public EmailController(IEmailService emailService, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor,
        ITemplateService templateService, IMailSettings mailSettings, IWhatsAppSettingService whisatAppSettingService)
        {
            _emailService = emailService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _templateService = templateService;
            _mailSettings = mailSettings;
            _whisatAppSettingService = whisatAppSettingService;
        }

        public async Task<IActionResult> Index()
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
                CategoryMst = new List<CategoryMst>(),
                SubCategoryMst = new List<SubCategoryMst>()
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<JsonResult> GetEmailHistoryData()
        {
            var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = await _emailService.GetEmailHistoryByClientId(clientId);
            if (history != null)
            {
                return Json(new { data = history });
            }
            else
            {
                return Json(null);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetEmailById(int id)
        {
            try
            {
                var history = await _emailService.GetEmailById(id);
                return Json(new { data = history });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error While getting Message");
                return Json(new { Data = "No message found" });
            }


        }

        [HttpPost]
        public async Task<JsonResult> DeleteSelectedEmails([FromBody] DeleteSelectedRequest request)
        {
            if (request?.ids == null || request.ids.Length == 0)
            {
                return Json(new { success = false, message = "No IDs provided for deletion." });
            }

            try
            {
                bool isDeleted = await _emailService.DeleteEmailsHistory(request.ids);

                if (isDeleted)
                {
                    return Json(new { success = true, message = "History deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete History." });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in DeleteSelectedEmails");
                return Json(new { success = false, message = "An error occurred while deleting History." });
            }
        }

        public class DeleteSelectedRequest
        {
            public string[] ids { get; set; }
        }

        //[HttpPost]
        //public async Task<IActionResult> SendMail()
        //{
        //    var userMail = Request.Form["settingEmail"];
        //    var gmailAppKey = Request.Form["settingAppKey"];
        //    var displayName = Request.Form["setting.DisplayName"];
        //    var clientId = Request.Form["settingClientId"];
        //    var templateId = Request.Form["LastTemplateId"];
        //    var emailSubject = Request.Form["emailSubject"];
        //    var emailBody = Request.Form["emailBody"];
        //    var attachment = Request.Form.Files["attachment"];
        //    var emailsJson = Request.Form["emails"];
        //    var namesJson = Request.Form["names"];
        //    var useridsJson = Request.Form["userIds"];

        //    List<string> recipientEmails = new List<string>();
        //    List<string> recipientNames = new List<string>();
        //    List<string> recipientUserIds = new List<string>();
        //    List<string> successfulRecipients = new List<string>();
        //    List<string> alreadysentEmails = new List<string>();

        //    if (!string.IsNullOrEmpty(emailsJson) && !string.IsNullOrEmpty(namesJson) && !string.IsNullOrEmpty(useridsJson))
        //    {
        //        recipientEmails = JsonConvert.DeserializeObject<List<string>>(emailsJson);
        //        recipientNames = JsonConvert.DeserializeObject<List<string>>(namesJson);
        //        recipientUserIds = JsonConvert.DeserializeObject<List<string>>(useridsJson);
        //    }
        //    if (recipientEmails.Count < 0)
        //    {
        //        return Json(new { Status = "Failed", Message = "No Email found to send message" });
        //    }

        //    var emailBodyString = emailBody.ToString();

        //    for (int i = 0; i < recipientEmails.Count; i++)
        //    {
        //        var existingEmail = await _emailService.GetEmailByReceipient(recipientEmails[i]);

        //        //DateTime sixMonthsAgo = DateTime.Now.AddMonths(-6);

        //        //if (existingEmail != null && existingEmail.SentDate >= sixMonthsAgo)
        //        //{
        //        //    alreadysentEmails.Add(recipientEmails[i]);
        //        //    continue;
        //        //}

        //        var personalizedBody = emailBodyString
        //            .Replace("Send Connection", "")
        //            .Replace("{Your Name}", recipientNames[i]);

        //        try
        //        {
        //            bool result = await _emailService.SendEmail(userMail, gmailAppKey, displayName, recipientEmails[i], emailSubject, personalizedBody, attachment);
        //            if (result)
        //            {
        //                await _emailService.SaveEmailHistory(recipientNames[i], personalizedBody, recipientEmails[i], userMail, emailSubject, true, clientId, recipientUserIds[i]);
        //                successfulRecipients.Add(recipientNames[i]);
        //            }
        //            else
        //            {
        //                await _emailService.SaveEmailHistory(recipientNames[i], personalizedBody, recipientEmails[i], userMail, emailSubject, false, clientId, recipientUserIds[i]);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", $"Failed to send email to {recipientEmails[i]}: {ex.Message}");
        //            return Json(new { Status = "Failed", sent = false });
        //        }

        //    }
        //    var alreadySentList = string.Join(", ", alreadysentEmails);
        //    if (successfulRecipients.Count > 0)
        //    {
        //        var namesList = string.Join(", ", successfulRecipients);

        //        return Json(new { Status = "Success", sent = true, sentTo = namesList, alreadySent = alreadySentList });
        //    }

        //    return Json(new { Status = "Failed", Message = "No data ", alreadySent = alreadySentList });

        //}

        [HttpPost]
        public async Task<IActionResult> SendMail()
        {
            var userMail = Request.Form["settingEmail"];
            var gmailAppKey = Request.Form["settingAppKey"];
            var displayName = Request.Form["setting.DisplayName"];
            var clientId = Request.Form["settingClientId"];
            var emailSubject = Request.Form["emailSubject"];
            var emailBody = Request.Form["emailBody"];
            var attachment = Request.Form.Files["attachment"];
            var userDetailsJson = Request.Form["userDetails"];

            List<UserDetails> userDetailsList = string.IsNullOrEmpty(userDetailsJson)
                ? new List<UserDetails>()
                : JsonConvert.DeserializeObject<List<UserDetails>>(userDetailsJson);

            if (!userDetailsList.Any())
            {
                return Json(new { Status = "Failed", Message = "No user details provided for sending emails." });
            }

            var successfulRecipients = new List<string>();
            var alreadySentEmails = new List<string>();

            foreach (var user in userDetailsList)
            {
                try
                {
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    string titledUserName = textInfo.ToTitleCase(user.Name.ToLower());

                    var personalizedBody = emailBody.ToString()
                        .Replace("{Your Name}", titledUserName);

                    bool emailSent = await _emailService.SendEmail(userMail, gmailAppKey, displayName, user.Email, emailSubject, personalizedBody, attachment);

                    await _emailService.SaveEmailHistory(user.Name, personalizedBody, user.Email, userMail, emailSubject, emailSent, clientId, user.UserId);

                    if (emailSent)
                    {
                        successfulRecipients.Add(user.Name);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to send email to {user.Email}: {ex.Message}");
                }
            }

            return Json(new
            {
                Status = successfulRecipients.Any() ? "Success" : "Failed",
                Sent = successfulRecipients.Any(),
                SentTo = string.Join(", ", successfulRecipients),
                AlreadySent = string.Join(", ", alreadySentEmails)
            });
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
    }
}
