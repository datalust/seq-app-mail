using System;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;
using Seq.Apps;
using Seq.Mail;

namespace Seq.App.Mail.Microsoft365
{
    [SeqApp("Microsoft 365 Mail",
        Description = "Send events and notifications by email, using Microsoft 365.")]
    public class Microsoft365MailApp: MailApp
    {
        Microsoft365Options? _options;

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override async Task SendAsync(MimeMessage message, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }
    }
}
