using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MimeKit;
using MimeKit.Text;
using Seq.Apps;
using Seq.Mail.BuiltIns;
using Seq.Mail.Encoding;
using Seq.Syntax.Expressions;
using Seq.Syntax.Expressions.Compilation;
using Seq.Syntax.Templates;
using Seq.Syntax.Templates.Encoding;
using Serilog.Events;
using Serilog.Formatting;

namespace Seq.Mail;

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
        bool bodyIsPlainText,
        string timeZoneName,
        string dateFormat,
        App app,
        Host host)
    {
        var mailAppNameResolver = new MailAppNameResolver(timeZoneName, dateFormat, app, host);
            
        _from = from;
        _to = toTemplates.Select(to => CompileTemplate(to, mailAppNameResolver)).ToArray();
        _bodyIsPlainText = bodyIsPlainText;
        _subject = CompileTemplate(subjectTemplate, mailAppNameResolver);
        _body = CompileTemplate(bodyTemplate, mailAppNameResolver, encoder: bodyIsPlainText ? null : new TemplateOutputHtmlEncoder());
    }

    static ExpressionTemplate CompileTemplate(string template, NameResolver builtInNameResolver, TemplateOutputEncoder? encoder = null)
    {
        return new ExpressionTemplate(
            template,
            nameResolver: new OrderedNameResolver(new[]
            {
                new StaticMemberNameResolver(typeof(MailAppBuiltInFunctions)),
                builtInNameResolver                     
            }),
            encoder: encoder);
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