namespace OrganizeMe.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Options;
    using SendGrid;
    using SendGrid.Helpers.Mail;

    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            this.Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } // set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return this.Execute(this.Options.Key, this.Options.Host, this.Options.SupportName, this.Options.SupportEmail, subject, message, email);
        }

        public Task Execute(string apiKey, string host, string fromName, string from, string subject, string message, string email)
        {
            if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Subject and message should be provided.");
            }

            var client = new SendGridClient(apiKey, host);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(from, fromName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message,
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
