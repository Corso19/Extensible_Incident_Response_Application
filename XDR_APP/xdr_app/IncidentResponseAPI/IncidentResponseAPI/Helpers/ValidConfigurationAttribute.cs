using IncidentResponseAPI.Models;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace IncidentResponseAPI.Helpers
{
    public class ValidConfigurationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string configurationJson)
            {
                try
                {
                    // Deserialize the JSON into the Configuration object
                    var configuration = JsonConvert.DeserializeObject<Configuration>(configurationJson);

                    // Perform validation on the deserialized object
                    var validationResults = new List<ValidationResult>();
                    var context = new ValidationContext(configuration);
                    bool isValid = Validator.TryValidateObject(configuration, context, validationResults, true);

                    if (!isValid)
                    {
                        // Combine all validation errors into a single message
                        var errorMessages = string.Join("; ", validationResults.Select(v => v.ErrorMessage));
                        return new ValidationResult($"Invalid Configuration: {errorMessages}");
                    }
                }
                catch (JsonException)
                {
                    return new ValidationResult("Invalid JSON format for Configuration.");
                }
            }

            return ValidationResult.Success;
        }
    }
}