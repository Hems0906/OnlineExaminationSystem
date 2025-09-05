using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_OES.Models;
using OfficeOpenXml;

namespace MVC_OES.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Questions()
        {
            var questions = db.Questions.ToList();
            return View(questions);
        }

        [HttpGet]
        public ActionResult UploadQuestions()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadQuestions(HttpPostedFileBase file, string questionType /* MCQ or TrueFalse */)
        {
            if (file == null || file.ContentLength == 0 || Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            {
                TempData["ErrorMessage"] = "Invalid file or no file selected.";
                return RedirectToAction("UploadQuestions");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(file.InputStream))
            {
                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null)
                {
                    TempData["ErrorMessage"] = "Excel file has no worksheet.";
                    return RedirectToAction("UploadQuestions");
                }

                // Validate template format based on chosen type
                if (questionType == "TrueFalse")
                {
                    if (!ValidateTrueFalseFormat(ws))
                    {
                        TempData["ErrorMessage"] = "Invalid True/False template format.";
                        return RedirectToAction("UploadQuestions");
                    }
                }
                else // MCQ
                {
                    if (!ValidateMCQFormat(ws))
                    {
                        TempData["ErrorMessage"] = "Invalid MCQ template format.";
                        return RedirectToAction("UploadQuestions");
                    }
                }

                // Read rows starting from row 2 (skip header)
                int startRow = 2;
                int totalRows = ws.Dimension.End.Row;

                for (int row = startRow; row <= totalRows; row++)
                {
                    try
                    {
                        var question = new Question();

                        question.Subject = ws.Cells[row, 1].Text.Trim();
                        question.QuestionType = ws.Cells[row, 2].Text.Trim();
                        question.QuestionText = ws.Cells[row, 3].Text.Trim();

                        if (questionType == "TrueFalse")
                        {
                            // True/False does not have options
                            question.OptionA = null;
                            question.OptionB = null;
                            question.OptionC = null;
                            question.OptionD = null;

                            question.CorrectAnswer = ws.Cells[row, 4].Text.Trim();
                        }
                        else
                        {
                            // MCQ options
                            question.OptionA = ws.Cells[row, 4].Text.Trim();
                            question.OptionB = ws.Cells[row, 5].Text.Trim();
                            question.OptionC = ws.Cells[row, 6].Text.Trim();
                            question.OptionD = ws.Cells[row, 7].Text.Trim();

                            question.CorrectAnswer = ws.Cells[row, 8].Text.Trim();
                        }

                        // Validate required fields to avoid adding empty rows
                        if (string.IsNullOrEmpty(question.Subject) ||
                            string.IsNullOrEmpty(question.QuestionType) ||
                            string.IsNullOrEmpty(question.QuestionText) ||
                            string.IsNullOrEmpty(question.CorrectAnswer))
                        {
                            // Skip incomplete rows
                            continue;
                        }

                        db.Questions.Add(question);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle errors if needed
                        // For now, just skip problematic rows
                        continue;
                    }
                }

                db.SaveChanges();
            }

            TempData["SuccessMessage"] = "Questions uploaded successfully!";
            return RedirectToAction("Questions");
        }

        private bool ValidateMCQFormat(ExcelWorksheet ws)
        {
            return string.Equals(ws.Cells[1, 1].Text.Trim(), "Subject", StringComparison.OrdinalIgnoreCase)
                && string.Equals(ws.Cells[1, 2].Text.Trim(), "QuestionType", StringComparison.OrdinalIgnoreCase)
                && string.Equals(ws.Cells[1, 3].Text.Trim(), "QuestionText", StringComparison.OrdinalIgnoreCase)
                && string.Equals(ws.Cells[1, 4].Text.Trim(), "OptionA", StringComparison.OrdinalIgnoreCase)
                && string.Equals(ws.Cells[1, 5].Text.Trim(), "OptionB", StringComparison.OrdinalIgnoreCase)
                && string.Equals(ws.Cells[1, 6].Text.Trim(), "OptionC", StringComparison.OrdinalIgnoreCase)
                && string.Equals(ws.Cells[1, 7].Text.Trim(), "OptionD", StringComparison.OrdinalIgnoreCase)
                && string.Equals(ws.Cells[1, 8].Text.Trim(), "CorrectAnswer", StringComparison.OrdinalIgnoreCase);
        }

        private bool ValidateTrueFalseFormat(ExcelWorksheet ws)
        {
            return string.Equals(ws.Cells[1, 1].Text.Trim(), "Subject", StringComparison.OrdinalIgnoreCase)
                && string.Equals(ws.Cells[1, 2].Text.Trim(), "QuestionType", StringComparison.OrdinalIgnoreCase)
                && string.Equals(ws.Cells[1, 3].Text.Trim(), "QuestionText", StringComparison.OrdinalIgnoreCase)
                && string.Equals(ws.Cells[1, 4].Text.Trim(), "CorrectAnswer", StringComparison.OrdinalIgnoreCase);
        }

        [HttpGet]
        public ActionResult AddQuestion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddQuestion(Question question)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();

                AppendQuestionToExcel(question);

                TempData["SuccessMessage"] = "Question added successfully.";
                return RedirectToAction("Questions");
            }

            return View(question);
        }

        private void AppendQuestionToExcel(Question question)
        {
            string filePath = Server.MapPath("~/ques/Questions.xlsx");
            FileInfo fileInfo = new FileInfo(filePath);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = fileInfo.Exists ? new ExcelPackage(fileInfo) : new ExcelPackage())
            {
                ExcelWorksheet worksheet;

                if (package.Workbook.Worksheets.Count == 0)
                {
                    worksheet = package.Workbook.Worksheets.Add("Questions");

                    worksheet.Cells[1, 1].Value = "Subject";
                    worksheet.Cells[1, 2].Value = "QuestionText";
                    worksheet.Cells[1, 3].Value = "QuestionType";
                    worksheet.Cells[1, 4].Value = "OptionA";
                    worksheet.Cells[1, 5].Value = "OptionB";
                    worksheet.Cells[1, 6].Value = "OptionC";
                    worksheet.Cells[1, 7].Value = "OptionD";
                    worksheet.Cells[1, 8].Value = "CorrectAnswer";
                }
                else
                {
                    worksheet = package.Workbook.Worksheets.First();
                }

                int newRow = worksheet.Dimension?.End.Row + 1 ?? 2;

                worksheet.Cells[newRow, 1].Value = question.Subject;
                worksheet.Cells[newRow, 2].Value = question.QuestionText;
                worksheet.Cells[newRow, 3].Value = question.QuestionType;
                worksheet.Cells[newRow, 4].Value = question.OptionA;
                worksheet.Cells[newRow, 5].Value = question.OptionB;
                worksheet.Cells[newRow, 6].Value = question.OptionC;
                worksheet.Cells[newRow, 7].Value = question.OptionD;
                worksheet.Cells[newRow, 8].Value = question.CorrectAnswer;

                package.SaveAs(fileInfo);
            }
        }

        public ActionResult DownloadTemplate(string type)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (string.IsNullOrEmpty(type))
                type = "MCQ";

            byte[] fileBytes;
            string fileName;

            if (type == "TrueFalse")
            {
                fileBytes = GenerateTrueFalseTemplateExcel();
                fileName = "TrueFalse_Question_Template.xlsx";
            }
            else
            {
                fileBytes = GenerateMCQTemplateExcel();
                fileName = "MCQ_Question_Template.xlsx";
            }

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private byte[] GenerateMCQTemplateExcel()
        {
            using (var ms = new MemoryStream())
            {
                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("MCQ Template");
                    ws.Cells[1, 1].Value = "Subject";
                    ws.Cells[1, 2].Value = "QuestionType";  // should be MCQ for MCQ template
                    ws.Cells[1, 3].Value = "QuestionText";
                    ws.Cells[1, 4].Value = "OptionA";
                    ws.Cells[1, 5].Value = "OptionB";
                    ws.Cells[1, 6].Value = "OptionC";
                    ws.Cells[1, 7].Value = "OptionD";
                    ws.Cells[1, 8].Value = "CorrectAnswer";

                    package.SaveAs(ms);
                }
                return ms.ToArray();
            }
        }

        private byte[] GenerateTrueFalseTemplateExcel()
        {
            using (var ms = new MemoryStream())
            {
                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("TrueFalse Template");
                    ws.Cells[1, 1].Value = "Subject";
                    ws.Cells[1, 2].Value = "QuestionType";  // should be TrueFalse for this template
                    ws.Cells[1, 3].Value = "QuestionText";
                    ws.Cells[1, 4].Value = "CorrectAnswer";
                    package.SaveAs(ms);
                }
                return ms.ToArray();
            }
        }

        public ActionResult DownloadAllQuestions()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("AllQuestions");

                ws.Cells[1, 1].Value = "Subject";
                ws.Cells[1, 2].Value = "QuestionText";
                ws.Cells[1, 3].Value = "QuestionType";
                ws.Cells[1, 4].Value = "OptionA";
                ws.Cells[1, 5].Value = "OptionB";
                ws.Cells[1, 6].Value = "OptionC";
                ws.Cells[1, 7].Value = "OptionD";
                ws.Cells[1, 8].Value = "CorrectAnswer";

                var questions = db.Questions.ToList();
                int row = 2;

                foreach (var q in questions)
                {
                    ws.Cells[row, 1].Value = q.Subject;
                    ws.Cells[row, 2].Value = q.QuestionText;
                    ws.Cells[row, 3].Value = q.QuestionType;
                    ws.Cells[row, 4].Value = q.OptionA;
                    ws.Cells[row, 5].Value = q.OptionB;
                    ws.Cells[row, 6].Value = q.OptionC;
                    ws.Cells[row, 7].Value = q.OptionD;
                    ws.Cells[row, 8].Value = q.CorrectAnswer;
                    row++;
                }

                var fileBytes = package.GetAsByteArray();

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "All_Questions.xlsx");
            }
        }

        public ActionResult DeleteQuestion(int id)
        {
            var question = db.Questions.Find(id);
            if (question != null)
            {
                db.Questions.Remove(question);
                db.SaveChanges();
            }

            return RedirectToAction("Questions");
        }
    }
}
