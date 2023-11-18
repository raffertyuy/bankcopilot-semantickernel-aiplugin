using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models;
using Newtonsoft.Json;

namespace NativeFunctions.BankSkill;

public class UpdateBillingAddress
{
    private readonly ILogger _logger;

    public UpdateBillingAddress(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<UpdateBillingAddress>();
    }

    [OpenApiOperation(operationId: "UpdateBillingAddress", tags: new[] { "BankSkill" }, Description = "Transfer funds from one bank account to another.")]
    [OpenApiRequestBody(contentType: "text/plain", bodyType: typeof(string), Required = true, Description = "The new billing address.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the fund transfer success or failure message.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    [Function("UpdateBillingAddress")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        if (req is null)
        {
            throw new ArgumentNullException(nameof(req));
        }

        using (var reader = new StreamReader(req.Body))
        {
            var requestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
            _logger.LogInformation($"UpdateBillingAddress function started. Request body: {requestBody}");

            var success = LocalRun(requestBody, out var responseMessage);
            if (success)
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain");
                response.WriteString(responseMessage);

                _logger.LogInformation($"UpdateBillingAddress function processed a request. {responseMessage}");

                return response;
            }
            else
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "text/plain");
                response.WriteString(responseMessage);

                _logger.LogError($"UpdateBillingAddress function processed a request. {responseMessage}");

                return response;
            }
        }
    }

    public bool LocalRun(string newAddress, out string responseMessage)
    {
        if (string.IsNullOrEmpty(newAddress))
        {
            responseMessage = "A new address needs to be specified";
            return false;
        }

        BankDataContext.Instance.CurrentCustomer.BillingAddress = newAddress;
        responseMessage = $"Update address successful";
        return true;
    }
}
