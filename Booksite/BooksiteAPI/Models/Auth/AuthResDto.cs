namespace BooksiteAPI.Models.Auth
{
	public class AuthResDto
	{
		public bool IsSuccess { get; set; }
		public string? Message { get; set; }
		public string? AccessToken { get; set; }
		public string? RefreshToken { get; set; }
	}
}
