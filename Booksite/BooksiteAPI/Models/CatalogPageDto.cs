namespace BooksiteAPI.Models
{
	public class CatalogPageDto
	{
		public List<BookDto>? books { get; set; }
		public int count { get; set; }
	}
}
