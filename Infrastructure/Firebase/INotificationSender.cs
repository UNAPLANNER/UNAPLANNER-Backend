namespace UNAPLANNER_API.Infrastructure.Firebase;

public interface INotificationSender
{
    Task<bool> SendAsync(string token, NotificationPayload payload);
    Task<int> SendToMultipleAsync(IEnumerable<string> tokens, NotificationPayload payload);
}
// This class represents the payload of a notification, including the title, body, and any additional data.
public class NotificationPayload
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = new();
}
