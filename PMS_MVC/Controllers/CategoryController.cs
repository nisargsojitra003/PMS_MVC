using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Text;
using System.Web;


namespace PMS_MVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HttpClient client;
        private readonly NotificationMessages NotificationMessages;
        private readonly APIUrls APIUrls;
        public CategoryController(IHttpClientFactory httpClientFactory, NotificationMessages notificationMessages, APIUrls _aPIUrls)
        {
            client = httpClientFactory.CreateClient("MyApiClient");
            NotificationMessages = notificationMessages;
            APIUrls = _aPIUrls;
        }

        #region GetAllCategories
        /// <summary>
        /// Category list method
        /// </summary>
        /// <returns>all category apply all filter and pagination</returns>
        public IActionResult list()
        {
            string token = HttpContext.Session.GetString("jwtToken") ?? "";
            if (!string.IsNullOrEmpty(token))
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "login");
            }
        }

        /// <summary>
        /// main category list's partial view.
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        public async Task<ActionResult<Category>> listshared(SearchFilter searchFilter)
        {
            searchFilter.categoryPageNumber = HttpContext.Session.GetString("catPageNumber") ?? "1";
            searchFilter.categoryPageSize = HttpContext.Session.GetString("catPageSize") ?? "5";
            searchFilter.userId = HttpContext.Session.GetInt32("userId");
            int totalRecords = 0;
            ViewBag.Currentpagesize = searchFilter.categoryPageSize;

            List<Category> categoriesList = new List<Category>();

            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);

            query["searchName"] = searchFilter.searchName;
            query["SearchCode"] = searchFilter.SearchCode;
            query["description"] = searchFilter.description;
            query["categoryPageNumber"] = searchFilter.categoryPageNumber;
            query["categoryPageSize"] = searchFilter.categoryPageSize;
            query["sortType"] = searchFilter.sortType.ToString();
            query["userId"] = searchFilter.userId.ToString();

            string queryString = query.ToString();

            string token = HttpContext.Session.GetString("jwtToken") ?? "";

            HttpResponseMessage response = new HttpResponseMessage();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            response = await client.GetAsync(client.BaseAddress + APIUrls.getallCategory + queryString);

            SharedListResponse<Category> categoryListResponse = new SharedListResponse<Category>();

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                categoryListResponse = JsonConvert.DeserializeObject<SharedListResponse<Category>>(apiResponse);
                categoriesList = categoryListResponse.List;
                totalRecords = categoryListResponse.TotalRecords;
            }

            int totalPages = (int)Math.Ceiling((double)totalRecords / int.Parse(searchFilter.categoryPageSize));
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = int.Parse(searchFilter.categoryPageNumber);

            return PartialView("listshared", categoriesList);
        }
        #endregion

        #region CreateOrEditCategory
        /// <summary>
        /// Create or Edit method of category.
        /// </summary>
        /// <param name="Id">category id</param>
        /// <param name="type">it's either create or edit</param>
        /// <returns>return view</returns>
        [HttpGet]
        public async Task<IActionResult> Save(int id, bool type)
        {
            try
            {
                if (type)
                {
                    Category category = new Category();
                    category.Id = 0;
                    return View("add", category);
                }
                else
                {
                    string token = HttpContext.Session.GetString("jwtToken") ?? "";
                    int? userId = HttpContext.Session.GetInt32("userId");
                    HttpResponseMessage response = new HttpResponseMessage();
                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    response = await client.GetAsync(client.BaseAddress + APIUrls.getCategory + id + APIUrls.userId + userId);
                    Category addCategory = new Category();
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        addCategory = JsonConvert.DeserializeObject<Category>(data);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return View("invalid");
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Error retrieving category data.");
                    }
                    return View("add", addCategory);
                }
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Post method for save category
        /// </summary>
        /// <param name="category">custom model of category</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Save(Category category)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                string token = HttpContext.Session.GetString("jwtToken") ?? "";
                category.UserId = HttpContext.Session.GetInt32("userId");

                HttpResponseMessage response = new HttpResponseMessage();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                content.Add(new StringContent(category.Name), nameof(category.Name));
                content.Add(new StringContent(category.Code), nameof(category.Code));
                content.Add(new StringContent(category.Description), nameof(category.Description));
                content.Add(new StringContent(category.UserId.ToString()), nameof(category.UserId));

                response = await client.PostAsync(client.BaseAddress + APIUrls.createCategory, content);

                //if (response.IsSuccessStatusCode)
                //{
                //    TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.savedSuccessToaster.Replace("{1}", "Category");
                //    return RedirectToAction("list");
                //}
                //else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                //{
                //    TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.categoryWarningToaster;
                //    return View("add", category);
                //}
                //else
                //{
                //    TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.systemErrorToaster;
                //    return View("add", category);
                //}

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.savedSuccessToaster.Replace("{1}", "Category");
                        return RedirectToAction("list");

                    case System.Net.HttpStatusCode.BadRequest:
                        TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.categoryWarningToaster;
                        return View("add", category);

                    default:
                        TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.systemErrorToaster;
                        return View("add", category);
                }
            }
        }

        /// <summary>
        /// Edit post method
        /// </summary>
        /// <param name="category">custom model of category</param>
        /// <param name="id">category id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Edit(Category category, int id)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                string token = HttpContext.Session.GetString("jwtToken") ?? "";
                category.UserId = HttpContext.Session.GetInt32("userId");

                HttpResponseMessage response = new HttpResponseMessage();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                content.Add(new StringContent(category.Id.ToString()), nameof(category.Id));
                content.Add(new StringContent(category.Name), nameof(category.Name));
                content.Add(new StringContent(category.Code), nameof(category.Code));
                content.Add(new StringContent(category.Description), nameof(category.Description));
                content.Add(new StringContent(category.UserId.ToString()), nameof(category.UserId));

                response = await client.PutAsync(client.BaseAddress + APIUrls.editCategory + id, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.savedSuccessToaster.Replace("{1}", "Category");
                    return RedirectToAction("list");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.categoryWarningToaster;
                    return View("add", category);
                }
                else
                {
                    TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.systemErrorToaster;
                    return View("add", category);
                }
            }
        }
        #endregion

        #region CategoryDetail
        /// <summary>
        /// Detail page of category
        /// </summary>
        /// <param name="Id">Category id</param>
        /// <returns>return category details</returns>
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            string token = HttpContext.Session.GetString("jwtToken") ?? "";
            int? userId = HttpContext.Session.GetInt32("userId");

            HttpResponseMessage response = new HttpResponseMessage();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            response = await client.GetAsync(client.BaseAddress + APIUrls.getCategory + id + APIUrls.userId + userId);

            Category addCategory = new Category();

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                addCategory = JsonConvert.DeserializeObject<Category>(data);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return View("invalid");
            }
            return View(addCategory);
        }
        #endregion

        #region DeleteProduct
        /// <summary>
        /// Soft delete category by id
        /// </summary>
        /// <param name="id">category id</param>
        /// <returns></returns>
        public async Task<JsonResult> Delete(int id)
        {
            string token = HttpContext.Session.GetString("jwtToken") ?? "";

            HttpResponseMessage response = new HttpResponseMessage();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            string data = JsonConvert.SerializeObject(id);

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            response = await client.PostAsync(client.BaseAddress + APIUrls.deleteCategory + id, content);
            if (response.IsSuccessStatusCode)
            {
                ChangePage(1);
                return Json(new { success = true });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Json(new { success = false });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        #endregion

        #region SessionVariable
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
        #endregion
    }
}