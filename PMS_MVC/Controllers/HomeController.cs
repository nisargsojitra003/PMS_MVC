using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMS_MVC.Models;
using System.Diagnostics;

namespace PMS_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        Uri baseAddress = new Uri("https://localhost:44390");
        private readonly HttpClient client;
        private readonly NotificationMessages NotificationMessages;
        public HomeController(ILogger<HomeController> logger, NotificationMessages notificationMessages)
        {
            _logger = logger;
            client = new HttpClient();
            client.BaseAddress = baseAddress;
            NotificationMessages = notificationMessages;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string Token = HttpContext.Session.GetString("jwtToken") ?? "";
            if (!string.IsNullOrEmpty(Token))
            {
                TotalCount totalCount = new TotalCount();

                int? id = HttpContext.Session.GetInt32("userId");
                HttpResponseMessage response = null;
                if (!string.IsNullOrEmpty(Token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
                }
                response = await client.GetAsync(client.BaseAddress + $"home/index?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    totalCount = JsonConvert.DeserializeObject<TotalCount>(data);
                }
                return View(totalCount);
            }
            else
            {
                return RedirectToAction("login", "login");
            }
               
        }

        public  ActionResult LogOut()
        {
            //HttpResponseMessage response = null;
            //string Token = HttpContext.Session.GetString("jwtToken");
            //if (!string.IsNullOrEmpty(Token))
            //{
            //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            //}
            //response = await client.PostAsync() 
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");
            return RedirectToAction("login","login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}