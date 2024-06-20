﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Text;
using System.Web;

namespace PMS_MVC.Controllers
{
    public class CategoryController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44390");
        private readonly HttpClient client;
        private readonly NotificationMessages NotificationMessages;
        public CategoryController(NotificationMessages notificationMessages)
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
            NotificationMessages = notificationMessages;
        }
        public IActionResult list()
        {
            string Token = HttpContext.Session.GetString("jwtToken");
            if (!string.IsNullOrEmpty(Token))
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "login");
            }
            
        }

        public async Task<ActionResult<Category>> listshared(SearchFilter searchFilter)
        {
            searchFilter.categoryPageNumber = HttpContext.Session.GetString("catPageNumber") ?? "1";
            searchFilter.categoryPageSize = HttpContext.Session.GetString("catPageSize") ?? "5";
            searchFilter.userId = HttpContext.Session.GetInt32("userId");
            int totalRecords = 0;

            List<Category> categoriesList = new List<Category>();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["searchName"] = searchFilter.searchName;
            query["SearchCode"] = searchFilter.SearchCode;
            query["description"] = searchFilter.description;
            query["categoryPageNumber"] = searchFilter.categoryPageNumber;
            query["categoryPageSize"] = searchFilter.categoryPageSize;
            query["sortType"] = searchFilter.sortType.ToString();
            query["userId"] = searchFilter.userId.ToString();

            string queryString = query.ToString();

            string Token = HttpContext.Session.GetString("jwtToken");

            HttpResponseMessage response = null;
            if (!string.IsNullOrEmpty(Token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }

            response = await client.GetAsync(client.BaseAddress + "category/getallcategories?" + queryString);

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(apiResponse);
                categoriesList = responseObject.categoriesList.ToObject<List<Category>>();
                totalRecords = responseObject.totalRecords;
            }
            var totalPages = (int)Math.Ceiling((double)totalRecords / int.Parse(searchFilter.categoryPageSize));
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = int.Parse(searchFilter.categoryPageNumber);


            return PartialView("listshared", categoriesList);
        }

        [HttpGet]
        public async Task<IActionResult> Save(int Id, bool type)
        {
            try
            {
                if (type)
                {
                    Category addCategory = new Category();
                    addCategory.Id = 0;
                    return View("add", addCategory);
                }
                else
                {
                    string Token = HttpContext.Session.GetString("jwtToken");

                    HttpResponseMessage response = null;
                    if (!string.IsNullOrEmpty(Token))
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
                    }

                    response = await client.GetAsync(client.BaseAddress + "category/getcategory/" + Id);
                    Category addCategory = new Category();
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        addCategory = JsonConvert.DeserializeObject<Category>(data);
                    }
                    return View("add", addCategory);
                }
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int Id)
        {
            string Token = HttpContext.Session.GetString("jwtToken");

            HttpResponseMessage response = null;
            if (!string.IsNullOrEmpty(Token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }
            response = await client.GetAsync(client.BaseAddress + "category/getcategory/" + Id);
            Category addCategory = new Category();
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                addCategory = JsonConvert.DeserializeObject<Category>(data);
            }
            return View(addCategory);
        }

        [HttpPost]
        public async Task<ActionResult> Save(Category category)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                string Token = HttpContext.Session.GetString("jwtToken");
                category.UserId = HttpContext.Session.GetInt32("userId");
                HttpResponseMessage response = null;
                if (!string.IsNullOrEmpty(Token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
                }
                content.Add(new StringContent(category.Name), nameof(category.Name));
                content.Add(new StringContent(category.Code), nameof(category.Code));
                content.Add(new StringContent(category.Description), nameof(category.Description));
                content.Add(new StringContent(category.UserId.ToString()), nameof(category.UserId));

                response = await client.PostAsync(client.BaseAddress + "category/create", content);

                if (response.IsSuccessStatusCode) 
                {
                    TempData[NotificationType.success.ToString()] = NotificationMessages.deleteSuccessToaster.Replace("{1}", "Category");
                    return RedirectToAction("list");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData[NotificationType.error.ToString()] = NotificationMessages.categoryWarningToaster;
                    return View("add", category);
                }
                else
                {
                    TempData[NotificationType.error.ToString()] = NotificationMessages.systemErrorToaster;
                    return View("add", category);
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Category category, int id)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                string Token = HttpContext.Session.GetString("jwtToken");
                category.UserId = HttpContext.Session.GetInt32("userId");
                HttpResponseMessage response = null;
                if (!string.IsNullOrEmpty(Token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
                }

                content.Add(new StringContent(category.Id.ToString()), nameof(category.Id));
                content.Add(new StringContent(category.Name), nameof(category.Name));
                content.Add(new StringContent(category.Code), nameof(category.Code));
                content.Add(new StringContent(category.Description), nameof(category.Description));
                content.Add(new StringContent(category.UserId.ToString()), nameof(category.UserId));

                response = await client.PutAsync(client.BaseAddress + "category/edit/" + id, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData[NotificationType.success.ToString()] = NotificationMessages.savedSuccessToaster.Replace("{1}", "Category");
                    return RedirectToAction("list");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData[NotificationType.error.ToString()] = NotificationMessages.categoryWarningToaster;
                    return View("add", category);
                }
                else
                {
                    TempData[NotificationType.error.ToString()] = NotificationMessages.systemErrorToaster;
                    return View("add", category);
                }
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            string Token = HttpContext.Session.GetString("jwtToken");

            HttpResponseMessage response = null;
            if (!string.IsNullOrEmpty(Token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }
            string data = JsonConvert.SerializeObject(id);
            
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            response = await client.PostAsync(client.BaseAddress + "category/delete/" + id, content);
            if (response.IsSuccessStatusCode)
            {
                TempData[NotificationType.success.ToString()] = NotificationMessages.deleteSuccessToaster.Replace("{1}", "Category"); ;
                return RedirectToAction("list");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData[NotificationType.error.ToString()] = NotificationMessages.unabledeleteToaster;
                return RedirectToAction("list");
            }
            else
            {
                TempData[NotificationType.error.ToString()] = NotificationMessages.systemErrorToaster;
                return RedirectToAction("list");
            }
        }

        public JsonResult ChangePage(int pageNumberCategory)
        {
            if (pageNumberCategory != 0)
            {
                HttpContext.Session.SetString("catPageNumber", pageNumberCategory.ToString());
            }
            return Json(new { success = true });
        }

        public JsonResult ChangePageSize(int catPageSize)
        {
            if (catPageSize != 0)
            {
                HttpContext.Session.SetString("catPageSize", catPageSize.ToString());
                ChangePage(1);
            }
            return Json(new { success = true });
        }


    }
}
