using System;
using System.Collections.Generic;

namespace BooksiteAPI.Data
{
    public partial class RefreshSession
    {
        public long RsId { get; set; }
        public int RsUserId { get; set; }
        public Guid RsRefreshToken { get; set; }
        public string RsFingerprint { get; set; } = null!;
        public DateTime RsExpiresIn { get; set; }
        public DateTime RsCreatedAt { get; set; }

        public virtual User RsUser { get; set; } = null!;
    }
}
