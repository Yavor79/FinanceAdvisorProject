using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class DummyEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine($"Sending email to {email}: {subject}");
        Console.WriteLine(htmlMessage);
        return Task.CompletedTask;
    }
}
