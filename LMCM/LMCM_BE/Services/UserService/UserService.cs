using Azure.Core;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Repositories.UserRepositoriy;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserLoginResponseDto> Login(GoogleLoginRequest request)
        {
            return await _userRepository.Login(request);
        }

        public async Task<PagedResult<ListUserResponseDto>> GetListUser(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _userRepository.GetListUser(searchKey, pageIndex, pageSize);
        }

        public async Task<bool> CreateStaff(StaffRequest request)
        {
            return await _userRepository.CreateStaff(request);
        }

        public async Task<UserProfileResponseDto> GetProfile(string userId)
        {
            return await _userRepository.GetProfile(userId);
        }

        public async Task<UserProfileResponseDto> GetProfileFromCookie()
        {
            return await _userRepository.GetProfileFromCookie();
        }
        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            return await  _userRepository.AssignRoleAsync(userId, role);
        }
    }
}
