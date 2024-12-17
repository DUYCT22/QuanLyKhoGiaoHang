using NguyenNhutDuy_2122110447.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using PagedList;
using DocumentFormat.OpenXml.Drawing.Charts;
namespace NguyenNhutDuy_2122110447.Controllers
{
    public class OrderController : BaseController
    {
        private static System.Data.DataTable _previewTable;
        QuanLyKhoGiaoHangEntities1 data = new QuanLyKhoGiaoHangEntities1();
        // GET: DonHang
        public ActionResult Index(int? page, string searchTerm, string sortOrder)
        {
            var orders = data.DonHangs.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                orders = orders.Where(o => o.Ten.Contains(searchTerm));
            }
            switch (sortOrder)
            {
                case "asc":
                    orders = orders.OrderBy(o => o.GiaTriDonHang);
                    break;
                case "desc":
                    orders = orders.OrderByDescending(o => o.GiaTriDonHang);
                    break;
                default:
                    orders = orders.OrderBy(o => o.Id);
                    break;
            }
            int pageSize = 4;
            int pageNumber = page ?? 1;
            var pagedOrders = orders.ToPagedList(pageNumber, pageSize);
            return View(pagedOrders);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension == ".xlsx" || fileExtension == ".xls")
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var dt = new System.Data.DataTable();
                        for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
                        {
                            var columnName = worksheet.Cells[1, i].Text.Trim();
                            dt.Columns.Add(columnName);
                        }
                        for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                        {
                            var row = dt.NewRow();
                            for (int j = 1; j <= worksheet.Dimension.End.Column; j++)
                            {
                                row[j - 1] = worksheet.Cells[i, j].Text.Trim();
                            }
                            dt.Rows.Add(row);
                        }
                        var previewData = dt.AsEnumerable().Select(row => new PreviewItem
                        {
                            Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                            DiaChi = row["Địa chỉ"].ToString(),
                            Ten = row["Tên"].ToString(),
                            Loai = row["Loại"].ToString(),
                            CanNang = row["Cân nặng"] != DBNull.Value ? Convert.ToDouble(row["Cân nặng"]) : 0,
                            TenNguoiNhan = row["Tên người nhận"].ToString(),
                            Sdt = row["Sdt"].ToString(),
                            GiaTriDonHang = row["Giá trị đơn hàng"] != DBNull.Value ? Convert.ToDecimal(row["Giá trị đơn hàng"]) : 0,
                            TongTien = row["Tổng tiền"] != DBNull.Value ? Convert.ToDecimal(row["Tổng tiền"]) : 0,
                            GhiChu = row["Ghi chú"].ToString()
                        }).ToList();
                        Session["PreviewData"] = previewData;
                    }
                    return View("Create");
                }
            }
            ModelState.AddModelError("", "Vui lòng chọn file Excel hợp lệ.");
            return View("Create");
        }


        [HttpPost]
        public ActionResult ConfirmImport()
        {
            var previewData = Session["PreviewData"] as List<PreviewItem>;
            if (previewData != null)
            {
                using (var db = new QuanLyKhoGiaoHangEntities1())
                {
                    foreach (var row in previewData)
                    {
                        // Kiểm tra trùng lặp Id
                        if (!db.DonHangs.Any(d => d.Id == row.Id))
                        {
                            var donHang = new DonHang
                            {
                                Id = row.Id,
                                DiaChi = row.DiaChi,
                                Ten = row.Ten,
                                Loai = row.Loai,
                                CanNang = row.CanNang,
                                TenNguoiNhan = row.TenNguoiNhan,
                                Sdt = row.Sdt,
                                GiaTriDonHang = row.GiaTriDonHang,
                                TongTien = row.TongTien,
                                GhiChu = row.GhiChu
                            };

                            db.DonHangs.Add(donHang);
                        }
                    }

                    db.SaveChanges();
                }

                Session.Remove("PreviewData");
                ViewBag.Message = "Dữ liệu đã được thêm thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Không có dữ liệu để thêm.");
            return View("Index");
        }

        public ActionResult Detail(int Id)
        {
            var dl = data.DonHangs.Find(Id);
            if (dl == null)
            {
                return Content("Không tìm thấy đơn hàng.");
            }
            return PartialView("Detail", dl);
        }

        public ActionResult Edit(int Id)
        {
            var dl = data.DonHangs.FirstOrDefault(d => d.Id == Id);
            if (dl == null)
            {
                return HttpNotFound();
            }
            return View(dl);
        }

        [HttpPost]
        public ActionResult Update(int Id, DonHang updatedData)
        {
            var dl = data.DonHangs.FirstOrDefault(d => d.Id == Id);
            if (dl == null)
            {
                return HttpNotFound();
            }
            dl.Ten = updatedData.Ten;
            dl.DiaChi = updatedData.DiaChi;
            dl.Loai = updatedData.Loai;
            dl.CanNang = updatedData.CanNang;
            dl.TenNguoiNhan = updatedData.TenNguoiNhan;
            dl.Sdt = updatedData.Sdt;
            dl.GiaTriDonHang = updatedData.GiaTriDonHang;
            dl.TongTien = updatedData.TongTien;
            dl.GhiChu = updatedData.GhiChu;
            data.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int id)
        {
            var dl = data.DonHangs.FirstOrDefault(d => d.Id == id);
            if (dl == null)
            {
                return HttpNotFound();
            }
            data.DonHangs.Remove(dl);
            data.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Distribution()
        {
            var nhanVienGiaoHang = data.NhanViens
                                    .Where(nv => nv.Khoi == "Giao hàng")
                                    .ToList();
            var donHangChuaGiao = data.DonHangs
                                    .Where(dh => !data.GiaoHangs.Any(gh => gh.IdDonHang == dh.Id))
                                    .ToList();
            if (!nhanVienGiaoHang.Any() || !donHangChuaGiao.Any())
            {
                ViewBag.Message = "Không có nhân viên hoặc đơn hàng để phân chia.";
                return View();
            }
            int soLuongNhanVien = nhanVienGiaoHang.Count;
            int indexNhanVien = 0;
            foreach (var donHang in donHangChuaGiao)
            {
                var lichsu = new LichSuPhanPhoi
                {
                    IdDonHang = donHang.Id,
                    IdNhanVienGiaoHang = nhanVienGiaoHang[indexNhanVien].IdNhanVien,
                    NgayPhanPhoi = DateTime.Now,
                    SoTienChuoc = donHang.GiaTriDonHang
                };
                var giaoHang = new GiaoHang
                {
                    IdDonHang = donHang.Id,
                    IdNhanVienGiaoHang = nhanVienGiaoHang[indexNhanVien].IdNhanVien,
                    TenNguoiNhan = donHang.TenNguoiNhan,
                    Sdt = donHang.Sdt,
                    TongTien = donHang.TongTien,
                    TrangThai = "Đang giao",
                    GhiChu = donHang.GhiChu
                };
                data.LichSuPhanPhois.Add(lichsu);
                data.GiaoHangs.Add(giaoHang);
                indexNhanVien = (indexNhanVien + 1) % soLuongNhanVien;
            }
            data.SaveChanges();
            ViewBag.Message = "Đã phân chia đơn hàng thành công.";
            return RedirectToAction("Index");
            
        }
    }
}