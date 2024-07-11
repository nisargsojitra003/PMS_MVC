using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace PMS_MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient client;
        private readonly NotificationMessages NotificationMessages;
        private readonly APIUrls APIUrls;
        public LoginController(IHttpClientFactory httpClientFactory, NotificationMessages notificationMessages, APIUrls aPIUrls)
        {
            client = httpClientFactory.CreateClient("MyApiClient");
            NotificationMessages = notificationMessages;
            APIUrls = aPIUrls;
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
                return RedirectToAction("index", "dashboard");
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
        public async Task<ActionResult> Login([FromForm] UserInfo userInfo)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(userInfo.Email), nameof(userInfo.Email));
                content.Add(new StringContent(userInfo.Password), nameof(userInfo.Password));

                HttpResponseMessage response = await client.PostAsync(client.BaseAddress + APIUrls.login, content);

                UserResponse userResponse = new UserResponse();

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    userResponse = JsonConvert.DeserializeObject<UserResponse>(apiResponse);

                    HttpContext.Session.SetString("email", userInfo.Email);
                    HttpContext.Session.SetString("jwtToken", userResponse.JwtToken);
                    HttpContext.Session.SetInt32("userId", userResponse.UserId);
                    HttpContext.Session.SetString("role", userResponse.UserRole);

                    ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    identity.AddClaim(new Claim(ClaimTypes.Name, userInfo.Email));
                    identity.AddClaim(new Claim(ClaimTypes.Role, userResponse.UserRole));
                    
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.loginSuccessToaster;
                    
                    return RedirectToAction(userResponse.ActionName, userResponse.ControllerName);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.loginErrorToaster;
                    
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    TempData[nameof(NotificationTypeEnum.warning)] = NotificationMessages.loginWarningToaster;
                    
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
            string token = HttpContext.Session.GetString("jwtToken") ?? "";
            
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("index", "dashboard");
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Check account is not exist if exist than redirect to login page.
        /// </summary>
        /// <param name="userInfo">user's information</param>
        /// <returns>by successfully register return to login method</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAccount([FromForm] UserInfo userInfo)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(userInfo.Email), nameof(userInfo.Email));
                content.Add(new StringContent(userInfo.Password), nameof(userInfo.Password));
                content.Add(new StringContent(userInfo.ConfirmPassword), nameof(userInfo.ConfirmPassword));

                HttpResponseMessage response = await client.PostAsync(client.BaseAddress + APIUrls.createAccount, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.accountCreatedSuccessToaster;
                    
                    return RedirectToAction("Login", "Login");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.accountCreatedErrorToaster;
                    
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.systemErrorToaster;
                    
                    return View(userInfo);
                }
            }
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult> Translate(string text, string targetLanguage)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string _accessToken = "AIzaSyCvWxXoM0g00lJ81Vjp7ifD3dNWEUQ0ldA";
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                    var translationRequest = new
                    {
                        q = text,
                        target = targetLanguage,
                    };

                    var jsonRequest = JsonConvert.SerializeObject(translationRequest);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("https://translation.googleapis.com/language/translate/v2", content);
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    return Content(jsonResponse, "application/json");
                }
            }
            catch (Exception ex)
            {
                return Content($"Translation failed: {ex.Message}");
            }
        }
    }
}