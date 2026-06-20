using System.ComponentModel.DataAnnotations;

namespace UNAPLANNER_API.DTOs.Requests;

public class RegisterDeviceTokenRequest
{
    [Required(ErrorMessage = "El token FCM es requerido")]
    [MinLength(1, ErrorMessage = "El token FCM no puede estar vacío")]
    public string FcmToken { get; set; } = string.Empty;

    public string? DeviceName { get; set; }
}
