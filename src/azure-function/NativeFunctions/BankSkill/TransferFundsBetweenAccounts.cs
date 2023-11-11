using System.Globalization;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace NativeFunctions.BankSkill;

public class TransferFundsBetweenAccounts
{
    private readonly ILogger _logger;

    public TransferFundsBetweenAccounts(ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<TransferFundsBetweenAccounts>();
    }

    [OpenApiOperation(operationId: "TransferFundsBetweenAccounts", tags: new[] { "BankSkill" }, Description = "Transfer funds from one bank account to another")]
    [OpenApiParameter(name: "sourceAccount", Description = "The source account to debit funds", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "destinationAccount", Description = "The destination account to credit funds", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "amount", Description = "The amount to transfer", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "remarks", Description = "User remarks for the fund transfer", Required = true, In = ParameterLocation.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the fund transfer success or failure message.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]
    [Function("TransferFundsBetweenAccounts")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        if (req is null)
        {
            throw new System.ArgumentNullException(nameof(req));
        }

        throw new NotImplementedException();
    }
}
