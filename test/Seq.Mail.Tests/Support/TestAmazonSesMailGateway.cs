using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmail.Model;
using Seq.App.Mail.AmazonSes;

namespace Seq.Mail.Tests.Support;

class TestAmazonSesMailGateway : IAmazonSesMailGateway
{
    public List<(AmazonSesOptions, SendEmailRequest)> Received { get; } = [];
    
    public Task SendAsync(AmazonSesOptions options, SendEmailRequest request, CancellationToken cancel)
    {
        Received.Add((options, request));
        return Task.CompletedTask;
    }
}
