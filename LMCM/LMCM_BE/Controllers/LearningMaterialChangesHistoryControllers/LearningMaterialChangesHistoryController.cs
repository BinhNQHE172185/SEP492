using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.LearningMaterialChangesHistoryService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.LearningMaterialChangesHistoryControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LearningMaterialChangesHistoryController : ControllerBase
    {
        private readonly ILearningMaterialChangesHistorySerivce _changesService;

        public LearningMaterialChangesHistoryController(
            ILearningMaterialChangesHistorySerivce changesService
            )
        {
            _changesService = changesService;
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
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLearningMaterialChangesHistory([FromBody] CreateLearningMaterialChangesHistoryDto historyDto)
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

                bool isSuccess = await _changesService.CreateLearningMaterialChangesHistoryAsync(historyDto);
                
                if (isSuccess)
                    return Ok(new
                    {
                        Success = true,
                        Message = "Tạo lịch sử thành công"
                    });
                return BadRequest(new
                {
                    Success = false,
                    Message = "Không thể lịch sử. Vui lòng kiểm tra lại dữ liệu hoặc thử lại sau."
                });
            }
            catch (InvalidDataException ex)
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
        [HttpPut("update/{historyId}")]
        public async Task<IActionResult> UpdateLearningMaterialChangesHistory(Guid historyId, [FromBody] UpdateLearningMaterialChangesHistoryDto request)
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

                bool isSuccess = await _changesService.UpdateLearningMaterialChangesHistoryAsync(historyId, request);

                if (isSuccess)
                    return Ok(new
                    {
                        Success = true,
                        Message = "Tạo lịch sử thành công"
                    });
                return BadRequest(new
                {
                    Success = false,
                    Message = "Không thể lịch sử. Vui lòng kiểm tra lại dữ liệu hoặc thử lại sau."
                });
            }
            catch (InvalidDataException ex)
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        [HttpDelete("{historyId}")]
        public async Task<IActionResult> DeleteLearningMaterialChangesHistory(Guid historyId)
        {
            try
            {
                var result = await _changesService.SoftDeleteLearningMaterialChangesHistoryAsync(historyId);
                return result ? Ok(new { message = "Xóa lịch sử thay đổi thu thành công." }) : NotFound(new { message = "Không tìm thấy lịch sử thay đổi hoặc đã bị xóa trước đó." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
