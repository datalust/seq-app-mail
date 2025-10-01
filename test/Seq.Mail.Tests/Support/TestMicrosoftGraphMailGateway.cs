using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Seq.App.Mail.Microsoft365;

namespace Seq.Mail.Tests.Support;

class TestMicrosoftGraphMailGateway : IMicrosoftGraphMailGateway
{
    public List<(Microsoft365Options, Message)> Received { get; } = [];
    
    public Task SendAsync(Microsoft365Options options, Message message, CancellationToken cancel)
    {
        Received.Add((options, message));
        return Task.CompletedTask;
    }
}
