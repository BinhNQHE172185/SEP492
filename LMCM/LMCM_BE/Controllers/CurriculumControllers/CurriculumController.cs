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
        private readonly ISubjectService _subjectService;
        private readonly IPloService _ploService;

        public CurriculumController(ICurriculumService curriculumService, ISubjectService subjectService, IPloService ploService)
        {
            _curriculumService = curriculumService;
            _subjectService = subjectService;
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
            var curriculum = await _curriculumService.GetCurriculumDetailAsync(curriculumId);

            if (curriculum == null)
                return NotFound(new { message = "Không tìm thấy chương trình giảng dạy hoặc chương trình này không hoạt động." });

            return Ok(curriculum);
        }

        [HttpGet("{curriculumId}/plos")]
        public async Task<IActionResult> GetPloDetails(Guid curriculumId)
        {
            var plos = await _ploService.GetPloDetailsByCurriculumIdAsync(curriculumId);

            if (plos == null || plos.Count == 0)
                return NotFound(new { message = "Không tìm thấy PLO nào cho chương trình giảng dạy này." });

            return Ok(plos);
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
                        ExcelWorksheet curriculumSubjectSheet = package.Workbook.Worksheets["Curriculum Subject"];
                        ExcelWorksheet curriculumSheet = package.Workbook.Worksheets["Curriculum"];
                        ExcelWorksheet ploSheet = package.Workbook.Worksheets["PLO"];
                        ExcelWorksheet ploSubjectSheet = package.Workbook.Worksheets["PLO Mappings"];

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
                        // Function to validate headers sheet
                        if (curriculumSheet == null || ploSheet == null || curriculumSubjectSheet == null || ploSubjectSheet == null)
                            return BadRequest(new { message = "Định dạng tệp Excel không hợp lệ." });

                        // Function to validate headers with expected cell locations
                        bool ValidateHeaders(ExcelWorksheet sheet, List<(string Header, string Cell)> expectedHeaders)
                        {
                            foreach (var (expectedHeader, cellAddress) in expectedHeaders)
                            {
                                // Get the actual header text from the specified cell
                                string actualHeader = sheet.Cells[cellAddress].Text.Trim();

                                // Validate against the expected header
                                if (!expectedHeader.Equals(actualHeader, StringComparison.OrdinalIgnoreCase))
                                {
                                    return false; // Header does not match
                                }
                            }
                            return true; // All headers are correct
                        }

                        // Validate headers for each sheet
                        var invalidSheets = new List<string>();

                        foreach (var sheetName in expectedHeaders.Keys)
                        {
                            var worksheet = package.Workbook.Worksheets[sheetName];

                            if (worksheet == null)
                            {
                                invalidSheets.Add(sheetName); // Sheet is missing
                                continue;
                            }

                            if (!ValidateHeaders(worksheet, expectedHeaders[sheetName]))
                            {
                                invalidSheets.Add(sheetName); // Headers are incorrect
                            }
                        }


                        if (invalidSheets.Any())
                        {
                            return BadRequest(new { message = "Các sheet sau có tiêu đề không hợp lệ:", invalidSheets });
                        }

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
                            CurriculumsSubjects = new List<CurriculumsSubject>()
                        };

                        // Retrieve Curriculum Subjects
                        var tempCurriculumsSubjects = new List<TempCurriculumsSubject>();
                        var subjectCodes = new List<string>();

                        int row = 2; // Start from row 2 (row 1 contains headers)
                        while (!string.IsNullOrWhiteSpace(curriculumSubjectSheet.Cells[row, 1].Text))
                        {
                            var tempCurriculumsSubject = new TempCurriculumsSubject
                            {
                                SubjectCode = curriculumSubjectSheet.Cells[row, 1].Text,
                                TermNo = int.TryParse(curriculumSubjectSheet.Cells[row, 4].Text, out int termNo) ? termNo : (int?)null,
                                Credit = int.TryParse(curriculumSubjectSheet.Cells[row, 5].Text, out int credit) ? credit : (int?)null,
                                Options = int.TryParse(curriculumSubjectSheet.Cells[row, 6].Text, out int options) ? options : (int?)null,
                            };

                            subjectCodes.Add(tempCurriculumsSubject.SubjectCode);
                            tempCurriculumsSubjects.Add(tempCurriculumsSubject);
                            row++;
                        }

                        // Validate Subjects in Database
                        var existingSubjects = await _subjectService.GetActiveSubjectsByCodesAsync(subjectCodes);
                        var missingSubjects = subjectCodes.Except(existingSubjects.Select(s => s.SubjectCode)).ToList();

                        if (missingSubjects.Any())
                        {
                            return BadRequest(new { message = "Các môn học sau đây không tồn tại hoặc không hoạt động:", missingSubjects });
                        }

                        // Step 4: Convert Temporary Subjects to CurriculumsSubjects
                        var curriculumsSubjects = new List<CurriculumsSubject>();
                        foreach (var tempSubject in tempCurriculumsSubjects)
                        {
                            var existingSubject = existingSubjects.FirstOrDefault(s => s.SubjectCode == tempSubject.SubjectCode);
                            if (existingSubject == null)
                                continue; // Skip if not found in DB

                            curriculumsSubjects.Add(new CurriculumsSubject
                            {
                                CurriculumId = curriculum.CurriculumId,
                                SubjectId = existingSubject.SubjectId,
                                TermNo = tempSubject.TermNo,
                                Credit = tempSubject.Credit,
                                Options = tempSubject.Options,
                                Status = "Active",
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }

                        // Read PLO data
                        var plos = new List<Plo>();
                        int ploRow = 2; // first row is headers
                        while (!string.IsNullOrWhiteSpace(ploSheet.Cells[ploRow, 2].Text)) // Check if PLO Name exists
                        {
                            plos.Add(new Plo
                            {
                                PloId = Guid.NewGuid(),
                                CurriculumId = curriculum.CurriculumId,
                                PloName = ploSheet.Cells[ploRow, 2].Text,
                                PloDescription = ploSheet.Cells[ploRow, 3].Text,
                                Status = "Active",
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                PloSubjects = new List<PloSubject>(),
                            });
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
                            var subject = existingSubjects.FirstOrDefault(s => s.SubjectCode == subjectCode);
                            if (subject == null)
                                continue;

                            for (int ploCol = columnStart; ploCol <= ploSubjectSheet.Dimension.End.Column; ploCol++)
                            {
                                var ploName = ploSubjectSheet.Cells[2, ploCol].Text; // PLO name from row 2
                                var plo = plos.FirstOrDefault(p => p.PloName == ploName);
                                if (plo != null && ploSubjectSheet.Cells[subjectRow, ploCol].Text == "ü")
                                {
                                    ploSubjectMappings.Add(new PloSubject
                                    {
                                        PloId = plo.PloId,
                                        SubjectId = subject.SubjectId,
                                        Status = "Active",
                                        CreatedAt = DateTime.UtcNow,
                                        UpdatedAt = DateTime.UtcNow
                                    });
                                }
                            }
                        }
                        // Assign to curriculum

                        // Assign curriculum subjects
                        curriculum.CurriculumsSubjects = curriculumsSubjects;

                        // Assign PLOs to the curriculum
                        curriculum.Plos = plos;

                        // Assign PLO-Subject mappings to each PLO
                        foreach (var plo in curriculum.Plos)
                        {
                            plo.PloSubjects = ploSubjectMappings.Where(ps => ps.PloId == plo.PloId).ToList();
                        }

                        // Save curriculum and mappings
                        var isSuccess = await _curriculumService.ImportCurriculumAsync(curriculum);

                        if (isSuccess)
                        {
                            return Ok(new { message = "Nhập chương trình đào tạo thành công." });
                        }
                        return BadRequest(new { message = "Nhập chương trình đào tạo thất bại." });

                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }



    }
}
