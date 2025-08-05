namespace FinanceAdvisor.Application.DTOs
{
    public class ApplicationUserDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            return $"User Info:\n" +
                   $"- ID: {Id}\n" +
                   $"- Email: {Email ?? "[null]"}\n" +
                   $"- Username: {UserName ?? "[null]"}\n" +
                   $"- Created At: {CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
                   $"- Is Deleted: {(IsDeleted ? "Yes" : "No")}";
        }
    }
}
