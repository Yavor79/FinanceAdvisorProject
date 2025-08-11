
using FinanceAdvisor.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceAdvisor.Web.Models
{
    public class CreateCreditConsultationCycleViewModel
    {
        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public Guid AdvisorId { get; set; }

        [Required]
        public CreditType CreditType { get; set; }

        public IEnumerable<ChooseAdvisorViewModel> ChooseAdvisors { get; set; } = Enumerable.Empty<ChooseAdvisorViewModel>();
        public IEnumerable<ChooseUserViewModel> ChooseUsers { get; set; } = Enumerable.Empty<ChooseUserViewModel>();
    }

}
