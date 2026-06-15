using AdvancedVotingSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace AdvancedVotingSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Seed Roles
            var roles = new[] { "Admin", "Supervisor", "CommitteeHead" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Default Admin
            var adminNationalId = "admin";
            if (await userManager.FindByNameAsync(adminNationalId) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminNationalId,
                    NationalId = adminNationalId,
                    FullName = "مدير النظام",
                    Surname = "الرئيسي",
                    Gender = "ذكر",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "admin"); // Relaxed password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
