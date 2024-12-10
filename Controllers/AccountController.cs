using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using NguyenNhutDuy_2122110447.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using static NguyenNhutDuy_2122110447.Controllers.AccountController;

namespace NguyenNhutDuy_2122110447.Controllers
{
    public class AccountController : Controller
    {
        QuanLyKhoGiaoHangEntities2 data = new QuanLyKhoGiaoHangEntities2();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(string name, string password)
        {
            var user = data.Accounts.FirstOrDefault(a => a.TenDangNhap == name);
            if (user != null)
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.MatKhau);
                if (isPasswordValid)
                {
                    Session["LoggedInUser"] = user.TenDangNhap;
                    var id = data.NhanViens.FirstOrDefault(n => n.IdNhanVien == user.IdNhanVien);
                    Session["IdUser"] = id.IdNhanVien;
                    Session["UserName"] = id.TenNhanVien;
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View();
        }
        public ActionResult Profile()
        {
            if (Session["IdUser"] != null)
            {
                int id = (int)Session["IdUser"];
                var user = data.NhanViens.FirstOrDefault(a => a.IdNhanVien == id);
                return View(user);
            }
            return RedirectToAction("Login", "Account");
        }
        public ActionResult Logout()
        {
            Session["LoggedInUser"] = null;
            Session["IdUser"] = null;
            Session["UserName"] = null;
            return RedirectToAction("Login", "Account");
        }
        public ActionResult ForgotPassword()
        {
            if (Session["LoggedInUser"] == null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}