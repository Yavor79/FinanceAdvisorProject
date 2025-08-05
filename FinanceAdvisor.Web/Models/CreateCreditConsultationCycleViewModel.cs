
using FinanceAdvisor.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class CreateCreditConsultationCycleViewModel
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public Guid AdvisorId { get; set; }

    [Required]
    public CreditType CreditType { get; set; }
}
