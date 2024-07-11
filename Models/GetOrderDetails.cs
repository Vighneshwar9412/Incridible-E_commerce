using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanc.Models
{
    public class GetOrderDetails
    {
        public int ID { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public bool Isreturned { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<int> sallerid { get; set; }
        public string productname { get; set; }
        public string productimage { get; set; }
    }
}