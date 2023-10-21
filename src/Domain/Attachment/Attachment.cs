namespace Domain.Attachment;

public class Attachment
{
    public int Id { get; set; }

    public string Type { get; set; }

    public string OriFileName { get; set; }

    public string FileName { get; set; }

    public string Path { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }
}
