using MailKit.Net.Smtp;
using MimeKit;
using Notification.API.Models;
using Microsoft.Extensions.Configuration;

namespace Notification.API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(EmailRequest request)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config["SmtpSettings:Username"]));
                email.To.Add(MailboxAddress.Parse(request.ToEmail));
                email.Subject = request.Subject;

                var builder = new BodyBuilder { HtmlBody = request.Body };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _config["SmtpSettings:Host"],
                      int.Parse(_config["SmtpSettings:Port"]),
                      MailKit.Security.SecureSocketOptions.StartTls
                      );

                await smtp.AuthenticateAsync(
                    _config["SmtpSettings:Username"],
                    _config["SmtpSettings:Password"]
                    );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch(Exception ex)
            {
                Console.WriteLine("EMAIL ERROR : " + ex.Message);
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }
    }
}
