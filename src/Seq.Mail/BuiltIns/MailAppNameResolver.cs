using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Seq.Apps;
using Seq.Syntax.Expressions;

namespace Seq.Mail.BuiltIns;

class MailAppNameResolver: NameResolver
{
    readonly App _app;
    readonly Host _host;
        
    public MailAppNameResolver(string timeZoneName, string dateFormat, App app, Host host)
    {
        _host = host;

        // It turned out that passing all of the "real" settings here was a questionable idea,
        // because they're not consistently cased (e.g. at runtime they're uppercase, but this
        // isn't guaranteed).
        var settings = new Dictionary<string, string>
        {
            [nameof(MailApp.TimeZoneName)] = timeZoneName,
            [nameof(MailApp.DateTimeFormat)] = dateFormat
        };
            
        _app = new App(app.Id, app.Title, settings, app.StoragePath);
    }

    public override bool TryResolveBuiltInPropertyName(string alias, [NotNullWhen(true)] out string? target)
    {
        target = alias switch
        {
            "App" => "MailAppInstance()",
            "Host" => "MailAppHost()",
            _ => null
        };

        return target != null;
    }

    public override bool TryBindFunctionParameter(ParameterInfo parameter, [NotNullWhen(true)] out object? boundValue)
    {
        if (parameter.ParameterType == typeof(Host))
        {
            boundValue = _host;
            return true;
        }
        
        if (parameter.ParameterType == typeof(App))
        {
            boundValue = _app;
            return true;
        }
        
        return base.TryBindFunctionParameter(parameter, out boundValue);
    }
}