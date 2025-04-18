using IBTSS.Repository.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Entities
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsDelete { get; set; } = false;

        public virtual ICollection<Trip>? Trips { get; set; }
    }
}
