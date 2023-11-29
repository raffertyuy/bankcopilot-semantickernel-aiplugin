using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Models;

public class CrmSettings
{
    public string DefaultCustomerName { get; set; } = string.Empty;

    public string SendNoteApiUrl { get; set; } = string.Empty;
}
