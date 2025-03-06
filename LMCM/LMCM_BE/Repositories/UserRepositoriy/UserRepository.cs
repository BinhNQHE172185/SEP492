using AutoMapper;
using Google.Apis.Auth;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
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
        public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IMapper mapper, LMCM_DBContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _dbContext = dbContext;
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

            if (user == null)
            {
                return null;
            }
            else if (user.Name.IsNullOrEmpty())
            {
                {
                    user.Name = payload.Name;
                    user.Picture = payload.Picture;
                    user.Status = "2";
                    await _userManager.UpdateAsync(user);
                }
            }
            var token = GenerateJwtToken(user);
            var data = new UserLoginResponseDto
            {
                Id = user.Id,
                Token = token,
            };
            return data;
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
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
    }
}
