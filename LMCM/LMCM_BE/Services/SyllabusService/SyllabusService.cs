using AutoMapper;
using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.ConstructivistQuestionDtos;
using LMCM_BE.DTOs.GradingStructureDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ScheduleDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CLORepository;
using LMCM_BE.Repositories.ConstructivistQuestionRepository;
using LMCM_BE.Repositories.GradingStructureRepository;
using LMCM_BE.Repositories.ScheduleRepository;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;
using LMCM_BE.Repositories.SyllabusRepository;
using LMCM_BE.Services.LearningMaterialService;
using LMCM_BE.UnitOfWork;
using OfficeOpenXml;

namespace LMCM_BE.Services.SyllabusService
{
    public class SyllabusService : ISyllabusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISyllabusRepository _syllabusRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ICLORepository _CLORepository;
        private readonly IConstructivistQuestionRepository _ConstructivistQuestionRepository;
        private readonly IScheduleRepository _ScheduleRepository;
        private readonly ILearningMaterialService _LearningMaterialService;
        private readonly IGradingStructureRepository _GradingStructureRepository;
        private readonly IMapper _mapper;
        public SyllabusService(IUnitOfWork unitOfWork, ISyllabusRepository syllabusRepository, ISubjectRepository subjectRepository, IMapper mapper, ICLORepository cLORepository,
            IConstructivistQuestionRepository constructivistQuestionRepository, IScheduleRepository scheduleRepository,
            ILearningMaterialService learningMaterialService, IGradingStructureRepository gradingStructureRepository)
        {
            _unitOfWork = unitOfWork;
            _syllabusRepository = syllabusRepository;
            _subjectRepository = subjectRepository;
            _mapper = mapper;
            _CLORepository = cLORepository;
            _ConstructivistQuestionRepository = constructivistQuestionRepository;
            _ScheduleRepository = scheduleRepository;
            _LearningMaterialService = learningMaterialService;
            _GradingStructureRepository = gradingStructureRepository;
        }

        public async Task<bool> DeleteSyllabusAsync(Guid id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                Syllabus syllabus = await _syllabusRepository.GetSyllabusByIdAsync(id);
                if (syllabus == null) throw new KeyNotFoundException("Không tìm thấy đề cương.");

                await _syllabusRepository.DeleteSyllabusAsync(syllabus);

                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var (data, totalCount) = await _syllabusRepository.GetSyllabusesAsync(searchKey, pageIndex, pageSize);

            var dataDtos = _mapper.Map<List<SyllabusListViewDto>>(data);

            return new PagedResult<SyllabusListViewDto>
            {
                Items = dataDtos,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<List<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey)
        {
            var data = await _syllabusRepository.GetSyllabusesAsync(searchKey);

            var dataDtos = _mapper.Map<List<SyllabusListViewDto>>(data);

            return dataDtos;
        }
        public async Task<PagedResult<SyllabusListViewDto>> GetSyllabusChangeHistoriesAsync(Guid? subjectId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            if (subjectId == Guid.Empty)
                throw new ArgumentNullException("Subject Id là bắt buộc");

            var subject = await _subjectRepository.GetSubjectByIdAsync((Guid)subjectId);

            if (subject == null)
                throw new KeyNotFoundException("Không tìm thấy môn học.");
            var (data, totalCount) = await _syllabusRepository.GetSyllabusChangeHistoriesAsync(subject.SubjectCode, searchKey, pageIndex, pageSize);

            var dataDtos = _mapper.Map<List<SyllabusListViewDto>>(data);

            return new PagedResult<SyllabusListViewDto>
            {
                Items = dataDtos,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<SyllabusListViewDto?> GetActiveSyllabusBySubjectIdAsync(Guid subjectId)
        {
            var syllabus = await _syllabusRepository.GetActiveSyllabusBySubjectIdAsync(subjectId);

            var syllabusDto = _mapper.Map<SyllabusListViewDto>(syllabus);

            return syllabusDto;
        }

        public async Task<SyllabusDetailDto> GetSyllabusDetailAsync(Guid? syllabusId)
        {
            if (syllabusId == null)
                throw new ArgumentNullException(nameof(syllabusId), "Syllabus ID là bắt buộc.");

            var syllabus = await _syllabusRepository.GetSyllabusDetailAsync(syllabusId);

            if (syllabus == null)
                throw new KeyNotFoundException($"Không tìm thấy đề cương với ID: {syllabusId}");

            var syllabusDto = _mapper.Map<SyllabusDetailDto>(syllabus);

            return syllabusDto;
        }

        public async Task<SyllabusListViewDto> GetSyllabusByIdAsync(Guid? syllabusId)
        {
            if (syllabusId == null)
                throw new ArgumentNullException(nameof(syllabusId), "Syllabus ID là bắt buộc.");

            var syllabus = await _syllabusRepository.GetSyllabusByIdAsync(syllabusId);

            if (syllabus == null)
                throw new KeyNotFoundException($"Không tìm thấy đề cương với ID: {syllabusId}");
            var syllabusDto = _mapper.Map<SyllabusListViewDto>(syllabus);

            return syllabusDto;
        }
        public async Task<bool> ImportSyllabusAsync(ExcelWorkbook workbook, bool keepUserCreated)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                // Import different sheets (CLO, Schedule, Grading, etc.)
                var cLODtos = await ImportCLOSheet(workbook.Worksheets["CLO"]);
                var scheduleDtos = await ImportScheduleSheet(workbook.Worksheets["Schedule"]);
                var gradingStructureDtos = await ImportGradingStructureSheet(workbook.Worksheets["Grading structure"]);
                var constructivistQuestionDtos = await ImportConstructivistQuestionSheet(workbook.Worksheets["Constructivist Question"]);
                var learningMaterialDtos = await ImportMaterialsSheet(workbook.Worksheets["Materials"]);

                if (scheduleDtos == null)
                    throw new ArgumentNullException("Schedule là bắt buộc.");
                if (cLODtos == null)
                    throw new ArgumentNullException("CLOs là bắt buộc.");
                if (gradingStructureDtos == null)
                    throw new ArgumentNullException("Grading Structures là bắt buộc.");

                // Import Syllabus
                return await ImportSyllabusSheet(workbook.Worksheets["Syllabus"], scheduleDtos, cLODtos, gradingStructureDtos, constructivistQuestionDtos, learningMaterialDtos, keepUserCreated);
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of any error
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message); // Rethrow the exception
            }
        }

