using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanc.Models
{
    public class SubcategoryDto
    {
        public int id { get; set; }
        public Nullable<int> catid { get; set; }
        public string subcategorname { get; set; }
        public HttpPostedFileBase file { get; set; }
        public string ParentCategoryName { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<int> DeliveryPin { get; set; }
        public Nullable<int> SelleroradminId { get; set; }
    }
}