using DocumentFormat.OpenXml.Office2010.Excel;
using NguyenNhutDuy_2122110447.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenNhutDuy_2122110447.Controllers
{
    public class PersonnelController : BaseController
    {
        // GET: Personnel
        QuanLyKhoGiaoHangQuanLyKhoGiaoHangEntities1Entities data = new QuanLyKhoGiaoHangEntities1();
        public ActionResult Index(int? page, string searchTerm, string sortPersonnel)
        {
            var personnels = data.NhanViens.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                personnels = personnels.Where(o => o.TenNhanVien.Contains(searchTerm));
            }
            switch (sortPersonnel)
            {
                case "asc":
                    personnels = personnels.OrderBy(o => o.Luong);
                    break;
                case "desc":
                    personnels = personnels.OrderByDescending(o => o.Luong);
                    break;
                default:
                    personnels = personnels.OrderBy(o => o.IdNhanVien);
                    break;
            }
            int pageSize = 5;
            int pageNumber = page ?? 1;
            var pagedPersonnel = personnels.ToPagedList(pageNumber, pageSize);
            return View(pagedPersonnel);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Save(NhanVien nhanVien) 
        {
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            var dl = data.NhanViens;
            dl.Add(nhanVien);
            data.SaveChanges();
            return RedirectToAction("Index"); 
        }


        public ActionResult Detail(int Id)
        {
            var dl = data.NhanViens.Find(Id);
            if (dl == null)
            {
                return Content("Không tìm thấy nhân viên.");
            }
            return PartialView("Detail", dl);
        }

        public ActionResult Edit(int IdNhanVien)
        {
            var dl = data.NhanViens.FirstOrDefault(d => d.IdNhanVien == IdNhanVien);
            if (dl == null)
            {
                return HttpNotFound();
            }
            return View(dl);
        }

        [HttpPost]
        public ActionResult Update(int IdNhanVien, NhanVien updatedData)
        {
            var dl = data.NhanViens.FirstOrDefault(d => d.IdNhanVien == IdNhanVien);
            if (dl == null)
            {
                return HttpNotFound();
            }
            dl.TenNhanVien = updatedData.TenNhanVien;
            dl.ThuongTru = updatedData.ThuongTru;
            dl.Sdt = updatedData.Sdt;
            dl.Gmail = updatedData.Gmail;
            dl.Luong = updatedData.Luong;
            dl.Sdt = updatedData.Sdt;
            data.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int IdNhanVien)
        {
            var dl = data.NhanViens.FirstOrDefault(d => d.IdNhanVien == IdNhanVien);
            if (dl == null)
            {
                return HttpNotFound();
            }
            data.NhanViens.Remove(dl);
            data.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Violate(int? page, string searchTerm, string sortViolate)
        {
            var violates = data.LoiNhanViens.Include("NhanVien").AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                violates = violates.Where(o => o.MoTaLoi.Contains(searchTerm));
            }

            switch (sortViolate)
            {
                case "asc":
                    violates = violates.OrderBy(o => o.ChiPhiPhatSinh);
                    break;
                case "desc":
                    violates = violates.OrderByDescending(o => o.ChiPhiPhatSinh);
                    break;
                default:
                    violates = violates.OrderBy(o => o.IdNhanVien);
                    break;
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;
            var pagedViolates = violates.ToPagedList(pageNumber, pageSize);

            return View(pagedViolates);
        }


        public ActionResult CreateViolate()
        {
            var nhanViens = data.NhanViens.ToList(); // Lấy danh sách nhân viên từ database
            ViewBag.IdNhanVien = new SelectList(nhanViens, "IdNhanVien", "TenNhanVien"); // Tạo SelectList
            return View();
        }

        [HttpPost]
        public ActionResult SaveViolate(LoiNhanVien nhanVien)
        {
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            var dl = data.LoiNhanViens;
            dl.Add(nhanVien);
            data.SaveChanges();
            return RedirectToAction("Violate");
        }
        public ActionResult DeleteViolate(int Id)
        {
            var dl = data.LoiNhanViens.FirstOrDefault(d => d.Id == Id);
            if (dl == null)
            {
                return HttpNotFound();
            }
            data.LoiNhanViens.Remove(dl);
            data.SaveChanges();
            return RedirectToAction("Violate");
        }


    }
}