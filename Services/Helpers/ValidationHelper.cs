using System.ComponentModel.DataAnnotations;

namespace Services.Helpers;

public static class ValidationHelper
{
    internal static void ModelValidation(object model)
    {
        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);
        if (!isValid) throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
    }
}