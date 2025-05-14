using BniConnect.Interface;
using BniConnect.Models;
using Microsoft.AspNetCore.Mvc;

namespace BniConnect.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ITemplateService _templateService;
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ITemplateService templateService, ISubscriptionService subscriptionService)
        {
            _templateService = templateService;
            _subscriptionService = subscriptionService;
        }
    

        [HttpPost]
        public async Task<ActionResult> CreateSubscription(Subscription model)
        {
            if (ModelState.IsValid)
            {
                await _subscriptionService.InsertSubscription(model);                
            }
            return Ok(model);
        }

        public async Task<JsonResult> getSubscriptionById(int id)
        {
            if (id > 0)
            {
               var subscription = await _subscriptionService.GetSubscriptionById(id);
                if (subscription != null)
                {
                    return Json( new {data =subscription, Message= "Data fetched succesfully"});
                }
            }
            return Json(new { data = "No data found" , Message = "Error while finding data"});
        }
    }
}
