namespace FinanceAdvisor.Domain.Entities
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

