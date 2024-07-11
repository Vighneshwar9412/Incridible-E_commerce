using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanc.Models
{
    public class ProductDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        //[AllowHtml]
        public string Description { get; set; }
        public Nullable<double> Price { get; set; }
        public int categoryId { get; set; }
        public Nullable<int> Discount { get; set; }
        public string Unit { get; set; }
        public Nullable<int> DeliveryPin { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<int> ImageId { get; set; }
        public Nullable<int> subcatid { get; set; }
        public Nullable<int> BrandID { get; set; }
        public string cashback { get; set; }
        public string Cashbacktype { get; set; }
        public string Image { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public string Subname { get; set; }
        public Nullable<int> SelleroradminId { get; set; }
        public HttpPostedFileBase file { get; set; }

        public Nullable<decimal> DP { get; set; }
        public Nullable<int> GST { get; set; }
        public List<string> im { get; set; }
        public string Dimensions { get; set; }
        public string Specification { get; set; }
        public Nullable<bool> active { get; set; }
        public string Type { get; set; }
        public string Weight { get; set; }
        public Nullable<bool> Sponser { get; set; }

        public string[] color { get; set; }
        public string productColor { get; set; }

        //Added iscount
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }
        public int ApplyDiscount { get; set; }
        public string DiscountType { get; set; }
        public string AverageOrPercentage { get; set; }
        public string[] Pincode { get; set; }
        public string[] Heading { get; set; }
        public string[] Para { get; set; }
        public string[] SHeading { get; set; }
        public string[] SPara { get; set; }
        public string sallerID { get; set; }
        public bool ApprovedByadmin { get; set; }
    }
}