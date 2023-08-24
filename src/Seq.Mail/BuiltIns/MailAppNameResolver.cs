using System.Diagnostics.CodeAnalysis;
using Seq.Apps;
using Seq.Mail.Expressions;
using Seq.Mail.Expressions.Ast;
using Serilog.Events;

namespace Seq.Mail.BuiltIns
{
    class MailAppNameResolver: NameResolver
    {
        readonly LogEventPropertyValue _app, _host, _settings;
        
        public MailAppNameResolver(string timeZoneName, string dateFormat, App app, Host host)
        {
            _settings = new StructureValue(
                new[]
                {
                    new LogEventProperty("TimeZoneName", new ScalarValue(timeZoneName)),
                    new LogEventProperty("DateFormat", new ScalarValue(dateFormat)),
                });

            _app = new StructureValue(
                new[]
                {
                    new LogEventProperty("Id", new ScalarValue(app.Id)),
                    new LogEventProperty("Title", new ScalarValue(app.Title))
                });

            _host = new StructureValue(
                new[]
                {
                    new LogEventProperty("BaseUri", new ScalarValue(host.BaseUri)),
                    new LogEventProperty("InstanceName", new ScalarValue(host.BaseUri)),
                });
        }

        internal override bool TryResolveBuiltInPropertyName(string alias, [NotNullWhen(true)] out Expression? target)
        {
            target = alias switch
            {
                "App" => new ConstantExpression(_app),
                "Host" => new ConstantExpression(_host),
                "Settings" => new ConstantExpression(_settings),
                _ => null
            };

            return target != null;
        }
    }
}