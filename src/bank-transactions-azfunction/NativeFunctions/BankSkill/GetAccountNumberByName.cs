using System.Globalization;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models;

namespace NativeFunctions.BankSkill;

public class GetAccountNumberByName
{
    private readonly ILogger _logger;

    public GetAccountNumberByName(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GetAccountNumberByName>();
    }

    [OpenApiOperation(operationId: "GetAccountNumberByName", tags: new[] { "BankSkill" }, Description = "Gets the balance of a bank account")]
    [OpenApiParameter(name: "accountName", Description = "The bank account name", Required = true, In = ParameterLocation.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the bank account number")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]
    [Function("GetAccountNumberByName")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        if (req is null)
        {
            throw new ArgumentNullException(nameof(req));
        }

        var accountName = req.Query["accountName"];
        if (!string.IsNullOrEmpty(accountName))
        {
            var accountNumber = LocalRun(accountName);
            if (string.IsNullOrEmpty(accountNumber))
            {
                HttpResponseData notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                notFoundResponse.Headers.Add("Content-Type", "text/plain");
                notFoundResponse.WriteString("Account not found");

                return notFoundResponse;
            }
            else
            {
                HttpResponseData okResponse = req.CreateResponse(HttpStatusCode.OK);
                okResponse.Headers.Add("Content-Type", "text/plain");
                okResponse.WriteString(accountNumber);

                _logger.LogInformation($"GetAccountNumberByName function processed a request. Account number for {accountName} is {accountNumber}");

                return okResponse;
            }
        }
        else
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString("Please pass an accountName on the query string");

            return response;
        }
    }

    public static string LocalRun(string accountName)
    {
        var data = BankDataContext.Instance;
        var account = data.Accounts.FirstOrDefault(x => x.AccountName == accountName);

        return account?.AccountNumber;
    }
}
