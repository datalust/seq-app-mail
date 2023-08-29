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
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seq.Apps.LogEvents;
using Serilog.Events;
using Serilog.Formatting.Compact.Reader;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Seq.Apps.Testing.Sink;

// This class is from `seqcli`.

class SeqAppContainer : IDisposable
{
    readonly SeqApp _seqApp;
    readonly IAppHost _host;

    readonly JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        DateParseHandling = DateParseHandling.None,
        Culture = CultureInfo.InvariantCulture
    });

    public SeqAppContainer(
        SeqApp app,
        IAppHost host)
    {
        _seqApp = app;
        _host = host;
        _seqApp.Attach(host);
    }

    public void Dispose()
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        (_seqApp as IDisposable)?.Dispose();
    }

    public async Task SendAsync(string clef)
    {
        if (clef == null) throw new ArgumentNullException(nameof(clef));

        if (_seqApp is ISubscribeToJsonAsync subscribeToJsonAsync)
        {
            // Shorter, cheaper path for the "modern" interface
            try
            {
                await subscribeToJsonAsync.OnAsync(clef);
            }
            catch (Exception ex)
            {
                ReadSerilogEvent(clef, out var eventId, out _);
                throw new Exception($"The event {eventId} could not be sent to {_host.App.Title}.", ex);
            }
        }
        else
        {
            await SendTypedEventAsync(clef);
        }
    }

    async Task SendTypedEventAsync(string clef)
    {
        var serilogEvent = ReadSerilogEvent(clef, out var eventId, out var eventType);
        try
        {
            if (_seqApp is ISubscribeTo<LogEventData> led)
            {
                led.On(EventFormat.FromRaw(eventId, eventType, serilogEvent));
            }
            else if (_seqApp is ISubscribeToAsync<LogEventData> leda)
            {
                await leda.OnAsync(EventFormat.FromRaw(eventId, eventType, serilogEvent));
            }
            else if (_seqApp is ISubscribeTo<LogEvent> sled)
            {
                sled.On(new Event<LogEvent>(eventId, eventType, serilogEvent.Timestamp.UtcDateTime, serilogEvent));
            }
            else if (_seqApp is ISubscribeToAsync<LogEvent> sleda)
            {
                await sleda.OnAsync(new Event<LogEvent>(eventId, eventType, serilogEvent.Timestamp.UtcDateTime, serilogEvent));
            }
            else
            {
                throw new SeqAppException("The app doesn't support any recognized subscriber interfaces.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"The event {eventId} could not be sent to {_host.App.Title}.", ex);
        }
    }

    LogEvent ReadSerilogEvent(string clef, out string eventId, out uint eventType)
    {
        var jValue = new JsonTextReader(new StringReader(clef));
        if (_serializer.Deserialize<JToken>(jValue) is not JObject jObject)
            throw new InvalidDataException($"The line is not a JSON object: `{clef.Trim()}`.");

        var raw = LogEventReader.ReadFromJObject(jObject);

        eventId = "event-0";
        if (raw.Properties.TryGetValue("@seqid", out var id) &&
            id is ScalarValue {Value: string sid})
            eventId = sid;

        eventType = 0u;
        if (raw.Properties.TryGetValue("@i", out var et) &&
            et is ScalarValue {Value: string set} && uint.TryParse(set, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var uet))
            eventType = uet;

        return raw;
    }
}
