using BooksiteAPI.Models.Mail;
using MailKit.Net.Smtp;
using MailKit;
using Microsoft.Extensions.Options;
using MimeKit;
using System.ComponentModel.DataAnnotations;

namespace BooksiteAPI.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mail;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mail = mailSettings.Value;
        }

        public async Task<bool> SendMailAsync(MailReqDto mailRequest)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(_mail.Address));
                message.Sender = MailboxAddress.Parse(_mail.Address);
                message.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                message.Subject = mailRequest.Subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = mailRequest.Body;
                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback =
                    (p1, p2, p3, p4) => { return true; };
                smtp.Connect(_mail.Host, _mail.Port,
                    MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_mail.Address, _mail.Password);
                await smtp.SendAsync(message);
                smtp.Disconnect(true);

                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        public bool ValidateEmail(string email) {
            email = email.Trim();
            if (email.EndsWith("."))
            {
                return false;
            }
            return new EmailAddressAttribute().IsValid(email);
        }
    }
}
