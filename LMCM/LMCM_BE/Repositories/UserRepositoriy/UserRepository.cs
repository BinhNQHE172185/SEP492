using Google.Apis.Auth;
using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Shared.Constant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.UserRepositoriy
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly LMCM_DBContext _dbContext;

        public UserRepository(
            UserManager<User> userManager,
            LMCM_DBContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<bool> CreateStaff(string request)
        {
            var user = await _userManager.FindByEmailAsync(request);

            if (user == null)
            {
                var newStaff = new User { UserName = request, Email = request, Status = UserStatus.Pending };
                var result = await _userManager.CreateAsync(newStaff);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newStaff, "Staff");
                    return true;
                }
            }
            return false;
        }
        public async Task<User?> GetProfile(string userId)
        {
            var data = await _userManager.FindByIdAsync(userId);
            if (data != null)
            {
                return data;
            }
            return null;
        }

        public async Task<List<string>> getRoleAsync(string userId)
        {
            var data = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(data);
            return roles.ToList();
        }

        public async Task<List<User>> GetListUser(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(search) ||
                                         s.Email.ToLower().Contains(search));
            }

            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            return items;
        }

        public async Task<User> Login(GoogleJsonWebSignature.Payload payload)
        {
            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null || user.Status == UserStatus.Stopped)
            {
                return null;
            }

            if (string.IsNullOrEmpty(user.Name))
            {
                user.Name = payload.Name;
                user.Picture = payload.Picture;
                user.Status = UserStatus.Active;
                await _userManager.UpdateAsync(user);
            }
            return user;
        }

        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Người dùng không tồn tại.");
            }

            var existingRoles = await _userManager.GetRolesAsync(user);
            if (existingRoles.Contains(role))
            {
                // Already has the correct role
                return true;
            }

            if (existingRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Không thể xóa vai trò cũ: {errors}");
                }
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Không thể gán vai trò: {errorMessages}");
            }

            return true;
        }

        public async Task<bool> UpdateStatusAsync(string userId, string status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Người dùng không tồn tại.");
            }

            user.Status = (UserStatus)Convert.ToInt32(status);

            _dbContext.Users.Update(user);
            var result = await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<int> UserCountAsync()
        {
            var data = await _dbContext.Users.CountAsync();
            return data;
        }
    }
}
