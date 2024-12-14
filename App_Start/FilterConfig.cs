using System.Web;
using System.Web.Mvc;

namespace NguyenNhutDuy_2122110447
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
