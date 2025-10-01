using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Seq.Syntax.Expressions;
using Serilog.Events;

namespace Seq.Syntax.BuiltIns;

class SeqBuiltInPropertyNameResolver: NameResolver
{
    public override bool TryResolveBuiltInPropertyName(string alias, [NotNullWhen(true)] out string? target)
    {
        target = alias switch
        {
            "Properties" => "{..@p, @seqid: undefined(), @i: undefined(), @tr: undefined(), @sp: undefined(), @ra: undefined(), @st: undefined(), @ps: undefined(), @sa: undefined()}",
            "Timestamp" => "@t",
            "Level" => "@l",
            "Message" => "@m",
            "MessageTemplate" => "@mt",
            "EventType" => "@i",
            "Exception" => "@x",
            "Id" => "@p['@seqid']",
            "TraceId" or "tr" => "@p['@tr']",
            "SpanId" or "sp" => "@p['@sp']",
            "Resource" or "ra" => "@p['@ra']",
            "Start" or "st" => "_AsDateTimeOffset(@p['@st'])",
            "ParentId" or "ps" => "@p['@ps']",
            "SpanKind" or "sk" => "@p['@sk']",
            "Scope" or "sa" => "@p['@sa']",
            "Elapsed" => "_Elapsed(@st, @t)",
            "Arrived" or "Document" or "Data" => "undefined()",
            _ => null
        };

        return target != null;
    }

    public override bool TryResolveFunctionName(string name, [NotNullWhen(true)] out MethodInfo? implementation)
    {
        if (name == nameof(_Elapsed) || name == nameof(_AsDateTimeOffset))
        {
            implementation = typeof(SeqBuiltInPropertyNameResolver).GetMethod(name)!;
            return true;
        }

        implementation = null;
        return false;
    }
    
    public static LogEventPropertyValue? _Elapsed(LogEventPropertyValue? from, LogEventPropertyValue? to)
    {
        if (AsDateTimeOffset(from) is {} f && AsDateTimeOffset(to) is {} t)
            return new ScalarValue(t - f);

        return null;
    }

    public static LogEventPropertyValue? _AsDateTimeOffset(LogEventPropertyValue? value)
    {
        if (AsDateTimeOffset(value) is { } dto)
            return new ScalarValue(dto);

        return null;
    }

    static DateTimeOffset? AsDateTimeOffset(LogEventPropertyValue? value)
    {
        if (value is ScalarValue { Value: DateTime dt })
            return dt;
        
        if (value is ScalarValue { Value: DateTimeOffset dto })
            return dto;

        if (value is ScalarValue { Value: string s } &&
            DateTimeOffset.TryParseExact(s, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
            return parsed;

        return null;
    }
}
