using BooksiteAPI.Models.Mail;
using MailKit.Net.Smtp;
using MailKit;
using Microsoft.Extensions.Options;
using MimeKit;

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
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mail.Address);
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = mailRequest.Body;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback =
                    (p1, p2, p3, p4) => { return true; };
                smtp.Connect(_mail.Host, _mail.Port,
                    MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_mail.Address, _mail.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }
    }
}
