//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace projeb.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class mesaj
    {
        public int id { get; set; }
        public int ogrtid { get; set; }
        public int ogrid { get; set; }
        public string ogrtadi { get; set; }
        public string ogradi { get; set; }
        public string konu { get; set; }
        public string mesaj1 { get; set; }
    
        public virtual ogrenci ogrenci { get; set; }
        public virtual ogretmen ogretmen { get; set; }
    }
}