using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS_MVC.Models;
using System.Text;
using System.Web;

namespace PMS_MVC.Controllers
{
    public class ProductController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44390");
        private readonly HttpClient client;
        public ProductController()
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }
        public IActionResult list()
        {
            return View();
        }

        public async Task<ActionResult<AddProduct>> ProductShared(SearchFilter searchFilter)
        {
            searchFilter.productPageNumber = HttpContext.Session.GetString("pageNumber") ?? "1";
            searchFilter.productPageSize = HttpContext.Session.GetString("pageSize") ?? "5";
            int totalRecords = 0;

            List<AddProduct> productList = new List<AddProduct>();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["searchProduct"] = searchFilter.searchProduct;
            query["searchCategoryTag"] = searchFilter.searchCategoryTag;
            query["searchDescription"] = searchFilter.searchDescription;
            query["productPageNumber"] = searchFilter.productPageNumber;
            query["productPageSize"] = searchFilter.productPageSize;

            string queryString = query.ToString();

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "product/getallproducts?" + queryString);

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(apiResponse);
                productList = responseObject.productList.ToObject<List<AddProduct>>();
                totalRecords = responseObject.totalProducts;
            }

            var totalPages = (int)Math.Ceiling((double)totalRecords / int.Parse(searchFilter.productPageSize));
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = int.Parse(searchFilter.productPageNumber);
            //PagedList<AddProduct> products = new PagedList<AddProduct>(productList, productList.Count(), int.Parse(productPageNumber), int.Parse(productPageSize));

            return PartialView("ProductShared", productList);
        }






        public async Task<IActionResult> Add()
        {
            AddProduct addProduct = new AddProduct();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "product/getaddcategorylist").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                addProduct = JsonConvert.DeserializeObject<AddProduct>(data);
            }
            return View(addProduct);
        }

        [HttpPost]
        public async Task<ActionResult> AddProductTodb([FromForm] AddProduct addProduct)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    // Add form data
                    content.Add(new StringContent(addProduct.ProductName), nameof(addProduct.ProductName));
                    content.Add(new StringContent(addProduct.Description), nameof(addProduct.Description));
                    content.Add(new StringContent(addProduct.Price.ToString()), nameof(addProduct.Price));
                    content.Add(new StringContent(addProduct.CategoryTag), nameof(addProduct.CategoryTag));
                    content.Add(new StringContent(addProduct.CategoryId.ToString()), nameof(addProduct.CategoryId));

                    // Add the file content
                    if (addProduct.Fileupload != null)
                    {
                        var fileContent = new StreamContent(addProduct.Fileupload.OpenReadStream());
                        fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                        {
                            Name = nameof(addProduct.Fileupload),
                            FileName = addProduct.Fileupload.FileName
                        };
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(addProduct.Fileupload.ContentType);
                        content.Add(fileContent);
                    }
                    // Post the data
                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress + "product/create", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Product has been saved successfully";
                        return RedirectToAction("list");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        TempData["error"] = "Product name or category is already exist!";
                        return RedirectToAction("list");
                    }
                    else
                    {
                        TempData["error"] = "An unexpected error occurred. Please try again later.";
                        return View("add", addProduct);
                    }

                }
                //return View("add", addProduct);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            EditProduct editProduct = new EditProduct();

            try
            {
                // Get product details
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "product/getproduct/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    editProduct = JsonConvert.DeserializeObject<EditProduct>(data);

                    if (editProduct == null)
                    {
                        // Handle null deserialization
                        return StatusCode(500, "Error deserializing product data.");
                    }
                }
                else
                {
                    // Handle unsuccessful status code
                    return StatusCode((int)response.StatusCode, "Error retrieving product data.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return View(editProduct);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            EditProduct editProduct = new EditProduct();

            try
            {
                // Get product details
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "product/getproduct/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    editProduct = JsonConvert.DeserializeObject<EditProduct>(data);

                    if (editProduct == null)
                    {
                        // Handle null deserialization
                        return StatusCode(500, "Error deserializing product data.");
                    }
                }
                else
                {
                    // Handle unsuccessful status code
                    return StatusCode((int)response.StatusCode, "Error retrieving product data.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return View(editProduct);
        }

        [HttpPost]
        public async Task<ActionResult> Edit([FromForm] EditProduct editProduct, int id, string TagRadios)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(editProduct.ProductId.ToString()), nameof(editProduct.ProductId));
                    content.Add(new StringContent(editProduct.ProductName ?? ""), nameof(editProduct.ProductName));
                    content.Add(new StringContent(editProduct.CategoryId.ToString()), nameof(editProduct.CategoryId));
                    content.Add(new StringContent(TagRadios ?? ""), nameof(editProduct.CategoryTag));
                    content.Add(new StringContent(editProduct.Description ?? ""), nameof(editProduct.Description));
                    content.Add(new StringContent(editProduct.Price.ToString()), nameof(editProduct.Price));

                    if (editProduct.Fileupload != null)
                    {
                        var fileStream = editProduct.Fileupload.OpenReadStream();
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(editProduct.Fileupload.ContentType);
                        content.Add(fileContent, nameof(editProduct.Fileupload), editProduct.Fileupload.FileName);
                    }

                    HttpResponseMessage response = await client.PutAsync(client.BaseAddress + "product/update/" + id, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Product has been saved successfully";
                        return RedirectToAction("list");
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        TempData["error"] = "Product name or code is already exist!";
                        return RedirectToAction("list");
                    }
                    else
                    {
                        HttpResponseMessage response2 = await client.GetAsync(client.BaseAddress + "product/getproduct/" + id);

                        if (response2.IsSuccessStatusCode)
                        {
                            string data = await response2.Content.ReadAsStringAsync();
                            editProduct = JsonConvert.DeserializeObject<EditProduct>(data);

                            if (editProduct == null)
                            {
                                return StatusCode(500, "Error deserializing product data.");
                            }
                        }
                        else
                        {
                            return StatusCode((int)response2.StatusCode, "Error retrieving product data.");
                        }
                        return View(editProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception details
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }

        

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Serialize the id
                string data = JsonConvert.SerializeObject(id);
                // Create StringContent
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                // Make the API call
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress + "product/delete/" + id, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Product deleted successfully!";
                    return RedirectToAction("list");
                }
                else
                {
                    // Log the status code and response content for debugging
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error deleting product. Status code: {response.StatusCode}, Response content: {responseContent}");

                    TempData["error"] = "Error deleting product!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }

        public async Task<IActionResult> RemoveProductImage(int id)
        {
            try
            {
                // Serialize the id
                string data = JsonConvert.SerializeObject(id);
                // Create StringContent
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                // Make the API call
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress + "product/deleteimage/" + id, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Product's image deleted successfully!";
                    return RedirectToAction("list");
                }
                else
                {
                    // Log the status code and response content for debugging
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error deleting product. Status code: {response.StatusCode}, Response content: {responseContent}");

                    TempData["error"] = "Error deleting product!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }

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




    }
}
