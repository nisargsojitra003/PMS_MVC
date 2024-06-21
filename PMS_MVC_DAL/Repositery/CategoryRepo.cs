using Microsoft.AspNetCore.Http;
using PMS_MVC_DAL.Interfaces;

namespace PMS_MVC_DAL.Repositery
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryRepo(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

       
    }
}
