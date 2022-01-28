using Microsoft.AspNetCore.Identity;

namespace HealthCareManager.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string UserType { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }

    public static class UserTypes
    {
        public const string Admin = nameof(Admin);
        
        public const string Doctor = nameof(Doctor);
        
        public const string Pharmacist = nameof(Pharmacist);
    }
}