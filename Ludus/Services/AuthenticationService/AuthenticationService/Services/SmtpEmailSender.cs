
using AuthenticationService.Models;

namespace AuthenticationService.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailConfiguration _config;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _config = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();

            await client.ConnectAsync(_config.SmtpServer, _config.Port, true);
            await client.AuthenticateAsync(_config.Username, _config.Password);

            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("Ludus Auth Service", _config.From));
            message.To.Add(MimeKit.MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new MimeKit.TextPart("html")
            {
                Text = body
            };

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
