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
    
    public partial class LichSuGiaoHang
    {
        public int IdDonHang { get; set; }
        public int IdNhanVienGiaoHang { get; set; }
        public string TrangThai { get; set; }
        public System.DateTime NgayGiao { get; set; }
        public int Id { get; set; }
    
        public virtual DonHang DonHang { get; set; }
        public virtual NhanVien NhanVien { get; set; }
    }
}