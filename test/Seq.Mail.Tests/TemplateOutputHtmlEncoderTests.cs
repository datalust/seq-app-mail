using Seq.Mail.Encoding;
using Xunit;

namespace Seq.Mail.Tests;

public class TemplateOutputHtmlEncoderTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("test", "test")]
    [InlineData("&test\"<data>'", "&amp;test&quot;&lt;data&gt;&#x27;")]
    public void OutputIsHtmlEncoded(string raw, string encoded)
    {
        var encoder = new TemplateOutputHtmlEncoder();
        var actual = encoder.Encode(raw);
        Assert.Equal(encoded, actual);
    }
}