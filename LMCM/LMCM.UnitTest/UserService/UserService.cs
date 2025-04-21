using AutoMapper;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Repositories.UserRepositoriy;
using LMCM_BE.Services.UserService;
using LMCM_BE.Models;
using Moq;
using LMCM_BE.Services.GoogleDriveService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IGoogleDriveService> _googleDriveServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IConfiguration> _configurationMock;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;

    private UserService _userService;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _googleDriveServiceMock = new Mock<IGoogleDriveService>();
        _mapperMock = new Mock<IMapper>();
        _configurationMock = new Mock<IConfiguration>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _googleDriveServiceMock.Object,
            _mapperMock.Object,
            _configurationMock.Object,
            _httpContextAccessorMock.Object);
    }


    [Test]
    public async Task GetListUser_ReturnsPagedResult_WithRoles()
    {
        // Arrange
        var searchKey = "";
        var pageIndex = 0;
        var pageSize = 0;

        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), Name = "User 1" },
            new User { Id = Guid.NewGuid(), Name = "User 2" }
        };

        var mappedUsers = users.Select(u => new ListUserResponseDto
        {
            Id = u.Id,
            Name = u.Name
        }).ToList();

        _userRepositoryMock
            .Setup(repo => repo.GetListUser(searchKey, pageIndex, pageSize))
            .ReturnsAsync(users);

        _mapperMock
            .Setup(m => m.Map<List<ListUserResponseDto>>(users))
            .Returns(mappedUsers);

        foreach (var user in users)
        {
            _userRepositoryMock
                .Setup(r => r.getRoleAsync(user.Id.ToString()))
                .ReturnsAsync(new List<string> { "Admin", "User" });
        }

        // Act
        var result = await _userService.GetListUser(searchKey, pageIndex, pageSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(mappedUsers.Count, result.Items.Count);
        Assert.AreEqual(users.Count, result.TotalCount);
        Assert.AreEqual(pageIndex, result.CurrentPage);
        Assert.AreEqual(pageSize, result.PageSize);

        foreach (var item in result.Items)
        {
            Assert.IsNotNull(item.Roles);
            CollectionAssert.AreEquivalent(new[] { "Admin", "User" }, item.Roles);
        }
    }

    [Test]
    public async Task CreateStaff_ShouldReturnTrue_WhenStaffIsCreatedAndFolderIsShared()
    {
        // Arrange
        var request = new StaffRequest { StaffId = "staff123" };
        var email = "staff123@fpt.edu.vn";

        _userRepositoryMock
            .Setup(repo => repo.CreateStaff(email))
            .ReturnsAsync(true);

        _googleDriveServiceMock
            .Setup(service => service.ShareFoldersWithUserAsync(email, false, "reader"))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.CreateStaff(request);

        // Assert
        Assert.IsTrue(result);
        _userRepositoryMock.Verify(repo => repo.CreateStaff(email), Times.Once);
        _googleDriveServiceMock.Verify(service => service.ShareFoldersWithUserAsync(email, false, "reader"), Times.Once);
    }

    [Test]
    public async Task CreateStaff_ShouldReturnFalse_WhenStaffCreationFails()
    {
        // Arrange
        var request = new StaffRequest { StaffId = "staff123" };
        var email = "staff123@fpt.edu.vn";

        _userRepositoryMock
            .Setup(repo => repo.CreateStaff(email))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.CreateStaff(request);

        // Assert
        Assert.IsFalse(result);
        _userRepositoryMock.Verify(repo => repo.CreateStaff(email), Times.Once);
        _googleDriveServiceMock.Verify(service => service.ShareFoldersWithUserAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task CreateStaff_ShouldLogWarning_WhenFolderSharingFails()
    {
        // Arrange
        var request = new StaffRequest { StaffId = "staff123" };
        var email = "staff123@fpt.edu.vn";

        _userRepositoryMock
            .Setup(repo => repo.CreateStaff(email))
            .ReturnsAsync(true);

        _googleDriveServiceMock
            .Setup(service => service.ShareFoldersWithUserAsync(email, false, "reader"))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.CreateStaff(request);

        // Assert
        Assert.IsTrue(result);
        _userRepositoryMock.Verify(repo => repo.CreateStaff(email), Times.Once);
        _googleDriveServiceMock.Verify(service => service.ShareFoldersWithUserAsync(email, false, "reader"), Times.Once);
    }
    [Test]
    public async Task GetProfile_ShouldReturnUserProfileResponseDto_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = new User
        {
            Id = Guid.Parse(userId),
            Name = "John Doe",
            Email = "johndoe@example.com"
        };
        var roles = new List<string> { "Admin", "User" };
        var expectedProfile = new UserProfileResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Roles = roles
        };

        _userRepositoryMock
            .Setup(repo => repo.GetProfile(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(repo => repo.getRoleAsync(userId))
            .ReturnsAsync(roles);

        _mapperMock
            .Setup(mapper => mapper.Map<UserProfileResponseDto>(user))
            .Returns(new UserProfileResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });

        // Act
        var result = await _userService.GetProfile(userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedProfile.Id, result.Id);
        Assert.AreEqual(expectedProfile.Name, result.Name);
        Assert.AreEqual(expectedProfile.Email, result.Email);
        Assert.AreEqual(expectedProfile.Roles, result.Roles);

        _userRepositoryMock.Verify(repo => repo.GetProfile(userId), Times.Once);
        _userRepositoryMock.Verify(repo => repo.getRoleAsync(userId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<UserProfileResponseDto>(user), Times.Once);
    }

    [Test]
    public void GetProfile_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        _userRepositoryMock
            .Setup(repo => repo.GetProfile(userId))
            .ReturnsAsync((User)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _userService.GetProfile(userId));
        Assert.AreEqual("User not found", exception.Message);

        _userRepositoryMock.Verify(repo => repo.GetProfile(userId), Times.Once);
        _userRepositoryMock.Verify(repo => repo.getRoleAsync(It.IsAny<string>()), Times.Never);
        _mapperMock.Verify(mapper => mapper.Map<UserProfileResponseDto>(It.IsAny<User>()), Times.Never);
    }
}
