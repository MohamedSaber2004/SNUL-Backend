using Microsoft.AspNetCore.Identity;
using SNUL.Shared.Enums;

namespace SNUL.Shared.Persistance.Seeding
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            try
            {
                var roleNames = Enum.GetNames<UserType>();

                foreach (var roleName in roleNames)
                {
                    if (string.IsNullOrWhiteSpace(roleName))
                        continue;

                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        var identityRole = new IdentityRole<Guid>
                        {
                            Id = Guid.NewGuid(),
                            Name = roleName,
                            NormalizedName = roleName.ToUpperInvariant()
                        };

                        await roleManager.CreateAsync(identityRole);
                    }
                }

                var obsoleteCustomerRole = await roleManager.FindByNameAsync("Customer");
                if (obsoleteCustomerRole != null)
                {
                    await roleManager.DeleteAsync(obsoleteCustomerRole);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
