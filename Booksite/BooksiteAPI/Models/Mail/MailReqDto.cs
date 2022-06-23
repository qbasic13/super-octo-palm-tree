namespace BooksiteAPI.Models.Mail
{
    public class MailReqDto
    {
        public string? ToEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
    }
}
