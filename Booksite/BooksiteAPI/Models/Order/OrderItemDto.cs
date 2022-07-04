namespace BooksiteAPI.Models.Order
{
    public class OrderItemDto
    {
        public string? Isbn { get; set; }
        public int Quantity { get; set; }
        public BookDetailsDto? Details { get; set; }
    }
}
