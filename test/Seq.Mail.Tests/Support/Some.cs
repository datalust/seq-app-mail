using System;
using System.IO;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Xunit.Sdk;

namespace Seq.Mail.Tests.Support;

static class Some
{
    public static string InformationEvent(string messageTemplate = "Hello, world!", params object?[] propertyValues)
    {
        return LogEvent(LogEventLevel.Information, messageTemplate, propertyValues);
    }
    
    static string LogEvent(LogEventLevel level, string messageTemplate = "Hello, world!", params object?[] propertyValues)
    {
        var log = new LoggerConfiguration().CreateLogger();
#pragma warning disable Serilog004 // Constant MessageTemplate verifier
        if (!log.BindMessageTemplate(messageTemplate, propertyValues, out var template, out var properties))
#pragma warning restore Serilog004 // Constant MessageTemplate verifier
        {
            throw new XunitException("Template could not be bound.");
        }

        var sw = new StringWriter();
        new CompactJsonFormatter().Format(
            new LogEvent(DateTimeOffset.Now, level, null, template, properties),
            sw);
        return sw.ToString();
    }
}