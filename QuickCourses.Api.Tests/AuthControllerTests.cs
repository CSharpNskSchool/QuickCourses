using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using QuickCourses.Models.Authentication;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace QuickCourses.Api.Tests
{
   [TestFixture]
    public class AuthControllerTests
    {
        TestServer server;
        HttpClient client;
        Ticket ticket;

        [SetUp]
        public void Setup()
        {
            server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
        
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
                RequestUri = new Uri(server.BaseAddress + "api/v0/auth"),
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
                RequestUri = new Uri(server.BaseAddress + "api/v0/auth"),
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
                RequestUri = new Uri(server.BaseAddress + "api/v0/courses")
            };

            request.Headers.Add("Authorization", "Bearer " + ticket.Source);

            var response = client.SendAsync(request).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test]
        [Order(2)]
        public void Auth_CantUseApiWithBrokenTicket()
        {
            Assert.NotNull(ticket);
            ticket.Source += "1";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(server.BaseAddress + "api/v0/courses")
            };

            request.Headers.Add("Authorization", "Bearer " + ticket.Source);

            var response = client.SendAsync(request).Result;

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }

        [Test]
        [Order(3)]
        public void Auth_CantUseApiWithOldTicket()
        {
            Assert.NotNull(ticket);

            var leftTime = (int)Math.Floor(ticket.Over.Subtract(DateTime.Now).TotalMilliseconds);

            if (leftTime > 0)
            {
                Thread.Sleep(leftTime);
            }
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(server.BaseAddress + "api/v0/courses")
            };

            request.Headers.Add("Authorization", "Bearer " + ticket.Source);

            var response = client.SendAsync(request).Result;

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }
        [Test]
        public void Auth_CantUseMethodApiWithoutTicket()
        {
            var response = client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(server.BaseAddress + "api/v0/courses")

            }).Result;

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }

        [TearDown]
        public void Dispose()
        {
            client.Dispose();
            server.Dispose();
        }
    }
}
