using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CLORepository;
using LMCM_BE.Repositories.ConstructivistQuestionRepository;
using LMCM_BE.Repositories.GradingStructureRepository;
using LMCM_BE.Repositories.ScheduleRepository;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;
using LMCM_BE.Repositories.SyllabusRepository;
using LMCM_BE.Services.LearningMaterialService;
using LMCM_BE.Services.SyllabusService;
using LMCM_BE.UnitOfWork;
using LMCM_BE.Shared.Constant;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;

namespace LMCM.UnitTest.SyllabusServiceTest
{
    [TestFixture]
    public class SyllabusServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ISyllabusRepository> _syllabusRepositoryMock;
        private Mock<ISubjectRepository> _subjectRepositoryMock;
        private Mock<ICLORepository> _cloRepositoryMock;
        private Mock<IConstructivistQuestionRepository> _constructivistQuestionRepositoryMock;
        private Mock<IScheduleRepository> _scheduleRepositoryMock;
        private Mock<ILearningMaterialService> _learningMaterialServiceMock;
        private Mock<IGradingStructureRepository> _gradingStructureRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private SyllabusService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _syllabusRepositoryMock = new Mock<ISyllabusRepository>();
            _subjectRepositoryMock = new Mock<ISubjectRepository>();
            _cloRepositoryMock = new Mock<ICLORepository>();
            _constructivistQuestionRepositoryMock = new Mock<IConstructivistQuestionRepository>();
            _scheduleRepositoryMock = new Mock<IScheduleRepository>();
            _learningMaterialServiceMock = new Mock<ILearningMaterialService>();
            _gradingStructureRepositoryMock = new Mock<IGradingStructureRepository>();
            _mapperMock = new Mock<IMapper>();

