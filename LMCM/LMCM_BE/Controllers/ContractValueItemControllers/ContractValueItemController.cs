using LMCM_BE.DTOs.ContractValueItemDtos;
using LMCM_BE.Services.ContractValueItemService;
using LMCM_BE.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.ContractValueItemControllers
{
    [Route("api/contract-value-items")]
    [ApiController]
    public class ContractValueItemController : ControllerBase
    {
        private readonly IContractValueItemService _contractValueItemService;
        private readonly IUserService _userService;

        public ContractValueItemController(IContractValueItemService contractValueItemService, IUserService userService)
        {
            _contractValueItemService = contractValueItemService;
            _userService = userService;
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
                var user = await _userService.CheckRole();
                if (user == null || !user.Contains("Head Of Deparment"))
                {
                    return Unauthorized(new { message = "Bạn không có quyền thực hiện hành động này." });
                }
                var result = await _contractValueItemService.UpdateAsync(newItems);
                return result != null ? Ok(result) : BadRequest(new { message = "Không thể cập nhật danh sách giá trị hợp đồng." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
