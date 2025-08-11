using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Web.Models
{
    public class ChooseAdvisorViewModel
    {
        public Guid AdvisorId { get; set; }
        
        public string AdvisorName { get; set; } = string.Empty;
    }
}
