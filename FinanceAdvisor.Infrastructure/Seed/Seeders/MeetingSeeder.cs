using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Infrastructure.Seed.DataTransferObjects;
using System;
using FinanceAdvisor.Infrastructure.Seed.Seeders;
using FinanceAdvisor.Common.Utilities;

public class MeetingSeeder : BaseSeeder<Meeting, ImportMeetingDto>
{
    protected override string JsonFilePath
            => FileLocator.FindJsonFile("application-meeting.json")
               ?? throw new FileNotFoundException("Seed file 'application-meeting.json' not found.");

    protected override bool EntityExists(Meeting existingEntity, ImportMeetingDto newDto)
    {
        return existingEntity.Id == newDto.Id;
    }
}
