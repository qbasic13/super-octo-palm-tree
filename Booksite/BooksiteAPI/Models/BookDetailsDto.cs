namespace BooksiteAPI.Models
{
    public class BookDetailsDto : BookDto
    {
        public string? Genre { get; set; }
        public int PublishYear { get; set; }
    }
}
