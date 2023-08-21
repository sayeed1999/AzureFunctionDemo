using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GithubMonitorApp
{
    public static class GithubMonitor
    {
        [FunctionName("GithubMonitor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
            AuthorizationLevel.Function, // requires an api key to access the endpoint
            //AuthorizationLevel.Anonymous, // anyone can access
            "get", 
            "post", 
            Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Our Github monitor pprocessed an action.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            log.LogInformation(requestBody);

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
