using BooksiteAPI.Models.Mail;

namespace BooksiteAPI.Services
{
    public interface IMailService
    {
        Task<bool> SendMailAsync(MailReqDto mailRequest);
        public bool ValidateEmail(string email);
    }
}
