using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineExaminationSystem.Controllers.Admin;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Models.Levels;
using Moq;
using NUnit.Framework;
using System.Data;
using System.Data.Entity;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace OES_Testing.AdminContollerTests
{
    [TestFixture]
    class AddLevels
    {
        [Test]
        public void AddLevels_ValidLevels_ReturnsSuccessMessage()
        {
            int courseId = 1;

            var courses = new List<cours>
            {
                new cours { course_Id = courseId, course_name = "Algorithms", status = true }
            }.AsQueryable();

            var mockCourseSet = new Mock<DbSet<cours>>();
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Provider).Returns(courses.Provider);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Expression).Returns(courses.Expression);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.ElementType).Returns(courses.ElementType);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.GetEnumerator()).Returns(courses.GetEnumerator());

            var mockLevelSet = new Mock<DbSet<Level>>();
            mockLevelSet.Setup(m => m.Add(It.IsAny<Level>())).Returns((Level l) => l);

            var mockDb = new Mock<OnlineExamSystemEntities2>();
            mockDb.Setup(db => db.courses).Returns(mockCourseSet.Object);
            mockDb.Setup(db => db.Levels).Returns(mockLevelSet.Object);
            mockDb.Setup(db => db.SaveChanges()).Returns(1);

            var controller = new AdminController();
            var dbField = typeof(AdminController).GetField("db", BindingFlags.NonPublic | BindingFlags.Instance);
            dbField.SetValue(controller, mockDb.Object);

            var levels = new List<LevelsModel>
            {
                new LevelsModel {
                    LevelNumber = 1,
                    LevelName = "Beginner",
                    PassingMarks = 40,
                    TotalQuestions = 10,
                    Duration = 30
                },
                new LevelsModel {
                    LevelNumber = 2,
                    LevelName = "Intermediate",
                    PassingMarks = 50,
                    TotalQuestions = 15,
                    Duration = 45
                }
            };

            var rawResult = controller.AddLevels(courseId, levels);

            var contentProperty = rawResult.GetType().GetProperty("Content");
            var content = contentProperty?.GetValue(rawResult);

            var messageProp = content?.GetType().GetProperty("message");
            var courseIdProp = content?.GetType().GetProperty("CourseID");

            Assert.Multiple(() =>
            {
                Assert.That(messageProp?.GetValue(content)?.ToString(), Is.EqualTo("All levels added successfully"));
                Assert.That(courseIdProp?.GetValue(content)?.ToString(), Is.EqualTo(courseId.ToString()));
            });
        }

        [Test]
        public void AddLevels_DuplicateLevelNumber_ReturnsBadRequest()
        {
            int courseId = 1;

            var courses = new List<cours>
            {
                new cours { course_Id = courseId, course_name = "Algorithms", status = true }
            }.AsQueryable();

            var existingLevels = new List<Level>
            {
                new Level {
                    course_id = courseId,
                    level_number = 1,
                    level_name = "Beginner",
                    passing_marks = 40,
                    tot_ques = 10,
                    duration = 30
                }
            }.AsQueryable();

            var mockCourseSet = new Mock<DbSet<cours>>();
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Provider).Returns(courses.Provider);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Expression).Returns(courses.Expression);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.ElementType).Returns(courses.ElementType);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.GetEnumerator()).Returns(courses.GetEnumerator());

            var mockLevelSet = new Mock<DbSet<Level>>();
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.Provider).Returns(existingLevels.Provider);
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.Expression).Returns(existingLevels.Expression);
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.ElementType).Returns(existingLevels.ElementType);
            mockLevelSet.As<IQueryable<Level>>().Setup(m => m.GetEnumerator()).Returns(existingLevels.GetEnumerator());

            // Simulate duplicate detection
            mockLevelSet.Setup(m => m.Add(It.IsAny<Level>())).Callback<Level>(level =>
            {
                if (existingLevels.Any(l => l.course_id == level.course_id && l.level_number == level.level_number))
                    throw new InvalidOperationException("Level already exists.");
            });

            var mockDb = new Mock<OnlineExamSystemEntities2>();
            mockDb.Setup(db => db.courses).Returns(mockCourseSet.Object);
            mockDb.Setup(db => db.Levels).Returns(mockLevelSet.Object);
            mockDb.Setup(db => db.SaveChanges()).Returns(1);

            var controller = new AdminController();
            var dbField = typeof(AdminController).GetField("db", BindingFlags.NonPublic | BindingFlags.Instance);
            dbField.SetValue(controller, mockDb.Object);

            var levels = new List<LevelsModel>
            {
                new LevelsModel {
                    LevelNumber = 1,
                    LevelName = "Beginner",
                    PassingMarks = 40,
                    TotalQuestions = 10,
                    Duration = 30
                }
            };

            IHttpActionResult result;
            try
            {
                result = controller.AddLevels(courseId, levels);
            }
            catch (InvalidOperationException ex)
            {
                result = new BadRequestErrorMessageResult(ex.Message, controller);
            }

            var badRequest = result as BadRequestErrorMessageResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest.Message, Is.EqualTo("Level already exists."));
        }
    }
}
