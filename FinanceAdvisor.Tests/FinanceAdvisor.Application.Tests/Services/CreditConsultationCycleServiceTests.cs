using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Application.Services;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.Enums;
using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using MockQueryable;

namespace FinanceAdvisor.Application.Tests.Services
{
    [TestFixture]
    public class CreditConsultationCycleServiceTests
    {
        private Mock<ICreditConsultationCycleRepository> _mockRepository = null!;
        private Mock<IAdvisorRepository> _mockAdvisorRepository = null!;
        private Mock<IApplicationUserRepository> _mockUserRepository = null!;
        private CreditConsultationCycleService _service = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ICreditConsultationCycleRepository>();
            _mockAdvisorRepository = new Mock<IAdvisorRepository>();
            _mockUserRepository = new Mock<IApplicationUserRepository>();

            _service = new CreditConsultationCycleService(
                _mockRepository.Object,
                _mockUserRepository.Object,
                _mockAdvisorRepository.Object);
        }

        private CreditConsultationCycleDto CreateDto(Guid? clientId = null, Guid? advisorId = null)
        {
            return new CreditConsultationCycleDto
            {
                Id = Guid.NewGuid(),
                ClientId = clientId ?? Guid.NewGuid(),
                AdvisorId = advisorId ?? Guid.NewGuid(),
                CreditType = CreditType.Mortgage,
                Status = Status.Pending,
                MeetingCount = 2,
                CreatedAt = DateTime.UtcNow
            };
        }

        private FinanceAdvisor.Domain.Entities.CreditConsultationCycle CreateEntity(Guid? clientId = null, Guid? advisorId = null)
        {
            return new FinanceAdvisor.Domain.Entities.CreditConsultationCycle
            {
                Id = Guid.NewGuid(),
                ClientId = clientId ?? Guid.NewGuid(),
                AdvisorId = advisorId ?? Guid.NewGuid(),
                CreditType = CreditType.Individual,
                Status = Status.Confirmed,
                Meetings = new List<FinanceAdvisor.Domain.Entities.Meeting> { new(), new() },
                CreatedAt = DateTime.UtcNow
            };
        }

        private void SetupUserRepoReturnsEmail(Guid userId, string? email)
        {
            _mockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(email == null ? null : new FinanceAdvisor.Domain.Entities.ApplicationUser { Id = userId, Email = email });
        }

        private void SetupAdvisorRepoReturnsUserId(Guid advisorId, Guid? userId, bool overrideBehavior)
        {
            _mockAdvisorRepository
                .Setup(r => r.GetByIdAsync(advisorId, overrideBehavior))
                .ReturnsAsync(userId == null ? null : new FinanceAdvisor.Domain.Entities.Advisor { AdvisorId = advisorId, UserId = userId.Value });
        }

        [Test]
        public async Task GetAllAsync_ReturnsCyclesWithNames()
        {
            var entityList = new List<FinanceAdvisor.Domain.Entities.CreditConsultationCycle> { CreateEntity(), CreateEntity() };

            var mockSet = new List<FinanceAdvisor.Domain.Entities.CreditConsultationCycle>(entityList).BuildMock();

            _mockRepository.Setup(r => r.GetAllAttached()).Returns(mockSet);

            foreach (var entity in entityList)
            {
                SetupUserRepoReturnsEmail(entity.ClientId, "client@example.com");
                var advisorID = Guid.NewGuid();
                SetupAdvisorRepoReturnsUserId(entity.AdvisorId, advisorID, true);
                SetupUserRepoReturnsEmail(advisorID, "advisor@example.com");
            }

            var result = await _service.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(entityList.Count(), result.Count());

            foreach (var dto in result)
            {
                Assert.AreEqual("client@example.com", dto.ClientName);
                Assert.AreEqual("advisor@example.com", dto.AdvisorName);
            }
        }

        [Test]
        public async Task GetAllByManagerIdAsync_ReturnsEmpty_WhenManagerIdIsNullOrEmpty()
        {
            var resultNull = await _service.GetAllByManagerIdAsync(null);
            var resultEmpty = await _service.GetAllByManagerIdAsync(Guid.Empty);

            Assert.IsEmpty(resultNull);
            Assert.IsEmpty(resultEmpty);
        }

        [Test]
        public async Task GetAllByManagerIdAsync_ReturnsFilteredCycles()
        {
            var managerId = Guid.NewGuid();
            var entities = new List<FinanceAdvisor.Domain.Entities.CreditConsultationCycle>
            {
                CreateEntity(advisorId: managerId),
                CreateEntity(advisorId: Guid.NewGuid())
            }.AsQueryable();

            var mockSet = new List<FinanceAdvisor.Domain.Entities.CreditConsultationCycle>(entities).BuildMock();
            _mockRepository.Setup(r => r.GetAllAttached()).Returns(mockSet);

            SetupUserRepoReturnsEmail(managerId, "managerClient@example.com");
            SetupAdvisorRepoReturnsUserId(managerId, Guid.NewGuid(), true);
            SetupUserRepoReturnsEmail(managerId, "managerAdvisor@example.com");

            var result = await _service.GetAllByManagerIdAsync(managerId);

            Assert.AreEqual(1, result.Count());
            var dto = result.First();
            Assert.AreEqual(managerId, dto.AdvisorId);
        }

