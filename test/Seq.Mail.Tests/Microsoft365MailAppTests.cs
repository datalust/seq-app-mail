using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Seq.App.Mail.Microsoft365;
using Seq.Apps;
using Seq.Apps.Testing.Hosting;
using Seq.Mail.Tests.Support;
using Serilog.Events;
using Xunit;

namespace Seq.Mail.Tests;

public class Microsoft365MailAppTests
{
    [Fact]
    public async Task EventsAreSentAsMessages()
    {
        var gateway = new TestMicrosoftGraphMailGateway();
            
        var app = new Microsoft365MailApp(gateway)
        {
            TenantId = "t",
            ClientId = "c",
            ClientSecret = "s",
            SaveToSentItems = true,
            From = "f@localhost",
            To = "t@localhost,r@localhost",
            Subject = "s",
            Body = "b",
            BodyIsPlainText = false,
            TimeZoneName = "Australia/Brisbane",
            DateTimeFormat = "R"
        };

        app.Attach(new TestAppHost());

        var evt = Some.InformationEvent();

        await app.OnAsync(new Event<LogEvent>("event-1", 123, DateTime.UtcNow, evt));

        var (options, message) = Assert.Single(gateway.Received);
        Assert.Equal("t", options.TenantId);
        Assert.Equal("c", options.ClientId);
        Assert.Equal("s", options.ClientSecret);
        Assert.True(options.SaveToSentItems);
        Assert.Equal("f@localhost", message.From!.EmailAddress!.Address);
        Assert.Equal(new[] { "t@localhost", "r@localhost" }, message.ToRecipients!.Select(t => t.EmailAddress!.Address));
        Assert.Equal("s", message.Subject);
        Assert.Equal("b", message.Body!.Content!.Trim());
        Assert.Equal(BodyType.Html, message.Body!.ContentType);
    }
}
