using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Requests;
public class DeleteUserRequest
{
    [Required(ErrorMessage = "La contraseña actual es obligatoria")]
    [StringLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
    public string ExistingPassword { get; set; } = string.Empty;
    
}