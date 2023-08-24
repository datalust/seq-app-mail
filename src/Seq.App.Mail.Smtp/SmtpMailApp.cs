using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Seq.Apps;
using Seq.Mail;

// ReSharper disable UnusedAutoPropertyAccessor.Global, UnusedType.Global

namespace Seq.App.Mail.Smtp;

[SeqApp("SMTP Mail",
    Description = "Send events and notifications by email, using SMTP with username/password authentication.")]
public class SmtpMailApp: MailApp
{
    SmtpOptions? _options;
        
    [SeqAppSetting(
        HelpText = "The DNS name of the SMTP server.")]
    public new string? Host { get; set; }

    [SeqAppSetting(
        IsOptional = true,
        HelpText = "The port on the SMTP server to send mail to. Leave this blank to use the standard SMTP " +
                   "port (25).")]
    public int? Port { get; set; }
        
    [SeqAppSetting(
        DisplayName = "Connection security",
        IsOptional = true,
        HelpText = "Protocol security requirement. The default is `RequireTls`, which will select the default " +
                   "security mode for the given port.")]
    public ProtocolSecurity ProtocolSecurity { get; set; }

    [SeqAppSetting(
        IsOptional = true,
        HelpText = "The username to use when authenticating to the SMTP server, if required.")]
    public string? Username { get; set; }

    [SeqAppSetting(
        IsOptional = true,
        InputType = SettingInputType.Password,
        HelpText = "The password to use when authenticating to the SMTP server, if required.")]
    public string? Password { get; set; }

    protected override void OnAttached()
    {
        if (string.IsNullOrEmpty(Host))
            throw new ArgumentException("The `Host` setting is required.");
            
        base.OnAttached();

        var port = Port ?? 25;

        var socketOptions = ProtocolSecurity switch
        {
            ProtocolSecurity.RequireTls => port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls,
            ProtocolSecurity.RequireStartTls => SecureSocketOptions.StartTls,
            ProtocolSecurity.RequireImplicitTls => SecureSocketOptions.SslOnConnect,
            ProtocolSecurity.None => SecureSocketOptions.None,
            _ => throw new ArgumentOutOfRangeException()
        };

        _options = new SmtpOptions(
            Host,
            port,
            socketOptions,
            Username,
            Password);
    }

    protected override async Task SendAsync(MimeMessage message, CancellationToken cancel)
    {
        using var client = new SmtpClient();             
        await client.ConnectAsync(_options!.Host, _options.Port, _options.SocketOptions, cancel);
        if (_options.RequiresAuthentication)
            await client.AuthenticateAsync(_options.Username, _options.Password, cancel);
        await client.SendAsync(message, cancel);
        await client.DisconnectAsync(true, cancel);
    }
}