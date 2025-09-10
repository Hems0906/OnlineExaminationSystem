using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Http.Results;
using System.Data.Entity;
using OnlineExaminationSystem.Controllers.Students;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Models.Profile;

namespace OES_Testing.StudentControllerTests
{
    [TestFixture]
    class UpdateProfile
    {
        [Test]
        public void UpdateProfile_ValidData_ReturnsSuccessMessage()
        {
            int userId = 1;
            int studentId = 101;

            var users = new List<User>
            {
                new User { user_Id = userId, role = "Student", reference_Id = studentId, email = "old@example.com" }
            }.AsQueryable();

            var students = new List<Student>
            {
                new Student {
                    stu_id = studentId,
                    stu_name = "Old Name",
                    mobile = "0000000000",
                    city = "Old City",
                    State = "Old State",
                    DOB = new System.DateTime(2000, 1, 1),
                    Qualification = "Old Degree",
                    Completion = "2020"
                }
            }.AsQueryable();

            var mockUserSet = new Mock<DbSet<User>>();
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockStudentSet = new Mock<DbSet<Student>>();
            mockStudentSet.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(students.Provider);
            mockStudentSet.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(students.Expression);
            mockStudentSet.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(students.ElementType);
            mockStudentSet.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(students.GetEnumerator());

            var mockDb = new Mock<OnlineExamSystemEntities2>();
            mockDb.Setup(db => db.Users).Returns(mockUserSet.Object);
            mockDb.Setup(db => db.Students).Returns(mockStudentSet.Object);
            mockDb.Setup(db => db.SaveChanges()).Returns(1);

            var controller = new ProfileController();
            var dbField = typeof(ProfileController).GetField("db", BindingFlags.NonPublic | BindingFlags.Instance);
            dbField.SetValue(controller, mockDb.Object);

            var model = new StudentProfileDto
            {
                email = "new@example.com",
                stu_name = "New Name",
                mobile = "9999999999",
                city = "New City",
                State = "New State",
                DOB = new System.DateTime(1999, 12, 31),
                Qualification = "New Degree",
                Completion = "2023"
            };

            var result = controller.UpdateProfile(model, userId);

            var okResult = result as OkNegotiatedContentResult<string>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content, Is.EqualTo("Profile updated successfully"));
        }

        [Test]
        public void UpdateProfile_UserNotFound_ReturnsNotFound()
        {
            int userId = 999; // non-existent

            var users = new List<User>().AsQueryable();
            var students = new List<Student>().AsQueryable();

            var mockUserSet = new Mock<DbSet<User>>();
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockStudentSet = new Mock<DbSet<Student>>();
            mockStudentSet.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(students.Provider);
            mockStudentSet.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(students.Expression);
            mockStudentSet.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(students.ElementType);
            mockStudentSet.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(students.GetEnumerator());

            var mockDb = new Mock<OnlineExamSystemEntities2>();
            mockDb.Setup(db => db.Users).Returns(mockUserSet.Object);
            mockDb.Setup(db => db.Students).Returns(mockStudentSet.Object);

            var controller = new ProfileController();
            var dbField = typeof(ProfileController).GetField("db", BindingFlags.NonPublic | BindingFlags.Instance);
            dbField.SetValue(controller, mockDb.Object);

            var model = new StudentProfileDto
            {
                email = "ghost@example.com",
                stu_name = "Ghost",
                mobile = "0000000000",
                city = "Nowhere",
                State = "None",
                DOB = new System.DateTime(1990, 1, 1),
                Qualification = "None",
                Completion = "1900"
            };

            var result = controller.UpdateProfile(model, userId);

            var contentResult = result as NegotiatedContentResult<string>;
            Assert.That(contentResult, Is.Not.Null);
            Assert.That(contentResult.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(contentResult.Content, Is.EqualTo("User not found"));
        }
    }
}
