//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NguyenNhutDuy_2122110447.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LoiNhanVien
    {
        public int Id { get; set; }
        public int IdNhanVien { get; set; }
        public string MoTaLoi { get; set; }
        public System.DateTime NgayPhatSinh { get; set; }
        public Nullable<decimal> ChiPhiPhatSinh { get; set; }
    
        public virtual NhanVien NhanVien { get; set; }
    }
}
