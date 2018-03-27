using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace QuickCourses.Api.Tests
{
    public static class Utilits
    {
        public static void CheckResponseValue<TResponse, TValue>(IActionResult response, TValue expectedValue)
            where TResponse : ObjectResult
        {
            Assert.IsInstanceOf(typeof(TResponse), response);

            var value = ((TResponse)response).Value;

            var compareLogic = new CompareLogic();
            var compareResult = compareLogic.Compare(expectedValue, value);

            Assert.IsTrue(compareResult.AreEqual);
        }

        public static HttpContext CreateContext(string scheme, string host, string path)
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(context => context.Request.Scheme).Returns(() => scheme);
            httpContext.Setup(context => context.Request.Host).Returns(() => new HostString(host));
            httpContext.Setup(context => context.Request.Path).Returns(() => new PathString($"/{path}"));
            httpContext.Setup(context => context.Request.Query).Returns(() => new QueryCollection());

            return httpContext.Object;
        }
    }
}
