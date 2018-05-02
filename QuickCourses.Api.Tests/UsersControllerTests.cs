using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using QuickCourses.Api.Controllers;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Authentication;
using QuickCourses.Models.Errors;

namespace QuickCourses.Api.Tests
{
    [TestFixture]
    public class UsersControllerTests
    {
        [Test]
        public void PostValidUserTest()
        {
            var user = new User
            {
                Login = "123"
            };

            var repo = new Mock<IUserRepository>();
            repo.Setup(x => x.ContainsAsync(It.IsAny<string>())).Returns(() => Task.FromResult(false));
            repo.Setup(x => x.InsertAsync(It.IsAny<User>())).Returns(() => Task.FromResult(string.Empty));
            var usersController = new UsersController(repo.Object);
            
            var response = usersController.PostUser(user).Result;

            Assert.IsInstanceOf<StatusCodeResult>(response);

            var result = (StatusCodeResult) response;
            var expectedResult = new StatusCodeResult(StatusCodes.Status201Created);
            
            Assert.AreEqual(expectedResult.StatusCode, result.StatusCode);
        }

        [Test]
        public void PostInvalidUserTest()
        {
            var usersController = new UsersController(null);
            var user = new User();

            var response = usersController.PostUser(user).Result;

            var extectedResult = new Error
            {
                Code = Error.ErrorCode.BadArgument,
                Message = "Invalid user object"
            };

            Utilits.CheckResponseValue<BadRequestObjectResult, Error>(response, extectedResult);
        }

        [Test]
        public void PostExistingUserTest()
        {
            var user = new User
            {
                Login = "123"
            };

            var repo = new Mock<IUserRepository>();
            repo
                .Setup(x => x.ContainsByLoginAsync(user.Login))
                .Returns(() => Task.FromResult(true));

            var usersController = new UsersController(repo.Object);

            var response = usersController.PostUser(user).Result;

            var extectedResult = new Error {
                Code = Error.ErrorCode.InvalidOperation,
                Message = $"User with login {user.Login} already exists"
            };

            Utilits.CheckResponseValue<BadRequestObjectResult, Error>(response, extectedResult);
        }
    }
}
