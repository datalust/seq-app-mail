using System.Diagnostics.CodeAnalysis;
using Seq.Syntax.Expressions;

namespace Seq.Syntax.BuiltIns;

class SeqBuiltInPropertyNameResolver: NameResolver
{
    public override bool TryResolveBuiltInPropertyName(string alias, [NotNullWhen(true)] out string? target)
    {
        target = alias switch
        {
            "Properties" => "@p",
            "Timestamp" => "@t",
            "Level" => "@l",
            "Message" => "@m",
            "MessageTemplate" => "@mt",
            "EventType" => "@i",
            "Exception" => "@x",
            "Id" => "@p['@seqid']",
            "TraceId" or "@tr" => "@p['@tr']",
            "SpanId" or "@sp" => "@p['@sp']",
            "Resource" or "@ra" => "@p['@ra']",
            "Arrived" or "Document" or "Data" => "undefined()",
            _ => null
        };

        return target != null;
    }
}
