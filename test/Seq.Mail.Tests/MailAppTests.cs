using Seq.Mail.BuiltIns;
using Seq.Syntax.Expressions;
using Seq.Syntax.Templates;
using Xunit;

namespace Seq.Mail.Tests;

public class MailAppTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void DefaultBodyTemplateCanBeParsed(bool bodyIsPlainText)
    {
        var template = MailApp.LoadDefaultBodyTemplate(bodyIsPlainText);
        Assert.True(
            ExpressionTemplate.TryParse(template, null, new StaticMemberNameResolver(typeof(MailAppBuiltInFunctions)), null, out _, out var error),
            error);
    }
}
