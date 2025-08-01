using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Infrastructure.Seed.DataTransferObjects
{
    public class ImportCreditConsultationCycleDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public Guid AdvisorId { get; set; }

        [Required]
        public CreditType CreditType { get; set; }

        [Required]
        public Status Status { get; set; }

        public DateTime CreatedAt { get; set; }

        // Assuming you want to ignore Meetings in seeding or handle separately
        // public ICollection<MeetingDto>? Meetings { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ImportCreditConsultationCycleDto, CreditConsultationCycle>();


        }


    }
}

//using System;
//using System.Collections.Generic;
//using FinanceAdvisor.Domain.Enums;

//namespace FinanceAdvisor.Application.DTOs
//{
//    public class CreditConsultationCycleDto
//    {
//        public Guid Id { get; set; }
//        public Guid ClientId { get; set; }
//        public Guid AdvisorId { get; set; }

//        public CreditType CreditType { get; set; }
//        public Status Status { get; set; }

//        // Optional: count of meetings or simplified list
//        public int MeetingCount { get; set; }

//        public DateTime CreatedAt { get; set; }


//    }
//}

