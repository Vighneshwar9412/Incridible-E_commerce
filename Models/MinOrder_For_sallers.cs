//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wanc.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MinOrder_For_sallers
    {
        public int id { get; set; }
        public Nullable<int> sellerid { get; set; }
        public Nullable<System.DateTime> craetedate { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> MinAmount { get; set; }
    }
}
