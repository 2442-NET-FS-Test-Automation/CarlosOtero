using System;
using System.Collections.Generic;
using System.Linq;

namespace  MusicKata.Domain
{
    public static class DictionaryExtensions
    {
        public static string ToPrettyString<TKey, TValue>(this Dictionary<TKey, TValue> dict, string separator = "➔")
        {
            if (dict == null) return "null";
            if (dict.Count == 0) return "{\n}";

            int maxKeyLength = dict.Keys.Max(k => k?.ToString()?.Length ?? 0);

            var lines = dict.Select(kvp => 
                $"  {kvp.Key?.ToString().PadRight(maxKeyLength)} {separator} {kvp.Value}"
            );

            return "{\n" + string.Join("\n", lines) + "\n}";
        }
    }
}