namespace BooksiteAPI.Models.Order
{
    public class OrderOperationResDto
    {
        public bool IsSuccess { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public OrderDto? Order { get; set; }
    }
}
