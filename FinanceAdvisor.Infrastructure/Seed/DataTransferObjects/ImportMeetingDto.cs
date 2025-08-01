using AutoMapper;
using FinanceAdvisor.Domain.Entities;
using System;

namespace FinanceAdvisor.Infrastructure.Seed.DataTransferObjects
{
    public class ImportMeetingDto
    {
        public Guid Id { get; set; }
        public Guid CreditConsultationCycleId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public int Type { get; set; } // or Enums.Type if available in the Application layer

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ImportMeetingDto, Meeting>();


        }
    }
}
