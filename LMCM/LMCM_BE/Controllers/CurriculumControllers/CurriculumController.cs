using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.CurriculumService;
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

        public CurriculumController(ICurriculumService curriculumService)
        {
            _curriculumService = curriculumService;
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
                return NotFound(new { message = "Data not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("importCurriculums")]
        public async Task<IActionResult> ImportCurriculumsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Please upload a valid Excel file." });

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        // Validate Headers
                        string[] expectedHeaders = {
                    "CurriculumCode", "CurriculumName", "CurriculumNameEnglish", "Description",
                    "VocationalCode", "VocationalName", "EnglishVocationalName",
                    "DecisionNo", "ApprovedDate", "Status"
                };

                        for (int col = 1; col <= expectedHeaders.Length; col++)
                        {
                            if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                            {
                                return BadRequest(new { message = "Invalid Excel format. Please use the correct template." });
                            }
                        }

                        // Read and Process Data
                        int rowCount = worksheet.Dimension.Rows;
                        List<Curriculum> curriculums = new List<Curriculum>();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var curriculum = new Curriculum
                            {
                                CurriculumId = Guid.NewGuid(),
                                CurriculumCode = worksheet.Cells[row, 1].Text,
                                CurriculumName = worksheet.Cells[row, 2].Text,
                                CurriculumNameEnglish = worksheet.Cells[row, 3].Text,
                                CurriculumDescription = worksheet.Cells[row, 4].Text,
                                VocationalCode = worksheet.Cells[row, 5].Text,
                                VocationalName = worksheet.Cells[row, 6].Text,
                                EnglishVocationalName = worksheet.Cells[row, 7].Text,
                                DecisionNo = worksheet.Cells[row, 8].Text,
                                ApprovedDate = DateTime.TryParse(worksheet.Cells[row, 9].Text, out DateTime approvedDate) ? approvedDate : null,
                                Status = worksheet.Cells[row, 10].Text,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            curriculums.Add(curriculum);
                        }

                        var isSuccess = await _curriculumService.ImportCurriculumsAsync(curriculums);

                        if (isSuccess)
                        {
                            return Ok(new { message = "Curriculums imported successfully." });
                        }
                        return BadRequest(new { message = "Import failed." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
