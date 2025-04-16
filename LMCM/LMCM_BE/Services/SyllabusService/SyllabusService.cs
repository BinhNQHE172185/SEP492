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
using LMCM_BE.Shared.Constant;
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

        public async Task<int> getSyllabusCountAsync()
        {
            return await _syllabusRepository.CountSyllabusByStatus(GenericStatus.Active);
        }

        public async Task<bool> DeleteSyllabusAsync(Guid id)
        {

            Syllabus syllabus = await _syllabusRepository.GetSyllabusDetailAsync(id);
            if (syllabus == null) throw new KeyNotFoundException("Không tìm thấy đề cương.");

            syllabus.Status = GenericStatus.Inactive;
            syllabus.UpdatedAt = DateTime.UtcNow;
            if (syllabus.Clos.Count > 0)
            {
                foreach(var obj in syllabus.Clos)
                {
                    obj.Status = GenericStatus.Inactive;
                    obj.UpdatedAt = DateTime.UtcNow;
                }
            }
            if (syllabus.ConstructivistQuestions.Count > 0)
            {
                foreach (var obj in syllabus.ConstructivistQuestions)
                {
                    obj.Status = GenericStatus.Inactive;
                    obj.UpdatedAt = DateTime.UtcNow;
                }
            }
            if (syllabus.GradingStructures.Count > 0)
            {
                foreach (var obj in syllabus.GradingStructures)
                {
                    obj.Status = GenericStatus.Inactive;
                    obj.UpdatedAt = DateTime.UtcNow;
                }
            }
            if (syllabus.Schedules.Count > 0)
            {
                foreach (var obj in syllabus.Schedules)
                {
                    obj.Status = GenericStatus.Inactive;
                    obj.UpdatedAt = DateTime.UtcNow;
                }
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _syllabusRepository.UpdateSyllabusAsync(syllabus);
                await _LearningMaterialService.DeleteLearningMaterialsBySyllabusAsync(id);

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
        public async Task<PagedResult<SyllabusHistoryList>> GetSyllabusChangeHistoriesAsync(Guid? subjectId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            if (subjectId == Guid.Empty)
                throw new ArgumentNullException("Subject Id là bắt buộc");

            var subject = await _subjectRepository.GetSubjectByIdAsync((Guid)subjectId);

            if (subject == null)
                throw new KeyNotFoundException("Không tìm thấy môn học.");
            var (data, totalCount) = await _syllabusRepository.GetSyllabusChangeHistoriesAsync(subject.SubjectCode, searchKey, pageIndex, pageSize);

            var dataDtos = _mapper.Map<List<SyllabusHistoryList>>(data);

            return new PagedResult<SyllabusHistoryList>
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

                // Import Syllabus
                var (oldSyllabusId, syllabusId) = await ImportSyllabusSheet(workbook.Worksheets["Syllabus"]);

                // Import different sheets (CLO, Schedule, Grading, etc.)
                await ImportCLOSheet(workbook.Worksheets["CLO"], syllabusId, oldSyllabusId);
                await ImportScheduleSheet(workbook.Worksheets["Schedule"], syllabusId, oldSyllabusId);
                await ImportGradingStructureSheet(workbook.Worksheets["Grading structure"], syllabusId, oldSyllabusId);
                await ImportConstructivistQuestionSheet(workbook.Worksheets["Constructivist Question"], syllabusId, oldSyllabusId);
                await ImportMaterialsSheet(workbook.Worksheets["Materials"], syllabusId, oldSyllabusId, keepUserCreated);

                // Commit the transaction after successful imports
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of any error
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task<(Guid? oldSyllabusId, Guid syllabusId)> ImportSyllabusSheet(ExcelWorksheet worksheet)
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

                var existingSyllabus = await _syllabusRepository.GetSyllabusByCourseCodeAsync(syllabus.CourseCode);

                syllabus.SyllabusId = Guid.NewGuid();
                syllabus.Status = GenericStatus.Active;
                syllabus.CreatedAt = DateTime.UtcNow;
                syllabus.UpdatedAt = DateTime.UtcNow;

                await _syllabusRepository.ImportSyllabusAsync(syllabus);

                // Delete old entities if syllabus already exists
                if (existingSyllabus != null)
                {
                    existingSyllabus.Status = GenericStatus.Inactive;
                    existingSyllabus.UpdatedAt= DateTime.UtcNow;    
                    await _syllabusRepository.UpdateSyllabusAsync(existingSyllabus);
                }

                return (existingSyllabus?.SyllabusId, syllabus.SyllabusId);
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy môn học {syllabusData.CourseCode}.");
            }
        }
        private async Task<bool> ImportScheduleSheet(ExcelWorksheet worksheet, Guid newSyllabusId, Guid? oldSyllabusId)
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

            var scheduleDtos = new List<ScheduleDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                // Skip if the entire row is empty
                if (worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column].All(cell => string.IsNullOrWhiteSpace(cell.Text)))
                    continue;
                var scheduleData = new ScheduleDto
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

                scheduleDtos.Add(scheduleData);
            }

            if (!scheduleDtos.Any())
            {
                throw new InvalidDataException("Không tìm thấy dữ liệu lịch trình trong trang.");
            }
            var schedules = _mapper.Map<List<Schedule>>(scheduleDtos);
            foreach (var schedule in schedules)
            {
                schedule.SyllabusId = newSyllabusId;
                schedule.ScheduleId = Guid.NewGuid();
                schedule.Status = GenericStatus.Active;
                schedule.CreatedAt = DateTime.UtcNow;
                schedule.UpdatedAt = DateTime.UtcNow;
            }

            await _ScheduleRepository.AddSchedulesAsync(schedules);
            if (oldSyllabusId!=null && oldSyllabusId != Guid.Empty)
            {
                var oldSchedules = await _ScheduleRepository.GetSchedulesBySyllabusAsync((Guid)oldSyllabusId);
                foreach (var schedule in oldSchedules)
                {
                    schedule.Status = GenericStatus.Inactive;
                    schedule.UpdatedAt = DateTime.UtcNow;
                }
                await _ScheduleRepository.UpdateSchedulesAsync(oldSchedules);
            }
            return true;
        }
        private async Task<bool> ImportCLOSheet(ExcelWorksheet worksheet, Guid newSyllabusId, Guid? oldSyllabusId)
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

            var cLODtos = new List<CLODto>();

            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                // Skip if the entire row is empty
                if (worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column].All(cell => string.IsNullOrWhiteSpace(cell.Text)))
                    continue;
                var cloData = new CLODto
                {
                    CloName = worksheet.Cells[row, 2].Text.Trim(),
                    CloDescription = worksheet.Cells[row, 3].Text.Trim()
                };

                cLODtos.Add(cloData);
            }

            if (!cLODtos.Any())
            {
                throw new InvalidDataException("Không tìm thấy CLOs trong trang.");
            }

            var cLOs = _mapper.Map<List<Clo>>(cLODtos);
            foreach (var clo in cLOs)
            {
                clo.SyllabusId = newSyllabusId;
                clo.CloId = Guid.NewGuid();
                clo.Status = GenericStatus.Active;
                clo.CreatedAt = DateTime.UtcNow;
                clo.UpdatedAt = DateTime.UtcNow;
            }
            await _CLORepository.AddCLOsAsync(cLOs);
            if (oldSyllabusId != null && oldSyllabusId != Guid.Empty)
            {
                var oldClos = await _CLORepository.GetCLOsBySyllabusASync((Guid)oldSyllabusId);
                foreach (var clo in oldClos)
                {
                    clo.Status = GenericStatus.Inactive;
                    clo.UpdatedAt = DateTime.UtcNow;
                }
                await _CLORepository.UpdateCLOsAsync(oldClos);
            }
            return true;
        }
        private async Task<bool> ImportGradingStructureSheet(ExcelWorksheet worksheet, Guid newSyllabusId, Guid? oldSyllabusId)
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

            var gradingStructureDtos = new List<GradingStructureDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                // Skip if the entire row is empty
                if (worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column].All(cell => string.IsNullOrWhiteSpace(cell.Text)))
                    continue;
                var gradingData = new GradingStructureDto
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

                gradingStructureDtos.Add(gradingData);
            }

            if (!gradingStructureDtos.Any())
            {
                throw new InvalidDataException("Không tìm thấy dữ liệu cấu trúc điểm trong trang.");
            }

            var gradingStructures = _mapper.Map<List<GradingStructure>>(gradingStructureDtos);
            foreach (var structure in gradingStructures)
            {
                structure.SyllabusId = newSyllabusId;
                structure.StructureId = Guid.NewGuid();
                structure.Status = GenericStatus.Active;
                structure.CreatedAt = DateTime.UtcNow;
                structure.UpdatedAt = DateTime.UtcNow;
            }
            await _GradingStructureRepository.AddGradingStructuresAsync(gradingStructures);
            if (oldSyllabusId != null && oldSyllabusId != Guid.Empty)
            {
                var oldGradingStructures = await _GradingStructureRepository.GetGradingStructuresBySyllabusAsync((Guid)oldSyllabusId);
                foreach (var gradingStructure in gradingStructures)
                {
                    gradingStructure.Status = GenericStatus.Inactive;
                    gradingStructure.UpdatedAt = DateTime.UtcNow;
                }
                await _GradingStructureRepository.UpdateGradingStructuresAsync(oldGradingStructures);
            }
            return true;
        }
        private async Task<bool> ImportConstructivistQuestionSheet(ExcelWorksheet worksheet, Guid newSyllabusId, Guid? oldSyllabusId)
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

            var constructivistQuestionDtos = new List<ConstructivistQuestionDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                // Skip if the entire row is empty
                if (worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column].All(cell => string.IsNullOrWhiteSpace(cell.Text)))
                    continue;
                var questionData = new ConstructivistQuestionDto
                {
                    SessionNo = int.TryParse(worksheet.Cells[row, 2].Text, out int sessionNo) ? sessionNo : 0,
                    QuestionName = worksheet.Cells[row, 3].Text.Trim(),
                    QuestionDetail = worksheet.Cells[row, 4].Text.Trim(),
                };

                constructivistQuestionDtos.Add(questionData);
            }

            if (!constructivistQuestionDtos.Any())
            {
                return true;
                //throw new InvalidDataException("Không tìm thấy dữ liệu câu hỏi trong trang.");
            }
            var constructivistQuestions = _mapper.Map<List<ConstructivistQuestion>>(constructivistQuestionDtos);
            if (constructivistQuestions != null)
            {
                foreach (var question in constructivistQuestions)
                {
                    question.SyllabusId = newSyllabusId;
                    question.QuestionId = Guid.NewGuid();
                    question.Status = GenericStatus.Active;
                    question.CreatedAt = DateTime.UtcNow;
                    question.UpdatedAt = DateTime.UtcNow;
                }
                await _ConstructivistQuestionRepository.AddConstructivistQuestionsAsync(constructivistQuestions);
                if (oldSyllabusId != null && oldSyllabusId != Guid.Empty)
                {
                    var oldConstructivistQuestions = await _ConstructivistQuestionRepository.GetConstructivistQuestionsBySyllabusAsync((Guid)oldSyllabusId);
                    foreach (var question in oldConstructivistQuestions)
                    {
                        question.Status = GenericStatus.Inactive;
                        question.UpdatedAt = DateTime.UtcNow;
                    }
                    await _ConstructivistQuestionRepository.UpdateConstructivistQuestionsAsync(oldConstructivistQuestions);
                }
            }
            return true;
        }
        private async Task<bool> ImportMaterialsSheet(ExcelWorksheet worksheet, Guid newSyllabusId, Guid? oldSyllabusId, bool keepUserCreated)
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

            var learningMaterialDtos = new List<LearningMaterialImportDto>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                // Skip if the entire row is empty
                if (worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column].All(cell => string.IsNullOrWhiteSpace(cell.Text)))
                    continue;
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

                learningMaterialDtos.Add(materialData);
            }

            if (!learningMaterialDtos.Any())
            {
                return true;
                //throw new InvalidDataException("Không tìm thấy dữ liệu câu hỏi trong trang.");
            }

            var learningMaterials = _mapper.Map<List<LearningMaterial>>(learningMaterialDtos);
            if (learningMaterials != null) 
                await _LearningMaterialService.ImportLearningMaterialsAsync(learningMaterials, oldSyllabusId, newSyllabusId, keepUserCreated);
            if (oldSyllabusId != null && oldSyllabusId != Guid.Empty) 
                await _LearningMaterialService.DeleteLearningMaterialsBySyllabusAsync(oldSyllabusId.Value);

            return true;
        }
    }
}
