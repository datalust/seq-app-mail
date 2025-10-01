using System;
using System.Threading.Tasks;
using Seq.App.Mail.AmazonSes;
using Seq.Apps;
using Seq.Apps.Testing.Hosting;
using Seq.Mail.Tests.Support;
using Serilog.Events;
using Xunit;

namespace Seq.Mail.Tests;

public class AmazonSesMailAppTests
{
    [Fact]
    public async Task EventsAreSentAsMessages()
    {
        var gateway = new TestAmazonSesMailGateway();
            
        var app = new AmazonSesMailApp(gateway)
        {
            AccessKeyId = "aki",
            SecretKey = "sk",
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

        var (options, request) = Assert.Single(gateway.Received);
        Assert.Equal("aki", options.AccessKeyId);
        Assert.Equal("sk", options.SecretKey);
        Assert.Equal("f@localhost", request.Source);
        Assert.Equal(new[] { "t@localhost", "r@localhost" }, request.Destination.ToAddresses);
        Assert.Equal("s", request.Message.Subject.Data);
        Assert.Equal("b", request.Message.Body.Html.Data.Trim());
        Assert.Null(request.Message.Body.Text);
    }
}
