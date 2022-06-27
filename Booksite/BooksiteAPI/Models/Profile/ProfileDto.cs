namespace BooksiteAPI.Models.Profile
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Phone { get; set; }
        public string? AccountType { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
