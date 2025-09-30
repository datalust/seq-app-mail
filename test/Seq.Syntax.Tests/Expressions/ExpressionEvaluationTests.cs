using System;
using System.Collections.Generic;
using System.Globalization;
using Seq.Syntax.Expressions;
using Seq.Syntax.Expressions.Runtime;
using Seq.Syntax.Tests.Support;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

namespace Seq.Syntax.Tests.Expressions;

public class ExpressionEvaluationTests
{
    public static IEnumerable<object[]> ExpressionEvaluationCases =>
        TestCases.ReadAsvCases("expression-evaluation-cases.asv");

    [Theory]
    [MemberData(nameof(ExpressionEvaluationCases))]
    public void ExpressionsAreCorrectlyEvaluated(string expr, string result)
    {
        var evt = Some.InformationEvent();

        evt.AddPropertyIfAbsent(
            new LogEventProperty("User", new StructureValue([
                new LogEventProperty("Id", new ScalarValue(42)),
                new LogEventProperty("Name", new ScalarValue("nblumhardt"))
            ])));
        
        evt.AddPropertyIfAbsent(new LogEventProperty("@st", new ScalarValue((evt.Timestamp - TimeSpan.FromMinutes(10)).ToString("o"))));

        var frFr = CultureInfo.GetCultureInfoByIetfLanguageTag("fr-FR");
        var actual = SerilogExpression.Compile(expr, formatProvider: frFr)(evt);
        var expected = SerilogExpression.Compile(result)(evt);

        if (expected is null)
        {
            Assert.True(actual is null, $"Expected value: undefined{Environment.NewLine}Actual value: {Display(actual)}");
        }
        else
        {
            Assert.True(
                Coerce.IsTrue(RuntimeOperators._Internal_Equal(StringComparison.OrdinalIgnoreCase, actual, expected)),
                $"Expected value: {Display(expected)}{Environment.NewLine}Actual value: {Display(actual)}");
        }
    }

    static string Display(LogEventPropertyValue? value)
    {
        if (value == null)
            return "undefined";

        return value.ToString();
    }

    [Fact]
    public void MessageRenderingSupportsNestedProperties()
    {
        // From the point of view of Seq and Seq Syntax, dotted identifiers in property names are paths into
        // nested objects. This differs from Serilog's interpretation, which is that they are flat names with
        // embedded dots. When Seq and Serilog are used together, Serilog.Sinks.Seq performs the conversion
        // from flat names to nested objects, so on the server, apps etc. need message rendering to work with
        // the nested data representation.
        
        var messageTemplate = new MessageTemplateParser().Parse("HTTP {request.method} {request.path}");
        var properties = new[]
        {
            new LogEventProperty("request", new StructureValue([
                new LogEventProperty("method", new ScalarValue("GET")),
                new LogEventProperty("path", new ScalarValue("/example"))
            ]))
        };
        
        var evt = new LogEvent(
            DateTimeOffset.Now,
            LogEventLevel.Debug,
            exception: null,
            messageTemplate,
            properties);

        var message = SerilogExpression.Compile("@m")(evt);
        var messageValue = Assert.IsType<ScalarValue>(message).Value;

        Assert.Equal("HTTP GET /example", messageValue);
    }
}