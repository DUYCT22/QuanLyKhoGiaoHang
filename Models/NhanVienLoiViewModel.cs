using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguyenNhutDuy_2122110447.Models
{
    public class NhanVienLoiViewModel
    {
        public IPagedList<LoiNhanVien> LoiNhanViens { get; set; }
        public List<NhanVien> NhanViens { get; set; }
    }
}