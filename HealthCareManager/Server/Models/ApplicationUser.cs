using Microsoft.AspNetCore.Identity;

namespace HealthCareManager.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserType UserType { get; set; }

        public string FullName { get; set; } = string.Empty;
    }

    public enum UserType
    {
        Admin,
        Doctor,
        Pharmacist
    }

    public record CreateUserDTO
    {
        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        
        public string ConfirmPassword { get; set; } = string.Empty;

        public string UserType { get; set; } = string.Empty;
    }
}