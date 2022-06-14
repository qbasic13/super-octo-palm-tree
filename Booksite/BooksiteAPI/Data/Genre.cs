using System;
using System.Collections.Generic;

namespace BooksiteAPI.Data
{
    public partial class Genre
    {
        public Genre()
        {
            Books = new HashSet<Book>();
        }

        public int GId { get; set; }
        public string GName { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; }
    }
}
