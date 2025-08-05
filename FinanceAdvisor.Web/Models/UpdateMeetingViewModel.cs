
public class UpdateMeetingViewModel
{
    public Guid Id { get; set; }
    public Guid CreditConsultationCycleId { get; set; }
    public DateTime Date { get; set; }
    public string? Topic { get; set; }
    public string? Notes { get; set; }
}