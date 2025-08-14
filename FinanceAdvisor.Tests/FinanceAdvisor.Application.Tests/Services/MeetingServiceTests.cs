using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceAdvisor.Application.Services;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using MockQueryable;
using Microsoft.Extensions.Logging;

// Helper class to mock async EF queries

[TestFixture]
public class MeetingServiceTests
{
    private Mock<IMeetingRepository> _repositoryMock = null!;
    private MeetingService _service = null!;
    private Mock<ILogger<MeetingService>> _logger = null!;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IMeetingRepository>();
        _logger = new Mock<ILogger<MeetingService>>();
        _service = new MeetingService(_repositoryMock.Object, _logger.Object);
        
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllMeetings()
    {
        var meetings = new List<Meeting>
        {
            new Meeting { Id = Guid.NewGuid(), CreditConsultationCycleId = Guid.NewGuid(), ScheduledDateTime = DateTime.UtcNow, Type = FinanceAdvisor.Domain.Enums.Type.Initial },
            new Meeting { Id = Guid.NewGuid(), CreditConsultationCycleId = Guid.NewGuid(), ScheduledDateTime = DateTime.UtcNow.AddDays(1), Type = FinanceAdvisor.Domain.Enums.Type.Secondary }
        };

        _repositoryMock.Setup(r => r.GetAllAttached()).Returns(new List<Meeting>(meetings).BuildMock());

        var result = await _service.GetAllAsync();

        Assert.AreEqual(2, result.Count());
        Assert.AreEqual(meetings[0].Id, result.First().Id);
    }

    [Test]
    public async Task GetAllByCycleIdAsync_EmptyGuid_ReturnsEmpty()
    {
        var result = await _service.GetAllByCycleIdAsync(Guid.Empty);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task GetAllByCycleIdAsync_ValidId_ReturnsFilteredMeetings()
    {
        var cycleId = Guid.NewGuid();
        var meetings = new List<Meeting>
        {
            new Meeting { Id = Guid.NewGuid(), CreditConsultationCycleId = cycleId, ScheduledDateTime = DateTime.UtcNow, Type = FinanceAdvisor.Domain.Enums.Type.Initial },
            new Meeting { Id = Guid.NewGuid(), CreditConsultationCycleId = Guid.NewGuid(), ScheduledDateTime = DateTime.UtcNow, Type = FinanceAdvisor.Domain.Enums.Type.Secondary }
        };

        _repositoryMock.Setup(r => r.GetAllAttached()).Returns(new List<Meeting>(meetings).BuildMock());

        var result = await _service.GetAllByCycleIdAsync(cycleId);

        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.All(m => m.CreditConsultationCycleId == cycleId));
    }

    [Test]
    public async Task GetByIdAsync_MeetingExists_ReturnsMeetingDto()
    {
        var id = Guid.NewGuid();
        var meeting = new Meeting { Id = id, CreditConsultationCycleId = Guid.NewGuid(), ScheduledDateTime = DateTime.UtcNow, Type = FinanceAdvisor.Domain.Enums.Type.Final };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(meeting);

        var result = await _service.GetByIdAsync(id);

        Assert.IsNotNull(result);
        Assert.AreEqual(meeting.Id, result!.Id);
    }

    [Test]
    public async Task GetByIdAsync_MeetingNotExists_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Meeting?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [Test]
    public async Task CreateAsync_ReturnsCreatedMeetingDto()
    {
        CreateMeetingDto createDto = new()
        {
            CreditConsultationCycleId = Guid.NewGuid(),
            ScheduledDateTime = DateTime.UtcNow.AddDays(5),
            Type = FinanceAdvisor.Domain.Enums.Type.Initial
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Meeting>())).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(createDto);

        Assert.AreEqual(createDto.CreditConsultationCycleId, result.CreditConsultationCycleId);
        Assert.AreEqual(createDto.Type, result.Type);
    }

    [Test]
    public async Task UpdateAsync_EntityExists_ReturnsTrue()
    {
        var existingMeeting = new Meeting
        {
            Id = Guid.NewGuid(),
            CreditConsultationCycleId = Guid.NewGuid(),
            ScheduledDateTime = DateTime.UtcNow,
            Type = FinanceAdvisor.Domain.Enums.Type.Initial
        };

        var updateDto = new UpdateMeetingDto
        {
            Id = existingMeeting.Id,
            ScheduledDateTime = DateTime.UtcNow.AddDays(1),
            Type = FinanceAdvisor.Domain.Enums.Type.Secondary
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(existingMeeting.Id)).ReturnsAsync(existingMeeting);
        _repositoryMock.Setup(r => r.UpdateAsync(existingMeeting)).ReturnsAsync(true);

        var result = await _service.UpdateAsync(updateDto);

        Assert.IsTrue(result);
        Assert.AreEqual(updateDto.ScheduledDateTime, existingMeeting.ScheduledDateTime);
        Assert.AreEqual(updateDto.Type, existingMeeting.Type);
    }

    [Test]
    public async Task UpdateAsync_EntityNotFound_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Meeting?)null);

        var updateDto = new UpdateMeetingDto
        {
            Id = Guid.NewGuid(),
            ScheduledDateTime = DateTime.UtcNow,
            Type = FinanceAdvisor.Domain.Enums.Type.Initial
        };

        var result = await _service.UpdateAsync(updateDto);

        Assert.IsFalse(result);
    }

    [Test]
    public async Task DeleteAsync_EntityExists_ReturnsTrue()
    {
        var meetingId = Guid.NewGuid();
        var meeting = new Meeting { Id = meetingId };

        _repositoryMock.Setup(r => r.GetByIdAsync(meetingId)).ReturnsAsync(meeting);
        _repositoryMock.Setup(r => r.DeleteAsync(meeting)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(meetingId, Guid.NewGuid());

        Assert.IsTrue(result);
    }

    [Test]
    public async Task DeleteAsync_EntityNotFound_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Meeting?)null);

        var result = await _service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.IsFalse(result);
    }

    [Test]
    public async Task GetUpcomingMeetingsAsync_ReturnsMeetingsAfterDate()
    {
        var fromDate = DateTime.UtcNow;
        var meetings = new List<Meeting>
        {
            new Meeting { Id = Guid.NewGuid(), ScheduledDateTime = fromDate.AddDays(1), CreditConsultationCycleId = Guid.NewGuid(), Type = FinanceAdvisor.Domain.Enums.Type.Initial },
            new Meeting { Id = Guid.NewGuid(), ScheduledDateTime = fromDate.AddDays(-1), CreditConsultationCycleId = Guid.NewGuid(), Type = FinanceAdvisor.Domain.Enums.Type.Secondary }
        };

        _repositoryMock.Setup(r => r.GetAllAttached()).Returns(new List<Meeting>(meetings).BuildMock());

        var result = await _service.GetUpcomingMeetingsAsync(fromDate);

        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.All(m => m.ScheduledDateTime > fromDate));
    }

    [Test]
    public async Task CountByCycleIdAsync_ReturnsCorrectCount()
    {
        var cycleId = Guid.NewGuid();

        var meetings = new List<Meeting>
        {
            new Meeting { CreditConsultationCycleId = cycleId },
            new Meeting { CreditConsultationCycleId = Guid.NewGuid() }
        };

        _repositoryMock.Setup(r => r.GetAllAttached()).Returns(new List<Meeting>(meetings).BuildMock());

        var count = await _service.CountByCycleIdAsync(cycleId);

        Assert.AreEqual(1, count);
    }
}
