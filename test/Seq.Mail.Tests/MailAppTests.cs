using Seq.Mail.BuiltIns;
using Seq.Mail.Expressions;
using Seq.Mail.Templates;
using Xunit;

namespace Seq.Mail.Tests;

public class MailAppTests
{
    [Fact]
    public void DefaultBodyTemplateCanBeParsed()
    {
        var template = MailApp.LoadDefaultBodyTemplate();
        Assert.True(
            ExpressionTemplate.TryParse(template, null, new StaticMemberNameResolver(typeof(MailAppBuiltInFunctions)), null, out _, out var error),
            error);
    }
}