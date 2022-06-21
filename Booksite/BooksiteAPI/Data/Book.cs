using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BooksiteAPI.Data
{
    public partial class Book
    {
		public Book()
		{
			M2mOrdersBooks = new HashSet<M2mOrdersBook>();
		}

		[JsonPropertyName("isbn")]
		public string BIsbn { get; set; } = null!;
		[JsonPropertyName("genre")]
		public int BGenre { get; set; }
		[JsonPropertyName("title")]
		public string BTitle { get; set; } = null!;
		[JsonPropertyName("author")]
		public string? BAuthor { get; set; }
		[JsonPropertyName("publishYear")]
		public int BPublishYear { get; set; }
		[JsonPropertyName("quantity")]
		public int BQuantity { get; set; }
		[JsonPropertyName("price")]
		public decimal? BPrice { get; set; }
		[JsonPropertyName("coverFile")]
		public string? BCoverFile { get; set; }

		public virtual Genre BGenreNavigation { get; set; } = null!;
		public virtual ICollection<M2mOrdersBook> M2mOrdersBooks { get; set; }
	}
}
