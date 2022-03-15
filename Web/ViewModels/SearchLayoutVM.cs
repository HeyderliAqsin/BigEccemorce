using Entities;

namespace Web.ViewModels
{
    public class SearchLayoutVM
    {
        public int? categoryId { get; set; }
        public List<Category> Categories { get; set; }
        public string? SearchTerm { get; set; }
    }
}
