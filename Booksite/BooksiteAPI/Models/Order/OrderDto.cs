namespace BooksiteAPI.Models.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? UserFirstName { get; set; }
        public string? UserLastName { get; set; }
        public string? UserMiddleName { get; set; }
        public string? Status { get; set; }
        public BookDetailsDto[]? Books { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CompletionDate { get; set; }

    }
}
