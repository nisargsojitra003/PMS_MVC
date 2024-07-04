namespace PMS_MVC.Models
{
    public class SearchFilter
    {
        //Category List property of Search filter.
        public string? searchName { get; set; }
        public string? SearchCode { get; set; }
        public string? description { get; set; }
        public int? sortType { get; set; } = 0;
        public string? categoryPageSize { get; set; }
        public string? categoryPageNumber { get; set; }
        public int? userId { get; set; }

        //Product List Property of Search filter.
        public string? searchProduct { get; set; }
        public string? searchCategoryTag { get; set; }
        public string? searchDescription { get; set; }
        public int? sortTypeProduct { get; set; } = 0;
        public int? searchCategory { get; set; } = 0;
        public string? productPageSize {get; set;}
        public string? productPageNumber {get; set;}

        //UserActivity list search filter property
        public string? searchActivity {  get; set; }
        public int? sortTypeActivity { get; set; } = 0;
        public string? activityPageSize { get; set;}
        public string? activityPageNumber {  get; set;}
    }
}
