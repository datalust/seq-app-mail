using Seq.Mail.Templates;
using Xunit;

namespace Seq.Mail.Tests.Templates;

public class ExpressionTemplateTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("{", "{{")]
    [InlineData("}", "}}")]
    [InlineData("a{b}c{{d}}", "a{{b}}c{{{{d}}}}")]
    public void LiteralTextCanBeEscaped(string literal, string escaped)
    {
        var actual = ExpressionTemplate.EscapeLiteralText(literal);
        Assert.Equal(escaped, actual);
    }
}