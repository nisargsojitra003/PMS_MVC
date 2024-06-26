using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Security.Claims;

namespace PMS_MVC.Controllers
{
    public class LoginController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44390");
        private readonly HttpClient client;
        private readonly NotificationMessages NotificationMessages;
        public LoginController(NotificationMessages notificationMessages)
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
            NotificationMessages = notificationMessages;
        }

        #region LoginMethod
        /// <summary>
        /// Login page view method.
        /// </summary>
        /// <returns>if user already login than can't go to loginpage</returns>
        public IActionResult Login()
        {
            string token = HttpContext.Session.GetString("jwtToken") ?? "";
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("index", "home");
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Login post method, check user is valid or not?
        /// </summary>
        /// <param name="userInfo">custom model of login information</param>
        /// <returns>If authenticate user are there than return to home page</returns>
        [HttpPost]
        public async Task<ActionResult> Login([FromForm] Login userInfo)
        {

            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                string actionName = "";
                string controllerName = "";
                string jwtToken = "";
                string role = "";
                int userId = 0;
                content.Add(new StringContent(userInfo.Email), nameof(userInfo.Email));
                content.Add(new StringContent(userInfo.Password), nameof(userInfo.Password));

                HttpResponseMessage response = await client.PostAsync(client.BaseAddress + "login/login", content);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dynamic responseObject = JsonConvert.DeserializeObject<dynamic>(apiResponse);
                    actionName = responseObject.action;
                    controllerName = responseObject.controller;
                    jwtToken = responseObject.jwtToken;
                    role = responseObject.role;
                    userId = responseObject.userId;
                    //Response.Cookies.Append("jwt", jwtToken);
                    HttpContext.Session.SetString("email", userInfo.Email);
                    HttpContext.Session.SetString("jwtToken", jwtToken);
                    HttpContext.Session.SetInt32("userId", userId);
                    HttpContext.Session.SetString("role", role);
                    ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, userInfo.Email));
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    TempData[NotificationType.success.ToString()] = NotificationMessages.loginSuccessToaster;
                    return RedirectToAction(actionName, controllerName);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData[NotificationType.error.ToString()] = NotificationMessages.loginErrorToaster;
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    TempData[NotificationType.warning.ToString()] = NotificationMessages.loginWarningToaster;
                    return RedirectToAction("Login", "Login");
                }
            }
        }
        #endregion

        #region CreateAccount
        /// <summary>
        /// Create account view
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateAccount()
        {
            return View();
        }

        /// <summary>
        /// Check account is not exist if exist than redirect to login page.
        /// </summary>
        /// <param name="userInfo">user's information</param>
        /// <returns>by successfully register return to login method</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAccount([FromForm] Login userInfo)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {

                content.Add(new StringContent(userInfo.Email), nameof(userInfo.Email));
                content.Add(new StringContent(userInfo.Password), nameof(userInfo.Password));
                content.Add(new StringContent(userInfo.ConfirmPassword), nameof(userInfo.ConfirmPassword));

                HttpResponseMessage response = await client.PostAsync(client.BaseAddress + "login/createaccount", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData[NotificationType.success.ToString()] = NotificationMessages.accountCreatedSuccessToaster;
                    return RedirectToAction("Login", "Login");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData[NotificationType.error.ToString()] = NotificationMessages.accountCreatedErrorToaster;
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    TempData[NotificationType.error.ToString()] = NotificationMessages.systemErrorToaster;
                    return View(userInfo);
                }
            }
        }
        #endregion
    }
}