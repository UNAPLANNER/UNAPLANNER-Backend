namespace UNAPLANNER_API.DTOs.Requests;

public class ChangePasswordRequest
{
    public string? CurrentPassword { get; set; }
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
}
