namespace Domain.Attachment;

public class Attachment
{
    public int Id { get; set; }

    public string OriFileName { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string Uri { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }
}
