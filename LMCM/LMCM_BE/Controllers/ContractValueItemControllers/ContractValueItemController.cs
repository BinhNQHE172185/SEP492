using LMCM_BE.Models;
using LMCM_BE.Services.ContractValueItemService;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.Controllers.ContractValueItemControllers
{
    [Route("api/contract-value-items")]
    [ApiController]
    public class ContractValueItemController : ControllerBase
    {
        private readonly IContractValueItemService _contractValueItemService;

        public ContractValueItemController(IContractValueItemService contractValueItemService)
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
        public async Task<IActionResult> UpdateContractValueItems([FromBody] List<ContractValueItem> newItems)
        {
            try
            {
                if (newItems == null || newItems.Count == 0)
                {
                    return BadRequest(new { message = "Danh sách không hợp lệ hoặc trống." });
                }

                await _contractValueItemService.UpdateAsync(newItems);
                return Ok(new { message = "Cập nhật danh sách giá trị hợp đồng thành công." });
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
