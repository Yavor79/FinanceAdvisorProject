using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Infrastructure.Seed.DataTransferObjects
{
    public class ImportAdvisorDto
    {
        [Required]
        public Guid AdvisorId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Specialization Specialization { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ImportAdvisorDto, Advisor>();


        }
    }
}
