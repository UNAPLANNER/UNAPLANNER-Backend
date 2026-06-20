using System.Text.Json.Serialization;

namespace UNAPLANNER_API.DTOs.Responses;

public class EvaluationResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string EvaluationType { get; set; } = string.Empty;

    public decimal Percentage { get; set; }

    [JsonPropertyName("grade")]
    public decimal? Score { get; set; }

    public DateTime? Date { get; set; }

    [JsonPropertyName("hasReminder")]
    public bool HasReminder { get; set; }
}
