using LMCM_BE.Models;
using LMCM_BE.Services.GoogleDriveService;
using Microsoft.AspNetCore.Identity;

namespace LMCM_BE
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var googleDriveService = serviceProvider.GetRequiredService<IGoogleDriveService>();

            string[] roles = { "Admin", "Staff" };

            // Create roles if they do not exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                }
            }

            // Create default Admin user
            string adminEmail = "binhnqhe172185@fpt.edu.vn";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Status = "2"
                };

                var result = await userManager.CreateAsync(newAdmin);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");

                    // Share Google Drive folders with the new admin
                    bool isShared = await googleDriveService.ShareFoldersWithUser(adminEmail, "reader");

                    if (!isShared)
                    {
                        Console.WriteLine("Failed to share Google Drive folder with user.");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to create admin user: " + string.Join(", ", result.Errors));
                }
            }
        }
    }
}
