using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Security;
using Seq.App.Mail.Smtp;
using Seq.Apps;
using Seq.Apps.Testing.Hosting;
using Seq.Mail.Tests.Support;
using Serilog.Events;
using Xunit;

namespace Seq.Mail.Tests;

public class SmtpMailAppTests
{
    [Fact]
    public async Task EventsAreSentAsMessages()
    {
        var gateway = new TestSmtpMailGateway();
            
        var app = new SmtpMailApp(gateway)
        {
            Host = "h",
            Port = 123,
            ProtocolSecurity = ProtocolSecurity.RequireImplicitTls,
            Username = "u",
            Password = "p",
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
        Assert.Equal("h", options.Host);
        Assert.Equal(123, options.Port);
        Assert.Equal(SecureSocketOptions.SslOnConnect, options.SocketOptions);
        Assert.Equal("u", options.Username);
        Assert.Equal("p", options.Password);
        Assert.Equal("f@localhost", message.From.ToString());
        Assert.Equal(new[] { "t@localhost", "r@localhost" }, message.To.Select(t => t.ToString()));
        Assert.Equal("s", message.Subject);
        Assert.Equal("b", message.HtmlBody.Trim());
        Assert.Null(message.TextBody);
    }
}