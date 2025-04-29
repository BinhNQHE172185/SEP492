using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.SubjectService;
using LMCM_BE.Services.SyllabusService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace LMCM_BE.Controllers.SyllabusControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyllabusController : ControllerBase
    {
        private readonly ISyllabusService _syllabusService;
        public SyllabusController(ISyllabusService syllabusService)
        {
            _syllabusService = syllabusService;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("getSyllabusesList")]
        public async Task<IActionResult> GetSyllabusesAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _syllabusService.GetSyllabusesAsync(request.SearchKey, request.pageIndex, request.PageSize);
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
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("getSyllabusesListNoPaging")]
        public async Task<IActionResult> GetSyllabusesNoPagingAsync(string? searchKey)
        {
            try
            {
                var data = await _syllabusService.GetSyllabusesAsync(searchKey);
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
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpPost("getSyllabusChangeHistoryList")]
        public async Task<IActionResult> GetSyllabusChangeHistoiesAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _syllabusService.GetSyllabusChangeHistoriesAsync(request.Id, request.SearchKey, request.pageIndex, request.PageSize);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpGet("getSyllabusDetail")]
        public async Task<IActionResult> GetSyllabusDetailAsync(Guid syllabusId)
        {
            try
            {
                var data = await _syllabusService.GetSyllabusDetailAsync(syllabusId);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpDelete("{syllabusId}")]
        public async Task<IActionResult> DeleteSyllabusAsync(Guid syllabusId)
        {
            try
            {
                bool result = await _syllabusService.DeleteSyllabusAsync(syllabusId);

                if (result)
                {
                    return Ok(new { message = "Giáo trình đã được xóa thành công." });
                }
                return NotFound(new { message = "Không tìm thấy giáo trình." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("importSyllabus")]
        public async Task<IActionResult> ImportSyllabusFromExcel(IFormFile file, [FromQuery] bool keepUserCreated = false)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Vui lòng tải lên tệp Excel hợp lệ." });

            // Check file extension
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Chỉ chấp nhận các tệp Excel có định dạng .xlsx hoặc .xls." });

            // Check file size (max 5MB)
            const long maxSize = 5 * 1024 * 1024;
            if (file.Length > maxSize)
                return BadRequest(new { message = "Dung lượng tệp không được vượt quá 5MB." });

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var requiredSheets = new List<string> { "Syllabus", "Schedule", "CLO", "Grading structure", "Constructivist Question", "Materials" };
                        var availableSheets = package.Workbook.Worksheets.Select(sheet => sheet.Name).ToList();
                        var missingSheets = requiredSheets.Except(availableSheets).ToList();

                        if (missingSheets.Any())
                        {
                            return BadRequest(new { message = $"Tệp Excel bị thiếu các trang sau: {string.Join(", ", missingSheets)}." });
                        }

                        if (await _syllabusService.ImportSyllabusAsync(package.Workbook, keepUserCreated))
                            return Ok(new { message = "Nhập vào hệ thống thành công." });
                        else
                            return BadRequest(new { message = "Nhập vào hệ thống thất bại." });
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
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
