using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using MimeKit;
using Seq.Apps;
using Seq.Mail;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Seq.App.Mail.Microsoft365;

[SeqApp("Microsoft 365 Mail",
    Description = "Send events and notifications by email, using Microsoft 365.")]
public class Microsoft365MailApp: MailApp
{
    Microsoft365Options? _options;
        
    [SeqAppSetting(
        DisplayName = "Tenant id",
        HelpText = "The directory (tenant) id that will be used to send mail.")]
    public string? TenantId { get; set; }
        
    [SeqAppSetting(
        DisplayName = "Client id",
        HelpText = "The application (client) id of an app registration with the `Mail.Send` permission for the Microsoft Graph API.")]
    public string? ClientId { get; set; }

    [SeqAppSetting(
        DisplayName = "Client secret",
        InputType = SettingInputType.Password,
        HelpText = "A client secret for the app registration.")]
    public string? ClientSecret { get; set; }
        
    [SeqAppSetting(
        IsOptional = true,
        DisplayName = "Save to sent items",
        HelpText = "If set, outgoing email will be saved to the `From` user's sent items.")]
    public bool SaveToSentItems { get; set; }
        
    protected override void OnAttached()
    {
        base.OnAttached();

        _options = new Microsoft365Options(
            TenantId ?? throw new InvalidOperationException("A tenant id is required."),
            ClientId ?? throw new InvalidOperationException("A client id is required."),
            ClientSecret ?? throw new InvalidOperationException("A client secret is required."),
            SaveToSentItems);
    }

    protected override async Task SendAsync(MimeMessage message, CancellationToken cancel)
    {
        // This implementation follows the article at:
        //   https://medium.com/medialesson/how-to-send-emails-in-net-with-the-microsoft-graph-a97b57430bbd
            
        var credential = new ClientSecretCredential(_options!.TenantId, _options.ClientId, _options.ClientSecret);
        using var graphClient = new GraphServiceClient(credential);

        var graphMessage = new Message
        {
            Subject = message.Subject,
            Body = new ItemBody
            {
                ContentType = message.HtmlBody != null ? BodyType.Html : BodyType.Text,
                Content = message.HtmlBody ?? message.TextBody
            },
            From = new Recipient
            {
                EmailAddress = AsGraphEmailAddress(message.From.Single()) 
            },
            ToRecipients = message.To.Cast<MailboxAddress>().Select(to =>
                new Recipient
                {
                    EmailAddress = AsGraphEmailAddress(to)
                }).ToList()
        };

        await graphClient.Users[From]
            .SendMail
            .PostAsync(new() { Message = graphMessage, SaveToSentItems = _options.SaveToSentItems }, cancellationToken: cancel);
    }

    static EmailAddress AsGraphEmailAddress(InternetAddress address)
    {
        var mailboxAddress = (MailboxAddress)address;
        return new EmailAddress
        {
            Name = mailboxAddress.Name,
            Address = mailboxAddress.Address
        };
    }
}