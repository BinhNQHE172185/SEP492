using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.BudgetPropasalService;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.BudgetPropasalControllers
{
    [Route("api/budgetPropasal")]
    [ApiController]
    public class BudgetPropasalController : Controller
    {
        private readonly IBudgetPropasalService _budgetPropasalService;

        public BudgetPropasalController(IBudgetPropasalService budgetPropasalService)
        {
            _budgetPropasalService = budgetPropasalService; 
        }

        [HttpPost("getBudgetPropasalList")]
        public async Task<IActionResult> GetBudgetPropasalsAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _budgetPropasalService.GetBudgetPropasalsAsync(request.SearchKey, request.pageIndex, request.PageSize);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("createBudgetPropasal")]
        public async Task<IActionResult> CreateBudgetPropasal([FromForm] BudgetProposalInsertDto propasalDto)
        {
            try
            {
                if (propasalDto.File == null)
                {
                    return BadRequest(new { Success = false, Message = "No file was uploaded." });
                }
                var propasal = await _budgetPropasalService.CreateBudgetPropasal(propasalDto);
                return Ok(new
                {
                    Success = true,
                    Message = "Biên bản nghiệm thu đã được tạo thành công.",
                    Data = propasal
                });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền tạo biên bản nghiệm thu.",
                    Error = ex.Message
                });
            }
            catch (ArgumentException ex) // Handle validation errors
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Dữ liệu đầu vào không hợp lệ.",
                    Error = ex.Message
                });
            }
            catch (Exception ex) // General error handling
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi tạo hợp đồng. Vui lòng thử lại sau.",
                    Error = ex.Message
                });
            }
        }
    }
}
