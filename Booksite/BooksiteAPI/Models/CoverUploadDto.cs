namespace BooksiteAPI.Models
{
    public class CoverUploadDto
    {
        public string? isbn { get; set; }
        public IFormFile? file { get; set; }
    }
}
