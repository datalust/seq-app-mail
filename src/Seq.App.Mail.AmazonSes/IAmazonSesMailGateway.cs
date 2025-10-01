using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmail.Model;

namespace Seq.App.Mail.AmazonSes;

interface IAmazonSesMailGateway
{
    Task SendAsync(AmazonSesOptions options, SendEmailRequest request, CancellationToken cancel);
}
