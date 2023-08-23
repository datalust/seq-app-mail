using System;
using System.Globalization;
using Seq.Mail.Expressions.Runtime;
using Seq.Mail.TimeZones;
using Serilog.Events;

namespace Seq.Mail.BuiltIns
{
    public static class MailAppBuiltInFunctions
    {
        public static LogEventPropertyValue? ToString(LogEventPropertyValue? value, LogEventPropertyValue? format)
        {
            if (value is not ScalarValue { Value: {} sv } ||
                format is not null and not ScalarValue { Value: string })
            {
                return null;
            }

            if (sv is IFormattable formattable && format is ScalarValue { Value: string f })
            {
                // Culture may end up having to be selectable.
                return new ScalarValue(formattable.ToString(f, CultureInfo.InvariantCulture));
            }

            return new ScalarValue(sv.ToString());
        }
        
        public static LogEventPropertyValue? UriEncode(LogEventPropertyValue? value)
        {
            if (Coerce.String(value, out var s))
                return new ScalarValue(Uri.EscapeDataString(s));

            return null;
        }

        public static LogEventPropertyValue? InTimeZone(LogEventPropertyValue? dateTime, LogEventPropertyValue? timeZoneName)
        {
            // Using `DateTimeOffset` avoids ending up with `DateTimeKind.Unspecified` after time zone conversion.
            DateTimeOffset dt;
            if (dateTime is ScalarValue { Value: DateTimeOffset dto })
            {
                dt = dto;
            }
            else if (dateTime is ScalarValue { Value: DateTime rdt })
            {
                dt = rdt.Kind == DateTimeKind.Unspecified ? new DateTime(rdt.Ticks, DateTimeKind.Utc) : rdt;
            }
            else if (dateTime is ScalarValue { Value: string str } && DateTimeOffset.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dts))
            {
                dt = dts;
            }
            else if (Coerce.Numeric(dateTime, out var num))
            {
                dt = new DateTime((long)num, DateTimeKind.Utc);
            }
            else
            {
                return null;
            }

            if (timeZoneName is not ScalarValue { Value: string tz })
            {
                return null;
            }

            var tzi = PortableTimeZoneInfo.FindSystemTimeZoneById(tz);
            dt = TimeZoneInfo.ConvertTime(dt, tzi);

            if (dt.Offset == TimeSpan.Zero)
            {
                // Use the idiomatic trailing `Z` formatting for ISO-8601 in UTC.
                return new ScalarValue(dt.UtcDateTime);
            }

            return new ScalarValue(dt);
        }
    }
}