using IBTSS.Repository.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.DTO.Response.User
{
    public class AddUserResponse
    {
        public string UserId { get; set; } = string.Empty; //

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsDelete { get; set; }

    }
}
