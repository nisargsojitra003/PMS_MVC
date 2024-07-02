namespace PMS_MVC.Models
{
    public class SharedListResponse<T>
    {
        public int TotalRecords { get; set; }
        public List<T> List { get; set; }
    }
}