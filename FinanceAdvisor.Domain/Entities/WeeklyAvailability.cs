using FinanceAdvisor.Domain.Enums;
namespace FinanceAdvisor.Domain.Entities
{
    public class WeeklyAvailability
    {
        public Guid AvailabilityId { get; set; }
        public Weekday Weekday { get; set; }
        public TimeSpan StartTime { get; set; }
        
        public int Duration { get; set; }
    }
}
