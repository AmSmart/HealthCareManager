using HealthCareManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Shared.DTOs
{
    public record CreateUserDTO
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must be the same")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }
    }
}
