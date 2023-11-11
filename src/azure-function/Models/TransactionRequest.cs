using Newtonsoft.Json;

public class TransactionRequest
{
    private string _sourceAccountNumber;
    private string _destinationAccountNumber;

    [JsonProperty("sourceAccountNumber")]
    public string SourceAccountNumber 
    { 
        get => _sourceAccountNumber; 
        set => _sourceAccountNumber = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException("Source account number cannot be null or empty."); 
    }

    [JsonProperty("destinationAccountNumber")]
    public string DestinationAccountNumber 
    { 
        get => _destinationAccountNumber; 
        set => _destinationAccountNumber = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException("Destination account number cannot be null or empty."); 
    }

    private decimal _amount;

    [JsonProperty("amount")]
    public decimal Amount 
    { 
        get => _amount; 
        set => _amount = value > 0 ? value : throw new ArgumentException("Amount must be greater than zero."); 
    }

    [JsonProperty("remarks")]
    public string? Remarks { get; set; }

    public TransactionRequest(string sourceAccountNumber, string destinationAccountNumber, decimal amount, string? remarks = null)
    {
        _sourceAccountNumber = sourceAccountNumber ?? throw new ArgumentNullException(nameof(sourceAccountNumber));
        _destinationAccountNumber = destinationAccountNumber ?? throw new ArgumentNullException(nameof(destinationAccountNumber));
        Amount = amount;
        Remarks = remarks;
    }
}
