using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wanc.Models
{
    public class CategoryDto
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int Parentid { get; set; }
        public string Description { get; set; }
        public Nullable<int> DeliveryPin { get; set; }
        public HttpPostedFileBase file { get; set; }
        public string ParentCategoryName { get; set; }
        public SelectList categoryList { get; set; }

        public Nullable<int> SelleroradminId { get; set; }
        public string UpdateBy { get; set; }
        public List<CategoryDto> ChildList { get; set; }

        public CategoryDto()
        {
            ChildList = new List<CategoryDto>();
        }
    }
}