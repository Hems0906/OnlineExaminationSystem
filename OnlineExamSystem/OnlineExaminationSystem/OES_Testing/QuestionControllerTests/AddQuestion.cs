using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Results;
using System.Data.Entity;
using OnlineExaminationSystem.Controllers.Admin;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Models.Questions;


namespace OES_Testing.QuestionControllerTests
{
    [TestFixture]
    class AddQuestion
    {
        [Test]
        public void AddQuestion_ValidQuestion_ReturnsSuccessMessage()
        {
            int courseId = 1;
            int levelId = 101;

            var courses = new List<cours>
            {
                new cours { course_Id = courseId, course_name = "Algorithms", status = true }
            }.AsQueryable();

            var levels = new List<Level>
            {
                new Level { level_id = levelId, course_id = courseId, level_number = 1 }
            }.AsQueryable();

            var questions = new List<Question>().AsQueryable();

            var mockCourseSet = new Mock<DbSet<cours>>();
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Provider).Returns(courses.Provider);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Expression).Returns(courses.Expression);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.ElementType).Returns(courses.ElementType);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.GetEnumerator()).Returns(courses.GetEnumerator());

            var mockLevelSet = new Mock<DbSet<Level>>();
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.Provider).Returns(levels.Provider);
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.Expression).Returns(levels.Expression);
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.ElementType).Returns(levels.ElementType);
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.GetEnumerator()).Returns(levels.GetEnumerator());

            var mockQuestionSet = new Mock<DbSet<Question>>();
            mockQuestionSet.As<IQueryable<Question>>().Setup(m => m.Provider).Returns(questions.Provider);
            mockQuestionSet.As<IQueryable<Question>>().Setup(m => m.Expression).Returns(questions.Expression);
            mockQuestionSet.As<IQueryable<Question>>().Setup(m => m.ElementType).Returns(questions.ElementType);
            mockQuestionSet.As<IQueryable<Question>>().Setup(m => m.GetEnumerator()).Returns(questions.GetEnumerator());

            mockQuestionSet.Setup(m => m.Add(It.IsAny<Question>())).Returns((Question q) =>
            {
                q.QuestionId = 501;
                return q;
            });

            var mockDb = new Mock<OnlineExamSystemEntities2>();
            mockDb.Setup(db => db.courses).Returns(mockCourseSet.Object);
            mockDb.Setup(db => db.Levels).Returns(mockLevelSet.Object);
            mockDb.Setup(db => db.Questions).Returns(mockQuestionSet.Object);
            mockDb.Setup(db => db.SaveChanges()).Returns(1);

            var controller = new QuestionController();
            var dbField = typeof(QuestionController).GetField("db", BindingFlags.NonPublic | BindingFlags.Instance);
            dbField.SetValue(controller, mockDb.Object);

            var model = new QuestionModel
            {
                QuestionText = "What is a binary tree?",
                OptionA = "A data structure",
                OptionB = "A sorting algorithm",
                OptionC = "A search method",
                OptionD = "None of the above",
                Answer = "A",
                Marks = 5
            };

            var rawResult = controller.AddQuestion(courseId, levels.First().level_number, model);

            var contentProperty = rawResult.GetType().GetProperty("Content");
            var content = contentProperty?.GetValue(rawResult);

            var messageProp = content?.GetType().GetProperty("message");
            var questionIdProp = content?.GetType().GetProperty("questionId");

            Assert.Multiple(() =>
            {
                Assert.That(messageProp?.GetValue(content)?.ToString(), Is.EqualTo("Question added successfully!"));
                Assert.That(questionIdProp?.GetValue(content)?.ToString(), Is.EqualTo("501"));
            });
        }

        [Test]
        public void AddQuestion_DuplicateQuestion_ReturnsBadRequest()
        {
            int courseId = 1;
            int levelId = 101;

            var courses = new List<cours>
            {
                new cours { course_Id = courseId, course_name = "Algorithms", status = true }
            }.AsQueryable();

            var levels = new List<Level>
            {
                new Level { level_id = levelId, course_id = courseId, level_number = 1 }
            }.AsQueryable();

            var existingQuestions = new List<Question>
            {
                new Question {
                    CourseId = courseId,
                    LevelNumber = 1,
                    QuestionText = "What is a binary tree?",
                    Status = true
                }
            }.AsQueryable();

            var mockCourseSet = new Mock<DbSet<cours>>();
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Provider).Returns(courses.Provider);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Expression).Returns(courses.Expression);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.ElementType).Returns(courses.ElementType);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.GetEnumerator()).Returns(courses.GetEnumerator());

            var mockLevelSet = new Mock<DbSet<Level>>();
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.Provider).Returns(levels.Provider);
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.Expression).Returns(levels.Expression);
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.ElementType).Returns(levels.ElementType);
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.GetEnumerator()).Returns(levels.GetEnumerator());

            var mockQuestionSet = new Mock<DbSet<Question>>();
            mockQuestionSet.As<IQueryable<Question>>().Setup(m => m.Provider).Returns(existingQuestions.Provider);
            mockQuestionSet.As<IQueryable<Question>>().Setup(m => m.Expression).Returns(existingQuestions.Expression);
            mockQuestionSet.As<IQueryable<Question>>().Setup(m => m.ElementType).Returns(existingQuestions.ElementType);
            mockQuestionSet.As<IQueryable<Question>>().Setup(m => m.GetEnumerator()).Returns(existingQuestions.GetEnumerator());

            var mockDb = new Mock<OnlineExamSystemEntities2>();
            mockDb.Setup(db => db.courses).Returns(mockCourseSet.Object);
            mockDb.Setup(db => db.Levels).Returns(mockLevelSet.Object);
            mockDb.Setup(db => db.Questions).Returns(mockQuestionSet.Object);

            var controller = new QuestionController();
            var dbField = typeof(QuestionController).GetField("db", BindingFlags.NonPublic | BindingFlags.Instance);
            dbField.SetValue(controller, mockDb.Object);

            var model = new QuestionModel
            {
                QuestionText = "What is a binary tree?",
                OptionA = "A data structure",
                OptionB = "A sorting algorithm",
                OptionC = "A search method",
                OptionD = "None of the above",
                Answer = "A",
                Marks = 5
            };

            var result = controller.AddQuestion(courseId, levels.First().level_number, model);

            var badRequest = result as BadRequestErrorMessageResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest.Message, Is.EqualTo("This question already exists for the selected course and level."));
        }
    }
}
