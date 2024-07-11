using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanc.Models
{
    public class KartDetailDto
    {
        public int ID { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<int> GST { get; set; }
        public Nullable<int> SizeId { get; set; }
        public Nullable<int> ColorId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public Nullable<int> ClientId { get; set; }
        public string Productname { get; set; }
        public string productImage { get; set; }
        public string Unit { get; set; }
        public string Image { get; set; }
        public string uniqueID { get; set; }
        public double Disvalue { get; set; }
        public bool IsReturn { get; set; }
        public int SellerID { get; set; }

        public double UnitMultiolyByPrize { get; set; }
        public double getTotal { get; set; }
        public int minOrderOfSaller { get; set; }
        public Nullable<double> DiscountedPrice
        {
            get
            {
                return Convert.ToDouble(Convert.ToDouble(Price) - Convert.ToDouble(Price * Discount) / 100);
            }
        }

        public KartDetailDto()
        {

        }

        public KartDetailDto(int id, int productid, int _Discount, int Quantity, int price, int clientid, string productname, int _Disvalue, string productimages, string unit)
        {
            ID = id;
            ProductId = productid;
            Discount = _Discount;
            this.Quantity = Quantity;
            Price = price;
            ClientId = clientid;
            Productname = productname;
            Disvalue = _Disvalue;
            productImage = productimages;
            Unit = Unit;

        }

        public KartDetailDto(int productid, int _Discount, int Quantity, int price, int clientid, string productname, int _Disvalue, string productimages, string unit)
        {
            ProductId = productid;
            Discount = _Discount;
            this.Quantity = Quantity;
            Price = price;
            ClientId = clientid;
            Productname = productname;
            Disvalue = _Disvalue;
            productImage = productimages;
            Unit = Unit;

        }
    }
}