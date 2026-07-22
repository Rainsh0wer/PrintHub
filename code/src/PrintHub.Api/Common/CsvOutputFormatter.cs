using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace PrintHub.Api.Common;

/// <summary>
/// Minimal CSV output formatter that enables content negotiation for report
/// endpoints (Accept: text/csv). Reflects an object's public scalar properties into
/// a header row plus one data row; a collection renders one row per item. Report
/// DTOs are deliberately flat so the same action serialises cleanly to JSON, XML,
/// and CSV depending purely on the Accept header.
/// </summary>
public class CsvOutputFormatter : TextOutputFormatter
{
    public CsvOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
        SupportedEncodings.Add(Encoding.UTF8);
    }

    protected override bool CanWriteType(Type? type) => type is not null;

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var buffer = new StringBuilder();

        if (context.Object is IEnumerable enumerable and not string)
        {
            var items = enumerable.Cast<object>().ToList();
            if (items.Count > 0)
            {
                var props = ScalarProps(items[0].GetType());
                buffer.AppendLine(string.Join(",", props.Select(p => Escape(p.Name))));
                foreach (var item in items)
                    buffer.AppendLine(string.Join(",", props.Select(p => Escape(p.GetValue(item)))));
            }
        }
        else if (context.Object is { } obj)
        {
            var props = ScalarProps(obj.GetType());
            buffer.AppendLine(string.Join(",", props.Select(p => Escape(p.Name))));
            buffer.AppendLine(string.Join(",", props.Select(p => Escape(p.GetValue(obj)))));
        }

        await context.HttpContext.Response.WriteAsync(buffer.ToString(), selectedEncoding);
    }

    private static PropertyInfo[] ScalarProps(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && IsScalar(p.PropertyType))
            .ToArray();

    private static bool IsScalar(Type t)
    {
        t = Nullable.GetUnderlyingType(t) ?? t;
        return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime);
    }

    private static string Escape(object? value)
    {
        var s = value switch
        {
            null => "",
            DateTime dt => dt.ToString("o", CultureInfo.InvariantCulture),
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? ""
        };

        if (s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r'))
            s = "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }
}
