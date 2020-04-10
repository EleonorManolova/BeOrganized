namespace BeOrganized.Services.Messaging
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity.UI.Services;

    // This class is used when sendGrid is no longer avaliable
    public class NullMessageSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }
}
