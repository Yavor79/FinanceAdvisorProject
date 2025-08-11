using FinanceAdvisor.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceAdvisor.Web.Models
{
    public class UpdateConsultationViewModel
    {
        [Required]
        public Guid Id { get; set; }


        //[Required]
        //public Guid ClientId { get; set; }

        [Required]
        public Guid AdvisorId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ScheduledAt { get; set; }
        [Required]
        public Status Status { get; set; }
        [Required]
        public ConsultationType ConsultationType { get; set; }

        public string AdvisorName { get; set; } = string.Empty;

        public IEnumerable<ChooseAdvisorViewModel> ChooseAdvisors { get; set; } = Enumerable.Empty<ChooseAdvisorViewModel>();
        

    }
}
