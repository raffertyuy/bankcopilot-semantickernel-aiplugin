namespace Models;

public class TransactionRecord
{
    public Account SourceAccount { get; set; }
    public Account DestinationAccount { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Remarks { get; set; }

    public TransactionRecord(Account sourceAccount, Account destinationAccount, decimal amount, string? remarks)
    {
        SourceAccount = sourceAccount ?? throw new ArgumentNullException(nameof(sourceAccount));
        DestinationAccount = destinationAccount ?? throw new ArgumentNullException(nameof(destinationAccount));
        Amount = amount;
        Date = DateTime.Now;
        Remarks = remarks ?? string.Empty;
    }
}
