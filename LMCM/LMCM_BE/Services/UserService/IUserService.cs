using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;

namespace LMCM_BE.Services.UserService
{
    public interface IUserService
    {
        Task<UserLoginResponseDto> Login(GoogleLoginRequest request);
        Task<bool> CreateStaff(StaffRequest request);
        Task<UserProfileResponseDto> GetProfile(string userId);
        Task<PagedResult<ListUserResponseDto>> GetListUser(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<UserProfileResponseDto> GetProfileFromCookie();
        Task<bool> AssignRoleAsync(string userId, string role);
    }
}
