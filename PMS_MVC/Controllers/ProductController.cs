﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Collections.Specialized;
using System.Net;
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

                HttpResponseMessage response = new HttpResponseMessage();
                
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
        /// <param name="searchFilter"></param> ProductShared
        /// <returns></returns>
        public async Task<ActionResult<AddProduct>> ProductListshared(SearchFilter searchFilter)
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

            HttpResponseMessage response = new HttpResponseMessage();
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            SharedListResponse<AddProduct> productListResponse = new SharedListResponse<AddProduct>();
            
            response = await client.GetAsync(client.BaseAddress + APIUrls.getAllProducts + queryString);

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                productListResponse = JsonConvert.DeserializeObject<SharedListResponse<AddProduct>>(apiResponse);
                productList = productListResponse.List;
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
            string token = HttpContext.Session.GetString("jwtToken") ?? "";
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("login", "login");
            }
            
            AddProduct addProduct = new AddProduct();

            HttpResponseMessage response = new HttpResponseMessage();
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            int? id = HttpContext.Session.GetInt32("userId");
            response = client.GetAsync(client.BaseAddress + APIUrls.createProductCategory + id).Result;

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    string data = await response.Content.ReadAsStringAsync();
                    addProduct = JsonConvert.DeserializeObject<AddProduct>(data);
                    break;

                default:
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
        public async Task<ActionResult> Add([FromForm] AddProduct addProduct, string TagRadios)
        {
            try
            {
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    addProduct.userId = HttpContext.Session.GetInt32("userId");

                    content.Add(new StringContent(addProduct.ProductName), nameof(addProduct.ProductName));
                    content.Add(new StringContent(addProduct.Description), nameof(addProduct.Description));
                    content.Add(new StringContent(addProduct.Price.ToString()), nameof(addProduct.Price));
                    
                    if (addProduct.ProductId == 0)
                    {
                        content.Add(new StringContent(addProduct.CategoryTag), nameof(addProduct.CategoryTag));
                    }
                    else
                    {
                        content.Add(new StringContent(TagRadios ?? ""), nameof(addProduct.CategoryTag));
                    }
                    
                    content.Add(new StringContent(addProduct.CategoryId.ToString()), nameof(addProduct.CategoryId));
                    content.Add(new StringContent(addProduct.userId.ToString()), nameof(addProduct.userId));
                    content.Add(new StringContent(addProduct.ProductId.ToString()), nameof(addProduct.ProductId));

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

                    if (addProduct.ProductId == 0)
                    {
                        string token = HttpContext.Session.GetString("jwtToken") ?? "";

                        HttpResponseMessage response = new HttpResponseMessage();

                        if (!string.IsNullOrEmpty(token))
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        }

                        response = await client.PostAsync(client.BaseAddress + APIUrls.createProduct, content);

                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.savedSuccessToaster.Replace("{1}", "Product");
                                return RedirectToAction("list");

                            case HttpStatusCode.BadRequest:
                                {
                                    string token1 = HttpContext.Session.GetString("jwtToken") ?? "";
                                    int? id = HttpContext.Session.GetInt32("userId");

                                    if (!string.IsNullOrEmpty(token1))
                                    {
                                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);
                                    }

                                    HttpResponseMessage response1 = await client.GetAsync(client.BaseAddress + APIUrls.createProductCategory + id);

                                    if (response1.IsSuccessStatusCode)
                                    {
                                        string data = await response1.Content.ReadAsStringAsync();
                                        addProduct = JsonConvert.DeserializeObject<AddProduct>(data);
                                        TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.productWarningToaster;
                                        return View("add", addProduct);
                                    }
                                    else
                                    {
                                        TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.systemErrorToaster;
                                        return RedirectToAction("list");
                                    }
                                }

                            default:
                                TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.systemErrorToaster;
                                return RedirectToAction("list");
                        }

                    }

                    else
                    {
                        string token = HttpContext.Session.GetString("jwtToken") ?? "";
                        
                        HttpResponseMessage response = new HttpResponseMessage();

                        if (!string.IsNullOrEmpty(token))
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        }

                        response = await client.PostAsync(client.BaseAddress + APIUrls.createProduct, content);

                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.savedSuccessToaster.Replace("{1}", "Product");
                                return RedirectToAction("list");

                            case HttpStatusCode.BadRequest:
                                {
                                    TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.productWarningToaster;
                                    int? userId = HttpContext.Session.GetInt32("userId");

                                    // Retrieve product details again to show in the edit form
                                    HttpResponseMessage getProductResponse = await client.GetAsync(client.BaseAddress + APIUrls.getProduct + addProduct.ProductId + APIUrls.userId + userId);

                                    if (getProductResponse.IsSuccessStatusCode)
                                    {
                                        string data = await getProductResponse.Content.ReadAsStringAsync();
                                        addProduct = JsonConvert.DeserializeObject<AddProduct>(data);

                                        if (addProduct == null)
                                        {
                                            return StatusCode(500, "Error deserializing product data.");
                                        }

                                        return View(addProduct);
                                    }
                                    else
                                    {
                                        return StatusCode((int)getProductResponse.StatusCode, "Error retrieving product data.");
                                    }
                                }

                            default:
                                return StatusCode((int)response.StatusCode, "Failed to update product.");
                        }

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
        public async Task<IActionResult> Get(int id)
        {
            EditProduct editProduct = new EditProduct();
            
            string token = HttpContext.Session.GetString("jwtToken") ?? "";
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("login", "login");
            }

            try
            {
                editProduct.userId = HttpContext.Session.GetInt32("userId");
                
                int? userId = HttpContext.Session.GetInt32("userId");

                NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
                
                query["id"] = id.ToString();
                query["userId"] = userId.ToString();

                string queryString = query.ToString();

                HttpResponseMessage response = new HttpResponseMessage();

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

            return View("edit",editProduct);
        }
        #endregion

        #region ProductDetails
        /// <summary>
        /// Detail page of product
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns>details of product</returns>
        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            EditProduct editProduct = new EditProduct();
            
            string token = HttpContext.Session.GetString("jwtToken") ?? "";
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("login", "login");
            }
            
            try
            {
                // Get product details
                int? userId = HttpContext.Session.GetInt32("userId");
                
                HttpResponseMessage response = new HttpResponseMessage();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                
                response = await client.GetAsync(client.BaseAddress + APIUrls.getProduct + id + APIUrls.userId + userId);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        
                        string data = await response.Content.ReadAsStringAsync();
                        editProduct = JsonConvert.DeserializeObject<EditProduct>(data);

                        if (editProduct == null)
                        {
                            return StatusCode(500, "Error deserializing product data.");
                        }
                        break;

                    case HttpStatusCode.NotFound:
                        
                        return View("invalid");

                    default:
                        return StatusCode((int)response.StatusCode, "Error retrieving product data.");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return View("detail",editProduct);
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

            HttpResponseMessage response = new HttpResponseMessage();
            
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
        public async Task<JsonResult> RemoveProductImage(int id)
        {
            try
            {
                string token = HttpContext.Session.GetString("jwtToken") ?? "";

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await client.PostAsync($"{client.BaseAddress}{APIUrls.deleteImage}{id}", null);

                string responseContent = await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        TempData[nameof(NotificationTypeEnum.success)] = NotificationMessages.deleteSuccessToaster.Replace("{1}", "Product's Image");
                        
                        return Json(new { success = true });

                    default:
                        Console.WriteLine($"Error deleting product. Status code: {response.StatusCode}, Response content: {responseContent}");
                        
                        TempData[nameof(NotificationTypeEnum.error)] = NotificationMessages.productErrorToaster;
                        
                        return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
                return Json(new { success = false });
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