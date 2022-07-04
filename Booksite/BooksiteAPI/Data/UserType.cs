using System;
using System.Collections.Generic;

namespace BooksiteAPI.Data
{
    public partial class UserType
    {
        public UserType()
        {
            M2muutUs = new HashSet<User>();
        }

        public int UtId { get; set; }
        public string UtName { get; set; } = null!;

        public virtual ICollection<User> M2muutUs { get; set; }
    }
}
