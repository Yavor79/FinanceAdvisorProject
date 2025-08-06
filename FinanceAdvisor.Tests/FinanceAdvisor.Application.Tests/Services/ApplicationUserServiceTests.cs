using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceAdvisor.Application.Services;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Domain.Entities;
using MockQueryable.Moq;  // <-- MockQueryable package for IQueryable mocks

[TestFixture]
public class ApplicationUserServiceTests
{
    private Mock<IApplicationUserRepository> _repoMock = null!;
    private ApplicationUserService _service = null!;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IApplicationUserRepository>();
        _service = new ApplicationUserService(_repoMock.Object);
    }

    [Test]
    public async Task GetAllAsync_ReturnsOnlyNotDeletedUsers()
    {
        var users = new List<ApplicationUser>
        {
            new() { Id = Guid.NewGuid(), Email = "user1@test.com", CreatedAt = DateTime.UtcNow, IsDeleted = false },
            new() { Id = Guid.NewGuid(), Email = "user2@test.com", CreatedAt = DateTime.UtcNow, IsDeleted = true }
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsFalse(result[0].IsDeleted);
        Assert.AreEqual(users[0].Email, result[0].Email);
    }

    [Test]
    public async Task GetAllDeletedAsync_ReturnsOnlyDeletedUsers()
    {
        var users = new List<ApplicationUser>
        {
            new() { Id = Guid.NewGuid(), Email = "user1@test.com", CreatedAt = DateTime.UtcNow, IsDeleted = false },
            new() { Id = Guid.NewGuid(), Email = "user2@test.com", CreatedAt = DateTime.UtcNow, IsDeleted = true }
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = (await _service.GetAllDeletedAsync()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].IsDeleted);
        Assert.AreEqual(users[1].Email, result[0].Email);
    }

    [Test]
    public async Task GetByIdAsync_UserExists_ReturnsUserDto()
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "user@test.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(userId);

        Assert.IsNotNull(result);
        Assert.AreEqual(userId, result!.Id);
        Assert.AreEqual(user.Email, result.Email);
        Assert.IsFalse(result.IsDeleted);
    }

    [Test]
    public async Task GetByIdAsync_UserNotExists_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ApplicationUser?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [Test]
    public async Task UpdateAsync_UserExists_UpdatesEmail()
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "old@test.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _repoMock.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var updateDto = new ApplicationUserDto { Email = "new@test.com" };

        await _service.UpdateAsync(userId, updateDto);

        Assert.AreEqual("new@test.com", user.Email);
        _repoMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_UserNotFound_DoesNothing()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ApplicationUser?)null);

        var updateDto = new ApplicationUserDto { Email = "new@test.com" };

        await _service.UpdateAsync(Guid.NewGuid(), updateDto);

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Test]
    public async Task SoftDeleteAsync_UserExists_SetsIsDeletedTrue()
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, IsDeleted = false };

        _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _repoMock.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        await _service.SoftDeleteAsync(userId);

        Assert.IsTrue(user.IsDeleted);
        _repoMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Test]
    public async Task SoftDeleteAsync_UserNotFound_DoesNothing()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ApplicationUser?)null);

        await _service.SoftDeleteAsync(Guid.NewGuid());

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Test]
    public async Task RestoreAsync_UserExistsAndDeleted_ReturnsTrue()
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, IsDeleted = true };

        _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _repoMock.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var result = await _service.RestoreAsync(userId);

        Assert.IsTrue(result);
        Assert.IsFalse(user.IsDeleted);
        _repoMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Test]
    public async Task RestoreAsync_UserNotFound_ReturnsFalse()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ApplicationUser?)null);

        var result = await _service.RestoreAsync(Guid.NewGuid());

        Assert.IsFalse(result);
    }

    [Test]
    public async Task RestoreAsync_UserNotDeleted_ReturnsFalse()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), IsDeleted = false };
        _repoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _service.RestoreAsync(user.Id);

        Assert.IsFalse(result);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_ValidUser_ReturnsTrue()
    {
        var dto = new ApplicationUserDto { Email = "valid@test.com" };

        _repoMock.Setup(r => r.AddAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(true);

        var result = await _service.CreateAsync(dto);

        Assert.IsTrue(result);
        _repoMock.Verify(r => r.AddAsync(It.Is<ApplicationUser>(u => u.Email == dto.Email)), Times.Once);
    }

    [Test]
    public void CreateAsync_NullDto_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreateAsync(null!));
    }

    [Test]
    public void CreateAsync_EmptyEmail_ThrowsArgumentException()
    {
        var dto = new ApplicationUserDto { Email = " " };
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreateAsync(dto));
    }
}
