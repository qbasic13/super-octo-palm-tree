using System;
using System.Collections.Generic;

namespace BooksiteAPI.Data
{
    public partial class M2mOrdersBook
    {
        public int M2mobOId { get; set; }
        public string M2mobBIsbn { get; set; } = null!;
        public decimal? M2mobPrice { get; set; }

        public virtual Book M2mobBIsbnNavigation { get; set; } = null!;
        public virtual Order M2mobO { get; set; } = null!;
    }
}
