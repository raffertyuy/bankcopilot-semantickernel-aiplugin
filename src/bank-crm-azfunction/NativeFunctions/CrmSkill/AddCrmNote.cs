using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models;
using Newtonsoft.Json;

namespace NativeFunctions.CrmSkill;

public class AddCrmNote
{
    private readonly ILogger _logger;

    public AddCrmNote(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AddCrmNote>();
    }

    [OpenApiOperation(operationId: "AddCrmNote", tags: new[] { "CrmSkill" }, Description = "Send a note to the CRM system.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CrmNote), Required = true, Description = "The CRM note parameters. Please pass a valid transaction request body with the format { \"customerName\": \"\", \"title\": \"\", \"description\": \"\" }")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the CRM note success or failure message.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    [Function("AddCrmNote")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        if (req is null)
        {
            throw new ArgumentNullException(nameof(req));
        }

        using (var reader = new StreamReader(req.Body))
        {
            var requestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
            _logger.LogInformation($"AddCrmNote function started. Request body: {requestBody}");

            var note = JsonConvert.DeserializeObject<CrmNote>(requestBody);

            if (note is null)
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "text/plain");
                response.WriteString("Please pass a CRM note request body with the format { \"customerName\": \"\", \"title\": \"\", \"description\": \"\" }");

                return response;
            }

            var success = await LocalRun(note);
            if (success)
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain");
                response.WriteString("AddCrmNote successful.");

                _logger.LogInformation($"AddCrmNote successful.");

                return response;
            }
            else
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "text/plain");
                response.WriteString("AddCrmNote failed.");

                _logger.LogError($"AddCrmNote failed.");

                return response;
            }
        }
    }

    public async Task<bool> LocalRun(CrmNote note)
    {
        if (note is null)
            return false;

        var appSettings = AppSettings.LoadSettings();

        // For demo purposes, we are using a default customer name.
        note.CustomerName = appSettings.Crm.DefaultCustomerName;

        var json = JsonConvert.SerializeObject(note);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.PostAsync(appSettings.Crm.SendNoteApiUrl, data);

            if (response.IsSuccessStatusCode)
                return true;
        }

        return false;
    }
}
