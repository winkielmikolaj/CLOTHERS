using Clothers.Constants;
using Microsoft.AspNetCore.Identity;

namespace Clothers.Data
{
    public class UserSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await CreateUserWithRole(userManager, "admin@clothers.com", "Admin123!", Roles.Admin);

            await CreateUserWithRole(userManager, "user@clothers.com", "User123!", Roles.SiteUser);

            await CreateUserWithRole(userManager, "company@clothers.com", "Company123!", Roles.Company);
        }

        private static async Task CreateUserWithRole(UserManager<IdentityUser> userManager,
            string email,
            string password,
            string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new Exception($"Błąd przy tworzeniu użytkownika przy użyciu emaila {user.Email}. Błąd: {string.Join(",", result.Errors)}");
                }
            }
        }
    }
}
