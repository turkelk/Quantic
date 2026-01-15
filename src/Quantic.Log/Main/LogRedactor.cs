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

            var patterns = CompilePatterns(propertyNames);
            if (patterns.Count == 0) return value;

            try
            {
                var node = JsonSerializer.SerializeToNode(value, value.GetType());
                if (node == null) return value;

                var path = new List<string>(capacity: 8);
                RedactNode(node, patterns, string.IsNullOrWhiteSpace(mask) ? "***" : mask, path);
                return node;
            }
            catch
            {
                // Never fail the primary operation due to logging.
                return value;
            }
        }

        private static IReadOnlyList<PathPattern> CompilePatterns(IEnumerable<string> patterns)
        {
            if (patterns == null) return Array.Empty<PathPattern>();

            var list = new List<PathPattern>();

            foreach (var raw in patterns)
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;
                var trimmed = raw.Trim();
                if (trimmed.Length == 0) continue;

                // Split on '.', ignore empty segments (e.g. accidental "..").
                var segs = trimmed
                    .Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0)
                    .ToArray();

                if (segs.Length == 0) continue;

                // Backwards compatible:
                // "Password" means redact any property named "Password" at ANY depth.
                var isSimpleName =
                    segs.Length == 1 &&
                    !string.Equals(segs[0], "*", StringComparison.Ordinal) &&
                    !string.Equals(segs[0], "**", StringComparison.Ordinal) &&
                    !trimmed.Contains('.');

                if (isSimpleName)
                {
                    list.Add(new PathPattern(new[] { "**", segs[0] }));
                    continue;
                }

                list.Add(new PathPattern(segs));
            }

            return list;
        }

        private static void RedactNode(JsonNode node, IReadOnlyList<PathPattern> patterns, string mask, List<string> path)
        {
            if (node is JsonObject obj)
            {
                // Snapshot keys because we'll mutate values.
                var keys = obj.Select(kvp => kvp.Key).ToArray();
                foreach (var key in keys)
                {
                    path.Add(key);

                    if (ShouldRedact(path, patterns))
                    {
                        obj[key] = mask;
                        path.RemoveAt(path.Count - 1);
                        continue;
                    }

                    var child = obj[key];
                    if (child != null)
                        RedactNode(child, patterns, mask, path);

                    path.RemoveAt(path.Count - 1);
                }

                return;
            }

            if (node is JsonArray arr)
            {
                foreach (var child in arr)
                {
                    if (child != null)
                        RedactNode(child, patterns, mask, path);
                }
            }
        }

        private static bool ShouldRedact(IReadOnlyList<string> path, IReadOnlyList<PathPattern> patterns)
        {
            for (var i = 0; i < patterns.Count; i++)
            {
                if (patterns[i].IsMatch(path)) return true;
            }

            return false;
        }

        private sealed class PathPattern
        {
            private readonly string[] segments;

            public PathPattern(string[] segments)
            {
                this.segments = segments ?? Array.Empty<string>();
            }

            public bool IsMatch(IReadOnlyList<string> path)
            {
                if (path == null) return false;
                return IsMatch(pathIndex: 0, segIndex: 0, path);
            }

            // Supports:
            // - literal segment: "Payload"
            // - wildcard segment: "*"  (match exactly one segment)
            // - wildcard segment: "**" (match zero or more segments)
            //
            // Examples:
            // - "Password" -> compiled to "**.Password" (match at any depth)
            // - "Image.Base64" -> match only Base64 under Image
            // - "Payload.*.Token" -> match Token exactly 2 levels deep under Payload (Payload.<any>.Token)
            // - "Payload.**.Token" -> match Token anywhere under Payload
            private bool IsMatch(int pathIndex, int segIndex, IReadOnlyList<string> path)
            {
                if (segIndex >= segments.Length)
                    return pathIndex >= path.Count;

                var seg = segments[segIndex];

                if (string.Equals(seg, "**", StringComparison.Ordinal))
                {
                    // Try to match the rest of the pattern at any suffix of the path.
                    for (var i = pathIndex; i <= path.Count; i++)
                    {
                        if (IsMatch(i, segIndex + 1, path)) return true;
                    }
                    return false;
                }

                if (pathIndex >= path.Count) return false;

                if (string.Equals(seg, "*", StringComparison.Ordinal))
                    return IsMatch(pathIndex + 1, segIndex + 1, path);

                if (!string.Equals(seg, path[pathIndex], StringComparison.OrdinalIgnoreCase))
                    return false;

                return IsMatch(pathIndex + 1, segIndex + 1, path);
            }
        }
    }
}
