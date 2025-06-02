
using Microsoft.AspNetCore.Identity;
using LaundryApp.Models;

namespace LaundryApp.Data
{
    public class IdentityDataInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string adminEmail = configuration["AdminUser:Email"];
            string adminPassword = configuration["AdminUser:Password"];
            // Create Admin, Operator, Guest if they don't exist
            string[] roles = new[] { "Admin", "Operator", "Guest" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            // Create Admin User if it doesn't exist
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User"
                };
                await userManager.CreateAsync(adminUser, adminPassword);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

}
