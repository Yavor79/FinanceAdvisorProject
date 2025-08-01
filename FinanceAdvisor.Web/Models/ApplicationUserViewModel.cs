namespace FinanceAdvisor.Web.Models
{
    public class ApplicationUserViewModel
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
