using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Text;

namespace PMS_MVC.Controllers
{
    public class LoginController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44390");
        private readonly HttpClient client;
        public LoginController() 
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
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
                    Response.Cookies.Append("jwt",jwtToken);
                    TempData["success"] = "User logged in sucessfully!";
                    return RedirectToAction(actionName, controllerName);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData["error"] = "Email or password is incorrect!";
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    TempData["error"] = "First create account and than login.";
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
                    TempData["success"] = "Account created successfully!";
                    return RedirectToAction("Login", "Login");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData["error"] = "Email is already exist!";
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    TempData["error"] = "Something went wrong!";
                    return View(userInfo);
                }
            }
        }
    }
}
