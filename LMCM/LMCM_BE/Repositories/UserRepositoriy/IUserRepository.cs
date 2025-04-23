using Google.Apis.Auth;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Shared.Constant;

namespace LMCM_BE.Repositories.UserRepositoriy
{
    public interface IUserRepository
    {
        Task<User> Login(GoogleJsonWebSignature.Payload payload);
        Task<bool> CreateStaff(string request);
        Task<User?> GetProfile(string userId);
        Task<List<User>> GetListUser(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> AssignRoleAsync(string userId, string role);
        Task<List<string>> getRoleAsync(string userId);
        Task<int> UserCountAsync();
        Task<bool> UpdateStatusAsync(string userId, string status);
        Task<int> CountUserByStatusAsync(UserStatus status);
        Task<bool> UpdateUserAsync(string userId, string email);
        Task<bool> RemoveUserAsync(string userId);
    }
}
