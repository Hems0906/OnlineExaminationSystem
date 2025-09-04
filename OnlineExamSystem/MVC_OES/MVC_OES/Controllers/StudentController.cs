using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_OES.Filters;

namespace MVC_OES.Controllers
{
    [AuthorizeRoles("student")]
    public class StudentController : Controller
    {
        public ActionResult Dashboard()
        {
            //if (Session["UserRole"]?.ToString() != "student")
            //    return RedirectToAction("Login", "Main");

            return View();
        }
    }
}