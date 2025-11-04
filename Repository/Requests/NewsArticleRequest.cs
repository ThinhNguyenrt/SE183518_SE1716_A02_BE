using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Requests
{
    public class NewsArticleRequest
    {
        [Required(ErrorMessage = "News title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required")]
        [StringLength(500, ErrorMessage = "Headline cannot exceed 500 characters")]
        public string Headline { get; set; }

        [Required(ErrorMessage = "News content is required")]
        public string NewsContent { get; set; }

        [StringLength(200, ErrorMessage = "News source cannot exceed 200 characters")]
        public string NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        public bool NewsStatus { get; set; } = true;

        public List<Guid> TagIds { get; set; } = new List<Guid>();
    }
}
