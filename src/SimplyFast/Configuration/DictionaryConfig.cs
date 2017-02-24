using System;
using System.Collections.Generic;
using SimplyFast.Collections;

namespace SimplyFast.Configuration
{
    public class DictionaryConfig : IConfig
    {
        private readonly Dictionary<string, string> _config =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string this[string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));
                return _config.GetOrDefault(key);
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));
                if (value == null)
                    _config.Remove(key);
                else
                    _config[key] = value;
            }
        }
    }
}