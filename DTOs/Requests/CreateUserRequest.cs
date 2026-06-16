using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.OpenApi.Any;
using System.Text.Json.Serialization;
namespace UNAPLANNER_API.DTOs.Requests;

public class CreateUserRequest
{

    [Required(ErrorMessage = "El correo es obligatorio")]
    [RegularExpression(
        @"^[\p{L}]+\.[\p{L}]+\.[\p{L}]+@est\.una\.ac\.cr$",
        ErrorMessage = "El correo debe tener formato nombre.primerapellido.segundoapellido@est.una.ac.cr"
    )]
    [DefaultValue("nombre.primerapellido.segundoapellido@est.una.ac.cr")]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [JsonIgnore]
    public int RoleId { get; set; }

    //Apply if the role is Student
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año es obligatorio")]
    [Range(1900, int.MaxValue, ErrorMessage = "El año debe ser mayor o igual a 1900")]
    [CurrentYearOrLess(ErrorMessage = "El año no puede ser mayor al año actual")]
    [DefaultValue(2026)]
    public int? EnterYear { get; set; }

    public int? CareerId { get; set; }
    public int? StudyPlanId { get; set; }

    /// <summary>
    ///  Custom validation to ensure that the year is not greater than the current year.
    /// </summary>
    public class CurrentYearOrLessAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int year)
            {
                if (year > DateTime.Now.Year)
                {
                    return new ValidationResult(ErrorMessage ?? $"El año no puede ser mayor a {DateTime.Now.Year}");
                }
            }
            return ValidationResult.Success!;
        }

    }
}
