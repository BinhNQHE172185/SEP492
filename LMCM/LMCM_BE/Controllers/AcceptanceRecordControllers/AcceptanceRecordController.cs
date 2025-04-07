using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.AcceptanceRecordService;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMCM_BE.Controllers.AcceptanceRecordControllers
{
    [Route("api/acceptance-records")]
    [ApiController]
    public class AcceptanceRecordController : ControllerBase
    {
        private readonly IAcceptanceRecordService _acceptanceRecordService;

        public AcceptanceRecordController(IAcceptanceRecordService acceptanceRecordService)
        {
            _acceptanceRecordService = acceptanceRecordService;
        }

        [HttpPost("getAcceptanceRecordList")]
        public async Task<IActionResult> GetAcceptanceRecordsAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _acceptanceRecordService.GetAcceptanceRecordsAsync(request.SearchKey, request.pageIndex, request.PageSize);
                return data != null ? Ok(data) : NotFound(new { message = "Không tìm thấy dữ liệu biên bản nghiệm thu." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAcceptanceRecord([FromForm] AcceptanceRecordCreateDto acceptanceRecordDto)
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

                var acceptanceRecord = await _acceptanceRecordService.CreateAcceptanceRecordAsync(acceptanceRecordDto);
                return Ok(new
                {
                    Success = true,
                    Message = "Biên bản nghiệm thu đã được tạo thành công.",
                    Data = acceptanceRecord
                });
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
                return NotFound(new
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi hệ thống: " + ex.Message,
                    Error = ex.Message
                });
            }
        }

        [HttpPut("update/{acceptanceId}")]
        public async Task<IActionResult> UpdateAcceptanceRecord(Guid acceptanceId, [FromForm] AcceptanceRecordUpdateDto request)
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

                var updatedRecord = await _acceptanceRecordService.UpdateAcceptanceRecordAsync(acceptanceId, request);

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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ex.Message
                });
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

        [HttpDelete("{acceptanceId}")]
        public async Task<IActionResult> DeleteAcceptanceRecord(Guid acceptanceId)
        {
            try
            {
                var result = await _acceptanceRecordService.SoftDeleteAcceptanceRecordAsync(acceptanceId);
                return result ? Ok(new { message = "Xóa biên bản nghiệm thu thành công." }) : NotFound(new { message = "Không tìm thấy biên bản hoặc đã bị xóa trước đó." });
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

        [HttpGet("{acceptanceId}")]
        public async Task<IActionResult> GetAcceptanceRecordDetail(Guid acceptanceId)
        {
            try
            {
                var record = await _acceptanceRecordService.GetAcceptanceRecordDetailAsync(acceptanceId);
                return Ok(record);
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
