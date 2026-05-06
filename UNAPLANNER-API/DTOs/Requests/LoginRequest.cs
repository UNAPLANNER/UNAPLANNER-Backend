using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Requests;
public class LoginRequest
{
    [Required(ErrorMessage = "El correo de usuario es requerido")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La contraseña es requerida")]
    public string Password { get; set; } = string.Empty;
}