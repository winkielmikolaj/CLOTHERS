using Clothers.Constants;
using Microsoft.AspNetCore.Identity;

namespace Clothers.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (! await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(Roles.SiteUser))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.SiteUser));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Company))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Company));
            }
        }
    }
}
