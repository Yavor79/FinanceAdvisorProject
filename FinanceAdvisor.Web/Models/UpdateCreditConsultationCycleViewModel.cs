
using System.ComponentModel.DataAnnotations;

public class UpdateCreditConsultationCycleViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid AdvisorId { get; set; }

    [Required]
    public string Status { get; set; }
}
