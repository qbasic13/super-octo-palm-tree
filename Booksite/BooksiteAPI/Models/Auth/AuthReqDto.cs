namespace BooksiteAPI.Models.Auth
{
	public class AuthReqDto
	{
		public string? Email { get; set; }

		public string? Password { get; set; }

		public string? Fingerprint { get; set; }
	}
}
