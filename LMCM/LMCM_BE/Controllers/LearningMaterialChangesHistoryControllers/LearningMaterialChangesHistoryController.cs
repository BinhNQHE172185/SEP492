using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.ContractService;
using LMCM_BE.Services.LearningMaterialChangesHistoryService;
using LMCM_BE.Services.LearningMaterialService;
using LMCM_BE.Services.SyllabusService;
using LMCM_BE.Services.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.Controllers.LearningMaterialChangesHistoryControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningMaterialChangesHistoryController : ControllerBase
    {
        private readonly ILearningMaterialChangesHistorySerivce _changesService;
        private readonly IContractService _contractService;

        public LearningMaterialChangesHistoryController(
            ILearningMaterialChangesHistorySerivce changesService,
            IContractService contractService
            )
        {
            _changesService = changesService;
            _contractService = contractService;
        }

        [HttpPost("getChangesHistoryList")]
        public async Task<IActionResult> GetChangesHistoriesAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _changesService.GetChangesHistoriesAsync(request.SearchKey, request.pageIndex, request.PageSize);
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
        [HttpPost("getLearningMaterialChangesHistoriesOfSubjectList")]
        public async Task<IActionResult> GetLearningMaterialChangesHistoriesOfSubjectAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _changesService.GetLearningMaterialChangesHistoriesOfSubjectAsync(
                    request.Id, request.SearchKey, request.pageIndex, request.PageSize);

                if (data != null && data.Items.Any())
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateLearningMaterialChangesHistory([FromBody] CreateLearningMaterialChangesHistoryDto historyDto)
        {
            if (historyDto == null)
            {
                return BadRequest("Invalid data.");
            }
            if (historyDto.ContractId.HasValue)
            {
                var contract = await _contractService.GetContractByIdAsync(historyDto.ContractId.Value);
                if (contract == null)
                    return BadRequest(new { message = "Invalid ContractId." });
            }

            bool isSuccess = await _changesService.CreateLearningMaterialChangesHistoryAsync(historyDto);
            if (isSuccess)
                return Ok(new { message = "History created successfully" });

            return StatusCode(500, "Failed to create history.");
        }
        [HttpPut("update/{historyId}")]
        public async Task<IActionResult> UpdateAcceptanceRecord(Guid historyId, [FromBody] UpdateLearningMaterialChangesHistoryDto request)
        {
            try
            {
                var updatedRecord = await _changesService.UpdateLearningMaterialChangesHistoryAsync(historyId, request);

                if (updatedRecord.HasValue)
                    return Ok(new
                    {
                        message = "Update thành công.",
                        Data = updatedRecord,

                    });
                else
                {
                    return NotFound(new { message = "Dữ liệu không được tìm thấy." });
                }
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
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("{historyId}")]
        public async Task<IActionResult> DeleteAcceptanceRecord(Guid historyId)
        {
            try
            {
                var result = await _changesService.SoftDeleteLearningMaterialChangesHistoryAsync(historyId);
                return result ? Ok(new { message = "Xóa lịch sử thay đổi thu thành công." }) : NotFound(new { message = "Không tìm thấy lịch sử thay đổi hoặc đã bị xóa trước đó." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpGet("getHistoryOfChangeDetail/{id}")]
        public async Task<IActionResult> GetHistoryOfChangeDetail(Guid id)
        {
            try
            {
                var data = await _changesService.getHistoryOfChangeDetail(id);
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
    }
}
