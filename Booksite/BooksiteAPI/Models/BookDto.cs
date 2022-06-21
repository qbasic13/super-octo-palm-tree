namespace BooksiteAPI.Models
{
    public class BookDto
    {
        public string? Isbn { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? CoverFile { get; set; }
    }
}
