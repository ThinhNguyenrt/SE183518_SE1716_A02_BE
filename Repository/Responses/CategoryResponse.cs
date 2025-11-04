using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Responses
{
    public class CategoryResponse
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public bool IsActive { get; set; }
        public int NewsArticleCount { get; set; }
    }
    public class CategoryDetailResponseDto : CategoryResponse
    {
        public List<CategoryResponse> SubCategories { get; set; }
    }
}
