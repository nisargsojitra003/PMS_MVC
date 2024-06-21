using PMS_MVC_BAL.Interfaces;
using PMS_MVC_DAL.Interfaces;


namespace PMS_MVC_BAL.Services
{
    public class CategoryService : ICategory
    {
        private readonly ICategoryRepo _categoryRepository;

        public CategoryService(ICategoryRepo categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        
    }
}
