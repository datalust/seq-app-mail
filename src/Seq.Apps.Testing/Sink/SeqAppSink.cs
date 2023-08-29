// Copyright © Datalust Pty Ltd
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
using System.IO;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

namespace Seq.Apps.Testing.Sink;

class SeqAppSink : ILogEventSink, IDisposable
{
    readonly SeqAppContainer _container;
    readonly JsonValueFormatter _valueFormatter = new("$type");

    public SeqAppSink(SeqApp seqApp, IAppHost host)
    {
        _container = new SeqAppContainer(seqApp, host);
    }

    public void Emit(LogEvent logEvent)
    {
        using var output = new StringWriter();
        CompactJsonFormatter.FormatEvent(logEvent, output, _valueFormatter);
        var clef = output.ToString();
        _container.SendAsync(clef).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _container.Dispose();
    }
}
