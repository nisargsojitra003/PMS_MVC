using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Web;

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

        #region HomePage
        /// <summary>
        /// main page view(total count of category and product)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string Token = HttpContext.Session.GetString("jwtToken") ?? "";
            if (!string.IsNullOrEmpty(Token))
            {
                TotalCount totalCount = new TotalCount();

                int? id = HttpContext.Session.GetInt32("userId");
                HttpResponseMessage response = null;
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
               
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
        #endregion

        #region UserActivity
        /// <summary>
        /// User activity view.
        /// </summary>
        /// <returns></returns>
        public IActionResult UserActivity()
        {
            return View();
        }

        /// <summary>
        /// get all activity of logged in user and also apply filter and pagination
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        public async Task<ActionResult<UserActivity>> userActivityShared(SearchFilter searchFilter)
        {
            searchFilter.activityPageNumber = HttpContext.Session.GetString("activityPageNumber") ?? "1";
            searchFilter.activityPageSize = HttpContext.Session.GetString("activityPageSize") ?? "5";
            searchFilter.userId = HttpContext.Session.GetInt32("userId");
            int totalRecords = 0;
            ViewBag.Currentpagesize = searchFilter.activityPageSize;
            List<UserActivity> activityList = new List<UserActivity>();
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["activityPageNumber"] = searchFilter.activityPageNumber;
            query["activityPageSize"] = searchFilter.activityPageSize;
            query["searchActivity"] = searchFilter.searchActivity;
            query["sortTypeActivity"] = searchFilter.sortTypeActivity.ToString();
            query["userId"] = searchFilter.userId.ToString();

            string queryString = query.ToString();

            string Token = HttpContext.Session.GetString("jwtToken") ?? "";
            HttpResponseMessage response = null;
            if (!string.IsNullOrEmpty(Token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }
            response = await client.GetAsync(client.BaseAddress + "product/getallactivity?" + queryString);
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject<dynamic>(apiResponse);
                activityList = responseObject.activityList.ToObject<List<UserActivity>>();
                totalRecords = responseObject.totalActivities;
            }
            int totalPages = (int)Math.Ceiling((double)totalRecords / int.Parse(searchFilter.activityPageSize));
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = int.Parse(searchFilter.activityPageNumber);

            return PartialView("userActivityShared", activityList);
        }
        #endregion


        #region Logout
        /// <summary>
        /// Logout method.
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login", "login");
        }
        #endregion

        #region NotFound
        /// <summary>
        /// when unappropriate url than show this page.
        /// </summary>
        /// <returns></returns>
        public IActionResult NotFound()
        {
            string originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                originalPath = HttpContext.Items["originalPath"] as string;
            }
            return View();
        }
        #endregion

        #region SessionVariable
        public JsonResult ChangeActivityPage(int activityPageNumber)
        {
            if (activityPageNumber != 0)
            {
                HttpContext.Session.SetString("activityPageNumber", activityPageNumber.ToString());
            }
            return Json(new { success = true });
        }

        public JsonResult ChangeActivityPageSize(int activityPageSize)
        {
            if (activityPageSize != 0)
            {
                HttpContext.Session.SetString("activityPageSize", activityPageSize.ToString());
                ChangeActivityPage(1);
            }
            return Json(new { success = true });
        }
        #endregion

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