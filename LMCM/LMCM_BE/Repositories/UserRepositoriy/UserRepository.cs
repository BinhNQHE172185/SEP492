using AutoMapper;
using Google.Apis.Auth;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.GoogleDriveService;
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

        public async Task<bool> CreateStaff(StaffRequest request)
        {
            var email = request.StaffId + "@fpt.edu.vn";
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var newStaff = new User { UserName = email, Email = email, Status = "1" };
                var result = await _userManager.CreateAsync(newStaff);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newStaff, "Staff");

                    // Share Google Drive folders with the new staff
                    bool isShared = await _googleDriveService.ShareFoldersWithUser(email, "reader");

                    if (!isShared)
                    {
                        Console.WriteLine("Failed to share Google Drive folder with user.");
                    }
                    return true;
                }
            }
            return false;
        }
        public async Task<UserProfileResponseDto> GetProfile(string userId)
        {
            var data = await _userManager.FindByIdAsync(userId);
            if (data != null)
            {
                var profile = _mapper.Map<UserProfileResponseDto>(data);
                var roles = await _userManager.GetRolesAsync(data);
                profile.Roles = roles.ToList();
                return profile;
            }
            return null;
        }

        public async Task<PagedResult<ListUserResponseDto>> GetListUser(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(search) ||
                                         s.Email.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var data = _mapper.Map<List<ListUserResponseDto>>(items);

            return new PagedResult<ListUserResponseDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<UserLoginResponseDto> Login(GoogleLoginRequest request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { "433474498165-m4uv6c6h9hc3ss9vk74d9v7u8t57irr5.apps.googleusercontent.com" }
            });

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null || user.Status.Equals("3"))
            {
                return null;
            }

            if (string.IsNullOrEmpty(user.Name))
            {
                user.Name = payload.Name;
                user.Picture = payload.Picture;
                user.Status = "2";
                await _userManager.UpdateAsync(user);
            }
            var token = GenerateJwtToken(user);

            return new UserLoginResponseDto
            {
                Id = user.Id,
                Token = token,
            };
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(), ClaimValueTypes.String, "utf-8"),
        new Claim(JwtRegisteredClaimNames.Email, user.Email, ClaimValueTypes.String, "utf-8"),
        new Claim(JwtRegisteredClaimNames.Name, user.Name ?? "", ClaimValueTypes.String, "utf-8"),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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

    }
}
