using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanc.Models
{
    public class OrderDetailDTO
    {
        public int ID { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> SizeId { get; set; }
        public Nullable<int> ColorId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public bool Isreturned { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<int> GST { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }


        public Nullable<int> ClientId { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string DeliveryStatus { get; set; }
        public bool Iscancelled { get; set; }
        public Nullable<bool> Isconfirm { get; set; }
        public double OrderTotal { get; set; }

        public string Address { get; set; }
        public string ClientName { get; set; }
        public string Clientname { get; set; }
        public string saller_id { get; set; }
        public string ReturnRequest { get; set; }
        public string Reasonforreturn { get; set; }
        public string image { get; set; }


        public int Buyerid { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAddress { get; set; }

        public string OrderIDn { get; set; }
        public double DeliveryCharge { get; set; }
        public string CoupenOrFlatDiscount { get; set; }
        public decimal Amount { get; set; }

        public string paymentMethod { get; set; }
    }
}