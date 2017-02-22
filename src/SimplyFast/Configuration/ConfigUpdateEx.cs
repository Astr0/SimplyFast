using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace SF.Configuration
{
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public static class ConfigUpdateEx
    {
        private static string NullMap(string value)
        {
            return value;
        }

        public static T UpdateFromKeyValuePairs<T>(this T config, IEnumerable<KeyValuePair<string, string>> pairs,
            Func<string, string> mapKeys = null, Func<string, string> convertValues = null) where T : IConfig
        {
            if (mapKeys == null)
                mapKeys = NullMap;
            if (convertValues == null)
                convertValues = NullMap;
            foreach (var pair in pairs)
            {
                var mappedKey = mapKeys(pair.Key);
                if (mappedKey == null)
                    continue;
                var convertedValue = convertValues(pair.Value);
                if (convertedValue == null)
                    continue;
                config[mappedKey] = convertedValue;
            }
            return config;
        }

        public static T UpdateFromNameValueCollection<T>(this T config, NameValueCollection collection,
            Func<string, string> mapKeys = null, Func<string, string> convertValues = null) where T : IConfig
        {
            var keyValuePairs = collection.AllKeys.Select(key => new KeyValuePair<string, string>(key, collection[key]));
            return config.UpdateFromKeyValuePairs(keyValuePairs, mapKeys, convertValues);
        }

        //public static T UpdateFromAppSettings<T>(this T config, Func<string, string> mapKeys = null,
        //    Func<string, string> convertValues = null) where T : IConfig
        //{
        //    return config.UpdateFromNameValueCollection(ConfigurationManager.AppSettings, mapKeys, convertValues);
        //}

        public static T UpdateFromConf<T>(this T config, string filePath,
            Func<string, string> mapKeys = null, Func<string, string> convertValues = null) where T : IConfig
        {
            if (!File.Exists(filePath))
                return config;
            var dict = ReadConf(filePath);

            return config.UpdateFromKeyValuePairs(dict, mapKeys, convertValues);
        }

        private static Dictionary<string, string> ReadConf(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var dict = new Dictionary<string, string>();
            foreach (var srcLine in lines)
            {
                if (srcLine == null)
                    continue;
                var line = srcLine;
                line = line.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("//") || line[0] == '#')
                    continue;
                var index = line.IndexOf('=');
                if (index < 0)
                {
                    dict[line] = "";
                }
                else
                {
                    var key = line.Substring(0, index).Trim();
                    var value = line.Substring(index + 1).Trim();
                    dict[key] = value;
                }
            }
            return dict;
        }


        public const string ArgsKey = "args";
        public const string ArgsDelimiter = "\x01";

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static T UpdateFromArgs<T>(this T config, string[] args, string argsKey = ArgsKey, string argsDelimiter = ArgsDelimiter) where T: IConfig
        {
            config[argsKey] = string.Join(argsDelimiter, args);
            // TODO: Parse args
            return config;
        }

        //public static T UpdateFromConnectionStrings<T>(this T config, Func<string, string> mapKeys = null,
        //    Func<string, string> convertValues = null) where T : IConfig
        //{
        //    var keyValuePairs =
        //        ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>()
        //            .Select(x => KeyValuePairEx.Create(x.Name, x.ConnectionString));
        //    return config.UpdateFromKeyValuePairs(keyValuePairs, mapKeys, convertValues);
        //}
    }
}