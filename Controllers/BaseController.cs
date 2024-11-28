using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenNhutDuy_2122110447.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra nếu người dùng chưa đăng nhập
            if (Session["LoggedInUser"] == null)
            {
                // Chuyển hướng đến trang đăng nhập
                filterContext.Result = RedirectToAction("Login", "Account");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}