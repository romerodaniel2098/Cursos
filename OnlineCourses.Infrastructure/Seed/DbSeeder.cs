using Microsoft.AspNetCore.Identity;
using OnlineCourses.Infrastructure.Auth;

namespace OnlineCourses.Infrastructure.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        const string email = "test@demo.com";
        const string password = "Test123$";

        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null) return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
            throw new Exception("Seed user failed: " + errors);
        }
    }
}
