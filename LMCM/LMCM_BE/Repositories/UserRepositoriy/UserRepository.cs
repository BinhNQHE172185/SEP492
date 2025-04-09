using AutoMapper;
using Google.Apis.Auth;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Shared.Constant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LMCM_BE.Repositories.UserRepositoriy
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly LMCM_DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGoogleDriveService _googleDriveService;

        public UserRepository(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            LMCM_DBContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IGoogleDriveService googleDriveService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _googleDriveService = googleDriveService;
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

                    return null;
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
        public async Task<int> UserCountAsync()
        {
            var data = await _dbContext.Users.CountAsync();
            return data;
        }
    }
}
