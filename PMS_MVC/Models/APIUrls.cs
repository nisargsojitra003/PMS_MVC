namespace PMS_MVC.Models
{
    public class APIUrls
    {
        //dashboard controller urls:
        public string getAllActivity = "activity/getallactivity?";
        public string dashboard = "dashboard/dashboardinfo?id=";

        //login controller urls:
        public string login = "login/login";
        public string createAccount = "login/createaccount";

        //category controller urls:
        public string getallCategory = "category/getallcategories?";
        public string getCategory = "category/getcategory/";
        public string editCategory = "category/edit/";
        public string userId = "?userId=";
        public string createCategory = "category/create";
        public string deleteCategory = "category/delete/";

        //product controller urls:
        public string categoryListView = "product/getaddcategorylist?id=";
        public string getAllProducts = "product/getallproducts?";
        public string createProductCategory = "product/getaddcategorylist?id=";
        public string createProduct = "product/create";
        public string getProduct = "product/getproduct/";
        public string editProduct = "product/update/";
        public string deleteProduct = "product/delete/";
        public string deleteImage = "product/deleteimage/";
    }
}
