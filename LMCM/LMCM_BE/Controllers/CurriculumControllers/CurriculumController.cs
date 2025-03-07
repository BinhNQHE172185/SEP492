using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.CurriculumService;
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
        private readonly ISubjectService _subjectService;

        public CurriculumController(ICurriculumService curriculumService, ISubjectService subjectService)
        {
            _curriculumService = curriculumService;
            _subjectService = subjectService;
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
        [HttpPost("importCurriculum")]
        public async Task<IActionResult> ImportCurriculumFromExcel(IFormFile file)
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
                        ExcelWorksheet curriculumSubjectSheet = package.Workbook.Worksheets["Curriculum Subject"];
                        ExcelWorksheet curriculumSheet = package.Workbook.Worksheets["Curriculum"];
                        ExcelWorksheet ploSheet = package.Workbook.Worksheets["PLO"];
                        ExcelWorksheet ploSubjectSheet = package.Workbook.Worksheets["PLO Mappings"];

                        // Retrieve Curriculum Subjects
                        var curriculumSubjects = new List<CurriculumsSubject>();
                        var subjectCodes = new List<string>();
                        int row = 2; //  row 1 contains headers
                        while (!string.IsNullOrWhiteSpace(curriculumSubjectSheet.Cells[row, 1].Text))
                        {
                            var subjectCode = curriculumSubjectSheet.Cells[row, 1].Text;
                            subjectCodes.Add(subjectCode);
                            row++;
                        }

                        // Validate Subjects in Database
                        var existingSubjects = await _subjectService.GetActiveSubjectsByCodesAsync(subjectCodes);
                        var missingSubjects = subjectCodes.Except(existingSubjects.Select(s => s.SubjectCode)).ToList();

                        if (missingSubjects.Any())
                        {
                            return BadRequest(new { message = "The following subjects are missing or inactive:", missingSubjects });
                        }

                        if (curriculumSheet == null || ploSheet == null || curriculumSubjectSheet == null)
                            return BadRequest(new { message = "Invalid Excel file format." });

                        // Read Curriculum data
                        var curriculum = new Curriculum
                        {
                            CurriculumId = Guid.NewGuid(),
                            CurriculumCode = curriculumSheet.Cells["C2"].Text,
                            CurriculumName = curriculumSheet.Cells["C3"].Text,
                            CurriculumNameEnglish = curriculumSheet.Cells["C4"].Text,
                            CurriculumDescription = curriculumSheet.Cells["C5"].Text,
                            VocationalCode = curriculumSheet.Cells["C6"].Text,
                            VocationalName = curriculumSheet.Cells["C7"].Text,
                            EnglishVocationalName = curriculumSheet.Cells["C8"].Text,
                            DecisionNo = curriculumSheet.Cells["C9"].Text,
                            ApprovedDate = DateTime.TryParse(curriculumSheet.Cells["C10"].Text, out DateTime approvedDate) ? approvedDate : null,
                            Status = "Active",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Plos = new List<Plo>(),
                        };

                        // Read PLO data
                        var ploDictionary = new Dictionary<string, Plo>();
                        int ploRow= 2; // first row is headers
                        while (!string.IsNullOrWhiteSpace(ploSheet.Cells[ploRow, 2].Text)) // Check if PLO Name exists
                        {
                            var plo = new Plo
                            {
                                PloId = Guid.NewGuid(),
                                CurriculumId = curriculum.CurriculumId,
                                PloName = ploSheet.Cells[ploRow, 2].Text,
                                PloDescription = ploSheet.Cells[ploRow, 3].Text,
                                Status = "Active",
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                PloSubjects = new List<PloSubject>(),
                            };

                            curriculum.Plos.Add(plo);
                            ploDictionary[plo.PloName] = plo;
                            ploRow++;
                        }

                        // Read PLO-Subject Mapping (PloSubject)
                        var ploSubjectMappings = new List<PloSubject>();
                        int columnStart = 2; // PLOs start from column B
                        int subjectRowStart = 3; // Subjects start from row 3
                        
                        for (int subjectRow = subjectRowStart; subjectRow <= ploSubjectSheet.Dimension.End.Row; subjectRow++)
                        {
                            var subjectCode = ploSubjectSheet.Cells[subjectRow, 1].Text;

                            // Skip merged header rows
                            if (string.IsNullOrWhiteSpace(subjectCode) || ploSubjectSheet.Cells[subjectRow, 1].Merge)
                                continue;

                            // Get subject from curriculum subjects
                            var subject = curriculum.CurriculumsSubjects.FirstOrDefault(s => s.Subject.SubjectCode == subjectCode);
                            if (subject == null)
                                continue;

                            for (int ploCol = columnStart; ploCol <= ploSubjectSheet.Dimension.End.Column; ploCol++)
                            {
                                var ploName = ploSubjectSheet.Cells[2, ploCol].Text; // PLO name from row 2
                                if (ploDictionary.TryGetValue(ploName, out var plo) && ploSubjectSheet.Cells[subjectRow, ploCol].Text == "ü")
                                {
                                    ploSubjectMappings.Add(new PloSubject
                                    {
                                        PloId = plo.PloId,
                                        SubjectId = subject.Subject.SubjectId,
                                        Status = "Active",
                                        CreatedAt = DateTime.UtcNow,
                                        UpdatedAt = DateTime.UtcNow
                                    });
                                }
                            }
                        }
                        
                        // Save curriculum and mappings
                        var isSuccess = await _curriculumService.ImportCurriculumAsync(curriculum);

                        if (isSuccess)
                        {
                            return Ok(new { message = "Curriculum imported successfully." });
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
