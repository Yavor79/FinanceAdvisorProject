using FinanceAdvisor.Domain.Enums;

public class UpdateAdvisorViewModel
{
    public Guid AdvisorId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public Specialization Specialization { get; set; }
}
