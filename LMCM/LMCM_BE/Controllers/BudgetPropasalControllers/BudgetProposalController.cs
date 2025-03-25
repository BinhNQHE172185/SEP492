using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.BudgetPropasalService;
using LMCM_BE.Services.ContractorService;
using LMCM_BE.Services.ContractService;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.BudgetPropasalControllers
{
    [Route("api/budgetPropasal")]
    [ApiController]
    public class BudgetProposalController : Controller
    {
        private readonly IBudgetProposalService _budgetProposalService;
        private readonly IContractService _contractService;

        public BudgetProposalController(IBudgetProposalService budgetProposalService, IContractService contractService)
        {
            _budgetProposalService = budgetProposalService; 
            _contractService = contractService;
        }

        [HttpPost("getBudgetProposalList")]
        public async Task<IActionResult> GetBudgetProposalsAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _budgetProposalService.GetBudgetProposalsAsync(request.SearchKey, request.pageIndex, request.PageSize);
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
        [HttpGet("getBudgetProposalDetail")]
        public async Task<IActionResult> GetBudgetProposalDetailAsync(Guid proposalId)
        {
            try
            {
                var data = await _budgetProposalService.GetBudgetProposalByIdAsync(proposalId);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền xem tờ trình.",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("createBudgetProposal")]
        public async Task<IActionResult> CreateBudgetProposalAsync([FromForm] BudgetProposalInsertDto proposalDto)
        {
            try
            {
                if (proposalDto.File == null)
                {
                    return BadRequest(new { Success = false, Message = "Không tìm thấy file." });
                }
                // Check file type
                if (proposalDto.File.ContentType != "application/pdf")
                {
                    return BadRequest(new { Success = false, Message = "Chỉ file pdf mới được tải lên." });
                }

                // Check file size (5MB = 5 * 1024 * 1024 bytes)
                if (proposalDto.File.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { Success = false, Message = "Dung lượng file không được vượt quá 5MB." });
                }

                var propasal = await _budgetProposalService.CreateBudgetProposalAsync(proposalDto);
                return Ok(new
                {
                    Success = true,
                    Message = "Tờ trình đã được tạo thành công."
                });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền tạo tờ trình.",
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
                    Message = "Đã xảy ra lỗi khi tạo tờ trình. Vui lòng thử lại sau.",
                    Error = ex.Message
                });
            }
        }
        [HttpPut("updateBudgetProposal/{id}")]
        public async Task<IActionResult> UpdateBudgetProposalAsync(Guid id, [FromForm] BudgetProposalUpdateDto newPropasal)
        {
            try
            {
                Guid? propasalId = await _budgetProposalService.UpdateBudgetProposalAsync(id, newPropasal);
                if (propasalId.HasValue)
                    return Ok(new
                    {
                        message = "Update tờ trình thành công.",
                        Data = propasalId,

                    });
                else
                {
                    return NotFound(new { message = "Dữ liệu không được tìm thấy." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpDelete("deleteBudgetProposal/{proposalId}")]
        public async Task<IActionResult> DeleteBudgetProposalASync(Guid proposalId, Guid authorId)
        {
            try
            {
                if (await _contractService.HasActiveConntractsAsync(proposalId))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Không thể xóa do có hợp đồng lệ thuộc."
                    });
                }
                var result = await _budgetProposalService.SoftDeleteBudgetProposalAsync(proposalId, authorId);
                return result ? Ok(new { message = "Xóa thành công." }) : NotFound(new { message = "Không tìm thấy ." });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền xóa tờ trình.",
                    Error = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
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
