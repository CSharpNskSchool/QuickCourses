using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using QuickCourses.Api.Models.Authentication;
using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Client.Infrastructure
{
    public class ClientBase : IDisposable
    {
        private readonly HttpClient client;
        private readonly string apiUrl;
        private readonly string version;

        protected ClientBase(ApiVersion apiVersion, string apiUrl, HttpClient client = null)
        {
            this.version = Enum.GetName(typeof(ApiVersion), apiVersion);
            this.apiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
            this.client = client ?? new HttpClient();
        }

        public void Dispose()
        {
            client.Dispose();
        }

        protected Task InvokeApiMethod(
            HttpMethod httpMethod,
            string path,
            Ticket ticket = null,
            object content = null,
            Dictionary<string, string> headers = null)
        {
            return MakeRequest(httpMethod, path, ticket, content, headers);
        }

        protected async Task<T> InvokeApiMethod<T>(
            HttpMethod httpMethod,
            string path,
            Ticket ticket = null,
            object content = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParameters = null)
        {
            var response = await MakeRequest(httpMethod, path, ticket, content, headers, queryParameters);

            var serializedObject = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(serializedObject);
        }

        private async Task<HttpResponseMessage> MakeRequest(
            HttpMethod httpMethod,
            string path,
            Ticket ticket = null,
            object content = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParameters = null)
        {
            var uriBuilder = new UriBuilder($"{apiUrl}/api/{version}/{path}");

            if (queryParameters != null)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (var parameter in queryParameters)
                {
                    query[parameter.Key] = parameter.Value;
                }

                uriBuilder.Query = query.ToString();
            }

            var request = new HttpRequestMessage(httpMethod, uriBuilder.Uri);

            if (ticket != null)
            {
                if (!ticket.IsValid())
                {
                    throw new InvalidOperationException("Ticket is old");
                }

                request.Headers.Add("Authorization", "Bearer " + ticket.Source);
            }

            if (content != null)
            {
                var serializedContent = JsonConvert.SerializeObject(content);
                request.Content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                HandleError(response);
            }

            return response;
        }

        private void HandleError(HttpResponseMessage response)
        {
            var serializedObject = response.Content.ReadAsStringAsync().Result;
            var error = JsonConvert.DeserializeObject<Error>(serializedObject);

            if (error == null)
            {
                throw new Exception(Enum.GetName(typeof(HttpStatusCode), response.StatusCode));
            }

            switch (error.Code)
            {
                case Error.ErrorCode.BadArgument:
                    throw new ArgumentException(error.Message);
                case Error.ErrorCode.InvalidOperation:
                    throw new InvalidOperationException(error.Message);
                case Error.ErrorCode.NotFound:
                    throw new KeyNotFoundException(error.Message);
                default:
                    throw new Exception(error.Message);
            }
        }
    }
}
