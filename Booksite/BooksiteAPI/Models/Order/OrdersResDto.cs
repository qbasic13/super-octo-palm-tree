namespace BooksiteAPI.Models.Order
{
    public class OrdersResDto
    {
        public bool IsSuccess { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public OrderDto[]? Orders { get; set; }
    }
}
