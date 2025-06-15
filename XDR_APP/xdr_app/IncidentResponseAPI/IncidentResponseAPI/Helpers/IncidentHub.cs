using IncidentResponseAPI.Dtos;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace IncidentResponseAPI.Helpers;

public class IncidentHub : Hub
{
     public async Task NotifyNewIncidentCreated(IncidentDto incidentDto)
     {
         await Clients.All.SendAsync("ReceivedIncident", incidentDto);
     }
}