using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvcfinal.Models;

namespace mvcfinal.Controllers
{
   // [Authorize]
    public class StudentController : Controller
    {
        // Mock data (share with InstitutionController for demo)
        private static List<SubjectViewModel> subjects = InstitutionController.subjects;

        // Mock student exam results
        private static Dictionary<string, List<int>> studentResults = new Dictionary<string, List<int>>();

        public ActionResult Index()
        {
            return View(subjects);
        }

        [HttpGet]
        public ActionResult StartExam(int subjectId, int level)
        {
            var subject = subjects.FirstOrDefault(s => s.Id == subjectId);
            if (subject == null) return RedirectToAction("Index");

            // Get questions of selected level and shuffle
            var questions = subject.Questions
                .Where(q => q.Level == level)
                .OrderBy(q => Guid.NewGuid())
                .Take(5) // For demo, take 5 questions per level
                .ToList();

            ViewBag.SubjectName = subject.Name;
            ViewBag.Level = level;

            // Timer for level
            int timer = level == 1 ? subject.Level1Timer : level == 2 ? subject.Level2Timer : subject.Level3Timer;
            ViewBag.Timer = timer;

            return View(questions);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitExam(int subjectId, int level, List<string> answers)
        {
            // For demo, randomly calculate score
            Random rnd = new Random();
            int score = rnd.Next(0, answers.Count + 1);

            string key = User.Identity.Name + "_Subject_" + subjectId;
            if (!studentResults.ContainsKey(key))
                studentResults[key] = new List<int>();

            studentResults[key].Add(score);

            ViewBag.Score = score;
            ViewBag.Level = level;
            ViewBag.SubjectName = subjects.FirstOrDefault(s => s.Id == subjectId)?.Name;
            return View("Result");
        }
    }
}