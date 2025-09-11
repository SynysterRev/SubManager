using Microsoft.AspNetCore.Identity;
using SubManager.Domain.IdentityEntities;

namespace SubManager.API.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            var userRoles = new [] { "Admin", "User" };
            foreach (var role in userRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole(role));
                }
            }
        }
    }
}
