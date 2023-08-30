using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Seq.App.Mail.Smtp;

class MailkitMailGateway : ISmtpMailGateway
{
    public async Task SendAsync(SmtpOptions options, MimeMessage message, CancellationToken cancel)
    {
        using var client = new SmtpClient();

        if (options.DisableCertificateValidation)
        {
            client.CheckCertificateRevocation = false;
            client.ServerCertificateValidationCallback = delegate { return true; };
        }
        
        await client.ConnectAsync(options.Host, options.Port, options.SocketOptions, cancel);
        if (options.RequiresAuthentication)
            await client.AuthenticateAsync(options.Username, options.Password, cancel);
        await client.SendAsync(message, cancel);
        await client.DisconnectAsync(true, cancel);
    }
}
