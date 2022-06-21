using System;
using System.Collections.Generic;

namespace BooksiteAPI.Data
{
    public partial class Order
    {
        public Order()
        {
            M2mOrdersBooks = new HashSet<M2mOrdersBook>();
        }

        public int OId { get; set; }
        public int OStatus { get; set; }
        public int OCreator { get; set; }
        public decimal OTotalPrice { get; set; }
        public DateTime OCreationDt { get; set; }
        public DateTime? OCompletionDt { get; set; }

        public virtual User OCreatorNavigation { get; set; } = null!;
        public virtual OrderStatus OStatusNavigation { get; set; } = null!;
        public virtual ICollection<M2mOrdersBook> M2mOrdersBooks { get; set; }
    }
}
