using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_OES.Filters;

namespace MVC_OES.Controllers
{
    [AuthorizeRoles("admin")]
    public class AdminController : Controller
    {

        public ActionResult Dashboard()
        {
            //if (Session["UserRole"]?.ToString() != "admin")
                //return RedirectToAction("Login", "Main");

            return View();
        }
    }
}