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
    
    public partial class basvuru
    {
        public int id { get; set; }
        public int ilan_id { get; set; }
        public int ogrenci_id { get; set; }
    
        public virtual ilann ilann { get; set; }
        public virtual ogrenci ogrenci { get; set; }
    }
}
