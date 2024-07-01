namespace PMS_MVC.Models
{
    public class APIUrls
    {
        //dashboard controller urls:
        public string getAllActivity = "activity/list?";
        public string dashboard = "dashboard/dashboardinfo?id=";

        //login controller urls:
        public string login = "login/login";
        public string createAccount = "login/createaccount";

        //category controller urls:
        public string getallCategory = "category/list?";
        public string getCategory = "category/get/";
        public string editCategory = "category/edit/";
        public string userId = "?userId=";
        public string createCategory = "category/create";
        public string deleteCategory = "category/delete/";

        //product controller urls:
        public string categoryListView = "product/getaddcategorylist?id=";
        public string getAllProducts = "product/list?";
        public string createProductCategory = "product/getaddcategorylist?id=";
        public string createProduct = "product/create";
        public string getProduct = "product/get/";
        public string editProduct = "product/update/";
        public string deleteProduct = "product/delete/";
        public string deleteImage = "product/deleteimage/";
    }
}