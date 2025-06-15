using IncidentResponseAPI.Services.Implementations.Handlers;
using IncidentResponseAPI.Services.Interfaces;

namespace IncidentResponseAPI.Services.Implementations;

public class SensorHandlerFactory : ISensorHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public SensorHandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISensorHandler GetHandlerForSensorType(string sensorTypeKey)
    {
        return sensorTypeKey switch
        {
            "MicrosoftEmail" => _serviceProvider.GetRequiredService<EmailSensorHandler>(),
            "MicrosoftTeams" => _serviceProvider.GetRequiredService<TeamsSensorHandler>(),
            "MicrosoftSharePoint" => _serviceProvider.GetRequiredService<SharePointSensorHandler>(),
            _ => throw new NotSupportedException($"Sensor type '{sensorTypeKey}' is not supported.")
        };
    }
}