        [Test]
        public async Task GetAllByClientIdAsync_ReturnsEmpty_WhenIdIsNullOrEmpty()
        {
            var resultNull = await _service.GetAllByClientIdAsync(null);
            var resultEmpty = await _service.GetAllByClientIdAsync(Guid.Empty);

            Assert.IsEmpty(resultNull);
            Assert.IsEmpty(resultEmpty);
        }

        [Test]
        public async Task GetAllByClientIdAsync_ReturnsFilteredCycles()
        {
            var clientId = Guid.NewGuid();
            var advisorId = Guid.NewGuid();
            var advisorUserId = Guid.NewGuid();

            var entity = CreateEntity(clientId: clientId, advisorId: advisorId);
            entity.Advisor = new Advisor
            {
                AdvisorId = advisorId,
                UserId = advisorUserId
            };

            var entities = new List<CreditConsultationCycle>
            {
                entity,
                CreateEntity(clientId: Guid.NewGuid()) // unrelated, should be filtered out
            };

            var mockSet = new List<CreditConsultationCycle>(entities).BuildMock();
            _mockRepository.Setup(r => r.GetAllAttached()).Returns(mockSet);

            SetupUserRepoReturnsEmail(clientId, "client@example.com");
            SetupAdvisorRepoReturnsUserId(advisorId, advisorUserId, true);
            SetupUserRepoReturnsEmail(advisorUserId, "advisor@example.com");

            var result = await _service.GetAllByClientIdAsync(clientId);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(clientId, result.First().ClientId);
            Assert.AreEqual("client@example.com", result.First().ClientName);
            Assert.AreEqual("advisor@example.com", result.First().AdvisorName);
        }


        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenEntityNotFound()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((FinanceAdvisor.Domain.Entities.CreditConsultationCycle?)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsDtoWithNames()
        {
            var entity = CreateEntity();
            _mockRepository.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

            SetupUserRepoReturnsEmail(entity.ClientId, "client@example.com");
            var advisorID = Guid.NewGuid();
            SetupAdvisorRepoReturnsUserId(entity.AdvisorId, advisorID, true);
            SetupUserRepoReturnsEmail(advisorID, "advisor@example.com");

            var result = await _service.GetByIdAsync(entity.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(entity.Id, result!.Id);
            Assert.AreEqual("client@example.com", result.ClientName);
            Assert.AreEqual("advisor@example.com", result.AdvisorName);
        }

        [Test]
        public async Task CreateAsync_ReturnsTrueAndAddsEntity()
        {
            var createDto = new CreateCreditConsultationCycleDto
            {
                ClientId = Guid.NewGuid(),
                AdvisorId = Guid.NewGuid(),
                CreditType = CreditType.Mortgage
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<FinanceAdvisor.Domain.Entities.CreditConsultationCycle>())).Returns(Task.CompletedTask).Verifiable();

            var result = await _service.CreateAsync(createDto);

            Assert.IsTrue(result);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<FinanceAdvisor.Domain.Entities.CreditConsultationCycle>()), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ReturnsFalse_WhenEntityNotFound()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((FinanceAdvisor.Domain.Entities.CreditConsultationCycle?)null);

            var updateDto = new UpdateCreditConsultationCycleDto
            {
                Id = Guid.NewGuid(),
                CreditType = CreditType.Individual,
                Status = Status.Completed
            };

            var result = await _service.UpdateAsync(updateDto);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateAsync_ReturnsTrue_WhenUpdated()
        {
            var entity = CreateEntity();
            _mockRepository.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);
            _mockRepository.Setup(r => r.UpdateAsync(entity)).ReturnsAsync(true);

            var updateDto = new UpdateCreditConsultationCycleDto
            {
                Id = entity.Id,
                CreditType = CreditType.Mortgage,
                Status = Status.Cancelled
            };

            var result = await _service.UpdateAsync(updateDto);

            Assert.IsTrue(result);
            Assert.AreEqual(updateDto.CreditType, entity.CreditType);
            Assert.AreEqual(updateDto.Status, entity.Status);
        }

        [Test]
        public async Task DeleteSelfAsync_ReturnsFalse_WhenEntityNotFoundOrAdvisorMismatch()
        {
            var id = Guid.NewGuid();
            var advisorId = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((FinanceAdvisor.Domain.Entities.CreditConsultationCycle?)null);
            var resultNull = await _service.DeleteSelfAsync(id, advisorId);
            Assert.IsFalse(resultNull);

            var entity = CreateEntity(advisorId: Guid.NewGuid());
            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            var resultMismatch = await _service.DeleteSelfAsync(id, advisorId);
            Assert.IsFalse(resultMismatch);
        }

        [Test]
        public async Task DeleteSelfAsync_ReturnsTrue_WhenDeleted()
        {
            var id = Guid.NewGuid();
            var advisorId = Guid.NewGuid();

            var entity = CreateEntity(advisorId: advisorId);
            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockRepository.Setup(r => r.DeleteAsync(entity)).ReturnsAsync(true);

            var result = await _service.DeleteSelfAsync(id, advisorId);

            Assert.IsTrue(result);
            _mockRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
        }
    }
}
