using System;
using System.ComponentModel;
using SF.IoC;

namespace SF.Localization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LocalDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string _key;

        public LocalDisplayNameAttribute(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            _key = key;
        }

        public LocalDisplayNameAttribute(Type type, string key)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            _key = type.FullName + "." + key;
        }

        private string FallbackName
        {
            get { return "[" + _key + "]"; }
        }

        public override String DisplayName
        {
            get
            {
                var textProvider = ServiceLocator.Get<ITextProvider>();
                if (textProvider == null) 
                    return FallbackName;
                var text = textProvider[_key];
                return text ?? FallbackName;
            }
        }
    }
}