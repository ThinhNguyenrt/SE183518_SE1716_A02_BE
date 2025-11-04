using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Responses
{
    public class AccountResponse
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountEmail { get; set; }
        public int AccountRole { get; set; }
        public string RoleName { get; set; }
        public int CreatedNewsArticlesCount { get; set; }
        public int UpdatedNewsArticlesCount { get; set; }
        public bool CanDelete { get; set; }
    }
    public class AccountDetailResponseDto : AccountResponse
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
