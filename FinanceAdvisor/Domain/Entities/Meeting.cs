using FinanceAdvisor.Domain.Enums;
using Microsoft.AspNetCore.Identity;
namespace FinanceAdvisor.Domain.Entities
{
    public class Meeting
    {
        public Guid Id { get; set; }
        public string ClientId { get; set; } = null!;
        public IdentityUser Client { get; set; } = null!;
        public Guid AdvisorId { get; set; }
        public Advisor Advisor { get; set; } = null!;
        public DateTime ScheduledDateTime { get; set; }
        public Enums.Type Type { get; set; }
    }
}
