using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.CLOServices;
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
        private readonly ICLOServices _cloService;

        public SyllabusController(ISyllabusService syllabusService, ISubjectService subjectService, ICLOServices cloService)
        {
            _syllabusService = syllabusService;
            _subjectService = subjectService;
            _cloService = cloService;
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

        [HttpPost("deleteSyllabus")]
        public async Task<IActionResult> DeleteSyllabusAsync([FromBody] Guid syllabusId)
        {
            try
            {
                bool result = await _syllabusService.DeleteSyllabusAsync(syllabusId);

                if (result)
                {
                    return Ok(new { message = "Syllabus deleted successfully." });
                }
                return NotFound(new { message = "Syllabus not found or already inactive." });
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
                        var requiredSheets = new List<string> { "Syllabus", "Schedule", "CLO" };
                        var availableSheets = package.Workbook.Worksheets.Select(sheet => sheet.Name).ToList();
                        var missingSheets = requiredSheets.Except(availableSheets).ToList();

                        if (missingSheets.Any())
                        {
                            return BadRequest(new { message = $"The Excel file is missing the following sheets: {string.Join(", ", missingSheets)}." });
                        }

                        Syllabus syllabus = await ImportSyllabusSheet(package.Workbook.Worksheets["Syllabus"]);
                        if (syllabus != null)
                        {
                            var isCLOSuccess = await ImportCLOSheet(package.Workbook.Worksheets["CLO"], syllabus);
                        }
                        else
                        {
                            return BadRequest(new { message = "Import failed." });
                        }
                        return Ok(new { message = "Syllabus imported successfully." });

                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        private async Task<Syllabus> ImportSyllabusSheet(ExcelWorksheet worksheet)
        {
            // Validate expected headers
            string[] expectedHeaders = { "No", "Title", "Details" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new Exception("Invalid syllabus format. Please use the correct template.");
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
            Subject subject = await _subjectService.GetSubjectByCodeAsync(syllabusData.CourseCode);

            if (subject != null)
            {
                syllabusData.SubjectId = subject.SubjectId;
                return await _syllabusService.ImportSyllabusAsync(syllabusData);
            }
            else
            {
                throw new Exception("Can't find subject matched the syllabus.");
            }
        }
        private async Task<bool> ImportCLOSheet(ExcelWorksheet worksheet, Syllabus syllabus)
        {
            // Validate expected headers
            string[] expectedHeaders = { "No", "CLO Name", "CLO Description" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new Exception("Invalid CLO format. Please use the correct template.");
                }
            }

            var cloList = new List<CLOInsertDto>();

            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var cloData = new CLOInsertDto
                {
                    SyllabusId=syllabus.SyllabusId,
                    CloName = worksheet.Cells[row, 2].Text.Trim(),
                    CloDescription = worksheet.Cells[row, 3].Text.Trim()
                };

                cloList.Add(cloData);
            }

            if (!cloList.Any())
            {
                throw new Exception("No CLOs found in the sheet.");
            }
            if (syllabus != null && syllabus.PreviousVersionId!=null)
            {
                await _cloService.DeleteCLOBySyllabusAsync((Guid)syllabus.PreviousVersionId);
            }
            return await _cloService.ImportCLOsAsync(cloList);
        }

    }
}
