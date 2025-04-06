using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.CurriculumsSubjectDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.CurriculumService;
using LMCM_BE.Services.PloService;
using LMCM_BE.Services.SubjectService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

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

                if (data != null)
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
                return Ok(curriculum);
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
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        // Expected headers for each worksheet
                        var expectedHeaders = new Dictionary<string, List<(string Header, string Cell)>>
                        {
                            { "Curriculum Subject", new List<(string, string)>
                                {
                                    ("SubjectCode", "A1"),
                                    ("SubjectName", "B1"),
                                    ("English SubjectName", "C1"),
                                    ("TermNo", "D1"),
                                    ("Credits", "E1"),
                                    ("Options", "F1")
                                }
                            },
                            { "Curriculum", new List<(string, string)>
                                {
                                    ("No", "A1"),
                                    ("Title", "B1"),
                                    ("Details", "C1"),
                                    ("Curriculum Code", "B2"),
                                    ("Curriculum Name", "B3"),
                                    ("English Curriculum Name", "B4"),
                                    ("Curriculum Description", "B5"),
                                    ("Vocational Code", "B6"),
                                    ("Vocational Name", "B7"),
                                    ("English Vocational Name", "B8"),
                                    ("Decision No.", "B9"),
                                    ("Approved date", "B10")
                                }
                            },
                            { "PLO", new List<(string, string)>
                                {
                                    ("No", "A1"),
                                    ("PLO Name", "B1"),
                                    ("PLO Description", "C1")
                                }
                            },
                            { "PLO Mappings", new List<(string, string)>
                                {
                                    ("Subject Code", "A2")
                                }
                            }
                        };
                        var isSuccess = await _curriculumService.ImportCurriculumFromWorkbookAsync(package.Workbook, expectedHeaders);
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
