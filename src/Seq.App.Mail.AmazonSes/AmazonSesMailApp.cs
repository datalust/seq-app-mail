using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmail.Model;
using MimeKit;
using Seq.Apps;
using Seq.Mail;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Seq.App.Mail.AmazonSes;

[SeqApp("Amazon Simple Email Service (SES) Mail",
    Description = "Send events and notifications by email, using the Amazon Simple Email Service (SES) API.")]
public class AmazonSesMailApp: MailApp
{
    readonly IAmazonSesMailGateway _mailGateway;
    AmazonSesOptions? _options;

    internal AmazonSesMailApp(IAmazonSesMailGateway mailGateway)
    {
        _mailGateway = mailGateway;
    }
    
    public AmazonSesMailApp()
        : this (new AmazonSimpleEmailServiceClientMailGateway())
    {
    }

    [SeqAppSetting(
        DisplayName = "Access key id",
        HelpText = "An AWS access key id.")]
    public string? AccessKeyId { get; set; }
        
    [SeqAppSetting(
        DisplayName = "Secret key",
        HelpText = "An AWS secret key.",
        InputType = SettingInputType.Password)]
    public string? SecretKey { get; set; }

    protected override void OnAttached()
    {
        base.OnAttached();

        _options = new AmazonSesOptions(
            NormalizeOption(AccessKeyId) ?? throw new InvalidOperationException("An access key id is required."),
            NormalizeOption(SecretKey) ?? throw new InvalidOperationException("A secret key is required."));
    }

    protected override async Task SendAsync(MimeMessage message, CancellationToken cancel)
    {
        const string charset = "UTF-8";
        var subject = new Content { Data = message.Subject, Charset = charset };

        var body = new Body
        {
            Html = message.HtmlBody != null ? new Content { Data = message.HtmlBody, Charset = charset } : null,
            Text = message.TextBody != null ? new Content { Data = message.TextBody, Charset = charset } : null
        };

        var sesMessage = new Message
        {
            Subject = subject,
            Body = body
        };

        var destination = new Destination { ToAddresses = message.To.Select(addr => addr.ToString()).ToList() };

        var request = new SendEmailRequest
        {
            Source = message.From.Single().ToString(),
            Destination = destination,
            Message = sesMessage
        };

        await _mailGateway.SendAsync(_options!, request, cancel);
    }
}
