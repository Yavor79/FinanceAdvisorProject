using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Application.DTOs
{
    public class AdvisorDto
    {
        public Guid AdvisorId { get; set; }
        public Guid UserId { get; set; }
        public Specialization Specialization { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public string AdvisorName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