        private async Task<bool> ImportSyllabusSheet(ExcelWorksheet worksheet, List<ScheduleInsertDto> scheduleDtos,
            List<CLOInsertDto> cLODtos, List<GradingStructureInsertDto> gradingStructureDtos,
            List<ConstructivistQuestionInsertDto> constructivistQuestionDtos,
            List<LearningMaterialImportDto> learningMaterialDtos, bool keepUserCreated)
        {
            // Validate expected headers
            string[] expectedHeaders = { "No", "Title", "Details" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new InvalidDataException($"Định dạng Excel trang Syllabus không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
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
            Subject subject = await _subjectRepository.GetSubjectByCodeAsync(syllabusData.CourseCode);
            if (subject != null)
            {
                syllabusData.SubjectId = subject.SubjectId;
                var syllabus = _mapper.Map<Syllabus>(syllabusData);
                var schedules = _mapper.Map<List<Schedule>>(scheduleDtos);
                var cLOs = _mapper.Map<List<Clo>>(cLODtos);
                var gradingStructures = _mapper.Map<List<GradingStructure>>(gradingStructureDtos);
                var constructivistQuestions = _mapper.Map<List<ConstructivistQuestion>>(constructivistQuestionDtos);
                var learningMaterials = _mapper.Map<List<LearningMaterial>>(learningMaterialDtos);

                var existingSyllabus = await _syllabusRepository.GetSyllabusByCourseCodeAsync(syllabus.CourseCode);

                syllabus.SyllabusId = Guid.NewGuid();
                syllabus.Status = "Active";
                syllabus.CreatedAt = DateTime.UtcNow;
                syllabus.UpdatedAt = DateTime.UtcNow;

                // Import related entities
                await _CLORepository.ImportCLOsAsync(cLOs, syllabus.SyllabusId);
                if (constructivistQuestions != null) await _ConstructivistQuestionRepository.ImportConstructivistQuestionsAsync(constructivistQuestions, syllabus.SyllabusId);
                await _ScheduleRepository.ImportSchedulesAsync(schedules, syllabus.SyllabusId);
                await _GradingStructureRepository.ImportGradingStructuresAsync(gradingStructures, syllabus.SyllabusId);
                if (learningMaterials != null) await _LearningMaterialService.ImportLearningMaterialsAsync(learningMaterials, existingSyllabus?.SyllabusId, syllabus.SyllabusId, keepUserCreated);
                await _syllabusRepository.ImportSyllabusAsync(syllabus);

                // Delete old entities if syllabus already exists
                if (existingSyllabus != null)
                {
                    await DeleteSyllabusAsync(existingSyllabus.SyllabusId);
                    await _CLORepository.DeleteCLOBySyllabusAsync(existingSyllabus.SyllabusId);
                    await _ConstructivistQuestionRepository.DeleteConstructivistQuestionsBySyllabusAsync(existingSyllabus.SyllabusId);
                    await _ScheduleRepository.DeleteSchedulesBySyllabusAsync(existingSyllabus.SyllabusId);
                    await _GradingStructureRepository.DeleteGradingStructuresBySyllabusAsync(existingSyllabus.SyllabusId);
                    await _LearningMaterialService.DeleteLearningMaterialsBySyllabusAsync(existingSyllabus.SyllabusId);
                }

                // Commit the transaction after successful imports
                await _unitOfWork.CommitAsync();
                return true;
            }
            else
            {
                throw new KeyNotFoundException("Subject not found for syllabus.");
            }
        }
        private async Task<List<ScheduleInsertDto>> ImportScheduleSheet(ExcelWorksheet worksheet)
        {
            // Validate expected headers
            string[] expectedHeaders = { "Sess.", "Leaning-Teaching Method", "Content", "CLO", "ITU", "Student's materials", "Student's task", "Lecturer's Materials", "Lecturer's task", "Student's materials link", "Lecturer's Materials link" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new InvalidDataException($"Định dạng Excel trang Schedule không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
                }
            }

            var scheduleList = new List<ScheduleInsertDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var scheduleData = new ScheduleInsertDto
                {
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
                throw new InvalidDataException("Không tìm thấy dữ liệu lịch trình trong trang.");
            }

            return scheduleList;
        }
        private async Task<List<CLOInsertDto>> ImportCLOSheet(ExcelWorksheet worksheet)
        {
            // Validate expected headers
            string[] expectedHeaders = { "No", "CLO Name", "CLO Description" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new InvalidDataException($"Định dạng Excel trang CLO không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
                }
            }

            var cloList = new List<CLOInsertDto>();

            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var cloData = new CLOInsertDto
                {
                    CloName = worksheet.Cells[row, 2].Text.Trim(),
                    CloDescription = worksheet.Cells[row, 3].Text.Trim()
                };

                cloList.Add(cloData);
            }

            if (!cloList.Any())
            {
                throw new InvalidDataException("Không tìm thấy CLOs trong trang.");
            }

            return cloList;
        }
        private async Task<List<GradingStructureInsertDto>> ImportGradingStructureSheet(ExcelWorksheet worksheet)
        {
            // Validate expected headers
            string[] expectedHeaders = { "#", "Assessment Component\nHạng mục đánh giá", "Assessment Type", "Weight\nTrọng số %", "Part\nPhần", "Minimun value to meet Completion Criteria", "Duration", "CLO", "Type of questions", "Number of questions", "Scope of knowledge and skill of questions", "How?", "Note", "SessionNo", "Reference" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new InvalidDataException($"Định dạng Excel trang Grading Structure không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
                }
            }

            var gradingList = new List<GradingStructureInsertDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var gradingData = new GradingStructureInsertDto
                {
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
                throw new InvalidDataException("Không tìm thấy dữ liệu cấu trúc điểm trong trang.");
            }

            return gradingList;
        }
        private async Task<List<ConstructivistQuestionInsertDto>?> ImportConstructivistQuestionSheet(ExcelWorksheet worksheet)
        {
            // Validate expected headers
            string[] expectedHeaders = { "No", "SessionNo", "Name", "Detail" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new InvalidDataException($"Định dạng Excel trang Constructivist Question không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
                }
            }

            var questionList = new List<ConstructivistQuestionInsertDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var questionData = new ConstructivistQuestionInsertDto
                {
                    SessionNo = int.TryParse(worksheet.Cells[row, 2].Text, out int sessionNo) ? sessionNo : 0,
                    QuestionName = worksheet.Cells[row, 3].Text.Trim(),
                    QuestionDetail = worksheet.Cells[row, 4].Text.Trim(),
                };

                questionList.Add(questionData);
            }

            if (!questionList.Any())
            {
                return null;
                //throw new InvalidDataException("Không tìm thấy dữ liệu câu hỏi trong trang.");
            }

            return questionList;
        }
        private async Task<List<LearningMaterialImportDto>?> ImportMaterialsSheet(ExcelWorksheet worksheet)
        {
            // Validate expected headers
            string[] expectedHeaders = { "No", "MaterialDescription", "Purpose", "ISBN", "Type", "Note", "Author", "Publisher", "Published Date", "Edition" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new InvalidDataException($"Định dạng Excel trang Materials không hợp lệ tại cột {worksheet.Cells[1, col].Text.Trim()} phải là {expectedHeaders[col - 1]}. Vui lòng sử dụng mẫu đúng.");
                }
            }

