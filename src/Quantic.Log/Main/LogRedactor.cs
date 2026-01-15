using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Quantic.Log
{
    internal static class LogRedactor
    {
        public static object Redact(object value, IEnumerable<string> propertyNames, string mask)
        {
            if (value == null) return null;

            var names = (propertyNames ?? Array.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (names.Length == 0) return value;

            try
            {
                var node = JsonSerializer.SerializeToNode(value, value.GetType());
                if (node == null) return value;

                RedactNode(node, new HashSet<string>(names, StringComparer.OrdinalIgnoreCase), string.IsNullOrWhiteSpace(mask) ? "***" : mask);
                return node;
            }
            catch
            {
                // Never fail the primary operation due to logging.
                return value;
            }
        }

        private static void RedactNode(JsonNode node, HashSet<string> names, string mask)
        {
            if (node is JsonObject obj)
            {
                // Snapshot keys because we'll mutate values.
                var keys = obj.Select(kvp => kvp.Key).ToArray();
                foreach (var key in keys)
                {
                    if (names.Contains(key))
                    {
                        obj[key] = mask;
                        continue;
                    }

                    var child = obj[key];
                    if (child != null)
                        RedactNode(child, names, mask);
                }
                return;
            }

            if (node is JsonArray arr)
            {
                foreach (var child in arr)
                {
                    if (child != null)
                        RedactNode(child, names, mask);
                }
            }
        }
    }
}
