using LMCM_BE.DTOs.ContractValueItemDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.ContractValueItemService;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Shared.Constant;
using LMCM_BE.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LMCM_BE
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {

            await SeedRole(DefaultUsers.Roles, serviceProvider);

            await SeedUserAccount(DefaultUsers.HeadOfDepartmentEmails, serviceProvider);

            await SeedContractValueItems(ContractValueItemSeedData.Items, serviceProvider);
        }
        private static async Task SeedRole(List<string> roles, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            // Create roles if they do not exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                }
            }
        }
        private static async Task SeedContractValueItems(List<ContractValueItemDto> items, IServiceProvider serviceProvider)
        {
            var contractValueItemService = serviceProvider.GetRequiredService<IContractValueItemService>();

            var existingItems = await contractValueItemService.GetListAsync();
            if (existingItems.Any()) return; // Nếu đã có dữ liệu thì không seed nữa

            await contractValueItemService.UpdateAsync(items);
        }
        private static async Task SeedUserAccount(List<string> emails, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var googleDriveService = serviceProvider.GetRequiredService<IGoogleDriveService>();
            foreach (var email in emails)
            {

                // Create default Head of Department user
                string hodEmail = email;
                var hodUser = await userManager.FindByEmailAsync(hodEmail);

                if (hodUser == null)
                {
                    var newHOD = new User
                    {
                        UserName = hodEmail,
                        Email = hodEmail,
                        Status = UserStatus.Active,
                    };

                    var result = await userManager.CreateAsync(newHOD);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newHOD, "Head of Department");

                        // Share Google Drive folders with the Head of Department
                        bool isShared = await googleDriveService.ShareFoldersWithHeadOfDepartmentAsync(hodEmail, "reader");

                        if (!isShared)
                        {
                            Console.WriteLine("Failed to share Google Drive folder with user.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to create Head of Department user: " + string.Join(", ", result.Errors));
                    }
                }
            }
        }
    }
}
