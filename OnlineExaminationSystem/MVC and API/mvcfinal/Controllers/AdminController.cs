using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvcfinal.Models;

namespace mvcfinal.Controllers
{

    //[Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        private static List<InstitutionViewModel> institutions = new List<InstitutionViewModel>
        {
            new InstitutionViewModel { Id=1, Name="ABC College", Email="abc@college.com", IsActive=true },
            new InstitutionViewModel { Id=2, Name="XYZ Institute", Email="xyz@institute.com", IsActive=false }
        };

       
        [HttpGet]
        public ActionResult AddInstitution()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInstitution(InstitutionViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Simulate adding new institution
                model.Id = institutions.Count + 1;
                institutions.Add(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult ToggleStatus(int id)
        {
            var inst = institutions.Find(x => x.Id == id);
            if (inst != null)
                inst.IsActive = !inst.IsActive;

            return RedirectToAction("Index");
        }
    }
}