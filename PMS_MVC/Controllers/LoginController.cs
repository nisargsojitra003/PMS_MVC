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

        public IActionResult Login()
        {
            return View();
        }

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
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(apiResponse);
                    actionName = responseObject.action;
                    controllerName = responseObject.controller;
                    jwtToken = responseObject.jwtToken;
                    role = responseObject.role;
                    userId = responseObject.userId;
                    Response.Cookies.Append("jwt",jwtToken);

                    HttpContext.Session.SetString("jwtToken", jwtToken);
                    HttpContext.Session.SetInt32("userId", userId);

                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, userInfo.Email));
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    var principal = new ClaimsPrincipal(identity);
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

        public IActionResult CreateAccount()
        {
            return View();
        }

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
    }
}