            var materialList = new List<LearningMaterialImportDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var materialDescription = worksheet.Cells[row, 2].Text.Trim(); // Read MaterialDescription
                string materialName = null;
                string url = null;

                if (!string.IsNullOrEmpty(materialDescription))
                {
                    if (Uri.TryCreate(materialDescription, UriKind.Absolute, out Uri? uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    {
                        url = materialDescription;
                    }
                    materialName = materialDescription;
                }

                var materialData = new LearningMaterialImportDto
                {
                    MaterialName = materialName,
                    Purpose = worksheet.Cells[row, 3].Text.Trim(),
                    LearningType = worksheet.Cells[row, 5].Text.Trim(),
                    Note = worksheet.Cells[row, 6].Text.Trim(),
                    Url = url,
                    Isbn = worksheet.Cells[row, 4].Text.Trim(),
                    Author = worksheet.Cells[row, 7].Text.Trim(),
                    Publisher = worksheet.Cells[row, 8].Text.Trim(),
                    Edition = worksheet.Cells[row, 10].Text.Trim(),
                    PublishedDate = DateTime.TryParse(worksheet.Cells[row, 9].Text, out DateTime tempDate) ? tempDate : (DateTime?)null,
                };

                materialList.Add(materialData);
            }

            if (!materialList.Any())
            {
                return null;
                //throw new InvalidDataException("Không tìm thấy dữ liệu câu hỏi trong trang.");
            }

            return materialList;
        }
    }
}
