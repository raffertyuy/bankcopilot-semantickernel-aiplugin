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

public class ListAccounts
{
    private readonly ILogger _logger;

    public ListAccounts(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ListAccounts>();
    }

    [OpenApiOperation(operationId: "ListAccounts", tags: new[] { "BankSkill" }, Description = "List the bank accounts available to the user. The list contains the account number, account name and account balance of each account.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "Returns a list of bank accounts.")]
    [Function("ListAccounts")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        if (req is null)
            throw new ArgumentNullException(nameof(req));

        var accounts = LocalRun();

        HttpResponseData okResponse = req.CreateResponse(HttpStatusCode.OK);
        okResponse.Headers.Add("Content-Type", "application/json");
        okResponse.WriteString(JsonConvert.SerializeObject(accounts));

        _logger.LogInformation($"ListAccounts function processed a request. Accounts: {JsonConvert.SerializeObject(accounts)}");

        return okResponse;
    }

    public static ICollection<Account> LocalRun()
    {
      return BankDataContext.Instance.Accounts;
    }
}
