using System;
using System.Threading;
using System.Threading.Tasks;
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
    readonly ISmtpMailGateway _mailGateway;
    SmtpOptions? _options;

    internal SmtpMailApp(ISmtpMailGateway mailGateway)
    {
        _mailGateway = mailGateway;
    }
    
    public SmtpMailApp()
        : this (new MailkitMailGateway())
    {
    }
        
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
        base.OnAttached();

        var port = Port ?? 25;

        var socketOptions = ProtocolSecurity switch
        {
            ProtocolSecurity.RequireTls => port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls,
            ProtocolSecurity.RequireStartTls => SecureSocketOptions.StartTls,
            ProtocolSecurity.RequireImplicitTls => SecureSocketOptions.SslOnConnect,
            ProtocolSecurity.None => SecureSocketOptions.Auto,
            _ => throw new ArgumentOutOfRangeException()
        };

        var disableCertificateValidation = ProtocolSecurity == ProtocolSecurity.None;

        _options = new SmtpOptions(
            NormalizeOption(Host) ?? throw new ArgumentException("The `Host` setting is required."),
            port,
            socketOptions,
            Username,
            Password,
            disableCertificateValidation);
    }

    protected override async Task SendAsync(MimeMessage message, CancellationToken cancel)
    {
        await _mailGateway.SendAsync(_options!, message, cancel);
    }
}