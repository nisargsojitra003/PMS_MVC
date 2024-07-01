namespace PMS_MVC.Models
{
    public class CategoryListResponse
    {
        public int TotalRecords { get; set; }
        public List<Category> CategoryList { get; set; }
    }

    public class ProductListResponse
    {
        public int TotalRecords { get; set; }
        public List<AddProduct> ProductList { get; set; }
    }
    public class ActivityListResponse
    {
        public int TotalRecords { get; set; }
        public List<UserActivity> ActivityList { get; set; }
    }
}