using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CurriculumsSubjectRepository;
using LMCM_BE.Services.CurriculumsSubjectService;
using LMCM_BE.Shared.Constant;
using Moq;
using NUnit.Framework;

namespace LMCM.UnitTest.CurriculumTest
{
    [TestFixture]
    public class CurriculumsSubjectServiceTests
    {
        private Mock<ICurriculumsSubjectRepository> _curriculumsSubjectRepositoryMock;
        private CurriculumsSubjectService _service;

        [SetUp]
        public void SetUp()
        {
            _curriculumsSubjectRepositoryMock = new Mock<ICurriculumsSubjectRepository>();
            _service = new CurriculumsSubjectService(_curriculumsSubjectRepositoryMock.Object);
        }

        [Test]
        public async Task DeleteCurriculumsSubjectAsync_ValidCurriculumId_SubjectsExist_ReturnsTrue()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var subjects = new List<CurriculumsSubject>
            {
                new CurriculumsSubject { SubjectId = Guid.NewGuid(), CurriculumId = curriculumId, Status = GenericStatus.Active, UpdatedAt = DateTime.UtcNow.AddDays(-1) },
                new CurriculumsSubject { SubjectId = Guid.NewGuid(), CurriculumId = curriculumId, Status = GenericStatus.Active, UpdatedAt = DateTime.UtcNow.AddDays(-2) }
            };

            _curriculumsSubjectRepositoryMock
                .Setup(r => r.GetCurriculumsSubjectByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(subjects);

            _curriculumsSubjectRepositoryMock
                .Setup(r => r.UpdateRangeAsync(It.IsAny<List<CurriculumsSubject>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteCurriculumsSubjectAsync(curriculumId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteCurriculumsSubjectAsync_UpdatesStatusAndTimestampForAllSubjects()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var subjects = new List<CurriculumsSubject>
            {
                new CurriculumsSubject { SubjectId = Guid.NewGuid(), CurriculumId = curriculumId, Status = GenericStatus.Active, UpdatedAt = DateTime.UtcNow.AddDays(-1) },
                new CurriculumsSubject { SubjectId = Guid.NewGuid(), CurriculumId = curriculumId, Status = GenericStatus.Active, UpdatedAt = DateTime.UtcNow.AddDays(-2) }
            };

            _curriculumsSubjectRepositoryMock
                .Setup(r => r.GetCurriculumsSubjectByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(subjects);

            _curriculumsSubjectRepositoryMock
                .Setup(r => r.UpdateRangeAsync(It.IsAny<List<CurriculumsSubject>>()))
                .ReturnsAsync(true);

            var before = DateTime.UtcNow;

            // Act
            await _service.DeleteCurriculumsSubjectAsync(curriculumId);

            var after = DateTime.UtcNow;

            // Assert
            Assert.IsTrue(subjects.All(cs => cs.Status == GenericStatus.Inactive));
            Assert.IsTrue(subjects.All(cs => cs.UpdatedAt >= before && cs.UpdatedAt <= after));
        }

        [Test]
        public async Task DeleteCurriculumsSubjectAsync_CallsUpdateRangeAsyncWithCorrectSubjects()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var subjects = new List<CurriculumsSubject>
            {
                new CurriculumsSubject { SubjectId = Guid.NewGuid(), CurriculumId = curriculumId, Status = GenericStatus.Active, UpdatedAt = DateTime.UtcNow.AddDays(-1) },
                new CurriculumsSubject { SubjectId = Guid.NewGuid(), CurriculumId = curriculumId, Status = GenericStatus.Active, UpdatedAt = DateTime.UtcNow.AddDays(-2) }
            };

            _curriculumsSubjectRepositoryMock
                .Setup(r => r.GetCurriculumsSubjectByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(subjects);

            _curriculumsSubjectRepositoryMock
                .Setup(r => r.UpdateRangeAsync(It.IsAny<List<CurriculumsSubject>>()))
                .ReturnsAsync(true);

            // Act
            await _service.DeleteCurriculumsSubjectAsync(curriculumId);

            // Assert
            _curriculumsSubjectRepositoryMock.Verify(r =>
                r.UpdateRangeAsync(It.Is<List<CurriculumsSubject>>(list =>
                    list.All(cs => cs.Status == GenericStatus.Inactive)
                    && list.All(cs => cs.CurriculumId == curriculumId)
                )), Times.Once);
        }

        [Test]
        public async Task DeleteCurriculumsSubjectAsync_NoSubjectsExist_ReturnsFalse()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var emptyList = new List<CurriculumsSubject>();

            _curriculumsSubjectRepositoryMock
                .Setup(r => r.GetCurriculumsSubjectByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _service.DeleteCurriculumsSubjectAsync(curriculumId);

            // Assert
            Assert.IsFalse(result);
            _curriculumsSubjectRepositoryMock.Verify(r => r.UpdateRangeAsync(It.IsAny<List<CurriculumsSubject>>()), Times.Never);
        }

        [Test]
        public void DeleteCurriculumsSubjectAsync_RepositoryThrowsOnGetSubjects_ThrowsException()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();

            _curriculumsSubjectRepositoryMock
                .Setup(r => r.GetCurriculumsSubjectByCurriculumIdAsync(curriculumId))
                .ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _service.DeleteCurriculumsSubjectAsync(curriculumId);
            });
        }

        [Test]
        public async Task DeleteCurriculumsSubjectAsync_UpdateRangeAsyncFails_ReturnsFalse()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var subjects = new List<CurriculumsSubject>
            {
                new CurriculumsSubject { SubjectId = Guid.NewGuid(), CurriculumId = curriculumId, Status = GenericStatus.Active, UpdatedAt = DateTime.UtcNow.AddDays(-1) }
            };

            _curriculumsSubjectRepositoryMock
                .Setup(r => r.GetCurriculumsSubjectByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(subjects);

            _curriculumsSubjectRepositoryMock
                .Setup(r => r.UpdateRangeAsync(It.IsAny<List<CurriculumsSubject>>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteCurriculumsSubjectAsync(curriculumId);

            // Assert
            Assert.IsFalse(result);
        }
    }
}