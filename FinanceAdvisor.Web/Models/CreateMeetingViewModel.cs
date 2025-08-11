namespace FinanceAdvisor.Web.Models
{
    public class CreateMeetingViewModel
    {
        public Guid CreditConsultationCycleId { get; set; }
        public DateTime Date { get; set; }
        public FinanceAdvisor.Domain.Enums.Type Type { get; set; }
        public string? Topic { get; set; }
        public string? Notes { get; set; }
    }
}