using System.Threading;
using System.Threading.Tasks;
using MimeKit;

namespace Seq.App.Mail.Smtp;

interface ISmtpMailGateway
{
    Task SendAsync(SmtpOptions options, MimeMessage message, CancellationToken cancel);
}