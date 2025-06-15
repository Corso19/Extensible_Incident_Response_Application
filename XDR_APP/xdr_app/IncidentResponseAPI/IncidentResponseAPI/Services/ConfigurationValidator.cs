using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using IncidentResponseAPI.Models;
using IncidentResponseAPI.Services.Interfaces;

namespace IncidentResponseAPI.Services;

public class ConfigurationValidator : IConfigurationValidator
{
    public void Validate(string configurationJson)
    {
        if (string.IsNullOrWhiteSpace(configurationJson))
        {
            throw new ValidationException("Configuration cannot be empty.");
        }

        try
        {
            // Deserialize the JSON into the Configuration object
            var configuration = JsonConvert.DeserializeObject<Configuration>(configurationJson);

            // Perform validation on the deserialized object
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(configuration);

            if (!Validator.TryValidateObject(configuration, context, validationResults, true))
            {
                var errors = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Invalid Configuration: {errors}");
            }
        }
        catch (JsonException ex)
        {
            throw new ValidationException("Invalid JSON format for Configuration.", ex);
        }
    }
}