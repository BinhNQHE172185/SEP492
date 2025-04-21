using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.AcceptanceRecordRepository;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Services.UserService;
using LMCM_BE.UnitOfWork;
using LMCM_BE.Utilities;
using LMCM_BE.Services.AcceptanceRecordService;
using Moq;
using NUnit.Framework;
using LMCM_BE.Shared.Constant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LMCM.UnitTest.AcceptanceRecordTest
{
    [TestFixture]
    public class AcceptanceRecordServiceTests
    {
        private Mock<IAcceptanceRecordRepository> _acceptanceRecordRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IContractRepository> _contractRepositoryMock;
        private Mock<IGoogleDriveService> _googleDriveServiceMock;
        private Mock<IFileHelper> _fileHelperMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IConfiguration> _configuration;
        private AcceptanceRecordService _service;

        [SetUp]
        public void SetUp()
        {
            _acceptanceRecordRepositoryMock = new Mock<IAcceptanceRecordRepository>();
            _mapperMock = new Mock<IMapper>();
            _contractRepositoryMock = new Mock<IContractRepository>();
            _googleDriveServiceMock = new Mock<IGoogleDriveService>();
            _fileHelperMock = new Mock<IFileHelper>();
            _userServiceMock = new Mock<IUserService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _configuration = new Mock<IConfiguration>();

            _service = new AcceptanceRecordService(
                _acceptanceRecordRepositoryMock.Object,
                _mapperMock.Object,
                _contractRepositoryMock.Object,
                _googleDriveServiceMock.Object,
                _fileHelperMock.Object,
                _userServiceMock.Object,
                _unitOfWorkMock.Object,
                _configuration.Object
            );
        }

        [Test]
        public async Task GetAcceptanceRecordsAsync_ReturnsPagedResult_ForValidUser()
        {
            var user = new UserProfileResponseDto { Id = Guid.NewGuid(), Email = "test@example.com", Roles = new List<string> { "Staff" } };
            var items = new List<AcceptanceRecord> { new AcceptanceRecord() };
            var mappedItems = new List<AcceptanceRecordListDto> { new AcceptanceRecordListDto() };
            _userServiceMock.Setup(x => x.GetProfileFromCookie()).ReturnsAsync(user);
            _acceptanceRecordRepositoryMock.Setup(x => x.GetAcceptanceRecordsAsync(true, user.Id, "search", 1, 10)).ReturnsAsync((items, 1));
            _mapperMock.Setup(x => x.Map<List<AcceptanceRecordListDto>>(items)).Returns(mappedItems);

            var result = await _service.GetAcceptanceRecordsAsync("search", 1, 10);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(mappedItems, result.Items);
            Assert.AreEqual(1, result.CurrentPage);
            Assert.AreEqual(10, result.PageSize);
        }

        [Test]
        public async Task CreateAcceptanceRecordAsync_Succeeds_WithValidInput()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("file.pdf");
            var dto = new AcceptanceRecordCreateDto
            {
                Title = "UniqueTitle",
                ContractId = Guid.NewGuid(),
                File = fileMock.Object
            };

            var user = new UserProfileResponseDto { Id = Guid.NewGuid(), Email = "user@example.com", Roles = new List<string> { "Staff" } };
            var contract = new Contract();
            var acceptanceRecord = new AcceptanceRecord();

            _acceptanceRecordRepositoryMock.Setup(x => x.GetDuplicatedTitleIdAsync(dto.Title)).ReturnsAsync((Guid?)null);
            _userServiceMock.Setup(x => x.GetProfileFromCookie()).ReturnsAsync(user);
            _contractRepositoryMock.Setup(x => x.GetActiveContractByIdAsync(dto.ContractId)).ReturnsAsync(contract);
            _googleDriveServiceMock.Setup(x => x.UploadFileAsync(dto.File,"folderId")).ReturnsAsync("fileUrl");
            _googleDriveServiceMock.Setup(x => x.SharePdfFileWithUserAsync("fileUrl", user.Email,"reader")).ReturnsAsync(true);
            _mapperMock.Setup(x => x.Map<AcceptanceRecord>(dto)).Returns(acceptanceRecord);
            _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _acceptanceRecordRepositoryMock.Setup(x => x.CreateAcceptanceRecordAsync(It.IsAny<AcceptanceRecord>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _service.CreateAcceptanceRecordAsync(dto);

            Assert.IsTrue(result);
            _acceptanceRecordRepositoryMock.Verify(x => x.CreateAcceptanceRecordAsync(It.IsAny<AcceptanceRecord>()), Times.Once);
        }

        [Test]
        public async Task UpdateAcceptanceRecordAsync_UpdatesRecordAndFile_WhenAuthorized()
        {
            var acceptanceId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("file.pdf");
            var dto = new AcceptanceRecordUpdateDto
            {
                Title = "UniqueTitle",
                ContractId = Guid.NewGuid(),
                File = fileMock.Object
            };

            var acceptanceRecord = new AcceptanceRecord { AuthorId = Guid.NewGuid(), Url = "oldUrl" };
            var user = new UserProfileResponseDto { Id = acceptanceRecord.AuthorId, Email = "user@example.com", Roles = new List<string> { "Staff" } };
            var contract = new Contract();

            _acceptanceRecordRepositoryMock.Setup(x => x.GetDuplicatedTitleIdAsync(dto.Title)).ReturnsAsync((Guid?)null);
            _acceptanceRecordRepositoryMock.Setup(x => x.GetActiveAcceptanceRecordByIdAsync(acceptanceId)).ReturnsAsync(acceptanceRecord);
            _contractRepositoryMock.Setup(x => x.GetActiveContractByIdAsync(dto.ContractId)).ReturnsAsync(contract);
            _userServiceMock.Setup(x => x.GetProfileFromCookie()).ReturnsAsync(user);
            _fileHelperMock.Setup(x => x.ComputeFileHashAsync(dto.File)).ReturnsAsync("newhash");
            _googleDriveServiceMock.Setup(x => x.ComputeGoogleDriveFileHashAsync(acceptanceRecord.Url)).ReturnsAsync("oldhash");
            _googleDriveServiceMock.Setup(x => x.UploadFileAsync(dto.File, "folderId")).ReturnsAsync("newFileUrl");
            _googleDriveServiceMock.Setup(x => x.SharePdfFileWithUserAsync("newFileUrl", user.Email,"reader")).ReturnsAsync(true);
            _mapperMock.Setup(x => x.Map(dto, acceptanceRecord));
            _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _acceptanceRecordRepositoryMock.Setup(x => x.UpdateAcceptanceRecordAsync(acceptanceRecord)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _service.UpdateAcceptanceRecordAsync(acceptanceId, dto);

            Assert.AreEqual(acceptanceId, result);
            _acceptanceRecordRepositoryMock.Verify(x => x.UpdateAcceptanceRecordAsync(acceptanceRecord), Times.Once);
            Assert.AreEqual("newFileUrl", acceptanceRecord.Url);
        }

        [Test]
        public void GetAcceptanceRecordsAsync_ThrowsUnauthorized_WhenUserInvalid()
        {
            _userServiceMock.Setup(x => x.GetProfileFromCookie()).ReturnsAsync((UserProfileResponseDto)null);

            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.GetAcceptanceRecordsAsync(null, 1, 10)
            );
            Assert.That(ex.Message, Does.Contain("Không tìm thấy người dùng"));
        }

        [Test]
        public void CreateAcceptanceRecordAsync_ThrowsException_OnDuplicateTitle()
        {
            var dto = new AcceptanceRecordCreateDto { Title = "DuplicateTitle" };
            _acceptanceRecordRepositoryMock.Setup(x => x.GetDuplicatedTitleIdAsync(dto.Title)).ReturnsAsync(Guid.NewGuid());

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.CreateAcceptanceRecordAsync(dto)
            );
            Assert.That(ex.Message, Does.Contain("Tiêu đề đã tồn tại."));
        }

        [Test]
        public void UpdateOrDeleteAcceptanceRecordAsync_ThrowsException_WhenRecordNotFound()
        {
            var acceptanceId = Guid.NewGuid();
            var updateDto = new AcceptanceRecordUpdateDto { Title = "Title", ContractId = Guid.NewGuid() };
            _acceptanceRecordRepositoryMock.Setup(x => x.GetActiveAcceptanceRecordByIdAsync(acceptanceId)).ReturnsAsync((AcceptanceRecord)null);

            var updateEx = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _service.UpdateAcceptanceRecordAsync(acceptanceId, updateDto)
            );
            Assert.That(updateEx.Message, Does.Contain("Không tìm thấy biên bản nghiệm thu."));

            _acceptanceRecordRepositoryMock.Setup(x => x.GetActiveAcceptanceRecordByIdAsync(acceptanceId)).ReturnsAsync((AcceptanceRecord)null);

            var deleteEx = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _service.SoftDeleteAcceptanceRecordAsync(acceptanceId)
            );
            Assert.That(deleteEx.Message, Does.Contain("Không tìm thấy biên bản nghiệm thu hoặc đã bị xóa."));
        }

        [Test]
        public async Task SoftDeleteAcceptanceRecordAsync_SoftDeletesRecord_WhenAuthorized()
        {
            var acceptanceId = Guid.NewGuid();
            var acceptanceRecord = new AcceptanceRecord { AuthorId = Guid.NewGuid(), Status = GenericStatus.Active };
            var user = new UserProfileResponseDto { Id = acceptanceRecord.AuthorId, Email = "user@example.com", Roles = new List<string> { "Staff" } };

            _acceptanceRecordRepositoryMock.Setup(x => x.GetActiveAcceptanceRecordByIdAsync(acceptanceId)).ReturnsAsync(acceptanceRecord);
            _userServiceMock.Setup(x => x.GetProfileFromCookie()).ReturnsAsync(user);
            _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _acceptanceRecordRepositoryMock.Setup(x => x.UpdateAcceptanceRecordAsync(acceptanceRecord)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _service.SoftDeleteAcceptanceRecordAsync(acceptanceId);

            Assert.IsTrue(result);
            Assert.AreEqual(GenericStatus.Inactive, acceptanceRecord.Status);
            _acceptanceRecordRepositoryMock.Verify(x => x.UpdateAcceptanceRecordAsync(acceptanceRecord), Times.Once);
        }

        [Test]
        public void GetAcceptanceRecordDetailAsync_ThrowsUnauthorized_WhenUserNotPermitted()
        {
            var acceptanceId = Guid.NewGuid();
            var acceptanceRecord = new AcceptanceRecord { AuthorId = Guid.NewGuid(), Url = "url" };
            var user = new UserProfileResponseDto { Id = Guid.NewGuid(), Email = "user@example.com", Roles = new List<string> { "Staff" } };

            _acceptanceRecordRepositoryMock.Setup(x => x.GetAcceptanceRecordDetailAsync(acceptanceId)).ReturnsAsync(acceptanceRecord);
            _userServiceMock.Setup(x => x.GetProfileFromCookie()).ReturnsAsync(user);

            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.GetAcceptanceRecordDetailAsync(acceptanceId)
            );
            Assert.That(ex.Message, Does.Contain("Người dùng không có quyền xem biên bản nghiệm thu này."));
        }
    }
}