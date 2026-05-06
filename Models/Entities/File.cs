namespace UNAPLANNER_API.Models.Entities;

public class File
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? CourseId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty; // 'PDF', 'JPG', 'PNG', etc.

    public int SizeKB { get; set; } = 0;

    public DateTime UploadDate { get; set; } = DateTime.Now;

    // Relaciones
    public User User { get; set; } = null!;
    public Course? Course { get; set; }
}