using Moq;
using NUnit.Framework;
using AutoMapper;
using LMCM_BE.Services;
using LMCM_BE.Repositories;
using LMCM_BE.Models;
using LMCM_BE.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Repositories.CLORepository;
using LMCM_BE.Repositories.ConstructivistQuestionRepository;
using LMCM_BE.Repositories.GradingStructureRepository;
using LMCM_BE.Repositories.ScheduleRepository;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;
using LMCM_BE.Repositories.SyllabusRepository;
using LMCM_BE.Services.LearningMaterialService;
using LMCM_BE.Services.SyllabusService;
using LMCM_BE.Shared.Constant;
using LMCM_BE.UnitOfWork;

namespace LMCM.UnitTest.SyllabusServiceTests
{
    [TestFixture]
    public class SyllabusServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ISyllabusRepository> _syllabusRepoMock;
        private Mock<ISubjectRepository> _subjectRepoMock;
        private Mock<ICLORepository> _cloRepoMock;
        private Mock<IConstructivistQuestionRepository> _cqRepoMock;
        private Mock<IScheduleRepository> _scheduleRepoMock;
        private Mock<ILearningMaterialService> _lmServiceMock;
        private Mock<IGradingStructureRepository> _gradingRepoMock;
        private Mock<IMapper> _mapperMock;

        private SyllabusService _syllabusService;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _syllabusRepoMock = new Mock<ISyllabusRepository>();
            _subjectRepoMock = new Mock<ISubjectRepository>();
            _cloRepoMock = new Mock<ICLORepository>();
            _cqRepoMock = new Mock<IConstructivistQuestionRepository>();
            _scheduleRepoMock = new Mock<IScheduleRepository>();
            _lmServiceMock = new Mock<ILearningMaterialService>();
            _gradingRepoMock = new Mock<IGradingStructureRepository>();
            _mapperMock = new Mock<IMapper>();

            _syllabusService = new SyllabusService(
                _unitOfWorkMock.Object,
                _syllabusRepoMock.Object,
                _subjectRepoMock.Object,
                _mapperMock.Object,
                _cloRepoMock.Object,
                _cqRepoMock.Object,
                _scheduleRepoMock.Object,
                _lmServiceMock.Object,
                _gradingRepoMock.Object
            );
        }

        // ============================
        // Tests for DeleteSyllabusAsync
        // ============================

        [Test]
        public async Task DeleteSyllabusAsync_ValidId_DeletesSuccessfully()
        {
            var syllabusId = Guid.NewGuid();
            var syllabus = new Syllabus
            {
                SyllabusId = syllabusId,
                Status = GenericStatus.Active,
                Clos = new List<Clo> { new Clo() },
                ConstructivistQuestions = new List<ConstructivistQuestion> { new ConstructivistQuestion() },
                GradingStructures = new List<GradingStructure> { new GradingStructure() },
                Schedules = new List<Schedule> { new Schedule() }
            };

            _syllabusRepoMock.Setup(r => r.GetSyllabusDetailAsync(syllabusId))
                             .ReturnsAsync(syllabus);

            _lmServiceMock.Setup(s => s.DeleteLearningMaterialsBySyllabusAsync(syllabusId))
                          .ReturnsAsync(true);

            _syllabusRepoMock.Setup(r => r.UpdateSyllabusAsync(It.IsAny<Syllabus>()))
                         .ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _syllabusService.DeleteSyllabusAsync(syllabusId);

            Assert.IsTrue(result);
            Assert.AreEqual(GenericStatus.Inactive, syllabus.Status);
            _syllabusRepoMock.Verify(r => r.UpdateSyllabusAsync(It.Is<Syllabus>(s => s.SyllabusId == syllabusId)), Times.Once);
            _lmServiceMock.Verify(s => s.DeleteLearningMaterialsBySyllabusAsync(syllabusId), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public void DeleteSyllabusAsync_SyllabusNotFound_ThrowsKeyNotFoundException()
        {
            var syllabusId = Guid.NewGuid();
            _syllabusRepoMock.Setup(r => r.GetSyllabusDetailAsync(syllabusId)).ReturnsAsync((Syllabus)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _syllabusService.DeleteSyllabusAsync(syllabusId));
            Assert.That(ex.Message, Is.EqualTo("Không tìm thấy đề cương."));
        }

        // ============================
        // Tests for GetSyllabusesAsync
        // ============================

        [Test]
        public async Task GetSyllabusesAsync_ValidData_ReturnsPagedResult()
        {
            var syllabusList = new List<Syllabus> { new Syllabus { SyllabusId = Guid.NewGuid() } };
            var totalCount = 1;
            var searchKey = "some key";
            var pageIndex = 1;
            var pageSize = 10;

            _syllabusRepoMock.Setup(r => r.GetSyllabusesAsync(searchKey, pageIndex, pageSize))
                             .ReturnsAsync((syllabusList, totalCount));

            var result = await _syllabusService.GetSyllabusesAsync(searchKey, pageIndex, pageSize);

            Assert.AreEqual(totalCount, result.TotalCount);
            Assert.AreEqual(syllabusList.Count, result.Items.Count);
            _syllabusRepoMock.Verify(r => r.GetSyllabusesAsync(searchKey, pageIndex, pageSize), Times.Once);
        }

        [Test]
        public async Task GetSyllabusesAsync_NoSearch_ReturnsList()
        {
            var syllabusList = new List<Syllabus> { new Syllabus { SyllabusId = Guid.NewGuid() } };

            _syllabusRepoMock.Setup(r => r.GetSyllabusesAsync(null)).ReturnsAsync(syllabusList);

            var result = await _syllabusService.GetSyllabusesAsync(null);

            Assert.AreEqual(syllabusList.Count, result.Count);
            _syllabusRepoMock.Verify(r => r.GetSyllabusesAsync(null), Times.Once);
        }

        // ============================
        // Tests for GetSyllabusDetailAsync
        // ============================

        [Test]
        public async Task GetSyllabusDetailAsync_ValidId_ReturnsSyllabusDetailDto()
        {
            var syllabusId = Guid.NewGuid();
            var syllabus = new Syllabus { SyllabusId = syllabusId };

            _syllabusRepoMock.Setup(r => r.GetSyllabusDetailAsync(syllabusId)).ReturnsAsync(syllabus);

            var result = await _syllabusService.GetSyllabusDetailAsync(syllabusId);

            Assert.NotNull(result);
            Assert.AreEqual(syllabusId, result.SyllabusId);
            _syllabusRepoMock.Verify(r => r.GetSyllabusDetailAsync(syllabusId), Times.Once);
        }

        [Test]
        public void GetSyllabusDetailAsync_SyllabusNotFound_ThrowsKeyNotFoundException()
        {
            var syllabusId = Guid.NewGuid();
            _syllabusRepoMock.Setup(r => r.GetSyllabusDetailAsync(syllabusId)).ReturnsAsync((Syllabus)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _syllabusService.GetSyllabusDetailAsync(syllabusId));
            Assert.That(ex.Message, Is.EqualTo($"Không tìm thấy đề cương với ID: {syllabusId}"));
        }
    }
}