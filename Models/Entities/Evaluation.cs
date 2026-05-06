namespace UNAPLANNER_API.Models.Entities;

public class Evaluation
{
    public int Id { get; set; }

    public int StudentProgressId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Percentage { get; set; }

    public decimal? Score { get; set; }

    public DateTime? Date { get; set; }

    public string EvaluationType { get; set; } = string.Empty; // 'Examen', 'Tarea', 'Proyecto', etc.

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Relaciones
    public StudentProgress StudentProgress { get; set; } = null!;
}