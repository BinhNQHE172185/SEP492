using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.CurriculumService;
using LMCM_BE.Services.PloService;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMCM_BE.Controllers.CurriculumControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurriculumController : ControllerBase
    {
        private readonly ICurriculumService _curriculumService;
        private readonly IPloService _ploService;

        public CurriculumController(ICurriculumService curriculumService, IPloService ploService)
        {
            _curriculumService = curriculumService;
            _ploService = ploService;
        }

        [HttpPost("getCurriculumList")]
        public async Task<IActionResult> GetCurriculumsAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _curriculumService.GetCurriculumsAsync(request.SearchKey, request.pageIndex, request.PageSize);

                if (data.Items != null)
                {
                    return Ok(data);
                }

                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }

        [HttpGet("{curriculumId}")]
        public async Task<IActionResult> GetCurriculumDetail(Guid curriculumId)
        {
            try
            {
                var curriculum = await _curriculumService.GetCurriculumDetailAsync(curriculumId);
                if (curriculum != null)
                {
                    return Ok(curriculum);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{curriculumId}/plos")]
        public async Task<IActionResult> GetPloDetails(Guid curriculumId)
        {
            try
            {
                var plos = await _ploService.GetPloDetailsByCurriculumIdAsync(curriculumId);

                if (plos == null || plos.Count == 0)
                    return NotFound(new { message = "Không tìm thấy PLO nào cho chương trình giảng dạy này." });

                return Ok(plos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("importCurriculum")]
        public async Task<IActionResult> ImportCurriculumFromExcel(IFormFile file)
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
                        var isSuccess = await _curriculumService.ImportCurriculumFromWorkbookAsync(package.Workbook);
                        if (isSuccess)
                        {
                            return Ok(new { message = "Nhập chương trình đào tạo thành công." });
                        }
                        return BadRequest(new { message = "Nhập chương trình đào tạo thất bại." });
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        [HttpDelete("{curriculumId}")]
        public async Task<IActionResult> DeleteCurriculum(Guid curriculumId)
        {
            try
            {
                var result = await _curriculumService.SoftDeleteCurriculumAsync(curriculumId);
                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy chương trình giảng dạy hoặc đã bị xóa trước đó." });
                }

                return Ok(new { message = "Xóa chương trình giảng dạy thành công." });
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