            _service = new SyllabusService(
                _unitOfWorkMock.Object,
                _syllabusRepositoryMock.Object,
                _subjectRepositoryMock.Object,
                _mapperMock.Object,
                _cloRepositoryMock.Object,
                _constructivistQuestionRepositoryMock.Object,
                _scheduleRepositoryMock.Object,
                _learningMaterialServiceMock.Object,
                _gradingStructureRepositoryMock.Object
            );
        }

        [Test]
        public async Task Test_DeleteSyllabusAsync_SuccessfullyDeletesSyllabusAndDependencies()
        {
            var syllabusId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var syllabus = new Syllabus
            {
                SyllabusId = syllabusId,
                Status = GenericStatus.Active,
                Clos = new List<Clo>
                {
                    new Clo { Status = GenericStatus.Active, UpdatedAt = now.AddDays(-1) }
                },
                ConstructivistQuestions = new List<ConstructivistQuestion>
                {
                    new ConstructivistQuestion { Status = GenericStatus.Active, UpdatedAt = now.AddDays(-1) }
                },
                GradingStructures = new List<GradingStructure>
                {
                    new GradingStructure { Status = GenericStatus.Active, UpdatedAt = now.AddDays(-1) }
                },
                Schedules = new List<Schedule>
                {
                    new Schedule { Status = GenericStatus.Active, UpdatedAt = now.AddDays(-1) }
                }
            };

            _syllabusRepositoryMock.Setup(r => r.GetSyllabusDetailAsync(syllabusId)).ReturnsAsync(syllabus);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _syllabusRepositoryMock.Setup(r => r.UpdateSyllabusAsync(It.IsAny<Syllabus>())).ReturnsAsync(true);
            _learningMaterialServiceMock.Setup(l => l.DeleteLearningMaterialsBySyllabusAsync(syllabusId)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _service.DeleteSyllabusAsync(syllabusId);

            Assert.IsTrue(result);
            Assert.AreEqual(GenericStatus.Inactive, syllabus.Status);
            Assert.That((DateTime.UtcNow - syllabus.UpdatedAt)?.TotalSeconds, Is.LessThan(5));
            Assert.IsTrue(syllabus.Clos.All(c => c.Status == GenericStatus.Inactive));
            Assert.IsTrue(syllabus.ConstructivistQuestions.All(q => q.Status == GenericStatus.Inactive));
            Assert.IsTrue(syllabus.GradingStructures.All(g => g.Status == GenericStatus.Inactive));
            Assert.IsTrue(syllabus.Schedules.All(s => s.Status == GenericStatus.Inactive));
        }

        [Test]
        public async Task Test_ImportSyllabusAsync_ValidWorkbook_ImportsSuccessfully()
        {
            using var package = new ExcelPackage();
            var syllabusSheet = package.Workbook.Worksheets.Add("Syllabus");
            syllabusSheet.Cells[1, 1].Value = "No";
            syllabusSheet.Cells[1, 2].Value = "Title";
            syllabusSheet.Cells[1, 3].Value = "Details";
            syllabusSheet.Cells[3, 3].Value = "ProgramName";
            syllabusSheet.Cells[4, 3].Value = "DecisionNo";
            syllabusSheet.Cells[5, 3].Value = "CourseName";
            syllabusSheet.Cells[6, 3].Value = "CourseNameEnglish";
            syllabusSheet.Cells[7, 3].Value = "CourseCode";
            syllabusSheet.Cells[8, 3].Value = "LearningTeachingMethod";
            syllabusSheet.Cells[9, 3].Value = "3";
            syllabusSheet.Cells[10, 3].Value = "DegreeLevel";
            syllabusSheet.Cells[11, 3].Value = "TimeAllocation";
            syllabusSheet.Cells[12, 3].Value = "PreRequisite";
            syllabusSheet.Cells[13, 3].Value = "Description";
            syllabusSheet.Cells[14, 3].Value = "StudentTask";
            syllabusSheet.Cells[15, 3].Value = "Tools";
            syllabusSheet.Cells[16, 3].Value = "Note";
            syllabusSheet.Cells[17, 3].Value = "5";
            syllabusSheet.Cells[18, 3].Value = "10";
            syllabusSheet.Cells[19, 3].Value = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var cloSheet = package.Workbook.Worksheets.Add("CLO");
            cloSheet.Cells[1, 1].Value = "No";
            cloSheet.Cells[1, 2].Value = "CLO Name";
            cloSheet.Cells[1, 3].Value = "CLO Description";
            cloSheet.Cells[2, 2].Value = "CLO1";
            cloSheet.Cells[2, 3].Value = "Description1";
            cloSheet.Cells[3, 2].Value = "CLO2";
            cloSheet.Cells[3, 3].Value = "Description2";

            var scheduleSheet = package.Workbook.Worksheets.Add("Schedule");
            string[] scheduleHeaders = { "Sess.", "Leaning-Teaching Method", "Content", "CLO", "ITU", "Student's materials", "Student's task", "Lecturer's Materials", "Lecturer's task", "Student's materials link", "Lecturer's Materials link" };
            for (int i = 0; i < scheduleHeaders.Length; i++)
                scheduleSheet.Cells[1, i + 1].Value = scheduleHeaders[i];
            scheduleSheet.Cells[2, 1].Value = "1";
            scheduleSheet.Cells[2, 2].Value = "Method";
            scheduleSheet.Cells[2, 3].Value = "Content";
            scheduleSheet.Cells[2, 4].Value = "CLO";
            scheduleSheet.Cells[2, 5].Value = "ITU";
            scheduleSheet.Cells[2, 6].Value = "StudentMat";
            scheduleSheet.Cells[2, 7].Value = "StudentTask";
            scheduleSheet.Cells[2, 8].Value = "LecturerMat";
            scheduleSheet.Cells[2, 9].Value = "LecturerTask";
            scheduleSheet.Cells[2, 10].Value = "StudMatLink";
            scheduleSheet.Cells[2, 11].Value = "LectMatLink";

            var gradingSheet = package.Workbook.Worksheets.Add("Grading structure");
            string[] gradingHeaders = { "#", "Assessment Component\nHạng mục đánh giá", "Assessment Type", "Weight\nTrọng số %", "Part\nPhần", "Minimun value to meet Completion Criteria", "Duration", "CLO", "Type of questions", "Number of questions", "Scope of knowledge and skill of questions", "How?", "Note", "SessionNo", "Reference" };
            for (int i = 0; i < gradingHeaders.Length; i++)
                gradingSheet.Cells[1, i + 1].Value = gradingHeaders[i];
            gradingSheet.Cells[2, 1].Value = "1";
            gradingSheet.Cells[2, 2].Value = "Component";
            gradingSheet.Cells[2, 3].Value = "Type";
            gradingSheet.Cells[2, 4].Value = "50";
            gradingSheet.Cells[2, 5].Value = "1";
            gradingSheet.Cells[2, 6].Value = "10";
            gradingSheet.Cells[2, 7].Value = "60min";
            gradingSheet.Cells[2, 8].Value = "CLO";
            gradingSheet.Cells[2, 9].Value = "MCQ";
            gradingSheet.Cells[2, 10].Value = "10";
            gradingSheet.Cells[2, 11].Value = "Scope";
            gradingSheet.Cells[2, 12].Value = "How";
            gradingSheet.Cells[2, 13].Value = "Note";
            gradingSheet.Cells[2, 14].Value = "1";
            gradingSheet.Cells[2, 15].Value = "Ref";

            var cqSheet = package.Workbook.Worksheets.Add("Constructivist Question");
            cqSheet.Cells[1, 1].Value = "No";
            cqSheet.Cells[1, 2].Value = "SessionNo";
            cqSheet.Cells[1, 3].Value = "Name";
            cqSheet.Cells[1, 4].Value = "Detail";
            cqSheet.Cells[2, 2].Value = "1";
            cqSheet.Cells[2, 3].Value = "Q1";
            cqSheet.Cells[2, 4].Value = "Detail1";

            var materialsSheet = package.Workbook.Worksheets.Add("Materials");
            string[] materialsHeaders = { "No", "MaterialDescription", "Purpose", "ISBN", "Type", "Note", "Author", "Publisher", "Published Date", "Edition" };
            for (int i = 0; i < materialsHeaders.Length; i++)
                materialsSheet.Cells[1, i + 1].Value = materialsHeaders[i];
            materialsSheet.Cells[2, 2].Value = "Material1";
            materialsSheet.Cells[2, 3].Value = "Purpose1";
            materialsSheet.Cells[2, 4].Value = "ISBN1";
            materialsSheet.Cells[2, 5].Value = "Type1";
            materialsSheet.Cells[2, 6].Value = "Note1";
            materialsSheet.Cells[2, 7].Value = "Author1";
            materialsSheet.Cells[2, 8].Value = "Publisher1";
            materialsSheet.Cells[2, 9].Value = DateTime.UtcNow.ToString("yyyy-MM-dd");
            materialsSheet.Cells[2, 10].Value = "Edition1";

            var subject = new Subject { SubjectId = Guid.NewGuid(), SubjectCode = "CourseCode" };
            _subjectRepositoryMock.Setup(r => r.GetSubjectByCodeAsync(It.IsAny<string>())).ReturnsAsync(subject);
            _syllabusRepositoryMock.Setup(r => r.GetSyllabusByCourseCodeAsync(It.IsAny<string>())).ReturnsAsync((Syllabus)null);
            _syllabusRepositoryMock.Setup(r => r.ImportSyllabusAsync(It.IsAny<Syllabus>())).ReturnsAsync(true);
            _syllabusRepositoryMock.Setup(r => r.UpdateSyllabusAsync(It.IsAny<Syllabus>())).ReturnsAsync(true);
            _cloRepositoryMock.Setup(r => r.AddCLOsAsync(It.IsAny<List<Clo>>())).ReturnsAsync(true);
            _cloRepositoryMock.Setup(r => r.GetCLOsBySyllabusASync(It.IsAny<Guid>())).ReturnsAsync(new List<Clo>());
            _scheduleRepositoryMock.Setup(r => r.AddSchedulesAsync(It.IsAny<List<Schedule>>())).ReturnsAsync(true);
            _scheduleRepositoryMock.Setup(r => r.GetSchedulesBySyllabusAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Schedule>());
            _gradingStructureRepositoryMock.Setup(r => r.AddGradingStructuresAsync(It.IsAny<List<GradingStructure>>())).ReturnsAsync(true);
            _gradingStructureRepositoryMock.Setup(r => r.GetGradingStructuresBySyllabusAsync(It.IsAny<Guid>())).ReturnsAsync(new List<GradingStructure>());
            _constructivistQuestionRepositoryMock.Setup(r => r.AddConstructivistQuestionsAsync(It.IsAny<List<ConstructivistQuestion>>())).ReturnsAsync(true);
            _constructivistQuestionRepositoryMock.Setup(r => r.GetConstructivistQuestionsBySyllabusAsync(It.IsAny<Guid>())).ReturnsAsync(new List<ConstructivistQuestion>());
            _learningMaterialServiceMock.Setup(r => r.ImportLearningMaterialsAsync(It.IsAny<List<LearningMaterial>>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(true);
            _learningMaterialServiceMock.Setup(r => r.DeleteLearningMaterialsBySyllabusAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<Syllabus>(It.IsAny<object>())).Returns(new Syllabus { CourseCode = "CourseCode" });
            _mapperMock.Setup(m => m.Map<List<Clo>>(It.IsAny<object>())).Returns(new List<Clo> { new Clo() });
            _mapperMock.Setup(m => m.Map<List<Schedule>>(It.IsAny<object>())).Returns(new List<Schedule> { new Schedule() });
            _mapperMock.Setup(m => m.Map<List<GradingStructure>>(It.IsAny<object>())).Returns(new List<GradingStructure> { new GradingStructure() });
            _mapperMock.Setup(m => m.Map<List<ConstructivistQuestion>>(It.IsAny<object>())).Returns(new List<ConstructivistQuestion> { new ConstructivistQuestion() });
            _mapperMock.Setup(m => m.Map<List<LearningMaterial>>(It.IsAny<object>())).Returns(new List<LearningMaterial> { new LearningMaterial() });

            var result = await _service.ImportSyllabusAsync(package.Workbook, false);

            Assert.IsTrue(result);
            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task Test_GetSyllabusesAsync_WithSearchKeyAndPagination_ReturnsPagedResults()
        {
            string searchKey = "test";
            int pageIndex = 2;
            int pageSize = 5;
            var data = new List<Syllabus> { new Syllabus(), new Syllabus() };
            int totalCount = 10;
            _syllabusRepositoryMock.Setup(r => r.GetSyllabusesAsync(searchKey, pageIndex, pageSize)).ReturnsAsync((data, totalCount));
            var mappedDtos = new List<SyllabusListViewDto> { new SyllabusListViewDto(), new SyllabusListViewDto() };
            _mapperMock.Setup(m => m.Map<List<SyllabusListViewDto>>(data)).Returns(mappedDtos);

            var result = await _service.GetSyllabusesAsync(searchKey, pageIndex, pageSize);

            Assert.IsNotNull(result);
            Assert.AreEqual(mappedDtos, result.Items);
            Assert.AreEqual(totalCount, result.TotalCount);
            Assert.AreEqual(pageIndex, result.CurrentPage);
            Assert.AreEqual(pageSize, result.PageSize);
        }

        [Test]
        public void Test_DeleteSyllabusAsync_NonExistentSyllabus_ThrowsKeyNotFoundException()
        {
            var syllabusId = Guid.NewGuid();
            _syllabusRepositoryMock.Setup(r => r.GetSyllabusDetailAsync(syllabusId)).ReturnsAsync((Syllabus)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.DeleteSyllabusAsync(syllabusId));
            Assert.That(ex.Message, Does.Contain("Không tìm thấy đề cương"));
        }

        [Test]
        public void Test_ImportSyllabusAsync_InvalidHeaders_ThrowsInvalidDataException()
        {
            using var package = new ExcelPackage();
            var syllabusSheet = package.Workbook.Worksheets.Add("Syllabus");
            syllabusSheet.Cells[1, 1].Value = "WrongHeader1";
            syllabusSheet.Cells[1, 2].Value = "WrongHeader2";
            syllabusSheet.Cells[1, 3].Value = "WrongHeader3";
            syllabusSheet.Cells[3, 3].Value = "ProgramName";
            syllabusSheet.Cells[4, 3].Value = "DecisionNo";
            syllabusSheet.Cells[5, 3].Value = "CourseName";
            syllabusSheet.Cells[6, 3].Value = "CourseNameEnglish";
            syllabusSheet.Cells[7, 3].Value = "CourseCode";
            syllabusSheet.Cells[8, 3].Value = "LearningTeachingMethod";
            syllabusSheet.Cells[9, 3].Value = "3";
            syllabusSheet.Cells[10, 3].Value = "DegreeLevel";
            syllabusSheet.Cells[11, 3].Value = "TimeAllocation";
            syllabusSheet.Cells[12, 3].Value = "PreRequisite";
            syllabusSheet.Cells[13, 3].Value = "Description";
            syllabusSheet.Cells[14, 3].Value = "StudentTask";
            syllabusSheet.Cells[15, 3].Value = "Tools";
            syllabusSheet.Cells[16, 3].Value = "Note";
            syllabusSheet.Cells[17, 3].Value = "5";
            syllabusSheet.Cells[18, 3].Value = "10";
            syllabusSheet.Cells[19, 3].Value = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var cloSheet = package.Workbook.Worksheets.Add("CLO");
            cloSheet.Cells[1, 1].Value = "No";
            cloSheet.Cells[1, 2].Value = "CLO Name";
            cloSheet.Cells[1, 3].Value = "CLO Description";

            var scheduleSheet = package.Workbook.Worksheets.Add("Schedule");
            string[] scheduleHeaders = { "Sess.", "Leaning-Teaching Method", "Content", "CLO", "ITU", "Student's materials", "Student's task", "Lecturer's Materials", "Lecturer's task", "Student's materials link", "Lecturer's Materials link" };
            for (int i = 0; i < scheduleHeaders.Length; i++)
                scheduleSheet.Cells[1, i + 1].Value = scheduleHeaders[i];

            var gradingSheet = package.Workbook.Worksheets.Add("Grading structure");
            string[] gradingHeaders = { "#", "Assessment Component\nHạng mục đánh giá", "Assessment Type", "Weight\nTrọng số %", "Part\nPhần", "Minimun value to meet Completion Criteria", "Duration", "CLO", "Type of questions", "Number of questions", "Scope of knowledge and skill of questions", "How?", "Note", "SessionNo", "Reference" };
            for (int i = 0; i < gradingHeaders.Length; i++)
                gradingSheet.Cells[1, i + 1].Value = gradingHeaders[i];

            var cqSheet = package.Workbook.Worksheets.Add("Constructivist Question");
            cqSheet.Cells[1, 1].Value = "No";
            cqSheet.Cells[1, 2].Value = "SessionNo";
            cqSheet.Cells[1, 3].Value = "Name";
            cqSheet.Cells[1, 4].Value = "Detail";

            var materialsSheet = package.Workbook.Worksheets.Add("Materials");
            string[] materialsHeaders = { "No", "MaterialDescription", "Purpose", "ISBN", "Type", "Note", "Author", "Publisher", "Published Date", "Edition" };
            for (int i = 0; i < materialsHeaders.Length; i++)
                materialsSheet.Cells[1, i + 1].Value = materialsHeaders[i];

            _subjectRepositoryMock.Setup(r => r.GetSubjectByCodeAsync(It.IsAny<string>())).ReturnsAsync(new Subject { SubjectId = Guid.NewGuid(), SubjectCode = "CourseCode" });

            var ex = Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ImportSyllabusAsync(package.Workbook, false));
            Assert.That(ex.Message, Does.Contain("Định dạng Excel trang Syllabus không hợp lệ"));
        }

        [Test]
        public void Test_ImportSyllabusAsync_ExceptionDuringImport_RollsBackTransaction()
        {
            using var package = new ExcelPackage();
            var syllabusSheet = package.Workbook.Worksheets.Add("Syllabus");
            syllabusSheet.Cells[1, 1].Value = "No";
            syllabusSheet.Cells[1, 2].Value = "Title";
            syllabusSheet.Cells[1, 3].Value = "Details";
            syllabusSheet.Cells[3, 3].Value = "ProgramName";
            syllabusSheet.Cells[4, 3].Value = "DecisionNo";
            syllabusSheet.Cells[5, 3].Value = "CourseName";
            syllabusSheet.Cells[6, 3].Value = "CourseNameEnglish";
            syllabusSheet.Cells[7, 3].Value = "CourseCode";
            syllabusSheet.Cells[8, 3].Value = "LearningTeachingMethod";
            syllabusSheet.Cells[9, 3].Value = "3";
            syllabusSheet.Cells[10, 3].Value = "DegreeLevel";
            syllabusSheet.Cells[11, 3].Value = "TimeAllocation";
            syllabusSheet.Cells[12, 3].Value = "PreRequisite";
            syllabusSheet.Cells[13, 3].Value = "Description";
            syllabusSheet.Cells[14, 3].Value = "StudentTask";
            syllabusSheet.Cells[15, 3].Value = "Tools";
            syllabusSheet.Cells[16, 3].Value = "Note";
            syllabusSheet.Cells[17, 3].Value = "5";
            syllabusSheet.Cells[18, 3].Value = "10";
            syllabusSheet.Cells[19, 3].Value = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var cloSheet = package.Workbook.Worksheets.Add("CLO");
            cloSheet.Cells[1, 1].Value = "No";
            cloSheet.Cells[1, 2].Value = "CLO Name";
            cloSheet.Cells[1, 3].Value = "CLO Description";
            cloSheet.Cells[2, 2].Value = "CLO1";
            cloSheet.Cells[2, 3].Value = "Description1";

            var scheduleSheet = package.Workbook.Worksheets.Add("Schedule");
            string[] scheduleHeaders = { "Sess.", "Leaning-Teaching Method", "Content", "CLO", "ITU", "Student's materials", "Student's task", "Lecturer's Materials", "Lecturer's task", "Student's materials link", "Lecturer's Materials link" };
            for (int i = 0; i < scheduleHeaders.Length; i++)
                scheduleSheet.Cells[1, i + 1].Value = scheduleHeaders[i];
            scheduleSheet.Cells[2, 1].Value = "1";
            scheduleSheet.Cells[2, 2].Value = "Method";
            scheduleSheet.Cells[2, 3].Value = "Content";
            scheduleSheet.Cells[2, 4].Value = "CLO";
            scheduleSheet.Cells[2, 5].Value = "ITU";
            scheduleSheet.Cells[2, 6].Value = "StudentMat";
            scheduleSheet.Cells[2, 7].Value = "StudentTask";
            scheduleSheet.Cells[2, 8].Value = "LecturerMat";
            scheduleSheet.Cells[2, 9].Value = "LecturerTask";
            scheduleSheet.Cells[2, 10].Value = "StudMatLink";
            scheduleSheet.Cells[2, 11].Value = "LectMatLink";

            var gradingSheet = package.Workbook.Worksheets.Add("Grading structure");
            string[] gradingHeaders = { "#", "Assessment Component\nHạng mục đánh giá", "Assessment Type", "Weight\nTrọng số %", "Part\nPhần", "Minimun value to meet Completion Criteria", "Duration", "CLO", "Type of questions", "Number of questions", "Scope of knowledge and skill of questions", "How?", "Note", "SessionNo", "Reference" };
            for (int i = 0; i < gradingHeaders.Length; i++)
                gradingSheet.Cells[1, i + 1].Value = gradingHeaders[i];
            gradingSheet.Cells[2, 1].Value = "1";
            gradingSheet.Cells[2, 2].Value = "Component";
            gradingSheet.Cells[2, 3].Value = "Type";
            gradingSheet.Cells[2, 4].Value = "50";
            gradingSheet.Cells[2, 5].Value = "1";
            gradingSheet.Cells[2, 6].Value = "10";
            gradingSheet.Cells[2, 7].Value = "60min";
            gradingSheet.Cells[2, 8].Value = "CLO";
            gradingSheet.Cells[2, 9].Value = "MCQ";
            gradingSheet.Cells[2, 10].Value = "10";
            gradingSheet.Cells[2, 11].Value = "Scope";
            gradingSheet.Cells[2, 12].Value = "How";
            gradingSheet.Cells[2, 13].Value = "Note";
            gradingSheet.Cells[2, 14].Value = "1";
            gradingSheet.Cells[2, 15].Value = "Ref";

            var cqSheet = package.Workbook.Worksheets.Add("Constructivist Question");
            cqSheet.Cells[1, 1].Value = "No";
            cqSheet.Cells[1, 2].Value = "SessionNo";
            cqSheet.Cells[1, 3].Value = "Name";
            cqSheet.Cells[1, 4].Value = "Detail";
            cqSheet.Cells[2, 2].Value = "1";
            cqSheet.Cells[2, 3].Value = "Q1";
            cqSheet.Cells[2, 4].Value = "Detail1";

            var materialsSheet = package.Workbook.Worksheets.Add("Materials");
            string[] materialsHeaders = { "No", "MaterialDescription", "Purpose", "ISBN", "Type", "Note", "Author", "Publisher", "Published Date", "Edition" };
            for (int i = 0; i < materialsHeaders.Length; i++)
                materialsSheet.Cells[1, i + 1].Value = materialsHeaders[i];
            materialsSheet.Cells[2, 2].Value = "Material1";
            materialsSheet.Cells[2, 3].Value = "Purpose1";
            materialsSheet.Cells[2, 4].Value = "ISBN1";
            materialsSheet.Cells[2, 5].Value = "Type1";
            materialsSheet.Cells[2, 6].Value = "Note1";
            materialsSheet.Cells[2, 7].Value = "Author1";
            materialsSheet.Cells[2, 8].Value = "Publisher1";
            materialsSheet.Cells[2, 9].Value = DateTime.UtcNow.ToString("yyyy-MM-dd");
            materialsSheet.Cells[2, 10].Value = "Edition1";

            var subject = new Subject { SubjectId = Guid.NewGuid(), SubjectCode = "CourseCode" };
            _subjectRepositoryMock.Setup(r => r.GetSubjectByCodeAsync(It.IsAny<string>())).ReturnsAsync(subject);
            _syllabusRepositoryMock.Setup(r => r.GetSyllabusByCourseCodeAsync(It.IsAny<string>())).ReturnsAsync((Syllabus)null);
            _syllabusRepositoryMock.Setup(r => r.ImportSyllabusAsync(It.IsAny<Syllabus>())).ReturnsAsync(true);
            _syllabusRepositoryMock.Setup(r => r.UpdateSyllabusAsync(It.IsAny<Syllabus>())).ReturnsAsync(true);
            _cloRepositoryMock.Setup(r => r.AddCLOsAsync(It.IsAny<List<Clo>>())).ReturnsAsync(true);
            _cloRepositoryMock.Setup(r => r.GetCLOsBySyllabusASync(It.IsAny<Guid>())).ReturnsAsync(new List<Clo>());
            _scheduleRepositoryMock.Setup(r => r.AddSchedulesAsync(It.IsAny<List<Schedule>>())).ReturnsAsync(true);
            _scheduleRepositoryMock.Setup(r => r.GetSchedulesBySyllabusAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Schedule>());
            _gradingStructureRepositoryMock.Setup(r => r.AddGradingStructuresAsync(It.IsAny<List<GradingStructure>>())).ReturnsAsync(true);
            _gradingStructureRepositoryMock.Setup(r => r.GetGradingStructuresBySyllabusAsync(It.IsAny<Guid>())).ReturnsAsync(new List<GradingStructure>());
            _constructivistQuestionRepositoryMock.Setup(r => r.AddConstructivistQuestionsAsync(It.IsAny<List<ConstructivistQuestion>>())).ReturnsAsync(true);
            _constructivistQuestionRepositoryMock.Setup(r => r.GetConstructivistQuestionsBySyllabusAsync(It.IsAny<Guid>())).ReturnsAsync(new List<ConstructivistQuestion>());
            _learningMaterialServiceMock.Setup(r => r.ImportLearningMaterialsAsync(It.IsAny<List<LearningMaterial>>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(true);
            _learningMaterialServiceMock.Setup(r => r.DeleteLearningMaterialsBySyllabusAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ThrowsAsync(new Exception("Commit failed"));
            _unitOfWorkMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<Syllabus>(It.IsAny<object>())).Returns(new Syllabus { CourseCode = "CourseCode" });
            _mapperMock.Setup(m => m.Map<List<Clo>>(It.IsAny<object>())).Returns(new List<Clo> { new Clo() });
            _mapperMock.Setup(m => m.Map<List<Schedule>>(It.IsAny<object>())).Returns(new List<Schedule> { new Schedule() });
            _mapperMock.Setup(m => m.Map<List<GradingStructure>>(It.IsAny<object>())).Returns(new List<GradingStructure> { new GradingStructure() });
            _mapperMock.Setup(m => m.Map<List<ConstructivistQuestion>>(It.IsAny<object>())).Returns(new List<ConstructivistQuestion> { new ConstructivistQuestion() });
            _mapperMock.Setup(m => m.Map<List<LearningMaterial>>(It.IsAny<object>())).Returns(new List<LearningMaterial> { new LearningMaterial() });

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.ImportSyllabusAsync(package.Workbook, false));
            Assert.That(ex.Message, Is.EqualTo("Commit failed"));
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
        }
    }
}