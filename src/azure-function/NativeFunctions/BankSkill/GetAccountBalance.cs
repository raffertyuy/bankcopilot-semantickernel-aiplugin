using System.Globalization;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models;

namespace NativeFunctions.BankSkill;

public class GetAccountBalance
{
    private readonly ILogger _logger;

    public GetAccountBalance(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GetAccountBalance>();
    }

    [OpenApiOperation(operationId: "GetAccountBalance", tags: new[] { "BankSkill" }, Description = "Gets the balance of a bank account")]
    [OpenApiParameter(name: "accountNumber", Description = "The bank account number", Required = true, In = ParameterLocation.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the balance amount of the bank account.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]
    [Function("GetAccountBalance")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        if (req is null)
        {
            throw new ArgumentNullException(nameof(req));
        }

        var accountNumber = req.Query["accountNumber"];
        if (!string.IsNullOrEmpty(accountNumber))
        {
            var response = LocalRun(accountNumber);
            if (response is null)
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
                okResponse.WriteString(response.Value.ToString());

                _logger.LogInformation($"GetAccountBalance function processed a request. AccountNumber: {accountNumber}, Balance: {response.Value}");

                return okResponse;
            }
        }
        else
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString("Please pass an accountNumber on the query string");

            return response;
        }
    }

    public static double? LocalRun(string accountNumber)
    {
        var data = BankDataContext.Instance;
        var account = data.Accounts.FirstOrDefault(x => x.AccountNumber == accountNumber);

        return account?.AccountBalance;
    }
}
