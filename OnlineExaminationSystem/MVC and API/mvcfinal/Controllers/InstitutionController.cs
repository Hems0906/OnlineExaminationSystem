using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvcfinal.Models;

namespace mvcfinal.Controllers
{
   // [Authorize]
    public class InstitutionController : Controller
    {
        // Mock data for demo
        public static List<SubjectViewModel> subjects = new List<SubjectViewModel>();

        public ActionResult Index()
        {
            return View(subjects);
        }

        [HttpGet]
        public ActionResult AddSubject()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSubject(SubjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = subjects.Count + 1;
                // Default timers for 3 levels
                model.Level1Timer = 10; // minutes
                model.Level2Timer = 15;
                model.Level3Timer = 20;

                subjects.Add(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult AddQuestion(int subjectId)
        {
            var subject = subjects.Find(s => s.Id == subjectId);
            if (subject == null) return RedirectToAction("Index");

            ViewBag.SubjectName = subject.Name;
            ViewBag.SubjectId = subjectId;
            return View(new QuestionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestion(QuestionViewModel model, int subjectId)
        {
            var subject = subjects.Find(s => s.Id == subjectId);
            if (subject == null) return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                model.Id = subject.Questions.Count + 1;
                subject.Questions.Add(model);
                return RedirectToAction("Index");
            }

            ViewBag.SubjectName = subject.Name;
            ViewBag.SubjectId = subjectId;
            return View(model);
        }

    }
}