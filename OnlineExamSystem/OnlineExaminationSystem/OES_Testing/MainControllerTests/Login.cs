using Moq;
using NUnit.Framework;
using OnlineExaminationSystem.Controllers.Home;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Models.Home;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Http.Results;

namespace OES_Testing.MainControllerTests
{
    [TestFixture]
    public class LoginTests
    {
        [Test]
        public void Login_ValidStudentCredentials_ReturnsOk()
        {
            // Arrange
            var users = new List<User>
            {
                new User {
                    user_Id = 1,
                    email = "student@example.com",
                    password = "pass123",
                    role = "Student",
                    reference_Id = 101
                }
            }.AsQueryable();

            var students = new List<Student>
            {
                new Student {
                    stu_id = 101,
                    stu_name = "John Doe"
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

            Console.WriteLine("User count in mockDb: " + mockDb.Object.Users.Count());

            var controller = new MainController();
            var dbField = typeof(MainController).GetField("db", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(dbField, Is.Not.Null, "Reflection failed to find 'db' field");
            dbField.SetValue(controller, mockDb.Object);

            var login = new StudentLogin
            {
                Email = "student@example.com",
                Password = "pass123"
            };

            // Act
            var rawResult = controller.Login(login);
            Console.WriteLine("Returned type: " + rawResult.GetType().Name);

            
            var contentProperty = rawResult.GetType().GetProperty("Content");
            Assert.That(contentProperty, Is.Not.Null, "Result does not contain a 'Content' property");

            var content = contentProperty.GetValue(rawResult);
            Assert.That(content, Is.Not.Null, "Content is null");

            
            var roleProp = content.GetType().GetProperty("role");
            var emailProp = content.GetType().GetProperty("email");
            var userNameProp = content.GetType().GetProperty("userName");

            Assert.Multiple(() =>
            {
                Assert.That(roleProp?.GetValue(content)?.ToString(), Is.EqualTo("Student"));
                Assert.That(emailProp?.GetValue(content)?.ToString(), Is.EqualTo("student@example.com"));
                Assert.That(userNameProp?.GetValue(content)?.ToString(), Is.EqualTo("John Doe"));
            });
        }
    }
}
