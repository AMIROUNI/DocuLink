using System.ComponentModel.DataAnnotations;

namespace DocuLink.Models.ViewModels
{
    public class RequiredIfDoctorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as dynamic;
            if (model != null && model.IsDoctor && string.IsNullOrEmpty(value?.ToString()))
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
