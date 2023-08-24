using System.Diagnostics.CodeAnalysis;
using Seq.Mail.Expressions;
using Seq.Mail.Expressions.Ast;

namespace Seq.Mail.BuiltIns;

class SeqBuiltInPropertyNameResolver: NameResolver
{
    internal override bool TryResolveBuiltInPropertyName(string alias, [NotNullWhen(true)] out Expression? target)
    {
        target = alias switch
        {
            "Id" => new AccessorExpression(new AmbientNameExpression(BuiltInProperty.Properties, true), "@seqid"),
            "TraceId" => new AccessorExpression(new AmbientNameExpression(BuiltInProperty.Properties, true), "@tr"),
            "SpanId" => new AccessorExpression(new AmbientNameExpression(BuiltInProperty.Properties, true), "@sp"),
            "Resource" => new AccessorExpression(new AmbientNameExpression(BuiltInProperty.Properties, true), "@ra"),
            "Arrived" or "Document" or "Data" => new CallExpression(false, Operators.OpUndefined),
            _ => null
        };

        return target != null;
    }
    
    public override bool TryResolveBuiltInPropertyName(string alias, [NotNullWhen(true)] out string? target)
    {
        target = alias switch
        {
            "Properties" => BuiltInProperty.Properties,
            "Timestamp" => BuiltInProperty.Timestamp,
            "Level" => BuiltInProperty.Level,
            "Message" => BuiltInProperty.Message,
            "MessageTemplate" => BuiltInProperty.MessageTemplate,
            "EventType" => BuiltInProperty.EventId,
            "Exception" => BuiltInProperty.Exception,
            _ => null
        };

        return target != null;
    }
}
