using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Application.Services;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable.Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MockQueryable;

namespace FinanceAdvisor.Application.Tests.Services
{
    [TestFixture]
    public class AdvisorServiceTests
    {
        private Mock<IAdvisorRepository> _advisorRepoMock;
        private Mock<IApplicationUserRepository> _userRepoMock;
        private Mock<ILogger<AdvisorService>> _loggerMock;
        private AdvisorService _service;

        [SetUp]
        public void Setup()
        {
            _advisorRepoMock = new Mock<IAdvisorRepository>();
            _userRepoMock = new Mock<IApplicationUserRepository>();
            _loggerMock = new Mock<ILogger<AdvisorService>>();
            _service = new AdvisorService(_advisorRepoMock.Object, _userRepoMock.Object, _loggerMock.Object);
        }

        #region GetAllAsync
        [Test]
        public async Task GetAllAsync_ShouldReturnAllAdvisors_WithMappedNames()
        {
            var advisors = new List<Advisor>
            {
                new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid(), Specialization = Specialization.Credit, IsDeleted = false },
                new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid(), Specialization = Specialization.Investment, IsDeleted = true }
            }.BuildMock();

            _advisorRepoMock.Setup(r => r.GetAllAttached(true)).Returns(advisors);
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((Guid id) => new Domain.Entities.ApplicationUser { Id = id, Email = $"user-{id}@mail.com" });

