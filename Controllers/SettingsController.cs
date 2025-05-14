using BniConnect.Interface;
using BniConnect.Models;
using BniConnect.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BniConnect.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IMailSettings _imailSetting;
        private readonly IWhatsAppSettingService _whatsAppSettingService;

        public SettingsController(IMailSettings imailSetting, IWhatsAppSettingService whatsAppSettingService)
        {
            _imailSetting = imailSetting;
            _whatsAppSettingService = whatsAppSettingService;
        }
        public async Task<IActionResult> Index()
        {
            var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _imailSetting.GetMailSettingsByClientId(clientId);
            var whatsApModel = await _whatsAppSettingService.GetWhatsAppSettingsByClientId(clientId);
            var viewModel = new SettingsViewModel
            {
                MailSettings = model,
                WhatsAppSettings = whatsApModel
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSetting(MailSettings model)
        {
            var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.ClientId = clientId;
            if (model.Id > 0)
            {
                bool updateSetting = await _imailSetting.InsertMailSetting(model);
                return RedirectToAction("Index");
            }
            else
            {
                bool addSetting = await _imailSetting.InsertMailSetting(model);
                return RedirectToAction("Index");
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWhatsAppSetting(WhatsAppSetting model)
        {
            var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.ClientId = clientId;
            if (model.Id > 0)
            {
                bool updateSetting = await _whatsAppSettingService.InsertWhatsappSetting(model);
                return RedirectToAction("Index");
            }
            else
            {
                bool addSetting = await _whatsAppSettingService.InsertWhatsappSetting(model);
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public async Task<JsonResult> GetEmailConfigurationById(int id)
        {
            var email = await _imailSetting.GetMailSettingsById(id);

            if (email == null)
            {
                return Json(new { status = 500, message = "Email configuration not found" });
            }

            return Json(new { status = 200, data = email });
        }

        [HttpGet]
        public async Task<JsonResult> GetWhatsAppConfigurationById(int id)
        {
            var whatsApp = await _whatsAppSettingService.GetWhatsAppSettingsById(id);

            if (whatsApp == null)
            {
                return Json(new { status = 500, message = "WhatsApp configuration not found" });
            }

            return Json(new { status = 200, data = whatsApp });
        }

        public async Task<ActionResult> DeleteEmailConfiguration(int id)
        {
            if (id > 0)
            {
                await _imailSetting.DeleteEmail(id);
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<ActionResult> DeleteWhatsAppConfiguration(int id)
        {
            if (id > 0)
            {
                await _whatsAppSettingService.DeleteWhatsApp(id);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
