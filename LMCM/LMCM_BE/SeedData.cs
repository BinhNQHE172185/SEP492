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

            string[] roles = { "Head of Department", "Staff" };

            // Create roles if they do not exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                }
            }

            // Create default Head of Department user
            string hodEmail = "binhnqhe172185@fpt.edu.vn";
            var hodUser = await userManager.FindByEmailAsync(hodEmail);

            if (hodUser == null)
            {
                var newHOD = new User
                {
                    UserName = hodEmail,
                    Email = hodEmail,
                    Status = "2"
                };

                var result = await userManager.CreateAsync(newHOD);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newHOD, "Head of Department");

                    //// Share Google Drive folders with the new admin
                    //bool isShared = await googleDriveService.ShareFoldersWithUser(adminEmail, "reader");

                    //if (!isShared)
                    //{
                    //    Console.WriteLine("Failed to share Google Drive folder with user.");
                    //}
                }
                else
                {
                    Console.WriteLine("Failed to create Head of Department user: " + string.Join(", ", result.Errors));
                }
            }
        }
    }
}
