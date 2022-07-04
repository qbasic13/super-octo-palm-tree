namespace BooksiteAPI.Models.Profile
{
    public class ProfileResDto
    {
        public bool IsSuccess { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public ProfileDto? Profile { get; set; }
    }
}
