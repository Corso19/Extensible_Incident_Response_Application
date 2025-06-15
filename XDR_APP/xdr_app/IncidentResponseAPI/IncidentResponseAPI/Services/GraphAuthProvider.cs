using Azure.Identity;
using Microsoft.Graph;

namespace IncidentResponseAPI.Services;

// This class handles the authentication with Microsoft Graph using Microsoft.Identity.Web, it was awful to research for this. Information was very segmented over the internet.
public class GraphAuthProvider
{
    public async Task<GraphServiceClient> GetAuthenticatedGraphClient(string clientSecret, string applicationId, string tenantId)
    {
        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };
            
        var clientSecretCredential = new ClientSecretCredential(tenantId, applicationId, clientSecret, options);
        return await Task.FromResult(new GraphServiceClient(clientSecretCredential));
    }

    // public GraphServiceClient GetUserDelegatedClient(string accessToken)
    // {
    //     return new GraphServiceClient(new DelegateAuthenticationProvider(request =>
    //     {
    //         request.Headers.Authorization = 
    //             new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    //         return Task.CompletedTask;
    //     }));
    // }
}