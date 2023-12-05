using System.Diagnostics.CodeAnalysis;
using Seq.Syntax.Expressions;

namespace Seq.Syntax.BuiltIns;

class SeqBuiltInPropertyNameResolver: NameResolver
{
    public override bool TryResolveBuiltInPropertyName(string alias, [NotNullWhen(true)] out string? target)
    {
        target = alias switch
        {
            "Properties" => "{..@p, @seqid: undefined(), @i: undefined(), @tr: undefined(), @sp: undefined(), @ra: undefined()}",
            "Timestamp" => "@t",
            "Level" => "@l",
            "Message" => "@m",
            "MessageTemplate" => "@mt",
            "EventType" => "@i",
            "Exception" => "@x",
            "Id" => "@p['@seqid']",
            "TraceId" => "@tr",
            "SpanId" => "@sp",
            "Resource" or "@ra" => "@p['@ra']",
            "ParentId" or "@ps" => "@p['@ps']",
            "Arrived" or "Document" or "Data" => "undefined()",
            _ => null
        };

        return target != null;
    }
}
