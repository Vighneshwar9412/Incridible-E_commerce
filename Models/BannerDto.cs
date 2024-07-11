using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wanc.Models
{
    public class BannerDto
    {
        public int ID { get; set; }
        public string Images { get; set; }
        public HttpPostedFileBase ImagesFile { get; set; }
        public string Url { get; set; }

    }
}