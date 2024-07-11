using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanc.Models
{
    public class MyReturnProductsDto
    {
        public int ID { get; set; }
        public Nullable<int> OrderId { get; set; }


        public bool Isreturned { get; set; }
        public bool IsConfirm { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }

        public string ProductName { get; set; }
        public string Status { get; set; }
        public Nullable<int> ClientId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string DeliveryStatus { get; set; }
        public double OrderTotal { get; set; }
        public string Address { get; set; }
        public string ClientName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int categoryId { get; set; }
        public string seller_SelID { get; set; }
        public string Buyer_address { get; set; }
        public string Buyer_selfID { get; set; }
        public string isReturnApproved { get; set; }
        public Nullable<int> subcatid { get; set; }
        public Nullable<int> BrandID { get; set; }
        public string cashback { get; set; }
        public string Cashbacktype { get; set; }
        public string Image { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public string OrderIdN { get; set; }
        public string Subname { get; set; }
        public HttpPostedFileBase file { get; set; }

        public DateTime DDate { get; set; }
        public string trackid { get; set; }
        public DateTime DTime { get; set; }
        public string ReturnReason { get; set; }
        public string Returnproof { get; set; }
        public string TransactionId { get; set; }
        public int Od_ID { get; set; }
        public string Od_p_status { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<int> GST { get; set; }
        public Nullable<int> SizeId { get; set; }
        public Nullable<int> ColorId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }

        public string Productname { get; set; }
        public string productImage { get; set; }
        public string Unit { get; set; }

        public string uniqueID { get; set; }
        public double Disvalue { get; set; }
        public bool IsReturn { get; set; }

        public double UnitMultiolyByPrize { get; set; }
        public double TotalAmount { get; set; }
        public decimal DeliveryCharge { get; set; }
    }
}