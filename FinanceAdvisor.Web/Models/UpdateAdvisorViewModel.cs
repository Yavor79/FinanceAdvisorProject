using FinanceAdvisor.Domain.Enums;

public class UpdateAdvisorViewModel
{
    public Guid AdvisorId { get; set; }
    public string AdvisorName { get; set; } = "";
    public string Email { get; set; } = "";
    public Specialization Specialization { get; set; }
}
