using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Results;
using System.Data.Entity;
using OnlineExaminationSystem.Controllers.Admin; 
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Models.Courses;

namespace OES_Testing.AdminControllerTests
{
    [TestFixture]
    public class AddCourseTests
    {
        [Test]
        public void AddCourse_ValidCourse_ReturnsSuccessMessage()
        {
            
            var existingCourses = new List<cours>().AsQueryable();

            var mockCourseSet = new Mock<DbSet<cours>>();
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Provider).Returns(existingCourses.Provider);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Expression).Returns(existingCourses.Expression);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.ElementType).Returns(existingCourses.ElementType);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.GetEnumerator()).Returns(existingCourses.GetEnumerator());

            mockCourseSet.Setup(m => m.Add(It.IsAny<cours>())).Returns((cours c) =>
            {
                c.course_Id = 1; 
                return c;
            });

            var mockDb = new Mock<OnlineExamSystemEntities2>();
            mockDb.Setup(db => db.courses).Returns(mockCourseSet.Object);
            mockDb.Setup(db => db.SaveChanges()).Returns(1);

            var controller = new AdminController(); 
            var dbField = typeof(AdminController).GetField("db", BindingFlags.NonPublic | BindingFlags.Instance);
            dbField.SetValue(controller, mockDb.Object);

            var model = new CourseModel { CourseName = "Data Structures" };

            
            var rawResult = controller.AddCourse(model);

            
            var contentProperty = rawResult.GetType().GetProperty("Content");
            var content = contentProperty?.GetValue(rawResult);

            var messageProp = content?.GetType().GetProperty("message");
            var courseIdProp = content?.GetType().GetProperty("courseId");
            var courseStatusProp = content?.GetType().GetProperty("courseStatus");

            Assert.Multiple(() =>
            {
                Assert.That(messageProp?.GetValue(content)?.ToString(), Is.EqualTo("Course added successfully"));
                Assert.That(courseIdProp?.GetValue(content)?.ToString(), Is.EqualTo("1"));
                Assert.That(courseStatusProp?.GetValue(content)?.ToString(), Is.EqualTo("True"));
            });
        }

        [Test]
        public void AddCourse_DuplicateCourseName_ReturnsBadRequest()
        {
            // Arrange
            var existingCourses = new List<cours>
            {
                new cours { course_Id = 1, course_name = "Data Structures", status = true }
            }.AsQueryable();

            var mockCourseSet = new Mock<DbSet<cours>>();
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Provider).Returns(existingCourses.Provider);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.Expression).Returns(existingCourses.Expression);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.ElementType).Returns(existingCourses.ElementType);
            mockCourseSet.As<IQueryable<cours>>().Setup(m => m.GetEnumerator()).Returns(existingCourses.GetEnumerator());

            var mockDb = new Mock<OnlineExamSystemEntities2>();
            mockDb.Setup(db => db.courses).Returns(mockCourseSet.Object);

            var controller = new AdminController(); // <-- updated controller
            var dbField = typeof(AdminController).GetField("db", BindingFlags.NonPublic | BindingFlags.Instance);
            dbField.SetValue(controller, mockDb.Object);

            var model = new CourseModel { CourseName = "Data Structures" };

            // Act
            var result = controller.AddCourse(model);

            // Assert
            var badRequest = result as BadRequestErrorMessageResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest.Message, Is.EqualTo("Course already exists."));
        }
    }
}
