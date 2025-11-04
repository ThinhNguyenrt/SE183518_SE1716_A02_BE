using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Responses
{
    public class NewsArticleResponse
    {
        public Guid NewsArticleId { get; set; }
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public DateTime CreatedDate { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool NewsStatus { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public Guid? UpdatedById { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<TagResponseDto> Tags { get; set; } = new List<TagResponseDto>();
    }
}
