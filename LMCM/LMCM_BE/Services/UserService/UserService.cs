using AutoMapper;
using Azure.Core;
using Google.Apis.Auth;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.UserRepositoriy;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Shared.Constant;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LMCM_BE.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUserRepository userRepository,
            IGoogleDriveService googleDriveService,
            IMapper mapper,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> GetActiveUserCountAsync()
        {
            return await _userRepository.CountUserByStatusAsync(UserStatus.Stopped);
        }

        public async Task<UserLoginResponseDto> Login(GoogleLoginRequest request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { "433474498165-m4uv6c6h9hc3ss9vk74d9v7u8t57irr5.apps.googleusercontent.com" }
            });
            var user = await _userRepository.Login(payload);

            if (user == null)
            {
                return null;
            }

            var token = await GenerateJwtToken(user);

            return new UserLoginResponseDto
            {
                Id = user.Id,
                Token = token,
            };
        }

        public async Task<PagedResult<ListUserResponseDto>> GetListUser(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var users = await _userRepository.GetListUser(searchKey, pageIndex, pageSize);

            var data = _mapper.Map<List<ListUserResponseDto>>(users);

            foreach (var dto in data)
            {
                var roles = await _userRepository.getRoleAsync(dto.Id.ToString());
                dto.Roles = roles.ToList(); // Gán danh sách vai trò
            }

            return new PagedResult<ListUserResponseDto>
            {
                Items = data,
                TotalCount = users.Count(),
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<bool> CreateStaff(StaffRequest request)
        {
            var email = request.StaffId + "@fpt.edu.vn";
            if (await _userRepository.CreateStaff(email))
            {
                bool isShared = await _googleDriveService.ShareFoldersWithUserAsync(email,false, "reader");

                if (!isShared)
                {
                    Console.WriteLine("Không thể chia sẻ folder Google Drive với người dùng.");
                }
                return true;
            }
            return false;
        }

        public async Task<UserProfileResponseDto> GetProfile(string userId)
        {
            var user = await _userRepository.GetProfile(userId);
            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy người dùng.");
            var profile = _mapper.Map<UserProfileResponseDto>(user);
            var roles = await _userRepository.getRoleAsync(user.Id.ToString());
            profile.Roles = roles.ToList();
            return profile;
        }

        public async Task<UserProfileResponseDto> GetProfileFromCookie()
        {
            try
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var authToken = request.Cookies["AuthToken"];

                if (string.IsNullOrEmpty(authToken))
                {
                    return null;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

                try
                {
                    var principal = tokenHandler.ValidateToken(authToken, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    Console.WriteLine("Token validated successfully.");
                    foreach (var claim in principal.Claims)
                    {
                        Console.WriteLine($"🔹 Decoded Claim: {claim.Type} = {claim.Value}");
                    }
                    var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    Console.WriteLine($"Extracted User ID: {userId}");

                    if (string.IsNullOrEmpty(userId))
                    {
                        Console.WriteLine("User ID is empty.");
                        return null;
                    }

                    return await GetProfile(userId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Token validation error: {ex.Message}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving profile from cookie: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            var user=await _userRepository.GetProfile(userId);
            if (user == null||role==null)
            {
                return false;
            }
            var userRole = await _userRepository.getRoleAsync(userId);
            if (userRole.Contains("Staff") && role.Equals("Head of Department")){
                if (user.Email != null)
                {
                    bool isShared = await _googleDriveService.ShareFoldersWithUserAsync(user.Email, false, "reader");

                    if (isShared)
                    {
                        Console.WriteLine("Share Google Drive folder with user successfully.");
                    }
                }
            }else if (userRole.Contains("Head of Department") && role.Equals("Staff"))
            {
                if (user.Email != null){
                    bool isRevoked = await _googleDriveService.RevokePermissionFromFolderAsync(user.Email, true);

                    if (isRevoked)
                    {
                        Console.WriteLine("Revoke Google Drive folder access for user successfully.");
                    }
                }
            }
            return await _userRepository.AssignRoleAsync(userId, role);
        }
        private async Task<string> GenerateJwtToken(User user)
        {
            var roles = await _userRepository.getRoleAsync(user.Id.ToString());

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(), ClaimValueTypes.String, "utf-8"),
                new Claim(JwtRegisteredClaimNames.Email, user.Email, ClaimValueTypes.String, "utf-8"),
                new Claim(JwtRegisteredClaimNames.Name, user.Name ?? "", ClaimValueTypes.String, "utf-8"),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<int> UserCountAsync()
        {
            return await _userRepository.UserCountAsync();
        }
        public async Task<List<string>> CheckRole()
        {
            var user = await GetProfileFromCookie();
            var role = await _userRepository.getRoleAsync(user.Id.ToString());
            return role;
        }
        public async Task<bool> UpdateStatus(string userId, string status)
        {
            var currentUser = await GetProfileFromCookie();
            if (currentUser.Id.ToString() == userId)
            {
                throw new InvalidOperationException("Không thể thay đổi trạng thái của chính bạn.");
            }
            return await _userRepository.UpdateStatusAsync(userId, status);
        }
    }
}
