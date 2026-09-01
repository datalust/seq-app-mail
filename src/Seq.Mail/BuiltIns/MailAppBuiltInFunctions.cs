using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Seq.Apps;
using Seq.Mail.TimeZones;
using Seq.Syntax.Expressions;
// ReSharper disable ReturnTypeCanBeNotNullable
// ReSharper disable UnusedMember.Global

namespace Seq.Mail.BuiltIns;

public static class MailAppBuiltInFunctions
{
    public static EvaluationResult UriEncode(string value)
    {
        return EvaluationResult.Defined(JsonValue.Create(Uri.EscapeDataString(value)));
    }

    public static EvaluationResult InTimeZone(DateTimeOffset dateTime, string timeZoneName)
    {
        var tzi = PortableTimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
        dateTime = TimeZoneInfo.ConvertTime(dateTime, tzi);

        if (dateTime.Offset == TimeSpan.Zero)
        {
            // Use the idiomatic trailing `Z` formatting for ISO-8601 in UTC.
            return JsonValue.Create(dateTime.UtcDateTime);
        }

        return JsonValue.Create(dateTime);
    }

    public static EvaluationResult MailAppHost(Host host)
    {
        return new JsonObject
        {
            ["BaseUri"] = host.BaseUri,
            ["InstanceName"] = host.InstanceName,
        };
    }

    public static EvaluationResult MailAppInstance(App app)
    {
        return new JsonObject
        {
            ["Id"] = app.Id,
            ["Title"] = app.Title,
            ["Settings"] = new JsonObject(
                [.. app.Settings.Select(kvp => KeyValuePair.Create<string, JsonNode?>(kvp.Key, kvp.Value))])
        };
    }
}