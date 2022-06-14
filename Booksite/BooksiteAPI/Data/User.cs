using System;
using System.Collections.Generic;

namespace BooksiteAPI.Data
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
            RefreshSessions = new HashSet<RefreshSession>();
            M2muutUts = new HashSet<UserType>();
        }

        public int UId { get; set; }
        public string UEmail { get; set; } = null!;
        public string UPassword { get; set; } = null!;
        public string? ULastName { get; set; }
        public string UFirstName { get; set; } = null!;
        public string? UMiddleName { get; set; }
        public string UPhone { get; set; } = null!;
        public DateTime URegisterDt { get; set; }
        public string? UProfileFile { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RefreshSession> RefreshSessions { get; set; }

        public virtual ICollection<UserType> M2muutUts { get; set; }
    }
}
