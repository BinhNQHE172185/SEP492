using LMCM_BE.DTOs.ContractValueItemDtos;
using LMCM_BE.Services.ContractValueItemService;
using LMCM_BE.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.ContractValueItemControllers
{
    [Route("api/contract-value-items")]
    [ApiController]
    [Authorize(Roles ="Head of Department",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContractValueItemController : ControllerBase
    {
        private readonly IContractValueItemService _contractValueItemService;

        public ContractValueItemController(IContractValueItemService contractValueItemService, IUserService userService)
        {
            _contractValueItemService = contractValueItemService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetContractValueItems()
        {
            try
            {
                var items = await _contractValueItemService.GetListAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateContractValueItems([FromBody] List<ContractValueItemDto> newItems)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();

                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Dữ liệu đầu vào không hợp lệ.",
                        Errors = errors
                    });
                }
                var result = await _contractValueItemService.UpdateAsync(newItems);
                return result ? Ok(new { success = true }) : BadRequest(new { message = "Không thể cập nhật danh sách giá trị hợp đồng." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
