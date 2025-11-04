using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Requests
{
    public class UpdateTagRequest
    {
        [Required(ErrorMessage = "Tag ID is required")]
        public Guid TagId { get; set; }

        [Required(ErrorMessage = "Tag name is required")]
        [StringLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
        public string TagName { get; set; }

        [StringLength(200, ErrorMessage = "Note cannot exceed 200 characters")]
        public string Note { get; set; }
    }
}
