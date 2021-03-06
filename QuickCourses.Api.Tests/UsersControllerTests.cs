﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using QuickCourses.Api.Controllers;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Models.Authentication;
using QuickCourses.Api.Models.Authentication;
using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Api.Tests
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UserData userData;
        private RegistrationInfo registrationInfo;

        [SetUp]
        public void Init()
        {
            userData = new UserData {
                Login = "123",
                Id = "123"
            };

            registrationInfo = new RegistrationInfo {
                Login = userData.Login
            };
        }

        [Test]
        public void PostValidUserTest()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(x => x.ContainsAsync(It.IsAny<string>())).Returns(() => Task.FromResult(false));
            repo.Setup(x => x.InsertAsync(It.IsAny<UserData>())).Returns(() => Task.FromResult(string.Empty));
            var usersController = new UsersController(repo.Object);
            
            var response = usersController.PostUser(registrationInfo).Result;

            Assert.IsInstanceOf<StatusCodeResult>(response);

            var result = (StatusCodeResult) response;
            var expectedResult = new StatusCodeResult(StatusCodes.Status201Created);
            
            Assert.AreEqual(expectedResult.StatusCode, result.StatusCode);
        }

        [Test]
        public void PostInvalidUserTest()
        {
            var usersController = new UsersController(null);
            var invalidUser = new RegistrationInfo();

            var response = usersController.PostUser(invalidUser).Result;

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
            var repo = new Mock<IUserRepository>();
            repo
                .Setup(x => x.ContainsByLoginAsync(registrationInfo.Login))
                .Returns(() => Task.FromResult(true));
            
            var usersController = new UsersController(repo.Object);

            var response = usersController.PostUser(registrationInfo).Result;

            var extectedResult = new Error {
                Code = Error.ErrorCode.InvalidOperation,
                Message = $"User with login {registrationInfo.Login} already exists"
            };

            Utilits.CheckResponseValue<BadRequestObjectResult, Error>(response, extectedResult);
        }

        [Test]
        public void GetIdTest()
        {
            var repo = new Mock<IUserRepository>();
            repo
                .Setup(x => x.GetByLoginAsync(registrationInfo.Login))
                .Returns(() => Task.FromResult(userData));
            
            var usersController = new UsersController(repo.Object);

            var response = usersController.GetId(registrationInfo.Login).Result;

            Utilits.CheckResponseValue<OkObjectResult, string>(response, userData.Id);
        }
    }
}
