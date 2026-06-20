namespace UNAPLANNER_API.Infrastructure.Firebase;
// This factory class provides methods to create standardized notification payloads for different scenarios, such as activity reminders and course approvals.
public static class NotificationPayloadFactory
{
    public static NotificationPayload ActivityReminder(
        int calendarId, string activityTitle, string activityType, DateTime activityDate)
    {
        var (title, notifType) = activityType switch
        {
            "Examen"     => ("Recordatorio de Examen", "ExamReminder"),
            "Tarea"      => ("Recordatorio de Tarea", "TaskReminder"),
            "Proyecto"   => ("Recordatorio de Proyecto", "ProjectReminder"),
            "Exposicion" => ("Recordatorio de Exposición", "EventReminder"),
            "Evento"     => ("Recordatorio de Evento", "EventReminder"),
            _            => ("Recordatorio de Actividad", "General")
        };

        return new NotificationPayload
        {
            Title = title,
            Body = $"Tu actividad '{activityTitle}' está programada para el {activityDate:dd/MM/yyyy HH:mm}.",
            Data = new Dictionary<string, string>
            {
                ["calendarId"]    = calendarId.ToString(),
                ["type"]          = notifType,
                ["activityTitle"] = activityTitle,
                ["activityType"]  = activityType,
                ["activityDate"]  = activityDate.ToString("o")
            }
        };
    }
// This method creates a notification payload for when a course is approved, including the course name in the title and body, and relevant data for handling the notification on the client side.
    public static NotificationPayload CourseApproved(string courseName)
    {
        return new NotificationPayload
        {
            Title = "Curso Aprobado",
            Body = $"¡Felicidades! Has aprobado el curso '{courseName}'.",
            Data = new Dictionary<string, string>
            {
                ["type"]       = "CourseApproved",
                ["courseName"] = courseName
            }
        };
    }
// This method creates a general notification payload with a specified title and body, and includes a type of "General" in the data for client-side handling.
    public static NotificationPayload General(string title, string body)
    {
        return new NotificationPayload
        {
            Title = title,
            Body = body,
            Data = new Dictionary<string, string>
            {
                ["type"] = "General"
            }
        };
    }
}
