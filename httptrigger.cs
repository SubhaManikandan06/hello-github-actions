using System.IO;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MyProjFolder
{
    public class MyHttpTrigger
    {
        private readonly ILogger _logger;

        public MyHttpTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MyHttpTrigger>();
        }

        [Function("MyHttpTrigger")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Read query parameters
            var queryParameters = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string name = queryParameters["name"];

            // Read request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(requestBody);
            name = name ?? data.GetProperty("name").GetString();

            // Create a response
            var responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            await response.WriteStringAsync(responseMessage);

            return response;
        }
    }
}
