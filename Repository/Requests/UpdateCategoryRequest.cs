using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Requests
{
    public class UpdateCategoryRequest
    {
        [Required(ErrorMessage = "Category ID is required")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string CategoryDescription { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}
