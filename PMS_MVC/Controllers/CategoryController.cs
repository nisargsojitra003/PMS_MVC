using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Text;

namespace PMS_MVC.Controllers
{
    public class CategoryController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44390");
        private readonly HttpClient client;
        public CategoryController()
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }
        public IActionResult list()
        {
            return View();
        }

        public async Task<IActionResult> listshared()
        {
            List<Category> categoriesList = new List<Category>();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "category/getallcategories").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                categoriesList = JsonConvert.DeserializeObject<List<Category>>(data);
            }
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
                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "category/getcategory/" + Id);
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
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "category/getcategory/" + Id);
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
                content.Add(new StringContent(category.Name), nameof(category.Name));
                content.Add(new StringContent(category.Code), nameof(category.Code));
                content.Add(new StringContent(category.Description), nameof(category.Description));

                HttpResponseMessage response = await client.PostAsync(client.BaseAddress + "category/create", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Category has been saved successfully";
                    return RedirectToAction("list");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData["error"] = "Category name or code is already exist!";
                    return View("add", category);
                }
                else
                {
                    TempData["error"] = "An unexpected error occurred. Please contact your support team.";
                    return View("add", category);
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Category category, int id)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(category.Id.ToString()), nameof(category.Id));
                content.Add(new StringContent(category.Name), nameof(category.Name));
                content.Add(new StringContent(category.Code), nameof(category.Code));
                content.Add(new StringContent(category.Description), nameof(category.Description));

                HttpResponseMessage response = await client.PutAsync(client.BaseAddress + "category/edit/" + id, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Category has been saved successfully";
                    return RedirectToAction("list");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData["error"] = "Category name or code is already exist!";
                    return View("add", category);
                }
                else
                {
                    TempData["error"] = "An unexpected error occurred. Please contact your support team.";
                    return View("add", category);
                }
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            // Serialize the id
            string data = JsonConvert.SerializeObject(id);
            // Create StringContent
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            // Make the API call
            HttpResponseMessage response = await client.PostAsync(client.BaseAddress + "category/delete/" + id, content);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Product deleted successfully!";
                return RedirectToAction("list");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData["error"] = "First delete category's all product and than apply action!";
                return RedirectToAction("list");
            }
            else
            {
                TempData["error"] = "An unexpected error occurred. Please contact your support team.";
                return RedirectToAction("list");
            }
        }


    }
}
