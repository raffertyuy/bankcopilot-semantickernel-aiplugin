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

public class TransferFundsBetweenAccounts
{
    private readonly ILogger _logger;

    public TransferFundsBetweenAccounts(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TransferFundsBetweenAccounts>();
    }

    [OpenApiOperation(operationId: "TransferFundsBetweenAccounts", tags: new[] { "BankSkill" }, Description = "Transfer funds from one bank account to another.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(TransactionRequest), Required = true, Description = "The fund transfer request parameters. Please pass a valid transaction request body with the format { \"sourceAccountNumber\": \"\", \"destinationAccountNumber\": \"\", \"amount\": 1000, \"remarks\": \"\" }")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "Returns the fund transfer success or failure message.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]
    [Function("TransferFundsBetweenAccounts")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        if (req is null)
        {
            throw new ArgumentNullException(nameof(req));
        }

        using (var reader = new StreamReader(req.Body))
        {
            var requestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
            _logger.LogInformation($"TransferFundsBetweenAccounts function started. Request body: {requestBody}");

            var request = JsonConvert.DeserializeObject<TransactionRequest>(requestBody);

            if (request is null)
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "application/json");
                response.WriteString("Please pass a valid transaction request body with the format { \"sourceAccountNumber\": \"\", \"destinationAccountNumber\": \"\", \"amount\": 1000, \"remarks\": \"\" }");

                return response;
            }

            var success = LocalRun(request, out var responseMessage);
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
    }

    public bool LocalRun(TransactionRequest request, out string responseMessage)
    {
        if (request is null)
        {
            responseMessage = "Invalid transaction request body.";
            return false;
        }

        var data = BankDataContext.Instance;
        var sourceAccount = data.Accounts.FirstOrDefault(a => a.AccountNumber == request.SourceAccountNumber);
        var destinationAccount = data.Accounts.FirstOrDefault(a => a.AccountNumber == request.DestinationAccountNumber);

        if (sourceAccount is null)
        {
            responseMessage = $"Source account {request.SourceAccountNumber} does not exist.";
            return false;
        }

        if (destinationAccount is null)
        {
            responseMessage = $"Destination account {request.DestinationAccountNumber} does not exist.";
            return false;
        }

        if (sourceAccount.AccountBalance < request.Amount)
        {
            responseMessage = $"Insufficient funds in source account {request.SourceAccountNumber}.";
            return false;
        }

        if (request.Amount <= 0)
        {
            responseMessage = $"Invalid amount {request.Amount}. Should be more than 0.";
            return false;
        }

        _logger.LogInformation($"TransferFundsBetweenAccounts validation completed. Executing funds transfer from {request.SourceAccountNumber} to {request.DestinationAccountNumber} with the amount of {request.Amount}.");

        sourceAccount.AccountBalance -= request.Amount;
        destinationAccount.AccountBalance += request.Amount;
        data.TransactionHistory.Add(new TransactionRecord(sourceAccount, destinationAccount, request.Amount, request.Remarks));

        responseMessage = $"Successfully transferred ${request.Amount} from {sourceAccount.AccountNumber} to {destinationAccount.AccountNumber}. The new balance of {sourceAccount.AccountNumber} is ${sourceAccount.AccountBalance}.";
        return true;
    }
}
