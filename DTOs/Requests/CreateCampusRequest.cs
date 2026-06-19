using System.ComponentModel.DataAnnotations;
namespace UNAPLANNER_API.DTOs.Requests;
public class CreateCampusRequest
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "El codigo es requerido")]
    [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
    public string Code { get; set; }
}
