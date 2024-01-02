using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graph.Models;

namespace Seq.App.Mail.Microsoft365;

interface IMicrosoftGraphMailGateway
{
    Task SendAsync(Microsoft365Options options, Message message, CancellationToken cancel);
}
