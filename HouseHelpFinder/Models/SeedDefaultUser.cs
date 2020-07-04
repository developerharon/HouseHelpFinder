using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using System.Threading.Tasks;

namespace HouseHelpFinder.Models
{
    /// <summary>
    /// The class used to seed the default admin account in the database when the system is first used
    /// </summary>
    public class SeedDefaultUser
    {
        private class Authorization
        {
            public enum Roles
            {
                Administrators
            }

            public const string default_username = "admin";
            public const string default_email = "admin@example.com";
            public const string default_password = "#Secret123";
            public const Roles default_role = Roles.Administrators;
        }

        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Administrator Role
            if (!await roleManager.RoleExistsAsync(Authorization.Roles.Administrators.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.Administrators.ToString()));
            }

            // Seed the default user
            var defaultUser = new ApplicationUser { UserName = Authorization.default_username, Email = Authorization.default_email, EmailConfirmed = true, PhoneNumberConfirmed = true };

            if (!userManager.Users.Any(user => user.UserName == defaultUser.UserName))
            {
                await userManager.CreateAsync(defaultUser, Authorization.default_password);
                await userManager.AddToRoleAsync(defaultUser, Authorization.default_role.ToString());
            }
        }
    }
}
