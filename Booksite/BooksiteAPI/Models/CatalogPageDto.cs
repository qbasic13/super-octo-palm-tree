namespace BooksiteAPI.Models
{
	public class CatalogPageDto
	{
		public List<BookDto>? Books { get; set; }
		public int Count { get; set; }
	}
}
