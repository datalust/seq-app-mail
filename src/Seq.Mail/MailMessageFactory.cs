using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MimeKit;
using MimeKit.Text;
using Seq.Mail.Encoding;
using Seq.Mail.Expressions;
using Seq.Mail.Functions;
using Seq.Mail.Templates;
using Seq.Mail.Templates.Encoding;
using Serilog.Events;
using Serilog.Formatting;

namespace Seq.Mail
{
    class MailMessageFactory
    {
        const int MaxSubjectLength = 130;

        readonly MailboxAddress _from;
        readonly bool _bodyIsPlainText;
        readonly ExpressionTemplate _subject;
        readonly ExpressionTemplate _body;
        readonly ExpressionTemplate[] _to;

        public MailMessageFactory(
            MailboxAddress from,
            IEnumerable<string> toTemplates,
            string subjectTemplate,
            string bodyTemplate,
            bool bodyIsPlainText)
        {
            _from = from;
            _to = toTemplates.Select(to => CompileTemplate(to)).ToArray();
            _bodyIsPlainText = bodyIsPlainText;
            _subject = CompileTemplate(subjectTemplate);
            _body = CompileTemplate(bodyTemplate, encoder: bodyIsPlainText? new TemplateOutputHtmlEncoder() : null);
        }

        static ExpressionTemplate CompileTemplate(string template, TemplateOutputEncoder? encoder = null)
        {
            return new ExpressionTemplate(template, nameResolver: new StaticMemberNameResolver(typeof(MailAppBuiltInFunctions)), encoder: encoder);
        }

        public MimeMessage FromEvent(LogEvent evt)
        {
            var subject = Format(_subject, evt)
                .Trim()
                .Replace("\r", "", StringComparison.Ordinal)
                .Replace("\n", "", StringComparison.Ordinal);

            if (subject.Length > MaxSubjectLength)
                subject = subject[..MaxSubjectLength];

            var message = new MimeMessage();
            message.From.Add(_from);

            foreach (var to in _to)
            {
                var formatted = Format(to, evt);
                var addr = MailboxAddress.Parse(formatted);
                message.To.Add(addr);
            }
            
            message.Subject = subject;
            message.Body = new TextPart(_bodyIsPlainText ? TextFormat.Plain : TextFormat.Html)
            {
                Text = Format(_body, evt)
            };

            return message;
        }

        static string Format(ITextFormatter template, LogEvent evt)
        {
            var writer = new StringWriter();
            template.Format(evt, writer);
            return writer.ToString();
        }
    }
}
