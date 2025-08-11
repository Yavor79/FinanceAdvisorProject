
using FinanceAdvisor.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace FinanceAdvisor.Web.Models
{
    public class UpdateCreditConsultationCycleViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid AdvisorId { get; set; }

        public string AdvisorName { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; }

        public IEnumerable<ChooseAdvisorViewModel> ChooseAdvisors { get; set; } = Enumerable.Empty<ChooseAdvisorViewModel>();


    }

}