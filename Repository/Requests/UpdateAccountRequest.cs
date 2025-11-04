using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Requests
{
    public class UpdateAccountRequest
    {
        [Required(ErrorMessage = "Account ID is required")]
        public Guid AccountId { get; set; }

        [Required(ErrorMessage = "Account name is required")]
        [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string AccountEmail { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Range(1, 3, ErrorMessage = "Role must be 1 (Staff), 2 (Lecturer), or 3 (Admin)")]
        public int AccountRole { get; set; }
    }
}
