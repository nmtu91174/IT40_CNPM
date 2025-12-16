using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhacCuaTui.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            if(Session["Role"] == null)
            {
                return Redirect("~/Login/Login");
            }
            if (Session["Role"].Equals("Admin"))
            {
                return View();
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }
    }
}