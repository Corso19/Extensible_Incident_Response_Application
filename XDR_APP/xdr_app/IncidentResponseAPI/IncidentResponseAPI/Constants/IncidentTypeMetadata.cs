using IncidentResponseAPI.Models;

namespace IncidentResponseAPI.Constants;

public class IncidentTypeMetadata
{
    private static readonly Dictionary<IncidentType, (int Severity, string Description)> Metadata = new()
    {
        { IncidentType.UnusualEmailVolume, (SeverityLevels.Low, "Detected an unusually high number of emails sent by a single sender in a short period.") },
        { IncidentType.SuspiciousAttachment, (SeverityLevels.Medium, "Suspicious attachment detected, such as .zip or .exe.") },
        { IncidentType.ExternalSender, (SeverityLevels.High, "Email received from a domain that is not part of the trusted domain list.") },
        { IncidentType.RepeatedEventPattern, (SeverityLevels.Medium, "Repeated event pattern detected.") }
    };
    
    public static (int Severity, string Description) GetMetadata(IncidentType type)
    {
        return Metadata.TryGetValue(type, out var metadata) ? metadata : (SeverityLevels.Low, string.Empty);
    }
}