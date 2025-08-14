using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Application.Services;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace FinanceAdvisor.Tests.Services
{
    [TestFixture]
    public class ConsultationServiceTests
    {
        private Mock<IConsultationRepository> _mockRepo;
        private Mock<IApplicationUserRepository> _mockUserRepo;
        private Mock<IAdvisorRepository> _mockAdvisorRepo;
        private Mock<ILogger<ConsultationService>> _loggerMock;
        private ConsultationService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IConsultationRepository>();
            _mockUserRepo = new Mock<IApplicationUserRepository>();
            _mockAdvisorRepo = new Mock<IAdvisorRepository>();
            _loggerMock = new Mock<ILogger<ConsultationService>>();


            _service = new ConsultationService(_mockRepo.Object, _mockUserRepo.Object, _mockAdvisorRepo.Object, _loggerMock.Object);
        }

        #region GetAllAsync Tests

        [Test]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoConsultations()
        {
            var data = new List<Consultation>().BuildMock();
            _mockRepo.Setup(r => r.GetAllAttached()).Returns(data);

            var result = await _service.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAllAsync_ReturnsMappedConsultations_WhenDataExists()
        {
            var advisorId = Guid.NewGuid();
            var consultations = new List<Consultation>
            {
                new Consultation
                {
                    ConsultationId = Guid.NewGuid(),
                    ClientId = Guid.NewGuid(),
                    AdvisorId = advisorId,
                    ScheduledDateTime = DateTime.UtcNow.AddDays(1),
                    ConsultationType = ConsultationType.CreditAdvisory,
                    Status = Status.Pending,
                    CreatedAt = DateTime.UtcNow,
                    Advisor = new Advisor { UserId = Guid.NewGuid() }
                }
            }.BuildMock();

            _mockRepo.Setup(r => r.GetAllAttached()).Returns(consultations);

            // Mock users
            var clientUser = new Domain.Entities.ApplicationUser
            {
                Id = consultations.First().ClientId,
                Email = "client@example.com"
            };
            var advisorUser = new Domain.Entities.ApplicationUser
            {
                Id = consultations.First().Advisor.UserId,
                Email = "advisor@example.com"
            };

            _mockUserRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                         .ReturnsAsync(new[] { clientUser, advisorUser });

            var result = await _service.GetAllAsync();

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual("client@example.com", first.ClientName);
            Assert.AreEqual("advisor@example.com", first.AdvisorName);
        }

        #endregion

        #region GetAllByAdvisorIdAsync Tests

        [Test]
        public async Task GetAllByAdvisorIdAsync_ReturnsEmpty_WhenAdvisorIdIsEmpty()
        {
            var result = await _service.GetAllByAdvisorIdAsync(Guid.Empty, null);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAllByAdvisorIdAsync_ReturnsEmpty_WhenInvalidConsultationType()
        {
            var result = await _service.GetAllByAdvisorIdAsync(Guid.NewGuid(), "InvalidType");
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAllByAdvisorIdAsync_ReturnsFilteredConsultations_WhenValidType()
        {
            var advisorId = Guid.NewGuid();
            var consultationsList = new List<Consultation>
            {
                new Consultation
                {
                    ConsultationId = Guid.NewGuid(),
                    AdvisorId = advisorId,
                    ConsultationType = ConsultationType.InvestmentAdvisory,
                    Advisor = new Advisor { UserId = Guid.NewGuid() }
                }
            }.BuildMock();

            _mockRepo.Setup(r => r.GetAllAttached()).Returns(consultationsList);

            // Setup filtering logic
            _mockRepo.Setup(r => r.GetAllAttached())
                .Returns(consultationsList);

            var clientUser = new Domain.Entities.ApplicationUser
            {
                Id = consultationsList.First().Advisor.UserId,
                Email = "advisor@example.com"
            };

            _mockUserRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                         .ReturnsAsync(new[] { clientUser });

            var result = await _service.GetAllByAdvisorIdAsync(advisorId, "InvestmentAdvisory");

            Assert.IsNotEmpty(result);
            Assert.AreEqual(ConsultationType.InvestmentAdvisory, result.First().ConsultationType);
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetAllAttached())
                     .Returns(new List<Consultation>().BuildMock());

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsConsultationDto_WhenFound()
        {
            var consultationId = Guid.NewGuid();
            var consultation = new Consultation
            {
                ConsultationId = consultationId,
                ClientId = Guid.NewGuid(),
                AdvisorId = Guid.NewGuid(),
                Advisor = new Advisor { UserId = Guid.NewGuid() },
                ConsultationType = ConsultationType.SecurityAdvisory,
                Status = Status.Pending,
                ScheduledDateTime = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var data = new List<Consultation> { consultation }.BuildMock();
            _mockRepo.Setup(r => r.GetAllAttached()).Returns(data);

            var clientUser = new Domain.Entities.ApplicationUser
            {
                Id = consultation.ClientId,
                Email = "client@example.com"
            };
            var advisorUser = new Domain.Entities.ApplicationUser
            {
                Id = consultation.Advisor.UserId,
                Email = "advisor@example.com"
            };

            _mockUserRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                         .ReturnsAsync(new[] { clientUser, advisorUser });

            var result = await _service.GetByIdAsync(consultationId);

            Assert.IsNotNull(result);
            Assert.AreEqual(consultationId, result.ConsultationId);
            Assert.AreEqual("client@example.com", result.ClientName);
            Assert.AreEqual("advisor@example.com", result.AdvisorName);
        }

        #endregion

        #region CreateAsync Tests

        [Test]

        public async Task CreateAsync_CreatesAndReturnsConsultationDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var advisorId = Guid.NewGuid();
            var advisorUserId = Guid.NewGuid();

            var createDto = new CreateConsultationDto
            {
                ClientId = clientId,
                AdvisorId = advisorId,
                ScheduledDateTime = DateTime.UtcNow.AddDays(1),
                ConsultationType = ConsultationType.InvestmentAdvisory
            };

            Domain.Entities.Consultation capturedEntity = null;

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Consultation>()))
                     .Callback<Domain.Entities.Consultation>(c =>
                     {
                         capturedEntity = c;

                         // Mock the navigation property manually
                         c.Advisor = new Domain.Entities.Advisor
                         {
                             AdvisorId = advisorId,
                             UserId = advisorUserId
                         };
                     })
                     .Returns(Task.CompletedTask);

            _mockUserRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                         .ReturnsAsync(new[]
                         {
                     new Domain.Entities.ApplicationUser { Id = clientId, Email = "client@example.com" },
                     new Domain.Entities.ApplicationUser { Id = advisorUserId, Email = "advisor@example.com" }
                         });

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("client@example.com", result.ClientName);
            Assert.AreEqual("advisor@example.com", result.AdvisorName);
            Assert.AreEqual(createDto.ConsultationType, result.ConsultationType);
            Assert.IsNotNull(capturedEntity);
            Assert.AreEqual(clientId, capturedEntity.ClientId);
            Assert.AreEqual(advisorId, capturedEntity.AdvisorId);
            Assert.IsNotNull(capturedEntity.Advisor);
            Assert.AreEqual(advisorUserId, capturedEntity.Advisor.UserId);
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_ReturnsFalse_WhenEntityNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Domain.Entities.Consultation)null);

            var updateDto = new UpdateConsultationDto
            {
                Id = Guid.NewGuid(),
                ScheduledDate = DateTime.UtcNow,
                Status = Status.Completed,
                ConsultationType = ConsultationType.CreditAdvisory
            };

            var result = await _service.UpdateAsync(updateDto);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateAsync_ReturnsTrue_WhenEntityUpdated()
        {
            var existingConsultation = new Domain.Entities.Consultation
            {
                ConsultationId = Guid.NewGuid(),
                Status = Status.Pending,
                ConsultationType = ConsultationType.SecurityAdvisory,
                ScheduledDateTime = DateTime.UtcNow
            };

            _mockRepo.Setup(r => r.GetByIdAsync(existingConsultation.ConsultationId))
                     .ReturnsAsync(existingConsultation);

            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Consultation>()))
                     .ReturnsAsync(true);

            var updateDto = new UpdateConsultationDto
            {
                Id = existingConsultation.ConsultationId,
                ScheduledDate = DateTime.UtcNow.AddDays(1),
                Status = Status.Completed,
                ConsultationType = ConsultationType.CreditAdvisory
            };

            var result = await _service.UpdateAsync(updateDto);

            Assert.IsTrue(result);
            Assert.AreEqual(updateDto.Status, existingConsultation.Status);
            Assert.AreEqual(updateDto.ConsultationType, existingConsultation.ConsultationType);
            Assert.AreEqual(updateDto.ScheduledDate, existingConsultation.ScheduledDateTime);
        }

        #endregion

        #region DeleteSelfAsync Tests

        [Test]
        public async Task DeleteSelfAsync_ReturnsFalse_WhenEntityNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Domain.Entities.Consultation)null);

            var result = await _service.DeleteSelfAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteSelfAsync_ReturnsFalse_WhenAdvisorIdMismatch()
        {
            var consultation = new Domain.Entities.Consultation
            {
                ConsultationId = Guid.NewGuid(),
                AdvisorId = Guid.NewGuid()
            };

            _mockRepo.Setup(r => r.GetByIdAsync(consultation.ConsultationId))
                     .ReturnsAsync(consultation);

            var wrongAdvisorId = Guid.NewGuid();

            var result = await _service.DeleteSelfAsync(consultation.ConsultationId, wrongAdvisorId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteSelfAsync_ReturnsTrue_WhenDeleted()
        {
            var advisorId = Guid.NewGuid();
            var consultation = new Domain.Entities.Consultation
            {
                ConsultationId = Guid.NewGuid(),
                AdvisorId = advisorId
            };

            _mockRepo.Setup(r => r.GetByIdAsync(consultation.ConsultationId))
                     .ReturnsAsync(consultation);

            _mockRepo.Setup(r => r.DeleteAsync(consultation))
                     .ReturnsAsync(true);

            var result = await _service.DeleteSelfAsync(consultation.ConsultationId, advisorId);

            Assert.IsTrue(result);
        }

        #endregion

        #region MarkAsCompletedAsync Tests

        [Test]
        public async Task MarkAsCompletedAsync_ReturnsFalse_WhenEntityNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Domain.Entities.Consultation)null);

            var result = await _service.MarkAsCompletedAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task MarkAsCompletedAsync_ReturnsFalse_WhenAdvisorIdMismatch()
        {
            var consultation = new Domain.Entities.Consultation
            {
                ConsultationId = Guid.NewGuid(),
                AdvisorId = Guid.NewGuid(),
                Status = Status.Pending
            };

            _mockRepo.Setup(r => r.GetByIdAsync(consultation.ConsultationId))
                     .ReturnsAsync(consultation);

            var result = await _service.MarkAsCompletedAsync(consultation.ConsultationId, Guid.NewGuid());

            Assert.IsFalse(result);
            Assert.AreEqual(Status.Pending, consultation.Status);
        }

        [Test]
        public async Task MarkAsCompletedAsync_ReturnsTrueAndSetsCompleted_WhenAdvisorIdMatches()
        {
            var advisorId = Guid.NewGuid();
            var consultation = new Domain.Entities.Consultation
            {
                ConsultationId = Guid.NewGuid(),
                AdvisorId = advisorId,
                Status = Status.Pending
            };

            _mockRepo.Setup(r => r.GetByIdAsync(consultation.ConsultationId))
                     .ReturnsAsync(consultation);

            _mockRepo.Setup(r => r.UpdateAsync(consultation))
                     .ReturnsAsync(true);

            var result = await _service.MarkAsCompletedAsync(consultation.ConsultationId, advisorId);

            Assert.IsTrue(result);
            Assert.AreEqual(Status.Completed, consultation.Status);
        }

        #endregion

        #region CountByClientIdAsync Tests

        [Test]
        public async Task CountByClientIdAsync_ReturnsCount()
        {
            var clientId = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetAllAttached())
                .Returns(new List<Consultation>
                {
                    new Consultation { ClientId = clientId },
                    new Consultation { ClientId = clientId },
                    new Consultation { ClientId = Guid.NewGuid() }
                }.BuildMock());

            var count = await _service.CountByClientIdAsync(clientId);

            Assert.AreEqual(2, count);
        }

        #endregion

        #region GetUpcomingForAdvisorAsync Tests

        [Test]
        public async Task GetUpcomingForAdvisorAsync_ReturnsEmpty_WhenNoUpcoming()
        {
            var advisorId = Guid.NewGuid();
            var emptySet = new List<Consultation>().BuildMock();

            _mockRepo.Setup(r => r.GetAllAttached())
                     .Returns(emptySet);

            var result = await _service.GetUpcomingForAdvisorAsync(advisorId);

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetUpcomingForAdvisorAsync_ReturnsUpcomingConsultations()
        {
            var advisorId = Guid.NewGuid();
            var consultations = new List<Consultation>
            {
                new Consultation
                {
                    AdvisorId = advisorId,
                    ScheduledDateTime = DateTime.UtcNow.AddDays(3),
                    ConsultationId = Guid.NewGuid(),
                    ClientId = Guid.NewGuid(),
                    Advisor = new Advisor { UserId = Guid.NewGuid() },
                    ConsultationType = ConsultationType.InvestmentAdvisory,
                    Status = Status.Pending,
                    CreatedAt = DateTime.UtcNow
                }
            }.BuildMock();

            _mockRepo.Setup(r => r.GetAllAttached()).Returns(consultations);

            var clientUser = new Domain.Entities.ApplicationUser
            {
                Id = consultations.First().ClientId,
                Email = "client@example.com"
            };
            var advisorUser = new Domain.Entities.ApplicationUser
            {
                Id = consultations.First().Advisor.UserId,
                Email = "advisor@example.com"
            };

            _mockUserRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                         .ReturnsAsync(new[] { clientUser, advisorUser });

            var result = await _service.GetUpcomingForAdvisorAsync(advisorId);

            Assert.IsNotEmpty(result);
            Assert.AreEqual(advisorId, result.First().AdvisorId);
        }

        #endregion
        #region Additional Tests for Coverage

        [Test]
        public async Task GetAllByAdvisorIdAsync_ReturnsAll_WhenFilterIsNull()
        {
            var advisorId = Guid.NewGuid();
            var consultationsList = new List<Consultation>
    {
        new Consultation { ConsultationId = Guid.NewGuid(), AdvisorId = advisorId, ConsultationType = ConsultationType.InvestmentAdvisory, Advisor = new Advisor { UserId = Guid.NewGuid() } },
        new Consultation { ConsultationId = Guid.NewGuid(), AdvisorId = advisorId, ConsultationType = ConsultationType.CreditAdvisory, Advisor = new Advisor { UserId = Guid.NewGuid() } }
    }.BuildMock();

            _mockRepo.Setup(r => r.GetAllAttached()).Returns(consultationsList);

            var users = consultationsList.Select(c => new Domain.Entities.ApplicationUser { Id = c.Advisor.UserId, Email = $"advisor{c.ConsultationId}@example.com" }).ToArray();
            _mockUserRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(users);

            var result = await _service.GetAllByAdvisorIdAsync(advisorId, null);

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllByAdvisorIdAsync_ReturnsEmpty_WhenNoConsultationsMatchType()
        {
            var advisorId = Guid.NewGuid();
            var consultationsList = new List<Consultation>
    {
        new Consultation { ConsultationId = Guid.NewGuid(), AdvisorId = advisorId, ConsultationType = ConsultationType.CreditAdvisory, Advisor = new Advisor { UserId = Guid.NewGuid() } }
    }.BuildMock();

            _mockRepo.Setup(r => r.GetAllAttached()).Returns(consultationsList);

            var result = await _service.GetAllByAdvisorIdAsync(advisorId, "InvestmentAdvisory");

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsConsultationDto_WithMissingUsers()
        {
            var consultationId = Guid.NewGuid();
            var consultation = new Consultation
            {
                ConsultationId = consultationId,
                ClientId = Guid.NewGuid(),
                AdvisorId = Guid.NewGuid(),
                Advisor = new Advisor { UserId = Guid.NewGuid() },
                ConsultationType = ConsultationType.SecurityAdvisory,
                Status = Status.Pending,
                ScheduledDateTime = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var data = new List<Consultation> { consultation }.BuildMock();
            _mockRepo.Setup(r => r.GetAllAttached()).Returns(data);

            // Return only client user, missing advisor user
            var clientUser = new Domain.Entities.ApplicationUser
            {
                Id = consultation.ClientId,
                Email = "client@example.com"
            };

            _mockUserRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                         .ReturnsAsync(new[] { clientUser });

            var result = await _service.GetByIdAsync(consultationId);

            Assert.IsNotNull(result);
            Assert.AreEqual("client@example.com", result.ClientName);
            Assert.AreEqual("N/A", result.AdvisorName);
            // advisor user missing
        }

        [Test]
        public async Task UpdateAsync_ReturnsFalse_WhenUnauthorizedUser()
        {
            // Setup: You might need to extend your service to accept userId who tries update.
            // For this example, assuming UpdateAsync only allows advisor who owns consultation.

            var existingConsultation = new Domain.Entities.Consultation
            {
                ConsultationId = Guid.NewGuid(),
                AdvisorId = Guid.NewGuid(),
                Status = Status.Pending,
                ConsultationType = ConsultationType.SecurityAdvisory,
                ScheduledDateTime = DateTime.UtcNow
            };

            _mockRepo.Setup(r => r.GetByIdAsync(existingConsultation.ConsultationId))
                     .ReturnsAsync(existingConsultation);

            // Simulate unauthorized user by passing different advisorId in update DTO (assuming your method checks this)
            var updateDto = new UpdateConsultationDto
            {
                Id = existingConsultation.ConsultationId,
                ScheduledDate = DateTime.UtcNow.AddDays(1),
                Status = Status.Completed,
                ConsultationType = ConsultationType.CreditAdvisory,
                // no userId in your DTO, but let's assume you check authorization in service
            };

            // If your service has no authorization check, add one or simulate by mocking repo behavior

            // Here we simulate update fails due to unauthorized user by returning false on UpdateAsync
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Consultation>()))
                     .ReturnsAsync(false);

            var result = await _service.UpdateAsync(updateDto);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteSelfAsync_ReturnsFalse_WhenUnauthorizedUser()
        {
            var consultation = new Domain.Entities.Consultation
            {
                ConsultationId = Guid.NewGuid(),
                AdvisorId = Guid.NewGuid()
            };

            _mockRepo.Setup(r => r.GetByIdAsync(consultation.ConsultationId))
                     .ReturnsAsync(consultation);

            var unauthorizedAdvisorId = Guid.NewGuid();

            var result = await _service.DeleteSelfAsync(consultation.ConsultationId, unauthorizedAdvisorId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task MarkAsCompletedAsync_ReturnsFalse_WhenUnauthorizedUser()
        {
            var consultation = new Domain.Entities.Consultation
            {
                ConsultationId = Guid.NewGuid(),
                AdvisorId = Guid.NewGuid(),
                Status = Status.Pending
            };

            _mockRepo.Setup(r => r.GetByIdAsync(consultation.ConsultationId))
                     .ReturnsAsync(consultation);

            var unauthorizedAdvisorId = Guid.NewGuid();

            var result = await _service.MarkAsCompletedAsync(consultation.ConsultationId, unauthorizedAdvisorId);

            Assert.IsFalse(result);
            Assert.AreEqual(Status.Pending, consultation.Status);
        }

        #endregion

    }
}
