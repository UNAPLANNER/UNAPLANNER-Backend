namespace UNAPLANNER_API.DTOs.Responses;

public class CareerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TotalCredits { get; set; }
}
