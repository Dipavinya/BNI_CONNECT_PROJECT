using BniConnect.Authentication;
using BniConnect.Data;
using BniConnect.Interface;
using BniConnect.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace BniConnect.Controllers
{
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public class AdminController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
      
        public AdminController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        public async Task<IActionResult> Index()
        {
            var subscription =await _subscriptionService.GetSubscriptions();
            return View(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSubscription(Subscription model)
        {
            if (model.Id > 0)
            {
                bool updateSetting = await _subscriptionService.InsertSubscription(model);
                return RedirectToAction("Index");
            }
            else
            {
                bool addSetting = await _subscriptionService.InsertSubscription(model);
                return RedirectToAction("Index");
            }

        }

        public async Task<ActionResult> DeleteSubscription(int id)
        {
            if (id > 0)
            {
                bool deleted = await _subscriptionService.DeleteSubscription(id);
                if (deleted)
                {
                    return RedirectToAction("Index");

                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            HttpContext.Session.Clear();


            Request.Headers.Clear();

            Response.Headers.Clear();

            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());


            return RedirectToAction("Login", "Account"); 
        }
    }
}

