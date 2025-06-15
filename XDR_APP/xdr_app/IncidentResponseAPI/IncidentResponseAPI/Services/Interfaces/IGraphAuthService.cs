using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace IncidentResponseAPI.Services.Interfaces
{
    public interface IGraphAuthService
    {
        Task<Dictionary<string, List<Message>>> FetchEmailsForAllUsersAsync(
            string clientSecret, 
            string applicationId, 
            string tenantId, 
            DateTime? lastProcessedTime, 
            CancellationToken cancellationToken);
        Task<Message> FetchMessageContentAsync(string clientSecret, 
            string applicationId, 
            string tenantId, 
            string messageId);
        Task<IEnumerable<Attachment>> FetchAttachmentsAsync(
            string clientSecret, 
            string applicationId, 
            string tenantId, 
            string messageId, 
            string userPrincipalName,
            CancellationToken cancellationToken);
        Task<GraphServiceClient> GetAuthenticatedGraphClient(
            string clientSecret, 
            string applicationId, 
            string tenantId);
        
        //methods for Teams
        Task<List<ChatMessage>> FetchTeamsMessagesAsync(
            string clientSecret, 
            string applicationId, 
            string tenantId, 
            DateTime? since, 
            CancellationToken cancellationToken);
        
        Task<byte[]> FetchTeamsAttachmentAsync(
            string clientSecret, 
            string applicationId, 
            string tenantId,
            string messageId,
            string attachmentId, 
            CancellationToken cancellationToken);
        
        //methods for SharePoint
        
        /// <summary>
        /// Lists DriveItems (files/folders) in all SharePoint site document libraries
        /// modified since the given timestamp.
        /// </summary>
        Task<List<DriveItem>> FetchSharePointActivitiesAsync(
            string clientSecret, 
            string applicationId, 
            string tenantId, 
            DateTime? since, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Downloads the binary content of a specific DriveItem in SharePoint.
        /// </summary>
        Task<byte[]> FetchSharePointFileContentAsync(
            string clientSecret,
            string applicationId, 
            string tenantId, 
            string fileId, 
            CancellationToken cancellationToken);
    }
}