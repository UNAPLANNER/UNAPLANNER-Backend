using System.ComponentModel.DataAnnotations;

namespace UNAPLANNER_API.DTOs.Requests;

public class CreateCalendarEventRequest
{
    [Required(ErrorMessage = "El título del evento es obligatorio")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "La fecha del evento es obligatoria")]
    public DateTime ActivityDate { get; set; }

    [Required(ErrorMessage = "El tipo de actividad es obligatorio")]
    [StringLength(50, ErrorMessage = "El tipo de actividad no puede exceder 50 caracteres")]
    [RegularExpression(@"^(Examen|Tarea|Proyecto|Exposicion|Evento|Otro)$", 
        ErrorMessage = "El tipo de actividad debe ser uno de: Examen, Tarea, Proyecto, Exposicion, Evento, Otro")]
    public string ActivityType { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "El ID del curso debe ser válido si se proporciona")]
    public int? CourseId { get; set; }

    public bool HasReminder { get; set; } = false;

    public DateTime? ReminderDate { get; set; }
}
