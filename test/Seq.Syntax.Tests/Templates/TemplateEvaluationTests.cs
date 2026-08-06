using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Seq.Syntax.Templates;
using Seq.Syntax.Tests.Support;
using Xunit;

namespace Seq.Syntax.Tests.Templates;

public class TemplateEvaluationTests
{
    public static IEnumerable<object[]> TemplateEvaluationCases =>
        TestCases.ReadAsvCases("template-evaluation-cases.asv");

    [Theory]
    [MemberData(nameof(TemplateEvaluationCases))]
    public void TemplatesAreCorrectlyEvaluated(string template, string expected)
    {
        var evt = Some.InformationEvent("Hello, {Name}!", "nblumhardt");
        var frFr = CultureInfo.GetCultureInfoByIetfLanguageTag("fr-FR");
        var compiled = new ExpressionTemplate(template, culture: frFr);
        var output = new StringWriter();
        compiled.Format(evt, output);
        var actual = output.ToString();
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TraceIdsAreEvaluated()
    {
        var traceId = ActivityTraceId.CreateRandom();
        var spanId = ActivitySpanId.CreateRandom();
        var evt = Some.LogEvent(traceId: traceId, spanId: spanId);

        var compiled = new ExpressionTemplate("{@tr}/{@TraceId}/{@sp}/{@SpanId}");
        var output = new StringWriter();
        compiled.Format(evt, output);
        var actual = output.ToString();
        
        Assert.Equal($"{traceId}/{traceId}/{spanId}/{spanId}", actual);
    }
    
    [Fact]
    public void TraceIdsAreMissingWhenDefault()
    {
        var evt = Some.LogEvent(traceId: default, spanId: default);

        var compiled = new ExpressionTemplate("{@tr}/{@TraceId}/{@sp}/{@SpanId}");
        var output = new StringWriter();
        compiled.Format(evt, output);
        var actual = output.ToString();
        
        Assert.Equal("///", actual);
    }
}