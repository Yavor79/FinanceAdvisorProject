using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Web.Models
{
    public class AdvisorViewModel
    {
        public Guid AdvisorId { get; set; }
        public string AdvisorName { get; set; }
        public Guid UserId { get; set; }
        public Specialization Specialization { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
