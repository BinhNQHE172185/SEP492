using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.SubjectService;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace LMCM_BE.Controllers.SubjectControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpPost("getSubjectList")]
        public async Task<IActionResult> GetSubjectAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _subjectService.GetSubjectsAsync(request.SearchKey, request.pageIndex, request.PageSize);
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
        [HttpPost("importSubjects")]
        public async Task<IActionResult> ImportSubjectsFromExcelAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Vui lòng tải lên tệp Excel hợp lệ." });

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        var isSuccess = await _subjectService.ImportSubjectsAsync(worksheet);
                        if (isSuccess)
                        {
                            return Ok(new { message = "Nhập vào hệ thống thành công." });
                        }
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
        [HttpDelete("{subjectId}")]
        public async Task<IActionResult> DeleteSubjectAsync(Guid subjectId)
        {
            try
            {
                var result = await _subjectService.SoftDeleteSubjectAsync(subjectId);
                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy môn học hoặc đã bị xóa trước đó." });
                }

                return Ok(new { message = "Xóa môn học thành công." });
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
