using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LMCM_BE.DTOs.PloDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.PloRepository;
using LMCM_BE.Services.PloService;
using LMCM_BE.Shared.Constant;
using Moq;
using NUnit.Framework;

namespace LMCM.UnitTest.CurriculumTest
{
    [TestFixture]
    public class PloServiceTests
    {
        private Mock<IPloRepository> _repositoryMock;
        private Mock<IMapper> _mapperMock;
        private PloService _service;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IPloRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new PloService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetPloDetailsByCurriculumIdAsync_ReturnsMappedPloDetailDtoList_WhenCurriculumIdIsValid()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var plos = new List<Plo>
            {
                new Plo { PloId = Guid.NewGuid(), CurriculumId = curriculumId, PloName = "PLO 1", Status = GenericStatus.Active },
                new Plo { PloId = Guid.NewGuid(), CurriculumId = curriculumId, PloName = "PLO 2", Status = GenericStatus.Active }
            };

            var mappedDtos = new List<PloDetailDto>
            {
                new PloDetailDto { PloName = "PLO 1"},
                new PloDetailDto { PloName = "PLO 2" }
            };
            _repositoryMock.Setup(r => r.GetPloDetailsByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(plos);
            _mapperMock.Setup(m => m.Map<List<PloDetailDto>>(plos))
                .Returns(mappedDtos);

            // Act
            var result = await _service.GetPloDetailsByCurriculumIdAsync(curriculumId);

            // Assert
            Assert.AreEqual(mappedDtos, result);
            _repositoryMock.Verify(r => r.GetPloDetailsByCurriculumIdAsync(curriculumId), Times.Once);
            _mapperMock.Verify(m => m.Map<List<PloDetailDto>>(plos), Times.Once);
        }

        [Test]
        public async Task DeletePlosAsync_SetsPloStatusInactiveAndUpdatesTimestamps_WhenPlosExist()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var plo = new Plo { Status = GenericStatus.Active, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
            var plos = new List<Plo> { plo };

            _repositoryMock.Setup(r => r.GetPloByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(plos.ConvertAll(p => (Plo)p));
            _repositoryMock.Setup(r => r.UpdateRangeAsync(It.IsAny<List<Plo>>()))
                .ReturnsAsync(true);

            // Act
            await _service.DeletePlosAsync(curriculumId);

            // Assert
            Assert.AreEqual(GenericStatus.Inactive, plo.Status);
            Assert.That((DateTime.UtcNow - plo.UpdatedAt)?.TotalSeconds, Is.LessThan(5));
        }

        [Test]
        public async Task DeletePlosAsync_ReturnsTrue_WhenRepositoryUpdateSucceeds()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var plos = new List<Plo> { new Plo() };

            _repositoryMock.Setup(r => r.GetPloByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(plos.ConvertAll(p => (Plo)p));
            _repositoryMock.Setup(r => r.UpdateRangeAsync(It.IsAny<List<Plo>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeletePlosAsync(curriculumId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetPloDetailsByCurriculumIdAsync_ReturnsEmptyList_WhenNoPlosExist()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var plos = new List<Plo>();
            var mappedDtos = new List<PloDetailDto>();

            _repositoryMock.Setup(r => r.GetPloDetailsByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(plos);
            _mapperMock.Setup(m => m.Map<List<PloDetailDto>>(plos))
                .Returns(mappedDtos);

            // Act
            var result = await _service.GetPloDetailsByCurriculumIdAsync(curriculumId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task DeletePlosAsync_ReturnsFalse_WhenNoPlosExist()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var plos = new List<Plo>();

            _repositoryMock.Setup(r => r.GetPloByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(plos);

            // Act
            var result = await _service.DeletePlosAsync(curriculumId);

            // Assert
            Assert.IsFalse(result);
            _repositoryMock.Verify(r => r.UpdateRangeAsync(It.IsAny<List<Plo>>()), Times.Never);
        }

        [Test]
        public async Task DeletePlosAsync_ReturnsFalse_WhenRepositoryUpdateFails()
        {
            // Arrange
            var curriculumId = Guid.NewGuid();
            var plos = new List<Plo> { new Plo() };

            _repositoryMock.Setup(r => r.GetPloByCurriculumIdAsync(curriculumId))
                .ReturnsAsync(plos.ConvertAll(p => (Plo)p));
            _repositoryMock.Setup(r => r.UpdateRangeAsync(It.IsAny<List<Plo>>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeletePlosAsync(curriculumId);

            // Assert
            Assert.IsFalse(result);
        }
    }
}