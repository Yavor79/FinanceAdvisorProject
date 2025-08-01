using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Infrastructure.Seed.DataTransferObjects;
using System;
using FinanceAdvisor.Infrastructure.Seed.Seeders;

public class MeetingSeeder : BaseSeeder<Meeting, ImportMeetingDto>
{
    protected override string JsonFilePath =>
        @"C:\Users\USER\Programming\C_Sharp\Finance_Project\FinanceAdvisor\FinanceAdvisor.Infrastructure\Seed\seedData\application-meeting.json";

    protected override bool EntityExists(Meeting existingEntity, ImportMeetingDto newDto)
    {
        return existingEntity.Id == newDto.Id;
    }
}
