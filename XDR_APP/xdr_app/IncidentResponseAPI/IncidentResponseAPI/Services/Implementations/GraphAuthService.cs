using Azure.Identity;
using Microsoft.Graph.Models;
using Microsoft.Graph.Drives.Item;
using IncidentResponseAPI.Services.Interfaces;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace IncidentResponseAPI.Services.Implementations
{
    public class GraphAuthService : IGraphAuthService
    {
        private readonly GraphAuthProvider _graphAuthProvider;
        private readonly ILogger<GraphAuthService> _logger;

        public GraphAuthService(GraphAuthProvider graphAuthProvider, ILogger<GraphAuthService> logger)
        {
            _graphAuthProvider = graphAuthProvider;
            _logger = logger;
        }

        //Email related operations
        public async Task<Dictionary<string, List<Message>>> FetchEmailsForAllUsersAsync(
            string clientSecret, string applicationId, string tenantId, DateTime? lastProcessedTime,
            CancellationToken cancellationToken)
        {
            var graphClient = await GetAuthenticatedGraphClient(clientSecret, applicationId, tenantId);
            var emailsByUser = new Dictionary<string, List<Message>>();

            var users = await graphClient.Users.GetAsync(cancellationToken: cancellationToken);
            foreach (var user in users.Value)
            {
                var userId = user.Id;

                // Ensure strict greater than comparison
                var filter = lastProcessedTime.HasValue
                    ? $"receivedDateTime gt {lastProcessedTime.Value.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}"
                    : null;

                _logger.LogInformation("Fetching emails with filter: {Filter}", filter);

                var messages = await graphClient.Users[userId]
                    .MailFolders["Inbox"]
                    .Messages
                    .GetAsync(requestConfiguration =>
                    {
                        if (!string.IsNullOrEmpty(filter))
                        {
                            requestConfiguration.QueryParameters.Filter = filter;
                            requestConfiguration.QueryParameters.Orderby = new[] { "receivedDateTime" };
                        }
                    }, cancellationToken);

                if (messages?.Value != null)
                {
                    // Double-check timestamps to ensure no duplicates
                    var filteredMessages = messages.Value
                        .Where(m => !lastProcessedTime.HasValue ||
                                    m.ReceivedDateTime > lastProcessedTime.Value)
                        .OrderBy(m => m.ReceivedDateTime)
                        .ToList();

                    emailsByUser[userId] = filteredMessages;

                    _logger.LogInformation("Retrieved {Count} new messages for user {UserId}",
                        filteredMessages.Count, userId);
                }
            }

            return emailsByUser;
        }

        public async Task<Message> FetchMessageContentAsync(string clientSecret, string applicationId, string tenantId,
            string messageId)
        {
            var graphClient =
                await _graphAuthProvider.GetAuthenticatedGraphClient(clientSecret, applicationId, tenantId);
            return await graphClient.Me.Messages[messageId].GetAsync();
        }

        public async Task<IEnumerable<Attachment>> FetchAttachmentsAsync(string clientSecret, string applicationId,
            string tenantId, string messageId, string userPrincipalName, CancellationToken cancellationToken)
        {
            try
            {
                // Authenticate using client credentials
                var graphClient = await GetAuthenticatedGraphClient(clientSecret, applicationId, tenantId);

                // Use the specific user's mailbox
                var attachments = await graphClient.Users[userPrincipalName]
                    .Messages[messageId]
                    .Attachments
                    .GetAsync(cancellationToken: cancellationToken);

                return attachments.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching attachments for message ID: {MessageId}",
                    messageId);
                throw;
            }
        }

        //Teams related operations

        public async Task<List<ChatMessage>> FetchTeamsMessagesAsync(string clientSecret, string applicationId,
            string tenantId, DateTime? since,
            CancellationToken cancellationToken)
        {
            try
            {
                var graphClient = await GetAuthenticatedGraphClient(clientSecret, applicationId, tenantId);

                // Creating filter for fetching messages since specified time
                var filter = since.HasValue
                    ? $"createdDateTime gt {since.Value.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}"
                    : null;

                _logger.LogInformation("Fetching Teams messages with filter: {Filter}", filter);

                // Get all users first
                var users = await graphClient.Users.GetAsync(cancellationToken: cancellationToken);

                if (users?.Value == null)
                {
                    _logger.LogWarning("No users found");
                    return new List<ChatMessage>();
                }

                var allMessages = new List<ChatMessage>();

                // For each user, get their chats
                foreach (var user in users.Value)
                {
                    try
                    {
                        // This is the key change - getting chats per user instead of top-level
                        var chats = await graphClient.Users[user.Id].Chats
                            .GetAsync(cancellationToken: cancellationToken);

                        // For each chat, get its messages
                        if (chats?.Value != null)
                        {
                            foreach (var chat in chats.Value)
                            {
                                try
                                {
                                    var chatMessages = await graphClient.Chats[chat.Id].Messages.GetAsync(
                                        requestConfiguration =>
                                        {
                                            if (!string.IsNullOrEmpty(filter))
                                            {
                                                requestConfiguration.QueryParameters.Filter = filter;
                                                requestConfiguration.QueryParameters.Orderby =
                                                    new[] { "createdDateTime" };
                                            }
                                        },
                                        cancellationToken);

                                    if (chatMessages?.Value != null)
                                    {
                                        allMessages.AddRange(chatMessages.Value);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error fetching messages for chat {ChatId} of user {UserId}",
                                        chat.Id, user.Id);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error fetching chats for user {UserId}", user.Id);
                    }
                }

                // Order all messages by creation time
                return allMessages.OrderBy(m => m.CreatedDateTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Teams messages");
                throw;
            }
        }

        public async Task<byte[]> FetchTeamsAttachmentAsync(string clientSecret, string applicationId, string tenantId,
            string messageId,
            string attachmentId, CancellationToken cancellationToken)
        {
            try
            {
                var graphClient = await GetAuthenticatedGraphClient(clientSecret, applicationId, tenantId);
                var chats = await graphClient.Chats.GetAsync(cancellationToken: cancellationToken);

                foreach (var chat in chats.Value)
                {
                    try
                    {
                        var message = await graphClient.Chats[chat.Id].Messages[messageId]
                            .GetAsync(cancellationToken: cancellationToken);

                        if (message != null && message.Attachments != null)
                        {
                            var attachment = message.Attachments.FirstOrDefault(a => a.Id == attachmentId);
                            //Handling different attachment types
                            if (!string.IsNullOrEmpty(attachment.ContentUrl))
                            {
                                //For hosted content, download from the URL
                                using var httpClient = new HttpClient();
                                return await httpClient.GetByteArrayAsync(new Uri(attachment.ContentUrl),
                                    cancellationToken);
                            }
                            else if (attachment.Content != null)
                            {
                                //for inline content, return the byte array directly
                                return Convert.FromBase64String(attachment.Content);
                            }
                        }
                    }
                    catch (ServiceException)
                    {
                        _logger.LogInformation("Message not found in chat {ChatId} for message ID: {MessageId}",
                            chat.Id, messageId);
                        continue;
                    }
                }

                throw new KeyNotFoundException(
                    $"No attachment found for message ID: {messageId} and attachment ID: {attachmentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Teams attachment for message ID: {MessageId}",
                    messageId);
                throw;
            }
        }

        //SharePoint related operations

        // public async Task<List<DriveItem>> FetchSharePointActivitiesAsync(
        //     string clientSecret,
        //     string applicationId,
        //     string tenantId,
        //     DateTime? since,
        //     CancellationToken cancellationToken)
        // {
        //     try
        //     {
        //         var graphClient = await GetAuthenticatedGraphClient(clientSecret, applicationId, tenantId);
        //         var allItems = new List<DriveItem>();
        //
        //         // Get all users first
        //         var users = await graphClient.Users.GetAsync(cancellationToken: cancellationToken);
        //
        //         if (users?.Value == null)
        //         {
        //             _logger.LogWarning("No users found");
        //             return new List<DriveItem>();
        //         }
        //
        //         // For each user, get their OneDrive/SharePoint activities
        //         foreach (var user in users.Value)
        //         {
        //             try
        //             {
        //                 // Create the filter for retrieving items since specified time
        //                 var filter = since.HasValue
        //                     ? $"lastModifiedDateTime gt {since.Value.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}"
        //                     : null;
        //
        //                 // Get the user's OneDrive items ordered by last modified date
        //                 var driveItems = await graphClient.Users[user.Id].Drive.Items
        //                     .GetAsync(requestConfig =>
        //                     {
        //                         if (!string.IsNullOrEmpty(filter))
        //                         {
        //                             requestConfig.QueryParameters.Filter = filter;
        //                         }
        //                     }, cancellationToken);
        //
        //                 if (driveItems?.Value != null)
        //                 {
        //                     allItems.AddRange(driveItems.Value);
        //                 }
        //             }
        //             catch (Exception ex)
        //             {
        //                 _logger.LogError(ex, "Error fetching SharePoint activities for user {UserId}", user.Id);
        //                 // Continue with other users
        //             }
        //         }
        //
        //         return allItems.OrderBy(i => i.LastModifiedDateTime).ToList();
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error occurred while fetching SharePoint activities");
        //         throw;
        //     }
        // }


        // public async Task<byte[]> FetchSharePointFileContentAsync(
        //     string clientSecret,
        //     string applicationId,
        //     string tenantId,
        //     string itemId,
        //     CancellationToken cancellationToken)
        // {
        //     try
        //     {
        //         var graphClient = await GetAuthenticatedGraphClient(clientSecret, applicationId, tenantId);
        //
        //         // Find the item across all user drives
        //         var users = await graphClient.Users.GetAsync(cancellationToken: cancellationToken);
        //
        //         foreach (var user in users.Value)
        //         {
        //             try
        //             {
        //                 // Try to get item content through user's drive
        //                 var stream = await graphClient.Users[user.Id].Drive.Items[itemId].Content
        //                     .GetAsync(cancellationToken);
        //
        //                 if (stream != null)
        //                 {
        //                     using var memoryStream = new MemoryStream();
        //                     await stream.CopyToAsync(memoryStream, cancellationToken);
        //                     return memoryStream.ToArray();
        //                 }
        //             }
        //             catch
        //             {
        //                 // Item not found in this user's drive, continue to next user
        //                 continue;
        //             }
        //         }
        //
        //         _logger.LogWarning("File with ID {ItemId} not found in any user drives", itemId);
        //         return null;
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error occurred while fetching SharePoint file content");
        //         throw;
        //     }
        // }

        // SharePoint related operations

        // Don’t import Microsoft.Graph.Drives.Item – it conflicts with the generated builders.
// using Microsoft.Graph; using Microsoft.Graph.Models; etc.

        public async Task<List<DriveItem>> FetchSharePointActivitiesAsync(
            string clientSecret,
            string applicationId,
            string tenantId,
            DateTime? since,
            CancellationToken cancellationToken)
        {
            var graphClient = await GetAuthenticatedGraphClient(clientSecret, applicationId, tenantId);
            var allItems = new List<DriveItem>();

            // 1) List all users
            var users = await graphClient.Users
                .GetAsync(requestConfiguration: null, cancellationToken);

            foreach (var user in users.Value)
            {
                try
                {
                    // 2) Get the user's default drive to find its ID
                    var drive = await graphClient.Users[user.Id]
                        .Drive
                        .GetAsync(requestConfiguration: null, cancellationToken);

                    // 3) List the root folder's children via the special "root" ID
                    var children = await graphClient.Drives[drive.Id]
                        .Items["root"]
                        .Children
                        .GetAsync(requestConfig =>
                            {
                                if (since.HasValue)
                                {
                                    // OData filter on lastModifiedDateTime
                                    requestConfig.QueryParameters.Filter =
                                        $"lastModifiedDateTime gt {since.Value.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}";
                                    requestConfig.QueryParameters.Orderby = new[] { "lastModifiedDateTime" };
                                }
                            },
                            cancellationToken);
                    if (children?.Value != null)
                        allItems.AddRange(children.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching SharePoint activities for user {UserId}", user.Id);
                }
            }

            // 4) Return ordered by modification time
            return allItems
                .Where(i => i.LastModifiedDateTime.HasValue)
                .OrderBy(i => i.LastModifiedDateTime)
                .ToList();
        }


        public async Task<byte[]> FetchSharePointFileContentAsync(
            string clientSecret,
            string applicationId,
            string tenantId,
            string fileId,
            CancellationToken cancellationToken)
        {
            var graphClient = await GetAuthenticatedGraphClient(clientSecret, applicationId, tenantId);

            // 1) Iterate each user looking for the file
            var users = await graphClient.Users
                .GetAsync(requestConfiguration: null, cancellationToken);

            foreach (var user in users.Value)
            {
                try
                {
                    // 2) Get this user's drive ID
                    var drive = await graphClient.Users[user.Id]
                        .Drive
                        .GetAsync(requestConfiguration: null, cancellationToken);

                    // 3) Stream the content via /drives/{drive-id}/items/{item-id}/content
                    using var stream = await graphClient.Drives[drive.Id]
                        .Items[fileId]
                        .Content
                        .GetAsync(requestConfiguration: null, cancellationToken);

                    if (stream != null)
                    {
                        using var ms = new MemoryStream();
                        await stream.CopyToAsync(ms, cancellationToken);
                        return ms.ToArray();
                    }
                }
                catch
                {
                    // Not in this user's drive—try the next one
                }
            }

            _logger.LogWarning("File with ID {FileId} not found in any user drives", fileId);
            return null;
        }


        //General operations
        public async Task<GraphServiceClient> GetAuthenticatedGraphClient(string clientSecret, string applicationId,
            string tenantId)
        {
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(tenantId, applicationId, clientSecret, options);
            return new GraphServiceClient(clientSecretCredential);
        }
    }
}