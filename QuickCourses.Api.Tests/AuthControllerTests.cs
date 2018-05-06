using NUnit.Framework;
using QuickCourses.Models.Authentication;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System;
using System.Threading;
using QuickCourses.TestHelper;

namespace QuickCourses.Api.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        QuickCoursesTestServer server;
        HttpClient client;
        Ticket ticket;

        [OneTimeSetUp]
        public void Setup()
        {
            server = new QuickCoursesTestServer();
            server.UseCourses(TestCourses.CreateBasicSample());
            server.UseUsers(TestUsers.CreateSuperUserSample(), TestUsers.CreateUserSample());
            client = server.CreateClient();
        }

        [Test]
        public void Auth_CantEnterWithBadAccount()
        {
            var account = new AuthData
            {
                Login = "Vasa",
                Password = "12345"
            };

            var serializedAccount = JsonConvert.SerializeObject(account);

            var response = client.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(server.BaseAddress + "api/v1/auth"),
                Content = new StringContent(serializedAccount, Encoding.UTF8, "application/json")

            }).Result;


            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        [Order(0)]
        public void Auth_CanEnterWithExitsAccount()
        {
            var account = new AuthData
            {
                Login = "mihail",
                Password = "sexbandit"
            };

            var serializedAccount = JsonConvert.SerializeObject(account);

            var response = client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(server.BaseAddress + "api/v1/auth"),
                Content = new StringContent(serializedAccount, Encoding.UTF8, "application/json")

            }).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            ticket = JsonConvert.DeserializeObject<Ticket>(response.Content.ReadAsStringAsync().Result);
        }

        [Test]
        [Order(1)]
        public void Auth_CanUseApiWhenHaveTicket()
        {
            Assert.NotNull(ticket);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(server.BaseAddress + "api/v1/courses")
            };

            request.Headers.Add("Authorization", ticket.ToString());

            var response = client.SendAsync(request).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        [Order(2)]
        public void Auth_CantUseApiWithBrokenTicket()
        {
            Assert.NotNull(ticket);
            var badTicket = new Ticket
            {
                Source = ticket.Source + "hacked",
                ValidUntil = ticket.ValidUntil
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(server.BaseAddress + "api/v1/courses")
            };

            request.Headers.Add("Authorization", badTicket.ToString());

            var response = client.SendAsync(request).Result;

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }

        [Test]
        [Order(3)]
        public void Auth_CantUseApiWithOldTicket()
        {
            Assert.NotNull(ticket);

            var leftTime = (int)Math.Ceiling(ticket.ValidUntil.Subtract(DateTime.UtcNow).TotalMilliseconds);

            if (leftTime > 0)
            {
                Thread.Sleep(leftTime);
            }
            
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(server.BaseAddress + "api/v1/courses")
            };

            request.Headers.Add("Authorization", ticket.ToString());

            var response = client.SendAsync(request).Result;

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Auth_CantUseMethodApiWithoutTicket()
        {
            var response = client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(server.BaseAddress + "api/v1/courses")

            }).Result;

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }

        [Test]
        public void Auth_ClientCanGetUserDataByLogin() 
        {
            var account = new AuthData
            {
                Login = "bot",
                Password = "12345"
            };

            var serializedAccount = JsonConvert.SerializeObject(account);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(server.BaseAddress + "api/v1/auth"),
                Content = new StringContent(serializedAccount, Encoding.UTF8, "application/json")
            };

            var response = client.SendAsync(request).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var ticket = JsonConvert.DeserializeObject<Ticket>(response.Content.ReadAsStringAsync().Result);

            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(server.BaseAddress + "api/v1/auth"),
            };

            request.Headers.Add("Authorization", ticket.ToString());
            request.Headers.Add("Login", "mihail");
            response = client.SendAsync(request).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            client.Dispose();
            server.Dispose();
        }
    }
}
