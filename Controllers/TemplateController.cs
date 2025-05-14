using BniConnect.Interface;
using BniConnect.Models;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace BniConnect.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public async Task<ActionResult> Index()
        {
            var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var templates = await _templateService.GetTemplatesByClient(clientId);
            return View(templates);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTemplate(TemplateMaster template)
        {
            if (template != null)
            {
                bool result = await _templateService.InsertTemplate(template);
                if (result)
                { 
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Error", "Error at the time of creating template");
                }
            }
            return RedirectToAction("Index");

        }

        public async Task<ActionResult> Delete(int id)
        {
            if (id > 0)
            {
                await _templateService.DeleteTemplate(id);
                return RedirectToAction("Index");
            }
            return View();
        } 

        [HttpGet]
        public async Task<JsonResult> GetTemplateById(int id)
        {
            var template = await _templateService.GetTemplateById(id); 

            if (template == null)
            {
                return Json(new { status = 500, message = "Template not found" });
            }

            return Json(new { status = 200, data = template }); 
        }

    }
}
