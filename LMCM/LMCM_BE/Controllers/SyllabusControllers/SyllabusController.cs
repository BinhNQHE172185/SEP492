using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.SubjectService;
using LMCM_BE.Services.SyllabusService;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace LMCM_BE.Controllers.SyllabusControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyllabusController : ControllerBase
    {
        private readonly ISyllabusService _syllabusService;
        private readonly ISubjectService _subjectService;

        public SyllabusController(ISyllabusService syllabusService, ISubjectService subjectService)
        {
            _syllabusService = syllabusService;
            _subjectService = subjectService;
        }

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
                return NotFound(new { message = "Data not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("importSyllabus")]
        public async Task<IActionResult> ImportSyllabusFromExcel(IFormFile file)
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
                        var worksheet = package.Workbook.Worksheets["Syllabus"];
                        if (worksheet == null)
                        {
                            return BadRequest(new { message = "The Excel file does not contain a sheet named 'Syllabus'." });
                        }

                        // Validate expected headers
                        string[] expectedHeaders = {
                    "No", "Title", "Details"
                };

                        for (int col = 1; col <= expectedHeaders.Length; col++)
                        {
                            if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                            {
                                return BadRequest(new { message = "Invalid syllabus format. Please use the correct template." });
                            }
                        }

                        var syllabusData = new SyllabusInsertDto
                        {
                            ProgramName = worksheet.Cells[3, 3].Text.Trim(),
                            DecisionNo = worksheet.Cells[4, 3].Text.Trim(),
                            CourseName = worksheet.Cells[5, 3].Text.Trim(),
                            CourseNameEnglish = worksheet.Cells[6, 3].Text.Trim(),
                            CourseCode = worksheet.Cells[7, 3].Text.Trim(),
                            LearningTeachingMethod = worksheet.Cells[8, 3].Text.Trim(),
                            NoOfCredits = int.TryParse(worksheet.Cells[9, 3].Text, out int reality) ? reality : 0,
                            DegreeLevel = worksheet.Cells[10, 3].Text.Trim(),
                            TimeAllocation = worksheet.Cells[11, 3].Text.Trim(),
                            PreRequisite = worksheet.Cells[12, 3].Text.Trim(),
                            Description = worksheet.Cells[13, 3].Text.Trim(),
                            StudentTask = worksheet.Cells[14, 3].Text.Trim(),
                            Tools = worksheet.Cells[15, 3].Text.Trim(),
                            Note = worksheet.Cells[16, 3].Text.Trim(),
                            MinGpaToPass = int.TryParse(worksheet.Cells[17, 3].Text, out int minGPA) ? minGPA : 0,
                            ScoringScale = int.TryParse(worksheet.Cells[18, 3].Text, out int scoringScale) ? scoringScale : 0,
                            ApprovedDate = DateTime.TryParse(worksheet.Cells[19, 3].Text, out DateTime approvedDate) ? approvedDate : null
                        };

                        // Find subject for syllabus
                        Subject subject =await _subjectService.GetSubjectByCodeAsync(syllabusData.CourseCode);

                        var isSuccess = false;

                        if (subject != null)
                        {
                            syllabusData.SubjectId=subject.SubjectId;
                            isSuccess = await _syllabusService.ImportSyllabusAsync(syllabusData);
                        }
                        else return BadRequest(new { message = "Can't find subject matched the syllabus" });

                        return isSuccess
                            ? Ok(new { message = "Syllabus imported successfully." })
                            : BadRequest(new { message = "Import failed." });
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
