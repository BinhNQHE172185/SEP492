using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMCM_BE.Models;
using LMCM_BE.Repositories.PloSubjectRepository;
using LMCM_BE.Services.PloSubjectService;
using LMCM_BE.Shared.Constant;
using Moq;
using NUnit.Framework;

namespace LMCM.UnitTest.CurriculumTest
{
    public class PloSubjectServiceTests
    {
        private Mock<IPloSubjectRepository> _ploSubjectRepositoryMock;
        private PloSubjectService _service;

        [SetUp]
        public void SetUp()
        {
            _ploSubjectRepositoryMock = new Mock<IPloSubjectRepository>();
            _service = new PloSubjectService(_ploSubjectRepositoryMock.Object);
        }

        [Test]
        public async Task DeletePloSubjectsAsync_ShouldSoftDeleteActivePloSubjects_WhenActiveSubjectsExist()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var plo = new Plo
            {
                PloId = Guid.NewGuid(),
                CurriculumId = curriculumId, 
                PloName = "Sample PLO",
                Status = GenericStatus.Active
            };

            // Create a list of PloSubject instances and assign CurriculumId from Plo
            var ploSubjects = new List<PloSubject>
            {
                new PloSubject
                {
                    PloId = plo.PloId,
                    Plo=plo, 
                    Status = GenericStatus.Active,
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new PloSubject
                {
                    PloId = plo.PloId,
                    Plo=plo, 
                    Status = GenericStatus.Active,
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            _ploSubjectRepositoryMock
                .Setup(r => r.GetPloSubjectByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(ploSubjects);

            _ploSubjectRepositoryMock
                .Setup(r => r.UpdateRangeAsync(It.Is<List<PloSubject>>(p => p.All(s => s.PloId != Guid.Empty))))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeletePloSubjectsAsync(curriculumId);

            // Assert
            Assert.IsTrue(ploSubjects.All(ps => ps.Status == GenericStatus.Inactive));
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeletePloSubjectsAsync_ShouldReturnTrue_WhenUpdateRangeSucceeds()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var plo = new Plo
            {
                PloId = Guid.NewGuid(),
                CurriculumId = curriculumId,
                PloName = "Sample PLO",
                Status = GenericStatus.Active
            };

            // Create a list of PloSubject instances and assign CurriculumId from Plo
            var ploSubjects = new List<PloSubject>
            {
                new PloSubject
                {
                    PloId = plo.PloId,
                    Plo=plo,
                    Status = GenericStatus.Active,
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new PloSubject
                {
                    PloId = plo.PloId,
                    Plo=plo,
                    Status = GenericStatus.Active,
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            _ploSubjectRepositoryMock
                .Setup(r => r.GetPloSubjectByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(ploSubjects);

            _ploSubjectRepositoryMock
                .Setup(r => r.UpdateRangeAsync(It.Is<List<PloSubject>>(p => p.All(s => s.PloId != Guid.Empty))))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeletePloSubjectsAsync(curriculumId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeletePloSubjectsAsync_ShouldUpdateUpdatedAtField_WhenSoftDeletingSubjects()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var plo = new Plo
            {
                PloId = Guid.NewGuid(),
                CurriculumId = curriculumId,
                PloName = "Sample PLO",
                Status = GenericStatus.Active
            };

            // Create a list of PloSubject instances and assign CurriculumId from Plo
            var ploSubjects = new List<PloSubject>
            {
                new PloSubject
                {
                    PloId = plo.PloId,
                    Plo=plo,
                    Status = GenericStatus.Active,
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new PloSubject
                {
                    PloId = plo.PloId,
                    Plo=plo,
                    Status = GenericStatus.Active,
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            _ploSubjectRepositoryMock
                .Setup(r => r.GetPloSubjectByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(ploSubjects);

            _ploSubjectRepositoryMock
                .Setup(r => r.UpdateRangeAsync(It.Is<List<PloSubject>>(p => p.All(s => s.PloId != Guid.Empty))))
                .ReturnsAsync(true);

            // Act
            var before = DateTime.UtcNow;
            await _service.DeletePloSubjectsAsync(curriculumId);
            var after = DateTime.UtcNow;

            // Assert
            Assert.That(ploSubjects[0].UpdatedAt, Is.GreaterThanOrEqualTo(before).And.LessThanOrEqualTo(after));
        }

        [Test]
        public async Task DeletePloSubjectsAsync_ShouldReturnFalse_WhenNoActiveSubjectsExist()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var emptyList = new List<PloSubject>();

            _ploSubjectRepositoryMock
                .Setup(r => r.GetPloSubjectByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _service.DeletePloSubjectsAsync(curriculumId);

            // Assert
            Assert.IsFalse(result);
            _ploSubjectRepositoryMock.Verify(r => r.UpdateRangeAsync(It.Is<List<PloSubject>>(p => p.All(s => s.PloId != Guid.Empty))), Times.Never);
        }

        [Test]
        public async Task DeletePloSubjectsAsync_ShouldReturnFalse_WhenUpdateRangeFails()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var plo = new Plo
            {
                PloId = Guid.NewGuid(),
                CurriculumId = curriculumId,
                PloName = "Sample PLO",
                Status = GenericStatus.Active
            };

            // Create a list of PloSubject instances and assign CurriculumId from Plo
            var ploSubjects = new List<PloSubject>
            {
                new PloSubject
                {
                    PloId = plo.PloId,
                    Plo=plo,
                    Status = GenericStatus.Active,
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new PloSubject
                {
                    PloId = plo.PloId,
                    Plo=plo,
                    Status = GenericStatus.Active,
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            _ploSubjectRepositoryMock
                .Setup(r => r.GetPloSubjectByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(ploSubjects);

            _ploSubjectRepositoryMock
                .Setup(r => r.UpdateRangeAsync(It.Is<List<PloSubject>>(p => p.All(s => s.PloId != Guid.Empty))))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeletePloSubjectsAsync(curriculumId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeletePloSubjectsAsync_ShouldReturnFalse_WhenCurriculumIdIsInvalidOrNonExistent()
        {
            // Arrange
            var invalidCurriculumId = Guid.NewGuid();

            _ploSubjectRepositoryMock
                .Setup(r => r.GetPloSubjectByCurriculumIdAsync(invalidCurriculumId))
                .ReturnsAsync(new List<PloSubject>());

            // Act & Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                var result = await _service.DeletePloSubjectsAsync(invalidCurriculumId);
                Assert.IsFalse(result);
            });
        }
    }
}