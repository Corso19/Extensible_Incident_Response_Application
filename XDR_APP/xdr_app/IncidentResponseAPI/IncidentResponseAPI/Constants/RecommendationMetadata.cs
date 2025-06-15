using IncidentResponseAPI.Models;

namespace IncidentResponseAPI.Constants;

public static class RecommendationMetadata
{
    private static readonly Dictionary<IncidentType, string[]> Recommendations = new()
    {
        { IncidentType.UnusualEmailVolume, new[]
            {
                "Monitor sender's email patterns",
                "Check for automated sending",
                "Implement rate limiting"
            }
        },
        { IncidentType.SuspiciousAttachment, new[]
            {
                "Quarantine attachment",
                "Scan in sandbox",
                "Update blocking rules"
            }
        },
        { IncidentType.ExternalSender, new[]
            {
                "Verify sender identity",
                "Check domain reputation",
                "Update allowed senders"
            }
        },
        { IncidentType.RepeatedEventPattern, new[]
            {
                "Analyze pattern frequency",
                "Identify source",
                "Block if malicious"
            }
        }
    };

    public static string[] GetRecommendations(IncidentType type) =>
        Recommendations.TryGetValue(type, out var recommendations)
            ? recommendations
            : new[] { "Investigate and document the incident" };
}