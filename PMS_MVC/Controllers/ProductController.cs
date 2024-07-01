using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace PMS_MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient client;
        private readonly NotificationMessages NotificationMessages;
        private readonly APIUrls APIUrls;
        public ProductController(IHttpClientFactory httpClientFactory, NotificationMessages notificationMessages, APIUrls aPIUrls)
        {
            client = httpClientFactory.CreateClient("MyApiClient");
            NotificationMessages = notificationMessages;
            APIUrls = aPIUrls;
        }

        #region GetAllProducts
        /// <summary>
        /// Product list method
        /// </summary>
        /// <returns>all Product apply all filter and pagination</returns>
        public async Task<IActionResult> list()
        {
            string token = HttpContext.Session.GetString("jwtToken") ?? "";
            if (!string.IsNullOrEmpty(token))
            {
                AddProduct addProduct = new AddProduct();

                HttpResponseMessage response = null;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                int? id = HttpContext.Session.GetInt32("userId");
                response = client.GetAsync(client.BaseAddress + APIUrls.categoryListView + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    addProduct = JsonConvert.DeserializeObject<AddProduct>(data);
                }
                return View(addProduct);
            }
            else
            {
                return RedirectToAction("login", "login");
            }
        }

        /// <summary>
        /// main Product list's partial view.
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        public async Task<ActionResult<AddProduct>> ProductShared(SearchFilter searchFilter)
        {
            searchFilter.productPageNumber = HttpContext.Session.GetString("pageNumber") ?? "1";
            searchFilter.productPageSize = HttpContext.Session.GetString("pageSize") ?? "5";
            searchFilter.userId = HttpContext.Session.GetInt32("userId");
            int totalRecords = 0;
            ViewBag.Currentpagesize = searchFilter.productPageSize;

            List<AddProduct> productList = new List<AddProduct>();
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["searchProduct"] = searchFilter.searchProduct;
            query["searchCategoryTag"] = searchFilter.searchCategoryTag;
            query["searchDescription"] = searchFilter.searchDescription;
            query["productPageNumber"] = searchFilter.productPageNumber;
            query["productPageSize"] = searchFilter.productPageSize;
            query["userId"] = searchFilter.userId.ToString();
            query["sortTypeProduct"] = searchFilter.sortTypeProduct.ToString();
            query["searchCategory"] = searchFilter.searchCategory.ToString();

            string queryString = query.ToString();

            string token = HttpContext.Session.GetString("jwtToken") ?? "";

            HttpResponseMessage response = null;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            ProductListResponse productListResponse = new ProductListResponse();
            response = await client.GetAsync(client.BaseAddress + APIUrls.getAllProducts + queryString);

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                productListResponse = JsonConvert.DeserializeObject<ProductListResponse>(apiResponse);
                productList = productListResponse.ProductList;
                totalRecords = productListResponse.TotalRecords;
            }

            int totalPages = (int)Math.Ceiling((double)totalRecords / int.Parse(searchFilter.productPageSize));
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = int.Parse(searchFilter.productPageNumber);

            return PartialView("ProductShared", productList);
        }
        #endregion




        #region AddProduct
        /// <summary>
        /// Add product view
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Add()
        {
            AddProduct addProduct = new AddProduct();
            string token = HttpContext.Session.GetString("jwtToken") ?? "";

            HttpResponseMessage response = null;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            int? id = HttpContext.Session.GetInt32("userId");
            response = client.GetAsync(client.BaseAddress + APIUrls.createProductCategory + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                addProduct = JsonConvert.DeserializeObject<AddProduct>(data);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error retrieving product data.");
            }
            return View(addProduct);
        }

        /// <summary>
        /// add product post method
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Add([FromForm] AddProduct addProduct)
        {
            try
            {
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    addProduct.userId = HttpContext.Session.GetInt32("userId");

                    content.Add(new StringContent(addProduct.ProductName), nameof(addProduct.ProductName));
                    content.Add(new StringContent(addProduct.Description), nameof(addProduct.Description));
                    content.Add(new StringContent(addProduct.Price.ToString()), nameof(addProduct.Price));
                    content.Add(new StringContent(addProduct.CategoryTag), nameof(addProduct.CategoryTag));
                    content.Add(new StringContent(addProduct.CategoryId.ToString()), nameof(addProduct.CategoryId));
                    content.Add(new StringContent(addProduct.userId.ToString()), nameof(addProduct.userId));

                    if (addProduct.Fileupload != null)
                    {
                        StreamContent fileContent = new StreamContent(addProduct.Fileupload.OpenReadStream());

                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = nameof(addProduct.Fileupload),
                            FileName = addProduct.Fileupload.FileName
                        };

                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(addProduct.Fileupload.ContentType);
                        content.Add(fileContent);
                    }

                    string token = HttpContext.Session.GetString("jwtToken") ?? "";

                    HttpResponseMessage response = null;

                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    response = await client.PostAsync(client.BaseAddress + APIUrls.createProduct, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.savedSuccessToaster.Replace("{1}", "Product"); ;
                        return RedirectToAction("list");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string token1 = HttpContext.Session.GetString("jwtToken") ?? "";

                        HttpResponseMessage response1 = null;
                        if (!string.IsNullOrEmpty(token1))
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);
                        }
                        int? id = HttpContext.Session.GetInt32("userId");
                        response1 = client.GetAsync(client.BaseAddress + APIUrls.createProductCategory + id).Result;

                        if (response1.IsSuccessStatusCode)
                        {
                            string data = await response1.Content.ReadAsStringAsync();
                            addProduct = JsonConvert.DeserializeObject<AddProduct>(data);
                        }
                        TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.productWarningToaster;
                        return View("add", addProduct);
                    }
                    else
                    {
                        TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.systemErrorToaster;
                        return RedirectToAction("list");
                    }

                }

            }
            catch
            {
                return NotFound();
            }
        }
        #endregion

        #region EditProduct
        /// <summary>
        /// Edit get method
        /// </summary>
        /// <param name="id"></param>
        /// <returns>edit product's info.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            EditProduct editProduct = new EditProduct();

            try
            {
                string token = HttpContext.Session.GetString("jwtToken") ?? "";
                editProduct.userId = HttpContext.Session.GetInt32("userId");
                int? userId = HttpContext.Session.GetInt32("userId");

                NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
                query["id"] = id.ToString();
                query["userId"] = userId.ToString();

                string queryString = query.ToString();

                HttpResponseMessage response = null;

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                response = await client.GetAsync(client.BaseAddress + APIUrls.getProduct + id + APIUrls.userId + userId);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    editProduct = JsonConvert.DeserializeObject<EditProduct>(data);

                    if (editProduct == null)
                    {
                        return StatusCode(500, "Error deserializing product data.");
                    }
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return View("invalid");
                }

                else
                {
                    return StatusCode((int)response.StatusCode, "Error retrieving product data.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return View(editProduct);
        }

        /// <summary>
        /// edit product post method
        /// </summary>
        /// <param name="editProduct">custom model of product</param>
        /// <param name="id">product id</param>
        /// <param name="TagRadios"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Edit([FromForm] EditProduct editProduct, int id, string TagRadios)
        {
            try
            {
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    editProduct.userId = HttpContext.Session.GetInt32("userId");
                    content.Add(new StringContent(editProduct.ProductId.ToString()), nameof(editProduct.ProductId));
                    content.Add(new StringContent(editProduct.ProductName ?? ""), nameof(editProduct.ProductName));
                    content.Add(new StringContent(editProduct.CategoryId.ToString()), nameof(editProduct.CategoryId));
                    content.Add(new StringContent(TagRadios ?? ""), nameof(editProduct.CategoryTag));
                    content.Add(new StringContent(editProduct.Description ?? ""), nameof(editProduct.Description));
                    content.Add(new StringContent(editProduct.Price.ToString()), nameof(editProduct.Price));
                    content.Add(new StringContent(editProduct.userId.ToString()), nameof(editProduct.userId));

                    if (editProduct.Fileupload != null)
                    {
                        Stream fileStream = editProduct.Fileupload.OpenReadStream();
                        StreamContent fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(editProduct.Fileupload.ContentType);
                        content.Add(fileContent, nameof(editProduct.Fileupload), editProduct.Fileupload.FileName);
                    }

                    string token = HttpContext.Session.GetString("jwtToken") ?? "";
                    HttpResponseMessage response = null;

                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    response = await client.PutAsync(client.BaseAddress + APIUrls.editProduct + id, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.savedSuccessToaster.Replace("{1}", "Product");
                        return RedirectToAction("list");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.productWarningToaster;
                        int? userId = HttpContext.Session.GetInt32("userId");
                        // Retrieve product details again to show in the edit form
                        HttpResponseMessage getProductResponse = await client.GetAsync(client.BaseAddress + APIUrls.getProduct + id + APIUrls.userId + userId);

                        if (getProductResponse.IsSuccessStatusCode)
                        {
                            string data = await getProductResponse.Content.ReadAsStringAsync();
                            editProduct = JsonConvert.DeserializeObject<EditProduct>(data);

                            if (editProduct == null)
                            {
                                return StatusCode(500, "Error deserializing product data.");
                            }

                            return View(editProduct);
                        }
                        else
                        {
                            return StatusCode((int)getProductResponse.StatusCode, "Error retrieving product data.");
                        }
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Failed to update product.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }
        #endregion

        #region ProductDetails
        /// <summary>
        /// Detail page of product
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns>details of product</returns>
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            EditProduct editProduct = new EditProduct();

            try
            {
                // Get product details
                string token = HttpContext.Session.GetString("jwtToken") ?? "";
                int? userId = HttpContext.Session.GetInt32("userId");
                HttpResponseMessage response = null;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                response = await client.GetAsync(client.BaseAddress + APIUrls.getProduct + id + APIUrls.userId + userId);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    editProduct = JsonConvert.DeserializeObject<EditProduct>(data);

                    if (editProduct == null)
                    {
                        return StatusCode(500, "Error deserializing product data.");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return View("invalid");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error retrieving product data.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return View(editProduct);
        }
        #endregion

        #region DeleteProduct
        /// <summary>
        /// soft delete method of product
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns></returns>
        public async Task<JsonResult> Delete(int id)
        {
            string data = JsonConvert.SerializeObject(id);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            string token = HttpContext.Session.GetString("jwtToken") ?? "";

            HttpResponseMessage response = null;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            response = await client.PostAsync(client.BaseAddress + APIUrls.deleteProduct + id, content);

            if (response.IsSuccessStatusCode)
            {
                ChangePage(1);
                return Json(new { success = true });
            }
            else
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error deleting product. Status code: {response.StatusCode}, Response content: {responseContent}");
                return Json(new { success = false });
            }
        }
        #endregion

        #region RemoveProductImage
        /// <summary>
        /// remove product image by productid
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns></returns>
        public async Task<IActionResult> RemoveProductImage(int id)
        {
            try
            {
                string data = JsonConvert.SerializeObject(id);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                string token = HttpContext.Session.GetString("jwtToken") ?? "";

                HttpResponseMessage response = null;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                response = await client.PostAsync(client.BaseAddress + APIUrls.deleteImage + id, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.deleteSuccessToaster.Replace("{1}", "Product's Image");
                    return RedirectToAction("list");
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error deleting product. Status code: {response.StatusCode}, Response content: {responseContent}");

                    TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.productErrorToaster;
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }
        #endregion



        #region SessionVariable
        public JsonResult ChangePage(int productPageNumber)
        {
            if (productPageNumber != 0)
            {
                HttpContext.Session.SetString("pageNumber", productPageNumber.ToString());
            }
            return Json(new { success = true });
        }

        public JsonResult ChangePageSize(int pageSizeProduct)
        {
            if (pageSizeProduct != 0)
            {
                HttpContext.Session.SetString("pageSize", pageSizeProduct.ToString());
                ChangePage(1);
            }
            return Json(new { success = true });
        }
        #endregion
    }
}