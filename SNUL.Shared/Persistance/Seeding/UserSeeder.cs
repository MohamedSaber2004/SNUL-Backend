using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Enums;

namespace SNUL.Shared.Persistance.Seeding
{
    public static class UserSeeder
    {
        private const string JsonResourceName = "SNUL.Shared.Persistance.Seeding.User.json";
        private const string JsonFileName = "User.json";
        private static readonly string JsonFolderName = string.Concat("Persistance", System.IO.Path.DirectorySeparatorChar, "Seeding");

        public static async Task SeedUsersAsync(
            UserManager<ApplicationUser> userManager,
            string? jsonFilePath = null)
        {
            try
            {
                var json = ReadJson(jsonFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return;
                }

                var seedUsers = DeserializeSeedUsers(json);
                if (seedUsers == null || seedUsers.Count == 0)
                {
                    return;
                }

                foreach (var seedUser in seedUsers)
                {
                    await SeedUserAsync(userManager, seedUser);
                }
            }
            catch (Exception)
            {
            }
        }

        private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, UserSeedModel seedUser)
        {
            if (string.IsNullOrWhiteSpace(seedUser.Email) || string.IsNullOrWhiteSpace(seedUser.Password))
            {
                return;
            }

            var existingUser = await userManager.FindByEmailAsync(seedUser.Email);
            if (existingUser != null)
            {
                return;
            }

            var user = new ApplicationUser
            {
                FullName = seedUser.FullName,
                Email = seedUser.Email,
                UserName = string.IsNullOrWhiteSpace(seedUser.UserName) ? seedUser.Email : seedUser.UserName,
                Language = AppLanguageExtensions.FromCode(seedUser.Language),
                UserType = ResolveUserType(seedUser.Roles),
                EmailConfirmed = true,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(user, seedUser.Password);
            if (!createResult.Succeeded)
            {
                return;
            }

            if (seedUser.Roles != null && seedUser.Roles.Count > 0)
            {
                await userManager.AddToRolesAsync(user, seedUser.Roles);
            }
        }

        private static UserType ResolveUserType(IReadOnlyList<string>? roles)
        {
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (Enum.TryParse<UserType>(role, ignoreCase: true, out var userType))
                        return userType;
                }
            }

            return UserType.OrganizationUser;
        }

        private static string? ReadJson(string? jsonFilePath)
        {
            if (!string.IsNullOrWhiteSpace(jsonFilePath) && File.Exists(jsonFilePath))
            {
                return File.ReadAllText(jsonFilePath);
            }

            var assembly = typeof(UserSeeder).Assembly;

            using var stream = assembly.GetManifestResourceStream(JsonResourceName);
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }

            var possiblePaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, JsonFolderName, JsonFileName),
                Path.Combine(Directory.GetCurrentDirectory(), JsonFolderName, JsonFileName)
            };

            var filePath = possiblePaths.FirstOrDefault(File.Exists);
            return filePath != null ? File.ReadAllText(filePath) : null;
        }

        private static List<UserSeedModel>? DeserializeSeedUsers(string json)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                return JsonSerializer.Deserialize<List<UserSeedModel>>(json, options);
            }

            var single = JsonSerializer.Deserialize<UserSeedModel>(json, options);
            return single != null ? new List<UserSeedModel> { single } : null;
        }
    }
}
