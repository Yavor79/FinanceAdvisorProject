using System;
using System.Collections.Generic;
using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Application.DTOs
{
    public class CreditConsultationCycleDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid AdvisorId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string AdvisorName { get; set; } = string.Empty;
        public CreditType CreditType { get; set; }
        public Status Status { get; set; }

        // Optional: count of meetings or simplified list
        public int MeetingCount { get; set; }

        public DateTime CreatedAt { get; set; }


    }

    public class CreateCreditConsultationCycleDto
    {
        public Guid ClientId { get; set; }
        public Guid AdvisorId { get; set; }
        public CreditType CreditType { get; set; }
    }

    public class UpdateCreditConsultationCycleDto
    {
        public Guid Id { get; set; }
        public CreditType CreditType { get; set; }
        public Status Status { get; set; }
    }

}
