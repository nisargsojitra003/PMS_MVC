using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Web;

namespace PMS_MVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly HttpClient client;
        private readonly NotificationMessages NotificationMessages;
        private readonly APIUrls APIUrls;
        public DashboardController(ILogger<DashboardController> logger, IHttpClientFactory httpClientFactory, NotificationMessages notificationMessages, APIUrls aPIUrls)
        {
            _logger = logger;
            client = httpClientFactory.CreateClient("MyApiClient");
            NotificationMessages = notificationMessages;
            APIUrls = aPIUrls;
        }

        #region HomePage
        /// <summary>
        /// main page view(total count of category and product)
        /// </summary>
        /// <returns>Dashboard data</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string Token = HttpContext.Session.GetString("jwtToken") ?? "";
            if (!string.IsNullOrEmpty(Token))
            {
                DashboardData dashboardData = new DashboardData();

                int? id = HttpContext.Session.GetInt32("userId");
                HttpResponseMessage response = null;
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
               
                response = await client.GetAsync(client.BaseAddress + APIUrls.dashboard + id);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dashboardData = JsonConvert.DeserializeObject<DashboardData>(data);
                }
                return View(dashboardData);
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
            string token = HttpContext.Session.GetString("jwtToken")??"";

            if (!string.IsNullOrEmpty(token))
            {
                return View();
            }
            else
            {
                return RedirectToAction("login","login");
            }
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
            
            response = await client.GetAsync(client.BaseAddress + APIUrls.getAllActivity + queryString);

            SharedListResponse<UserActivity> activityListResponse = new SharedListResponse<UserActivity>();
            
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                activityListResponse = JsonConvert.DeserializeObject<SharedListResponse<UserActivity>>(apiResponse);
                activityList = activityListResponse.List;
                totalRecords = activityListResponse.TotalRecords;
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

        public IActionResult LoadModal()
        {
            return PartialView("deleteModal");
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