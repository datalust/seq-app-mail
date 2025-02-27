// Copyright © Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using Seq.Syntax.Expressions;
using Seq.Syntax.Templates.Rendering;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Parsing;

namespace Seq.Syntax.Templates.Compilation;

class CompiledMessageToken : CompiledTemplate
{
    readonly IFormatProvider? _formatProvider;
    readonly Alignment? _alignment;
    readonly JsonValueFormatter _jsonFormatter;

    public CompiledMessageToken(IFormatProvider? formatProvider, Alignment? alignment)
    {
        _formatProvider = formatProvider;
        _alignment = alignment;
        _jsonFormatter = new JsonValueFormatter("$type");
    }

    public override void Evaluate(EvaluationContext ctx, TextWriter output)
    {
        if (_alignment == null)
        {
            EvaluateUnaligned(ctx, output);
        }
        else
        {
            var writer = new StringWriter();
            EvaluateUnaligned(ctx, writer);
            Padding.Apply(output, writer.ToString(), _alignment.Value);
        }
    }

    void EvaluateUnaligned(EvaluationContext ctx, TextWriter output)
    {
        foreach (var token in ctx.LogEvent.MessageTemplate.Tokens)
        {
            switch (token)
            {
                case TextToken tt:
                {
                    output.Write(tt.Text);
                    break;
                }
                case PropertyToken pt:
                {
                    EvaluateProperty(ctx.LogEvent.Properties, pt, output);
                    break;
                }
                default:
                {
                    output.Write(token);
                    break;
                }
            }
        }
    }
    
    void EvaluateProperty(IReadOnlyDictionary<string, LogEventPropertyValue> properties, PropertyToken pt, TextWriter output)
    {
        var rest = pt.PropertyName.AsSpan();
        if (!TryGetNextStep(rest, out var name, out rest))
        {
            output.Write(pt);
            return;
        }

        if (!properties.TryGetValue(name.ToString(), out var value))
        {
            output.Write(pt.ToString());
            return;
        }
        
        while (TryGetNextStep(rest, out name, out rest))
        {
            if (value is not StructureValue obj)
            {
                output.Write(pt);
                return;
            }

            var nameString = name.ToString();
            var found = false;
            foreach (var property in obj.Properties)
            {
                if (property.Name == nameString)
                {
                    value = property.Value;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                output.Write(pt);
                return;
            }
        }
        
        if (pt.Alignment is null)
        {
            EvaluatePropertyUnaligned(value, output, pt.Format);
            return;
        }

        var buffer = new StringWriter();

        EvaluatePropertyUnaligned(value, buffer, pt.Format);

        var result = buffer.ToString();

        if (result.Length >= pt.Alignment.Value.Width)
            output.Write(result);
        else
            Padding.Apply(output, result, pt.Alignment.Value);
    }

    void EvaluatePropertyUnaligned(LogEventPropertyValue propertyValue, TextWriter output, string? format)
    {
        if (propertyValue is not ScalarValue scalar)
        {
            _jsonFormatter.Format(propertyValue, output);
            return;
        }

        var value = scalar.Value;

        if (value == null)
        {
            output.Write("null");
            return;
        }

        if (value is string str)
        {
            output.Write(str);
            return;
        }

        if (value is ValueType)
        {
            if (value is int or uint or long or ulong or decimal or byte or sbyte or short or ushort)
            {
                output.Write(((IFormattable)value).ToString(format, _formatProvider));
                return;
            }

            if (value is double d)
            {
                output.Write(d.ToString(format, _formatProvider));
                return;
            }

            if (value is float f)
            {
                output.Write(f.ToString(format, _formatProvider));
                return;
            }

            if (value is bool b)
            {
                output.Write(b);
                return;
            }
        }

        if (value is IFormattable formattable)
        {
            output.Write(formattable.ToString(format, _formatProvider));
            return;
        }

        output.Write(value);
    }
    
    static bool TryGetNextStep(ReadOnlySpan<char> path, out ReadOnlySpan<char> name, out ReadOnlySpan<char> rest)
    {
        if (path.Length == 0)
        {
            name = [];
            rest = [];
            return false;
        }
        
        var i = path.IndexOf('.');
        if (i == -1)
        {
            name = path;
            rest = [];
            return true;
        }
        
        name = path[..i];
        rest = i == name.Length - 1 ? [] : path[(i + 1)..];

        return true;
    }
}