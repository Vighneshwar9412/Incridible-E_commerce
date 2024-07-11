using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanc.Models
{
    public class ADDDiscountDTO
    {
        public int ID { get; set; }
        public int[] Brands { get; set; }
        public int[] Subcategories { get; set; }
        public int[] Product_IDS { get; set; }
        public string CouponCode { get; set; }
        public int Discount { get; set; }
        public Nullable<DateTime> fromdate { get; set; }
        public Nullable<DateTime> todate { get; set; }
        public string Appyon { get; set; }
        public string DiscountType { get; set; }
        public string averageOrPercentage { get; set; }
    }
}