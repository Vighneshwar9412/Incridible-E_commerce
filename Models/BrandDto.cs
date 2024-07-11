using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanc.Models
{
    public class BrandDto
    {
        public int ID { get; set; }
        public string Images { get; set; }
        public string BrandName { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<int> DeliveryPin { get; set; }
        public HttpPostedFileBase ImagesFile { get; set; }
    }
}