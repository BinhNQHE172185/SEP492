using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.AcceptanceRecordService;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
                if (acceptanceRecordDto.File == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Không tìm thấy file.",
                    });
                }

                // Check file type
                if (acceptanceRecordDto.File.ContentType != "application/pdf")
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Chỉ file pdf mới được tải lên.",
                    });
                }

                // Check file size (5MB = 5 * 1024 * 1024 bytes)
                if (acceptanceRecordDto.File.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Dung lượng file không được vượt quá 5MB.",
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
                    Message = "Bạn không có quyền tạo biên bản nghiệm thu.",
                    Error = ex.Message
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Dữ liệu đầu vào không hợp lệ.",
                    Error = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Dữ liệu đầu vào không hợp lệ.",
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
                if (request.File == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Không tìm thấy file.",
                        Error = "File is required."
                    });
                }

                // Check file type
                if (request.File.ContentType != "application/pdf")
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Chỉ file pdf mới được tải lên.",
                        Error = "Invalid file type."
                    });
                }

                // Check file size (5MB = 5 * 1024 * 1024 bytes)
                if (request.File.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Dung lượng file không được vượt quá 5MB.",
                        Error = "File size exceeds 5MB limit."
                    });
                }

                var updatedRecord = await _acceptanceRecordService.UpdateAcceptanceRecordAsync(acceptanceId, request);

                if (updatedRecord == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Không tìm thấy biên bản hoặc cập nhật thất bại.",
                        Error = "Record not found or update unsuccessful."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Cập nhật biên bản nghiệm thu thành công.",
                    Data = updatedRecord
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
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền cập nhật biên bản nghiệm thu.",
                    Error = ex.Message
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Dữ liệu đầu vào không hợp lệ.",
                    Error = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Dữ liệu đầu vào không hợp lệ.",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi hệ thống.",
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
