using System;

namespace SF.Configuration
{
    public static class ConfigReadEx
    {
        public static string[] GetArgs(this IReadOnlyConfig config,
            string argsKey = ConfigUpdateEx.ArgsKey, string argsDelimiter = ConfigUpdateEx.ArgsDelimiter)
        {
            var args = config.GetString(argsKey);
            return args?.Split(new[] {argsDelimiter}, StringSplitOptions.None);
        }

        private static T? GetStruct<T>(this IReadOnlyConfig config, string key, Func<string, T> parse)
            where T : struct
        {
            var value = config[key];
            return !string.IsNullOrWhiteSpace(value) ? parse(value) : (T?) null;
        }

        public static string GetString(this IReadOnlyConfig config, string key)
        {
            var value = config[key];
            return value;
        }

        public static int? GetInt32(this IReadOnlyConfig config, string key)
        {
            return config.GetStruct(key, int.Parse);
        }

        public static long? GetInt64(this IReadOnlyConfig config, string key)
        {
            return config.GetStruct(key, long.Parse);
        }

        public static T? GetEnum<T>(this IReadOnlyConfig config, string key)
            where T : struct
        {
            return config.GetStruct(key, s => (T) Enum.Parse(typeof(T), s, true));
        }

        public static TimeSpan? GetTimeSpan(this IReadOnlyConfig config, string key)
        {
            return config.GetStruct(key, TimeSpan.Parse);
        }

        public static bool? GetBool(this IReadOnlyConfig config, string key)
        {
            return config.GetStruct(key, ParseBool);
        }

        private static bool ParseBool(string arg)
        {
            switch (arg.ToLowerInvariant())
            {
                case "true":
                case "1":
                case "t":
                    return true;
                case "0":
                case "false":
                case "f":
                    return false;
                default:
                    throw new FormatException();
            }
        }
    }
}