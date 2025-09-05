using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using MVC_OES.Models.Questions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MVC_OES.Filters;
using System.IO;
using OfficeOpenXml;

namespace MVC_OES.Controllers.Admin
{
    [AuthorizeRoles("admin")]
    public class QuestionController : Controller
    {
        private readonly string baseUrl = "https://localhost:44377/api/admin/";

        [HttpGet]
        public async Task<ActionResult> GetQuestions(int courseId, int levelNumber)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (s, c, ch, e) => true;

            List<QuestionModel> questions = new List<QuestionModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync($"getQuestions/{courseId}/{levelNumber}");
                var raw = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    questions = JsonConvert.DeserializeObject<List<QuestionModel>>(raw);
                }
                else
                {
                    TempData["ApiError"] = $"API error: {response.StatusCode} — {raw}";
                }
            }

            ViewBag.CourseId = courseId;
            ViewBag.LevelNumber = levelNumber;

            return View(questions);
        }

        [HttpPost]
        public async Task<JsonResult> ToggleStatus(int id)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (s, c, ch, e) => true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var response = await client.PutAsync($"toggleQuestion/{id}", null);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Status updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update status." });
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditQuestion(int questionId, int courseId, int levelNumber)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (s, c, ch, e) => true;

            QuestionModel question = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync($"getQuestions/{courseId}/{levelNumber}");
                var raw = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var questions = JsonConvert.DeserializeObject<List<QuestionModel>>(raw);
                    question = questions.FirstOrDefault(q => q.QuestionId == questionId);
                }
            }

            if (question == null)
                return HttpNotFound();

            ViewBag.CourseId = courseId;
            ViewBag.LevelNumber = levelNumber;

            return View(question);
        }

        [HttpPost]
        public async Task<ActionResult> EditQuestion(QuestionModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.PutAsJsonAsync($"editQuestion/{model.QuestionId}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Question updated successfully!";
                    return RedirectToAction("GetQuestions", new { courseId = model.CourseId, levelNumber = model.LevelNumber });
                }
                else
                {
                    TempData["ApiError"] = "Failed to update question.";
                    return View(model);
                }
            }
        }

        [HttpGet]
        public ActionResult AddQuestion(int courseId, int levelNumber)
        {
            var model = new QuestionModel
            {
                CourseId = courseId,
                LevelNumber = levelNumber
            };

            return View(model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddQuestion(QuestionModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.PostAsJsonAsync(
                    $"addQuestion/{model.CourseId}/{model.LevelNumber}", model
                );

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Question added successfully!";
                    return RedirectToAction("GetQuestions", new
                    {
                        courseId = model.CourseId,
                        levelNumber = model.LevelNumber
                    });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Error: {error}");
                    return View(model);
                }
            }
        }

        [HttpGet]
        public ActionResult UploadQuestions(int courseId, int levelNumber)
        {
            ViewBag.CourseId = courseId;
            ViewBag.LevelNumber = levelNumber;
            return View(new Models.Questions.UploadQuestionModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadQuestions(int courseId, int levelNumber, Models.Questions.UploadQuestionModel model)
        {
            if (model.ExcelFile == null || model.ExcelFile.ContentLength == 0)
            {
                ModelState.AddModelError("ExcelFile", "Please select a valid Excel file.");
                return View(model);
            }

            var questions = new List<QuestionModel>();

            ExcelPackage.License.SetNonCommercialOrganization("Yogesh V");


            using (var package = new ExcelPackage(model.ExcelFile.InputStream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) 
                {
                    var question = new QuestionModel
                    {
                        QuestionText = worksheet.Cells[row, 1].Text,
                        OptionA = worksheet.Cells[row, 2].Text,
                        OptionB = worksheet.Cells[row, 3].Text,
                        OptionC = worksheet.Cells[row, 4].Text,
                        OptionD = worksheet.Cells[row, 5].Text,
                        Answer = worksheet.Cells[row, 6].Text,
                        Marks = int.TryParse(worksheet.Cells[row, 7].Text, out int marks) ? marks : 1,
                        CourseId = courseId,
                        LevelNumber = levelNumber,
                        Status = true
                    };

                    questions.Add(question);
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                foreach (var q in questions)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(q), System.Text.Encoding.UTF8, "application/json");
                    await client.PostAsync($"addQuestion/{courseId}/{levelNumber}", content);
                }
            }

            TempData["Success"] = "Questions uploaded successfully!";
            return RedirectToAction("GetQuestions", new { courseId = courseId, levelNumber = levelNumber });
        }

    }
}