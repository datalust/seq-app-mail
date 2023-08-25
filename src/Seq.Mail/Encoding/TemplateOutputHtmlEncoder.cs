using Seq.Syntax.Templates.Encoding;

namespace Seq.Mail.Encoding;

class TemplateOutputHtmlEncoder: TemplateOutputEncoder
{
    /// <summary>
    /// Replaces <c>&</c>, <c>&lt;</c>, <c>&gt;</c>, <c>&quot;</c>, and
    /// <c>&apos;</c> with their equivalent escape sequences. This renders the result safe for
    /// insertion into HTML attributes and element bodies apart from <c>script</c> and <c>style</c>.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <returns>The encoded string.</returns>
    public override string Encode(string value)
    {
        return System.Text.Encodings.Web.HtmlEncoder.Default.Encode(value);
    }
}