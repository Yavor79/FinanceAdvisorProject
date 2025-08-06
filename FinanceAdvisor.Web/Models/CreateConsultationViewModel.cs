using FinanceAdvisor.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceAdvisor.Web.Models
{
    public class CreateConsultationViewModel
    {
        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public Guid AdvisorId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ScheduledDateTime { get; set; }

        [Required]
        public ConsultationType ConsultationType { get; set; }
    }

}
