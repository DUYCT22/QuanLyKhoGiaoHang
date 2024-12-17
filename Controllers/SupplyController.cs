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
    public class SupplyController : BaseController
    {
        QuanLyKhoGiaoHangEntities1 data = new QuanLyKhoGiaoHangEntities1();
        public ActionResult Index(int? page, string searchTerm, string sortSupply)
        {
            var supplys = data.QuanLyVatTus.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                supplys = supplys.Where(o => o.TenVatTu.Contains(searchTerm));
            }
            switch (sortSupply)
            {
                case "asc":
                    supplys = supplys.OrderBy(o => o.GiaNhap);
                    break;
                case "desc":
                    supplys = supplys.OrderByDescending(o => o.GiaNhap);
                    break;
                default:
                    supplys = supplys.OrderBy(o => o.IdVatTu);
                    break;
            }
            int pageSize = 4;
            int pageNumber = page ?? 1;
            var pagedSupply = supplys.ToPagedList(pageNumber, pageSize);
            return View(pagedSupply);
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

                        // Đọc header
                        for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
                        {
                            var columnName = worksheet.Cells[1, i].Text.Trim();
                            dt.Columns.Add(columnName);
                        }

                        // Đọc dữ liệu
                        for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                        {
                            var row = dt.NewRow();
                            for (int j = 1; j <= worksheet.Dimension.End.Column; j++)
                            {
                                row[j - 1] = worksheet.Cells[i, j].Text.Trim();
                            }
                            dt.Rows.Add(row);
                        }

                        // Chuyển dữ liệu sang một danh sách có kiểu mạnh (PreviewItem)
                        var previewData = dt.AsEnumerable().Select(row => new PreviewItemSupply
                        {
                            IdVatTu = row["Id vật tư"] != DBNull.Value ? Convert.ToInt32(row["Id vật tư"]) : 0,
                            TenVatTu = row["Tên vật tư"].ToString(),
                            Loai = row["Loại"].ToString(),
                            SoLuong = row["Số lượng"] != DBNull.Value ? Convert.ToInt32(row["Số lượng"]) : 0,
                            DonViTinh = row["Đơn vị tính"].ToString(),
                            ThongSoKyThuat = row["Thông số kỹ thuật"].ToString(),
                            GiaNhap = row["Giá Nhập"] != DBNull.Value ? Convert.ToDecimal(row["Giá nhập"]) : 0,
                            NgayNhap = row["Ngày nhập"] != DBNull.Value ? Convert.ToDateTime(row["Ngày nhập"]) : default(DateTime),
                            TinhTrang = row["Tình trạng"].ToString(),
                            GiaKhauHao = row["Giá khấu hao"] != DBNull.Value ? Convert.ToDecimal(row["Giá khấu hao"]) : 0
                        }).ToList();

                        // Lưu vào Session để sử dụng trong ConfirmImport
                        Session["PreviewData"] = previewData;
                    }
                    return View("Create"); // Chuyển đến view preview
                }
            }
            ModelState.AddModelError("", "Vui lòng chọn file Excel hợp lệ.");
            return View("Create");
        }


        [HttpPost]
        public ActionResult ConfirmImport()
        {
            var previewData = Session["PreviewData"] as List<PreviewItemSupply>;
            if (previewData != null)
            {
                using (var db = new QuanLyKhoGiaoHangEntities1())
                {
                    foreach (var row in previewData)
                    {
                        // Kiểm tra trùng lặp Id
                        if (!db.QuanLyVatTus.Any(d => d.IdVatTu == row.IdVatTu))
                        {
                            var vattu = new QuanLyVatTu
                            {
                                IdVatTu = row.IdVatTu,
                                TenVatTu = row.TenVatTu,
                                Loai = row.Loai,
                                SoLuong = row.SoLuong,
                                DonViTinh = row.DonViTinh,
                                ThongSoKyThuat = row.ThongSoKyThuat,
                                GiaNhap = row.GiaNhap,
                                NgayNhap = row.NgayNhap,
                                TinhTrang = row.TinhTrang,
                                GiaKhauHao = row.GiaKhauHao
                            };

                            db.QuanLyVatTus.Add(vattu);
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
            var dl = data.QuanLyVatTus.Find(Id);
            if (dl == null)
            {
                return Content("Không tìm thấy vật tư.");
            }
            return PartialView("Detail", dl);
        }

        public ActionResult Edit(int IdVatTu, string format)
        {
            var dl = data.QuanLyVatTus.FirstOrDefault(d => d.IdVatTu == IdVatTu);
            if (dl == null)
            {
                return HttpNotFound();
            }
            
            return View(dl);
        }

        [HttpPost]
        public ActionResult Update(int IdVatTu, QuanLyVatTu updatedData)
        {
            var dl = data.QuanLyVatTus.FirstOrDefault(d => d.IdVatTu == IdVatTu);
            if (dl == null)
            {
                return HttpNotFound();
            }
            dl.TenVatTu = updatedData.TenVatTu;
            dl.Loai = updatedData.Loai;
            dl.SoLuong = updatedData.SoLuong;
            dl.DonViTinh = updatedData.DonViTinh;
            dl.ThongSoKyThuat = updatedData.ThongSoKyThuat;
            dl.GiaNhap = updatedData.GiaNhap;
            dl.NgayNhap = updatedData.NgayNhap;
            dl.TinhTrang = updatedData.TinhTrang;
            dl.GiaKhauHao = updatedData.GiaKhauHao;
            data.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int IdVatTu)
        {
            var dl = data.QuanLyVatTus.FirstOrDefault(d => d.IdVatTu == IdVatTu);
            if (dl == null)
            {
                return HttpNotFound();
            }
            data.QuanLyVatTus.Remove(dl);
            data.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}