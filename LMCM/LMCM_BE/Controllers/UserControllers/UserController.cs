using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LMCM_BE.Models;
using LMCM_BE.DTOs.UserDtos;

namespace LMCM_BE.Controllers.UserControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Login By Google
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "433474498165-m4uv6c6h9hc3ss9vk74d9v7u8t57irr5.apps.googleusercontent.com" }
                });

                var user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    return Unauthorized(new { success = false, message = "Tài khoản chưa được đăng ký trong hệ thống. Vui lòng liên hệ Trưởng Phòng." });
                }
                else if (user.Name.IsNullOrEmpty())
                {
                    {
                        user.Name = payload.Name;
                        user.Picture = payload.Picture;
                        await _userManager.UpdateAsync(user);
                    }
                }

                // Nếu tài khoản tồn tại, tạo JWT Token
                var token = GenerateJwtToken(user);
                return Ok(new { success = true, token, user });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { success = false, message = "Token Google không hợp lệ", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo tài khoản cho staff
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    var newStaff = new User { UserName = email, Email = email };
                    var result = await _userManager.CreateAsync(newStaff);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newStaff, "Staff");
                    }
                }
                else
                {
                    return Unauthorized(new { success = false, message = "Tài khoản đã được đăng ký trong hệ thống." });
                }

                return Ok(new { success = true, message = "Thêm thành công." });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { success = false, message = "ĐÃ xảy ra lỗi. Vui lòng kiểm tra lại.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get User Profile Information
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("profile")]
        public async Task<IActionResult> ProfileAsync([FromBody] string email)
        {
            try
            {
                var data = await _userManager.FindByEmailAsync(email);
                if (data != null)
                {
                    var response = new UserProfileResponseDto
                    {
                        Email = data.Email,
                        Name = data.Name,
                        Picture = data.Picture
                    };
                    return Ok(response);
                }
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
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

        public class GoogleLoginRequest
        {
            public string Token { get; set; }
        }
    }
}
