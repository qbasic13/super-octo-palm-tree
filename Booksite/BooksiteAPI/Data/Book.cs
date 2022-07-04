using System;
using System.Collections.Generic;

namespace BooksiteAPI.Data
{
    public partial class Book
    {
        public Book()
        {
            M2mOrdersBooks = new HashSet<M2mOrdersBook>();
        }

        public string BIsbn { get; set; } = null!;
        public int BGenre { get; set; }
        public string BTitle { get; set; } = null!;
        public string? BAuthor { get; set; }
        public int BPublishYear { get; set; }
        public int BQuantity { get; set; }
        public decimal? BPrice { get; set; }
        public string? BCoverFile { get; set; }

        public virtual Genre BGenreNavigation { get; set; } = null!;
        public virtual ICollection<M2mOrdersBook> M2mOrdersBooks { get; set; }
    }
}
