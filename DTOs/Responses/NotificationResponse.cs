namespace UNAPLANNER_API.DTOs.Responses;

public class NotificationResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? RelatedId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? SentDate { get; set; }
}
