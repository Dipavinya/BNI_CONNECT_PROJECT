using Microsoft.AspNetCore.Mvc;
using BniConnect.Interface;
using Serilog;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;
using BniConnect.Models;
using BniConnect.Services;
using BniConnect.ViewModel;
using Microsoft.AspNetCore.Identity;

public class WhatsappController : Controller
{
    private readonly IWhatsappService _whatsappService;
    private readonly IAccountService _accountService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITemplateService _templateService;
    private readonly IMailSettings _mailSettings;
    private readonly IWhatsAppSettingService _whisatAppSettingService;


    public WhatsappController(IWhatsappService whatsappService, IAccountService accountService, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, 
        ITemplateService templateService, IMailSettings mailSettings, IWhatsAppSettingService whisatAppSettingService)
    {
        _whatsappService = whatsappService;
        _accountService = accountService;
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
    public async Task<JsonResult> GetWhatsAppHistoryData()
    {
        var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var history = await _whatsappService.GetWhatsAppHistoryByClientId(clientId);
        if (history != null)
        {
            return Json(new { data = history});
        }
        else
        {
            return Json(new { data = "No data available"});
        }
    }

    [HttpPost]
    public async Task<JsonResult> DeleteSelectedWhatsApp([FromBody] DeleteSelectedRequest request)
    {
        if (request?.ids == null || request.ids.Length == 0)
        {
            return Json(new { success = false, message = "No IDs provided for deletion." });
        }

        try
        {
            bool isDeleted = await _whatsappService.DeleteWhatsAppHistory(request.ids);

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

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] WhatsAppMessageRequest request)
    {
        try
        {
            var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var url = "https://merabatuva.com/api/send";
            string formattedNumber = request.Number.Replace("+", "");
            var payload = new object();
            if (request.MediaUrl != null)
            {
                payload = new
                {
                    number = formattedNumber,
                    type = "text",
                    message = request.Message,
                    media_url = request.MediaUrl,
                    instance_id = request.InstanceId,
                    access_token = request.AccessToken
                };
            }
            else
            {
                payload = new
                {
                    number = formattedNumber,
                    type = "text",
                    message = request.Message,
                    instance_id = request.InstanceId,
                    access_token = request.AccessToken
                };
            }

            var jsonContent = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, content);
                var data = response.Content.ReadAsStringAsync();
                Log.Information(await data, "response after post api of send text on Whatsapp");
                var userData = await _accountService.GetUserByPhone(request.Number);
                if (response.IsSuccessStatusCode)
                {
                    if (userData != null)
                    {
                        await _whatsappService.SaveWhatsAppHistory(userData, true,request.Message,clientId);
                    }
                    return Json(new { success = true, message = "Message sent successfully." });
                }
                else
                {
                    if (userData != null)
                    {
                        await _whatsappService.SaveWhatsAppHistory(userData, false,request.Message, clientId);
                    }
                    return Json(new { success = false, message = "Failed to send message." });
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "error in sending Whatsapp msg");
            return Json(new { success = false, message = "Failed to send message." });
        }
    }

    [HttpGet]
    public async Task<JsonResult> GetMessageById(int id)
    {
        try
        {
            var message = await _whatsappService.GetWhatsAppMsgById(id);
            return Json(new { Data = message });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error While getting Message");
            return Json(new { Data = "No message found" });
        }
    }

    public class WhatsAppMessageRequest
    {
        public string Number { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public string InstanceId { get; set; }
        public string MediaUrl { get; set; }
        public string FileName {get; set;}
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
