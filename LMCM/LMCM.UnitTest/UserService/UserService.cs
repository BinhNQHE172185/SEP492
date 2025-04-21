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
}
