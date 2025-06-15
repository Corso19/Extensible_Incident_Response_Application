namespace IncidentResponseAPI.Services.Interfaces
{
    public interface IConfigurationValidator
    {
        void Validate(string configurationJson);
    }
}