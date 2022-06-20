namespace BooksiteAPI.Models.Auth
{
	public class RegisterDto : AuthReqDto
	{
		public string? LastName { get; set; }

		public string? FirstName { get; set; }
		
		public string? MiddleName { get; set; }

		public string? Phone { get; set; }
	}
}
