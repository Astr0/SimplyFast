using System;
using System.IO;

namespace SF.Configuration
{
    public static class ConfigFileUpdateEx
    {
        public static T UpdateFromConf<T>(this T config, string filePath,
            Func<string, string> mapKeys = null, Func<string, string> convertValues = null) where T : IConfig
        {
            if (!File.Exists(filePath))
                return config;
            return config.UpdateFromConf(File.ReadAllLines(filePath), mapKeys, convertValues);
        }
    }
}