using System.IO;
using System.Linq;
using Seq.Syntax.Templates;
using Seq.Syntax.Tests.Support;
using Serilog.Formatting.Compact.Reader;
using Xunit;

namespace Seq.Syntax.Tests.Expressions;

public class AppHostCompatibilityTests
{
    [Theory]
    [InlineData("TotalMilliseconds(@Elapsed)", "1037.495")]
    [InlineData("Round(TotalMilliseconds(@Elapsed), 0)", "1037")]
    [InlineData("IsSpan()", "true")]
    [InlineData("IsRootSpan()", "true")]
    [InlineData("@SpanKind", "Internal")]
    [InlineData("@Start", "2025-09-30T00:56:41.4493934Z")]
    [InlineData("FromUnixEpoch(@Start)", "20361.00:56:41.4493934")]
    [InlineData("TotalMilliseconds(FromUnixEpoch(@Start))", "1759193801449.3934")]
    [InlineData("FromUnixEpoch(@Timestamp)", "20361.00:56:42.4868884")]
    [InlineData("@EventType", "1615322914")]
    public void AppHostJsonProcessingIsReasonable(string expression, string expected)
    {
        var json = TestCases.ReadNDJsonCases("app-host-compatibility-case.json").Single();
        var evt = LogEventReader.ReadFromString(json);
        var template = $"{{ {expression} }}";
        var output = new StringWriter();
        new ExpressionTemplate(template).Format(evt, output);
        Assert.Equal(expected, output.ToString());
    }
}
