using Seq.Mail.Templates.Encoding;

namespace Seq.Mail.Tests.Support;

public class ParenthesizingEncoder : TemplateOutputEncoder
{
    public override string Encode(string value)
    {
        return $"({value})";
    }
}