using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Application.Services;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.Enums;
using Moq;
// !!! IMPORTANT for when doing async
// use MockQueryable library can mock IQueryable<> objects needed to be passed to the service from the Mock Repo
using MockQueryable;
using MockQueryable.Moq;
using MockQueryable.Core;
using NUnit.Framework;

namespace FinanceAdvisor.Application.Tests.Services
{
    [TestFixture]
    public class AdvisorServiceTests
    {
        private Mock<IAdvisorRepository> _repoMock;
        private AdvisorService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IAdvisorRepository>();
            _service = new AdvisorService(_repoMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllAdvisors()
        {
            var data = new List<Advisor>
            {
                new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid(), Specialization = Specialization.Credit, CreatedAt = DateTime.UtcNow, IsDeleted = false },
                new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid(), Specialization = Specialization.Investment, CreatedAt = DateTime.UtcNow, IsDeleted = false },
            }.BuildMock();

            //var mock = data.BuildMock();
            _repoMock.Setup(r => r.GetAllAttached()).Returns(data);

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnAdvisor_WhenExists()
        {
            var advisor = new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid(), Specialization = Specialization.Credit, CreatedAt = DateTime.UtcNow };
            _repoMock.Setup(r => r.GetByIdAsync(advisor.AdvisorId)).ReturnsAsync(advisor);

            var result = await _service.GetByIdAsync(advisor.AdvisorId);

            Assert.IsNotNull(result);
            Assert.AreEqual(advisor.AdvisorId, result.AdvisorId);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Advisor?)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByUserIdAsync_ShouldReturnAdvisor_WhenExists()
        {
            var userId = Guid.NewGuid();
            var advisor = new Advisor { AdvisorId = Guid.NewGuid(), UserId = userId, Specialization = Specialization.Credit, CreatedAt = DateTime.UtcNow };

            _repoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Advisor, bool>>>()))
                     .ReturnsAsync(advisor);

            var result = await _service.GetByUserIdAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
        }

        [Test]
        public async Task GetBySpecializationAsync_ShouldReturnFilteredAdvisors()
        {
            var spec = Specialization.Credit;
            var data = new List<Advisor>
            {
                new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid(), Specialization = spec, CreatedAt = DateTime.UtcNow },
                new Advisor { AdvisorId = Guid.NewGuid(), UserId = Guid.NewGuid(), Specialization = Specialization.Security, CreatedAt = DateTime.UtcNow },
            }.BuildMock();

            //var mock = data.BuildMock();
            _repoMock.Setup(r => r.GetAllAttached()).Returns(data);

            var result = await _service.GetBySpecializationAsync(spec);

            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.All(a => a.Specialization == spec));
        }

        [Test]
        public async Task CreateAsync_ShouldCallAddAsync()
        {
            var dto = new AdvisorDto
            {
                UserId = Guid.NewGuid(),
                Specialization = Specialization.Credit
            };

            await _service.CreateAsync(dto);

            _repoMock.Verify(r => r.AddAsync(It.IsAny<Advisor>()), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateAdvisor_WhenExists()
        {
            var advisorId = Guid.NewGuid();
            var existing = new Advisor { AdvisorId = advisorId, Specialization = Specialization.Credit };
            var updatedDto = new AdvisorDto { Specialization = Specialization.Investment };

            _repoMock.Setup(r => r.GetByIdAsync(advisorId)).ReturnsAsync(existing);

            await _service.UpdateAsync(advisorId, updatedDto);

            Assert.AreEqual(Specialization.Investment, existing.Specialization);
            _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldDoNothing_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Advisor?)null);

            await _service.UpdateAsync(Guid.NewGuid(), new AdvisorDto());

            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Advisor>()), Times.Never);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldSetIsDeletedTrue_WhenFound()
        {
            var id = Guid.NewGuid();
            var advisor = new Advisor { AdvisorId = id, IsDeleted = false };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(advisor);

            await _service.SoftDeleteAsync(id);

            Assert.IsTrue(advisor.IsDeleted);
            _repoMock.Verify(r => r.UpdateAsync(advisor), Times.Once);
        }

        [Test]
        public async Task RestoreAsync_ShouldRestore_WhenIsDeleted()
        {
            var id = Guid.NewGuid();
            var advisor = new Advisor { AdvisorId = id, IsDeleted = true };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(advisor);

            var result = await _service.RestoreAsync(id);

            Assert.IsTrue(result);
            Assert.IsFalse(advisor.IsDeleted);
            _repoMock.Verify(r => r.UpdateAsync(advisor), Times.Once);
        }

        [Test]
        public async Task RestoreAsync_ShouldReturnFalse_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Advisor?)null);

            var result = await _service.RestoreAsync(Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task RestoreAsync_ShouldReturnFalse_WhenNotDeleted()
        {
            var advisor = new Advisor { AdvisorId = Guid.NewGuid(), IsDeleted = false };

            _repoMock.Setup(r => r.GetByIdAsync(advisor.AdvisorId)).ReturnsAsync(advisor);

            var result = await _service.RestoreAsync(advisor.AdvisorId);

            Assert.IsFalse(result);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Advisor>()), Times.Never);
        }
    }
}
