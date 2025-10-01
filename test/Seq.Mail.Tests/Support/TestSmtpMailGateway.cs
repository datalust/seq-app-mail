using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;
using Seq.App.Mail.Smtp;

namespace Seq.Mail.Tests.Support;

class TestSmtpMailGateway : ISmtpMailGateway
{
    public List<(SmtpOptions, MimeMessage)> Received { get; } = [];
    
    public Task SendAsync(SmtpOptions options, MimeMessage message, CancellationToken cancel)
    {
        // The app will dispose the message after sending, so we have to clone it here.
        var s = new MemoryStream();
        message.WriteTo(s, cancel);
        s.Position = 0;
        Received.Add((options, MimeMessage.Load(s, cancel)));
        return Task.CompletedTask;
    }
}