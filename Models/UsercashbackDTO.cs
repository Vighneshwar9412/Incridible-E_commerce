using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanc.Models
{
    public class UsercashbackDTO
    {
        public int CashbackId { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public Nullable<Decimal> CashbackAmount { get; set; }
        public Nullable<int> Userid { get; set; }

       
    }
}