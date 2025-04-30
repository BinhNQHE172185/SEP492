using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.BudgetPropasalService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.BudgetPropasalControllers
{
    [Route("api/budgetProposal")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BudgetProposalController : Controller
    {
        private readonly IBudgetProposalService _budgetProposalService;

        public BudgetProposalController(IBudgetProposalService budgetProposalService)
        {
            _budgetProposalService = budgetProposalService; 
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
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
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
        [HttpPost("getBudgetProposalNoPagingList")]
        public async Task<IActionResult> GetBudgetProposalsNoPagingAsync(string? searchKey)
        {
            try
            {
                var data = await _budgetProposalService.GetBudgetProposalsAsync(searchKey);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
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
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
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
        [HttpPost("createBudgetProposal")]
        public async Task<IActionResult> CreateBudgetProposalAsync([FromForm] BudgetProposalInsertDto proposalDto)
        {
            try
            {
                // Check if ModelState is valid (DTO validation will catch issues)
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
                var propasal = await _budgetProposalService.CreateBudgetProposalAsync(proposalDto);
                return Ok(new
                {
                    Success = true,
                    Message = "Tờ trình đã được tạo thành công."
                });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
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
        [HttpPut("updateBudgetProposal/{id}")]
        public async Task<IActionResult> UpdateBudgetProposalAsync(Guid id, [FromForm] BudgetProposalUpdateDto newPropasal)
        {
            try
            {
                // Check if ModelState is valid (DTO validation will catch issues)
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
                bool isSuccess= await _budgetProposalService.UpdateBudgetProposalAsync(id, newPropasal);
                if (isSuccess)
                    return Ok(new
                    {
                        message = "Update tờ trình thành công.",
                    });
                else
                {
                    return NotFound(new { message = "Update tờ trình thất bại." });
                }
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
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
        [HttpDelete("deleteBudgetProposal/{proposalId}")]
        public async Task<IActionResult> DeleteBudgetProposalASync(Guid proposalId)
        {
            try
            {
                var result = await _budgetProposalService.SoftDeleteBudgetProposalAsync(proposalId);
                return result ? Ok(new { message = "Xóa thành công." }) : NotFound(new { message = "Không tìm thấy ." });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
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
