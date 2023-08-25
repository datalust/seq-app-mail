using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Seq.App.Mail.Microsoft365;

class GraphClientMailGateway : IMicrosoftGraphMailGateway
{
    public async Task SendAsync(Microsoft365Options options, Message message, CancellationToken cancel)
    {
        // This implementation follows the article at:
        //   https://medium.com/medialesson/how-to-send-emails-in-net-with-the-microsoft-graph-a97b57430bbd
            
        var credential = new ClientSecretCredential(options.TenantId, options.ClientId, options.ClientSecret);
        using var graphClient = new GraphServiceClient(credential);

        await graphClient.Users[message.From!.EmailAddress!.Address]
            .SendMail
            .PostAsync(new() { Message = message, SaveToSentItems = options.SaveToSentItems }, cancellationToken: cancel);
    }
}
