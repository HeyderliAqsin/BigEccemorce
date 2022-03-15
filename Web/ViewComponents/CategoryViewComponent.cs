using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Services;
using Web.ViewModels;

namespace Web.ViewComponents
{
    public class CategoryViewComponent:ViewComponent
    {
        private readonly CategoryManager _categoryManager;

        public CategoryViewComponent(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        public ViewViewComponentResult Invoke(bool IsSearch,int? categoryId) 
        {
            if (IsSearch)
            {
                SearchLayoutVM vm = new()
                {
                    categoryId = categoryId,
                };
                return View("_SearchForm",vm);
            }
            return View(_categoryManager.GetAll(c => c.IsFeatured));

        }
    }
}
