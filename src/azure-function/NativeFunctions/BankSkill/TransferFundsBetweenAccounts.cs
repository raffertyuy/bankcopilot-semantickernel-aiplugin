using System.Globalization;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models;

namespace NativeFunctions.BankSkill;

public class TransferFundsBetweenAccounts
{
    private readonly ILogger _logger;

    public TransferFundsBetweenAccounts(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TransferFundsBetweenAccounts>();
    }

    [OpenApiOperation(operationId: "TransferFundsBetweenAccounts", tags: new[] { "BankSkill" }, Description = "Transfer funds from one bank account to another")]
    [OpenApiParameter(name: "sourceAccountNumber", Description = "The source account number to debit funds", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "destinationAccountNumber", Description = "The destination account number to credit funds", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "amount", Description = "The amount to transfer", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "remarks", Description = "User remarks for the fund transfer", Required = false, In = ParameterLocation.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "Returns the fund transfer success or failure message.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]
    [Function("TransferFundsBetweenAccounts")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        if (req is null)
        {
            throw new ArgumentNullException(nameof(req));
        }

        var sourceAccountNumber = req.Query["sourceAccountNumber"];
        if (string.IsNullOrEmpty(sourceAccountNumber))
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString("Please pass an sourceAccountNumber on the query string");

            return response;
        }

        var destinationAccountNumber = req.Query["destinationAccountNumber"];
        if (string.IsNullOrEmpty(destinationAccountNumber))
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString("Please pass an destinationAccountNumber on the query string");

            return response;
        }

        var isAmountValid = double.TryParse(req.Query["amount"], out var amount);
        isAmountValid = isAmountValid && amount > 0;
        if (!isAmountValid)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString("Please pass a valid amount that is > 0 on the query string");

            return response;
        }

        var remarks = req.Query["remarks"];

        var success = LocalRun(sourceAccountNumber, destinationAccountNumber, amount, remarks, out var responseMessage);
        if (success)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(responseMessage);

            _logger.LogInformation($"TransferFundsBetweenAccounts function processed a request. {responseMessage}");

            return response;
        }
        else
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(responseMessage);

            _logger.LogError($"TransferFundsBetweenAccounts function processed a request. {responseMessage}");

            return response;
        }
    }

    public bool LocalRun(string sourceAccountNumber, string destinationAccountNumber, double amount, string? remarks, out string responseMessage)
    {
        var data = BankDataContext.Instance;
        var sourceAccount = data.Accounts.FirstOrDefault(a => a.AccountNumber == sourceAccountNumber);
        var destinationAccount = data.Accounts.FirstOrDefault(a => a.AccountNumber == destinationAccountNumber);

        if (sourceAccount is null)
        {
            responseMessage = $"Source account {sourceAccountNumber} does not exist.";
            return false;
        }

        if (destinationAccount is null)
        {
            responseMessage = $"Destination account {destinationAccountNumber} does not exist.";
            return false;
        }

        if (sourceAccount.AccountBalance < amount)
        {
            responseMessage = $"Insufficient funds in source account {sourceAccountNumber}.";
            return false;
        }

        if (amount <= 0)
        {
            responseMessage = $"Invalid amount {amount}. Should be more than 0.";
            return false;
        }

        sourceAccount.AccountBalance -= amount;
        destinationAccount.AccountBalance += amount;
        data.TransactionHistory.Add(new TransactionRecord(sourceAccount, destinationAccount, amount, remarks));

        responseMessage = $"Successfully transferred ${amount} from {sourceAccount.AccountNumber} to {destinationAccount.AccountNumber}. The new balance of {sourceAccount.AccountNumber} is ${sourceAccount.AccountBalance}.";
        return true;
    }
}
