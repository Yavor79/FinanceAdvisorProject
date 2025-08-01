using AutoMapper;
using FinanceAdvisor.Domain.Entities;
using System;

namespace FinanceAdvisor.Infrastructure.Seed.DataTransferObjects
{
    public class ImportConsultationDto
    {
        public Guid ConsultationId { get; set; }
        public Guid ClientId { get; set; }
        public Guid AdvisorId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public int Status { get; set; } // or Status enum if shared in DTO layer
        public int ConsultationType { get; set; } // or ConsultationType enum if shared in DTO layer
        public DateTime CreatedAt { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ImportConsultationDto, Consultation>();


        }
    }
}
