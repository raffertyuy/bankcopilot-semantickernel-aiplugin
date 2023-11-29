using Newtonsoft.Json;

public class CrmNote
{
    private string _title;
    private string _description;

    public string CustomerName { get; set; }

    public string Title
    {
        get => _title;
        set => _title = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException("Title account number cannot be null or empty.");
    }

    public string Description
    {
        get => _description;
        set => _description = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException("Description cannot be null or empty.");
    }
}
