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
using LMCM_BE.Services.UserService;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using LMCM_BE.DTOs.ShareDtos;

namespace LMCM_BE.Controllers.UserControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
                var data = await _userService.Login(request);

                if (data == null)
                {
                    return Unauthorized(new { success = false, message = "Tài khoản chưa được đăng ký trong hệ thống. Vui lòng liên hệ Trưởng Phòng." });
                }
                else
                {
                    return Ok(new { success = true, data });

                }
            }
            catch (Exception ex)
            {
                return Unauthorized(new { success = false, message = "Đã xảy ra lỗi. Vui lòng thử lại.", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo tài khoản cho staff
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] StaffRequest request)
        {
            try
            {
                var data = await _userService.CreateStaff(request);
                if (!data)
                {
                    return BadRequest(new { success = false, message = "Tài khoản đã được đăng ký trong hệ thống." });
                }

                return Ok(new { success = true, message = "Thêm thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "ĐÃ xảy ra lỗi. Vui lòng kiểm tra lại.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get User Profile Information
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("profile")]
        public async Task<IActionResult> ProfileAsync([FromBody] string staffId)
        {
            try
            {
                var data = await _userService.GetProfile(staffId);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        /// <summary>
        /// Lấy danh sách User
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [HttpGet("list-user")]
        public async Task<IActionResult> GetListUser([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _userService.GetListUser(request.SearchKey, request.pageIndex, request.PageSize);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Data not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
