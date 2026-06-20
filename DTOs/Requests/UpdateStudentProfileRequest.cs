using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;
namespace UNAPLANNER_API.DTOs.Requests;
public class UpdateStudentProfileRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string FullName { get; set; } = string.Empty;
    [Required(ErrorMessage = "La carrera es obligatoria")]
    [Range(1, int.MaxValue, ErrorMessage = "La carrera no es válida")]
    public int CareerId { get; set; }
    [Required(ErrorMessage = "El año es obligatorio")]
    [Range(1900, int.MaxValue, ErrorMessage = "El año debe ser mayor o igual a 1900")]
    [CurrentYearOrLess(ErrorMessage = "El año no puede ser mayor al año actual")]
    [DefaultValue(2026)]
    public int? EnterYear { get; set; }

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