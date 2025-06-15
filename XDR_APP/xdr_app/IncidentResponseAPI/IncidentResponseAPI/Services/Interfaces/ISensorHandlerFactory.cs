namespace IncidentResponseAPI.Services.Interfaces;

public interface ISensorHandlerFactory
{
    ISensorHandler GetHandlerForSensorType(string sensorTypeKey);
}