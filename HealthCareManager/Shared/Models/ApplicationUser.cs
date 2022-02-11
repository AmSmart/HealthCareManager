using Microsoft.AspNetCore.Identity;

namespace HealthCareManager.Shared.Models;

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