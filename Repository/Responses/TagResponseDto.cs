using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Responses
{
    public class TagResponseDto
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; }
        public string Note { get; set; }
    }
}
