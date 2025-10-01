using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Seq.App.Mail.AmazonSes;

class AmazonSimpleEmailServiceClientMailGateway : IAmazonSesMailGateway
{
    public async Task SendAsync(AmazonSesOptions options, SendEmailRequest request, CancellationToken cancel)
    {
        using var client = new AmazonSimpleEmailServiceClient(options.AccessKeyId, options.SecretKey);
        await client.SendEmailAsync(request, cancel);
    }
}
