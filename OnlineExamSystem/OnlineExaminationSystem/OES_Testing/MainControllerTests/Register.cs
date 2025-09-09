using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Results;
using System.Data.Entity;
using OnlineExaminationSystem.Controllers.Home;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Models.Home;

namespace OES_Testing.MainControllerTests
{
    [TestFixture]
    public class RegisterTests
    {
        [Test]
        public void Register_NewStudent_ReturnsSuccessMessage()
        {
            // Arrange
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

            // Setup Add and SaveChanges behavior
            mockStudentSet.Setup(m => m.Add(It.IsAny<Student>())).Returns((Student s) => s);
            mockUserSet.Setup(m => m.Add(It.IsAny<User>())).Returns((User u) => u);

            var mockDb = new Mock<OnlineExamSystemEntities2>();
            mockDb.Setup(db => db.Users).Returns(mockUserSet.Object);
            mockDb.Setup(db => db.Students).Returns(mockStudentSet.Object);
            mockDb.Setup(db => db.SaveChanges()).Returns(1);

            var controller = new MainController();
            var dbField = typeof(MainController).GetField("db", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(dbField, Is.Not.Null, "Reflection failed to find 'db' field");
            dbField.SetValue(controller, mockDb.Object);

            var studentRegister = new StudentRegister
            {
                Email = "newstudent@example.com",
                Password = "securepass",
                StuName = "Jane Doe",
                Mobile = "9876543210",
                City = "Bangalore",
                State = "Karnataka",
                DOB = new System.DateTime(2000, 1, 1),
                Qualification = "B.Tech",
                Completion = "2022"
            };

            // Act
            var result = controller.register(studentRegister);

            // Assert
            var okResult = result as OkNegotiatedContentResult<string>;
            Assert.That(okResult, Is.Not.Null, "Expected Ok result but got something else");
            Assert.That(okResult.Content, Is.EqualTo("Registration Successful!!"));
        }
    }
}

