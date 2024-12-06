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
using PagedList.Mvc;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Data.SqlClient;
namespace NguyenNhutDuy_2122110447.Controllers
{
    public class OrderController : BaseController
    {
        private static DataTable _previewTable;
        QuanLyKhoGiaoHangEntities2 data = new QuanLyKhoGiaoHangEntities2();
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
            int pageSize = 5;
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
                        var dt = new DataTable();

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
            var previewData = Session["PreviewData"] as List<PreviewItem>;
            if (previewData != null)
            {
                using (var db = new QuanLyKhoGiaoHangEntities2())
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

                Session.Remove("PreviewData"); // Xóa dữ liệu sau khi lưu thành công
                ViewBag.Message = "Dữ liệu đã được thêm thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Không có dữ liệu để thêm.");
            return View("Index");
        }

        public ActionResult Detail(int Id)
        {
            var dl = data.DonHangs.FirstOrDefault(d => d.Id == Id);
            return View(dl);
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
    }
}