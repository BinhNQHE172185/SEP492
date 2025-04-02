using LMCM_BE.DbContext;
using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.ConstructivistQuestionDtos;
using LMCM_BE.DTOs.GradingStructureDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ScheduleDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.CLOService;
using LMCM_BE.Services.ConstructivistQuestionService;
using LMCM_BE.Services.GradingStructureService;
using LMCM_BE.Services.LearningMaterialService;
using LMCM_BE.Services.ScheduleService;
using LMCM_BE.Services.SubjectService;
using LMCM_BE.Services.SyllabusService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Runtime.InteropServices;

namespace LMCM_BE.Controllers.SyllabusControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyllabusController : ControllerBase
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly ISyllabusService _syllabusService;
        private readonly ISubjectService _subjectService;
        private readonly ICLOService _cloService;
        private readonly IScheduleService _scheduleService;
        private readonly IGradingStructureService _gradingStructureService;
        private readonly IConstructivistQuestionService _questionService;
        private readonly ILearningMaterialService _learningMaterialService;
        private readonly ILearningMaterialDetailsService _learningMaterialDetailsService;
        public SyllabusController(LMCM_DBContext dBContext, ISyllabusService syllabusService, ISubjectService subjectService, ICLOService cloService,
            IScheduleService scheduleService, IGradingStructureService gradingStructureService, IConstructivistQuestionService questionService,
            ILearningMaterialService learningMaterialService, ILearningMaterialDetailsService learningMaterialDetailsService)
        {
            _dbContext = dBContext;
            _syllabusService = syllabusService;
            _subjectService = subjectService;
            _cloService = cloService;
            _scheduleService = scheduleService;
            _gradingStructureService = gradingStructureService;
            _questionService = questionService;
            _learningMaterialService = learningMaterialService;
            _learningMaterialDetailsService = learningMaterialDetailsService;
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
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("importSyllabus")]
        public async Task<IActionResult> ImportSyllabusFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Vui lòng tải lên tệp Excel hợp lệ." });

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())  // Start a database transaction
            {
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

                            // Import Syllabus
                            Syllabus syllabus = await ImportSyllabusSheet(package.Workbook.Worksheets["Syllabus"]);
                            if (syllabus == null)
                                throw new Exception("Nhập giáo trình thất bại.");

                            // Import CLOs
                            var isCLOSuccess = await ImportCLOSheet(package.Workbook.Worksheets["CLO"], syllabus);
                            if (!isCLOSuccess)
                                throw new Exception("Nhập CLO thất bại.");

                            // Import Schedule
                            var isScheduleSuccess = await ImportScheduleSheet(package.Workbook.Worksheets["Schedule"], syllabus);
                            if (!isScheduleSuccess)
                                throw new Exception("Nhập lịch học thất bại.");

                            // Import Grading Structures
                            var isGradingSuccess = await ImportGradingStructureSheet(package.Workbook.Worksheets["Grading structure"], syllabus);
                            if (!isGradingSuccess)
                                throw new Exception("Nhập cấu trúc đánh giá thất bại.");

                            // Import Constructivist Questions
                            var isQuestionSuccess = await ImportConstructivistQuestionSheet(package.Workbook.Worksheets["Constructivist Question"], syllabus);
                            if (!isQuestionSuccess)
                                throw new Exception("Nhập câu hỏi thất bại.");

                            // Import Materials 
                            var isMaterialsSuccess = await ImportMaterialsSheet(package.Workbook.Worksheets["Materials"], syllabus);
                            if (!isQuestionSuccess)
                                throw new Exception("Nhập học liệu thất bại.");

                            await _dbContext.SaveChangesAsync();
                            await transaction.CommitAsync();
                            return Ok(new { message = "Nhập vào hệ thống thành công." });
                        }
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); // Rollback if any step fails
                    return StatusCode(500, new { message = "Lỗi xảy ra: " + ex.Message });
                }
            }
        }

        private async Task<bool> ImportScheduleSheet(ExcelWorksheet worksheet, Syllabus syllabus)
        {
            // Validate expected headers
            string[] expectedHeaders = { "Sess.", "Leaning-Teaching Method", "Content", "CLO", "ITU", "Student's materials", "Student's task", "Lecturer's Materials", "Lecturer's task", "Student's materials link", "Lecturer's Materials link" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new Exception($"Định dạng Excel trang Schedule không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
                }
            }

            var scheduleList = new List<ScheduleInsertDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var scheduleData = new ScheduleInsertDto
                {
                    SyllabusId = syllabus.SyllabusId,
                    ScheduleNo = int.TryParse(worksheet.Cells[row, 1].Text, out int session) ? session : 0,
                    Method = worksheet.Cells[row, 2].Text.Trim(),
                    Content = worksheet.Cells[row, 3].Text.Trim(),
                    Clos = worksheet.Cells[row, 4].Text.Trim(),
                    Itu = worksheet.Cells[row, 5].Text.Trim(),
                    StudentMaterial = worksheet.Cells[row, 6].Text.Trim(),
                    StudentTask = worksheet.Cells[row, 7].Text.Trim(),
                    LecturerMaterial = worksheet.Cells[row, 8].Text.Trim(),
                    LecturerTask = worksheet.Cells[row, 9].Text.Trim(),
                    StudentMaterialUrl = worksheet.Cells[row, 10].Text.Trim(),
                    LecturerMaterialUrl = worksheet.Cells[row, 11].Text.Trim()
                };

                scheduleList.Add(scheduleData);
            }

            if (!scheduleList.Any())
            {
                throw new Exception("Không tìm thấy dữ liệu lịch trình trong trang.");
            }

            // Remove old schedule if syllabus has a previous version
            if (syllabus.PreviousVersionId != null)
            {
                await _scheduleService.DeleteSchedulesBySyllabusAsync((Guid)syllabus.PreviousVersionId);
            }

            return await _scheduleService.ImportSchedulesAsync(scheduleList);
        }

        private async Task<Syllabus> ImportSyllabusSheet(ExcelWorksheet worksheet)
        {
            // Validate expected headers
            string[] expectedHeaders = { "No", "Title", "Details" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new Exception($"Định dạng Excel trang Syllabus không hợp lệ cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
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
                throw new Exception("Không tìm thấy môn học cho giáo trình.");
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
                    throw new Exception($"Định dạng Excel trang CLO không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
                }
            }

            var cloList = new List<CLOInsertDto>();

            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var cloData = new CLOInsertDto
                {
                    SyllabusId = syllabus.SyllabusId,
                    CloName = worksheet.Cells[row, 2].Text.Trim(),
                    CloDescription = worksheet.Cells[row, 3].Text.Trim()
                };

                cloList.Add(cloData);
            }

            if (!cloList.Any())
            {
                throw new Exception("Không tìm thấy CLOs trong trang.");
            }
            if (syllabus != null && syllabus.PreviousVersionId != null)
            {
                await _cloService.DeleteCLOBySyllabusAsync((Guid)syllabus.PreviousVersionId);
            }
            return await _cloService.ImportCLOsAsync(cloList);
        }
        private async Task<bool> ImportGradingStructureSheet(ExcelWorksheet worksheet, Syllabus syllabus)
        {
            // Validate expected headers
            string[] expectedHeaders = { "#", "Assessment Component\nHạng mục đánh giá", "Assessment Type", "Weight\nTrọng số %", "Part\nPhần", "Minimun value to meet Completion Criteria", "Duration", "CLO", "Type of questions", "Number of questions", "Scope of knowledge and skill of questions", "How?", "Note", "SessionNo", "Reference" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new Exception($"Định dạng Excel trang Grading Structure không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
                }
            }

            var gradingList = new List<GradingStructureInsertDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var gradingData = new GradingStructureInsertDto
                {
                    SyllabusId = syllabus.SyllabusId,
                    StructureNo = int.TryParse(worksheet.Cells[row, 1].Text, out int structureNo) ? structureNo : 0,
                    AssessmentComponent = worksheet.Cells[row, 2].Text.Trim(),
                    AssessmentType = worksheet.Cells[row, 3].Text.Trim(),
                    Weight = decimal.TryParse(worksheet.Cells[row, 4].Text, out decimal weight) ? weight : 0,
                    Part = int.TryParse(worksheet.Cells[row, 5].Text, out int part) ? part : 0,
                    MinValue = int.TryParse(worksheet.Cells[row, 6].Text, out int minCriteria) ? minCriteria : 0,
                    Duration = worksheet.Cells[row, 7].Text.Trim(),
                    Clo = worksheet.Cells[row, 8].Text.Trim(),
                    QuestionType = worksheet.Cells[row, 9].Text.Trim(),
                    QuestionNo = worksheet.Cells[row, 10].Text.Trim(),
                    Scope = worksheet.Cells[row, 11].Text.Trim(),
                    How = worksheet.Cells[row, 12].Text.Trim(),
                    Note = worksheet.Cells[row, 13].Text.Trim(),
                    SessionNo = int.TryParse(worksheet.Cells[row, 14].Text, out int sessionNo) ? sessionNo : 0,
                    Reference = worksheet.Cells[row, 15].Text.Trim()
                };

                gradingList.Add(gradingData);
            }

            if (!gradingList.Any())
            {
                throw new Exception("Không tìm thấy dữ liệu cấu trúc điểm trong trang.");
            }

            // Remove old grading structures if syllabus has a previous version
            if (syllabus.PreviousVersionId != null)
            {
                await _gradingStructureService.DeleteGradingStructuresBySyllabusAsync((Guid)syllabus.PreviousVersionId);
            }

            return await _gradingStructureService.ImportGradingStructuresAsync(gradingList);
        }
        private async Task<bool> ImportConstructivistQuestionSheet(ExcelWorksheet worksheet, Syllabus syllabus)
        {
            // Validate expected headers
            string[] expectedHeaders = { "No", "SessionNo", "Name", "Detail" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new Exception($"Định dạng Excel trang Constructivist Question không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
                }
            }

            var questionList = new List<ConstructivistQuestionInsertDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var questionData = new ConstructivistQuestionInsertDto
                {
                    SyllabusId = syllabus.SyllabusId,
                    SessionNo = int.TryParse(worksheet.Cells[row, 2].Text, out int sessionNo) ? sessionNo : 0,
                    QuestionName = worksheet.Cells[row, 3].Text.Trim(),
                    QuestionDetail = worksheet.Cells[row, 4].Text.Trim(),
                };

                questionList.Add(questionData);
            }

            if (!questionList.Any())
            {
                return true;
                //throw new Exception("Không tìm thấy dữ liệu câu hỏi trong trang.");
            }

            // Remove old questions if syllabus has a previous version
            if (syllabus.PreviousVersionId != null)
            {
                await _questionService.DeleteConstructivistQuestionsBySyllabusAsync((Guid)syllabus.PreviousVersionId);
            }

            return await _questionService.ImportConstructivistQuestionsAsync(questionList);
        }
        private async Task<bool> ImportMaterialsSheet(ExcelWorksheet worksheet, Syllabus syllabus)
        {
            // Validate expected headers
            string[] expectedHeaders = { "No", "MaterialDescription", "Purpose", "ISBN", "Type", "Note", "Author", "Publisher", "Published Date", "Edition" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new Exception($"Định dạng Excel trang Materials không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
                }
            }

            var materialList = new List<LearningMaterialImportDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var materialDetail = new LearningMaterialDetailsInsertDto();
                var materialDescription = worksheet.Cells[row, 2].Text.Trim(); // Read MaterialDescription
                string materialName = null;
                string materialQuantity = "1";
                string url = null;
                Guid? materialDetailId = null;

                var isbn = worksheet.Cells[row, 4].Text.Trim();
                var author = worksheet.Cells[row, 7].Text.Trim();
                var publisher = worksheet.Cells[row, 8].Text.Trim();
                var edition = worksheet.Cells[row, 10].Text.Trim();
                DateTime? publishedDate = DateTime.TryParse(worksheet.Cells[row, 9].Text, out DateTime tempDate) ? tempDate : (DateTime?)null;

                // Check if all fields are empty
                if (!string.IsNullOrEmpty(isbn) ||
                    !string.IsNullOrEmpty(author) ||
                    !string.IsNullOrEmpty(publisher) ||
                    !string.IsNullOrEmpty(edition) ||
                    publishedDate.HasValue)
                {
                    var materialDetailData = new LearningMaterialDetailsInsertDto
                    {
                        Isbn = isbn,
                        Author = author,
                        Publisher = publisher,
                        PublishedDate = publishedDate,
                        Edition = edition,
                        Url = materialDescription,
                    };
                    LearningMaterialDetail detail = await _learningMaterialDetailsService.InsertMaterialDetailsAsync(materialDetailData);
                    materialDetailId = detail.MaterialDetailId;
                }

                if (!string.IsNullOrEmpty(materialDescription))
                {
                    // Define the possible material types
                    string[] materialTypes = { "Slide", "Lab", "Assignment", "Quiz", "Assigment" };

                    bool foundType = false;

                    foreach (var type in materialTypes)
                    {
                        if (materialDescription.StartsWith(type, StringComparison.OrdinalIgnoreCase))
                        {
                            // Remove the material type from the string and trim extra characters
                            string remainingText = materialDescription.Substring(type.Length).Trim(':', ' ');

                            // Try parsing the quantity, default to "1" if missing
                            materialName = type;
                            materialQuantity = int.TryParse(remainingText, out int quantity) ? quantity.ToString() : "1";

                            foundType = true;
                            break; // Stop checking other types once a match is found
                        }
                    }

                    if (!foundType)
                    {
                        if (Uri.TryCreate(materialDescription, UriKind.Absolute, out Uri? uriResult)
                            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                        {
                            url = materialDescription;
                        }
                        else
                        {
                             materialName= materialDescription;
                        }
                    }
                }

                var materialData = new LearningMaterialImportDto
                {
                    SyllabusId = syllabus.SyllabusId,
                    MaterialName=materialName,
                    MaterialDetailId = materialDetailId,
                    MaterialNo = int.TryParse(worksheet.Cells[row, 1].Text, out int materialNo) ? materialNo : 0,
                    MaterialQuantity = materialQuantity, // Extracted quantity
                    Purpose = worksheet.Cells[row, 3].Text.Trim(),
                    LearningType = worksheet.Cells[row, 5].Text.Trim(),
                    Note = worksheet.Cells[row, 6].Text.Trim(),
                    Url = url,
                };

                materialList.Add(materialData);
            }

            if (!materialList.Any())
            {
                return true;
                //throw new Exception("Không tìm thấy dữ liệu câu hỏi trong trang.");
            }

            // Remove old materials if syllabus has a previous version
            if (syllabus.PreviousVersionId != null)
            {
                await _learningMaterialService.DeleteLearningMaterialsBySyllabusAsync((Guid)syllabus.PreviousVersionId);
            }

            return await _learningMaterialService.ImportLearningMaterialsAsync(materialList,syllabus.PreviousVersionId,syllabus.SyllabusId);
        }
    }
}