            var result = (await _service.GetAllAsync()).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(r => r.AdvisorName != null));
        }
        #endregion

        #region GetByIdAsync
        [Test]
        public async Task GetByIdAsync_ShouldReturnDto_WhenExists()
        {
            var advisor = new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid() };
            _advisorRepoMock.Setup(r => r.GetByIdAsync(advisor.AdvisorId, false)).ReturnsAsync(advisor);
            var testUser = new ApplicationUser { Email = "user" };
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testUser);

            var result = await _service.GetByIdAsync(advisor.AdvisorId);

            Assert.NotNull(result);
            Assert.AreEqual(advisor.AdvisorId, result.AdvisorId);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _advisorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false)).ReturnsAsync((Advisor)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }
        #endregion

        #region GetByUserIdAsync
        [Test]
        public async Task GetByUserIdAsync_ShouldReturnDto_WhenExists()
        {
            var advisor = new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid() };
            _advisorRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Advisor, bool>>>()))
                            .ReturnsAsync(advisor);

            var result = await _service.GetByUserIdAsync(advisor.UserId);

            Assert.NotNull(result);
            Assert.AreEqual(advisor.UserId, result.UserId);
        }

        [Test]
        public async Task GetByUserIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _advisorRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Advisor, bool>>>()))
                            .ReturnsAsync((Advisor)null);

            var result = await _service.GetByUserIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }
        #endregion

        #region GetBySpecializationAsync
        [Test]
        public async Task GetBySpecializationAsync_ShouldReturnFilteredList()
        {
            var spec = Specialization.Credit;
            var advisors = new List<Advisor>
            {
                new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid(), Specialization = spec },
                new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid(), Specialization = Specialization.Security }
            }.BuildMock();

            _advisorRepoMock.Setup(r => r.GetAllAttached(false)).Returns(advisors);
            
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((Guid id) => new Domain.Entities.ApplicationUser { Id = id, Email = $"email-{id}@mail.com" });

            var result = (await _service.GetBySpecializationAsync(spec)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.All(r => r.Specialization == spec));
            _loggerMock.VerifyLogContains("[AdvisorService]", Times.AtLeastOnce());
        }
        #endregion

        #region CreateAsync
        [Test]
        public async Task CreateAsync_ShouldAddAdvisor_WithDefaults()
        {
            var dto = new AdvisorDto { UserId = Guid.NewGuid() };

            await _service.CreateAsync(dto);

            _advisorRepoMock.Verify(r => r.AddAsync(It.Is<Advisor>(a =>
                a.AdvisorId != Guid.Empty &&
                a.UserId == dto.UserId &&
                a.Specialization == Specialization.Credit &&
                !a.IsDeleted)), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldRespectProvidedSpecialization()
        {
            var dto = new AdvisorDto { UserId = Guid.NewGuid(), Specialization = Specialization.Investment };

            await _service.CreateAsync(dto);

            _advisorRepoMock.Verify(r => r.AddAsync(It.Is<Advisor>(a =>
                a.Specialization == Specialization.Investment)), Times.Once);
        }
        #endregion

        #region UpdateAsync
        [Test]
        public async Task UpdateAsync_ShouldModifySpecialization_WhenAdvisorExists()
        {
            var advisor = new Advisor { AdvisorId = Guid.NewGuid(), Specialization = Specialization.Credit };
            _advisorRepoMock.Setup(r => r.GetByIdAsync(advisor.AdvisorId, false)).ReturnsAsync(advisor);

            await _service.UpdateAsync(advisor.AdvisorId, new AdvisorDto { Specialization = Specialization.Investment });

            Assert.AreEqual(Specialization.Investment, advisor.Specialization);
            _advisorRepoMock.Verify(r => r.UpdateAsync(advisor), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldNotCallUpdate_WhenAdvisorNotFound()
        {
            _advisorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false)).ReturnsAsync((Advisor)null);

            await _service.UpdateAsync(Guid.NewGuid(), new AdvisorDto());

            _advisorRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Advisor>()), Times.Never);
        }
        #endregion

        #region SoftDeleteAsync
        [Test]
        public async Task SoftDeleteAsync_ShouldSetIsDeletedTrue_WhenFound()
        {
            var advisor = new Advisor { AdvisorId = Guid.NewGuid(), IsDeleted = false };
            _advisorRepoMock.Setup(r => r.GetByIdAsync(advisor.AdvisorId, false)).ReturnsAsync(advisor);

            await _service.SoftDeleteAsync(advisor.AdvisorId);

            Assert.IsTrue(advisor.IsDeleted);
            _advisorRepoMock.Verify(r => r.UpdateAsync(advisor), Times.Once);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldDoNothing_WhenNotFound()
        {
            _advisorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false)).ReturnsAsync((Advisor)null);

            await _service.SoftDeleteAsync(Guid.NewGuid());

            _advisorRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Advisor>()), Times.Never);
        }
        #endregion

        #region RestoreAsync
        [Test]
        public async Task RestoreAsync_ShouldRestore_WhenIsDeleted()
        {
            var advisor = new Advisor { AdvisorId = Guid.NewGuid(), IsDeleted = true };
            _advisorRepoMock.Setup(r => r.GetByIdAsync(advisor.AdvisorId, true)).ReturnsAsync(advisor);

            var result = await _service.RestoreAsync(advisor.AdvisorId);

            Assert.IsTrue(result);
            Assert.IsFalse(advisor.IsDeleted);
            _advisorRepoMock.Verify(r => r.UpdateAsync(advisor), Times.Once);
        }

        [Test]
        public async Task RestoreAsync_ShouldReturnFalse_WhenNotFound()
        {
            _advisorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), true)).ReturnsAsync((Advisor)null);

            var result = await _service.RestoreAsync(Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task RestoreAsync_ShouldReturnFalse_WhenNotDeleted()
        {
            var advisor = new Advisor { AdvisorId = Guid.NewGuid(), IsDeleted = false };
            _advisorRepoMock.Setup(r => r.GetByIdAsync(advisor.AdvisorId, true)).ReturnsAsync(advisor);

            var result = await _service.RestoreAsync(advisor.AdvisorId);

            Assert.IsFalse(result);
            _advisorRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Advisor>()), Times.Never);
        }
        #endregion
    }

    internal static class LoggerMockExtensions
    {
        public static void VerifyLogContains(this Mock<ILogger<AdvisorService>> loggerMock, string containsText, Times times)
        {
            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information || l == LogLevel.Debug),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString().Contains(containsText)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times);
        }
    }
}
