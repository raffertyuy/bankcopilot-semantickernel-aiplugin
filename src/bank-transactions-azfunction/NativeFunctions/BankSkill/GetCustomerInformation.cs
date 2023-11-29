using System.Globalization;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models;
using Newtonsoft.Json;

namespace NativeFunctions.BankSkill;

public class GetCustomerInformation
{
    private readonly ILogger _logger;

    public GetCustomerInformation(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GetCustomerInformation>();
    }

    [OpenApiOperation(operationId: "GetCustomerInformation", tags: new[] { "BankSkill" }, Description = "Gets the information of the current customer")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "Returns the information details of the customer.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]
    [Function("GetCustomerInformation")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        if (req is null)
        {
            throw new ArgumentNullException(nameof(req));
        }

        var response = LocalRun();
        HttpResponseData okResponse = req.CreateResponse(HttpStatusCode.OK);
        okResponse.Headers.Add("Content-Type", "application/json");
        okResponse.WriteString(response);

        _logger.LogInformation($"GetCustomerInformation function processed a request.");

        return okResponse;
    }

    public static string LocalRun()
    {
        var customer = BankDataContext.Instance.CurrentCustomer;
        var customerWithoutAccounts = new
        {
            customer.IdNumber,
            customer.FullName,
            customer.BillingAddress
        };
        return JsonConvert.SerializeObject(customerWithoutAccounts);
    }
}
