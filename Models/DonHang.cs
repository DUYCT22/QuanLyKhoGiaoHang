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
    
    public partial class DonHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonHang()
        {
            this.GiaoHangs = new HashSet<GiaoHang>();
            this.LichSuGiaoHangs = new HashSet<LichSuGiaoHang>();
            this.LichSuPhanPhois = new HashSet<LichSuPhanPhoi>();
        }
    
        public int Id { get; set; }
        public string DiaChi { get; set; }
        public string Ten { get; set; }
        public string Loai { get; set; }
        public Nullable<double> CanNang { get; set; }
        public string TenNguoiNhan { get; set; }
        public string Sdt { get; set; }
        public Nullable<decimal> GiaTriDonHang { get; set; }
        public Nullable<decimal> TongTien { get; set; }
        public string GhiChu { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaoHang> GiaoHangs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichSuGiaoHang> LichSuGiaoHangs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichSuPhanPhoi> LichSuPhanPhois { get; set; }
    }
}