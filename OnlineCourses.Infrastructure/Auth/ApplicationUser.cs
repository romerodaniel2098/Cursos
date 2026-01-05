using Microsoft.AspNetCore.Identity;

namespace OnlineCourses.Infrastructure.Auth;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}