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
                    Session["UserName"] = id.TenNhanVien;
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View();
        }

        public ActionResult Logout()
        {
            Session["LoggedInUser"] = null;
            return RedirectToAction("Login", "Account");
        }
    }
}