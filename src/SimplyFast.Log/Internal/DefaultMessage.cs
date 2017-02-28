using System.Collections;
using System.Collections.Generic;
using SimplyFast.Strings.Tokens;

namespace SimplyFast.Log.Internal
{
    internal class DefaultMessage : IMessage
    {
        private readonly IStringTokenStorage _storage;

        public DefaultMessage(Severity severity, IStringTokenStorage storage = null)
        {
            _storage = storage ?? StringTokenProviderEx.Sequential();
            _storage.Add(LogTokenEx.Severity(severity));
            Severity = severity;
        }

        public Severity Severity { get; }

        public IEnumerator<IStringToken> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        public IStringToken Get(string name)
        {
            return _storage.Get(name);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _storage).GetEnumerator();
        }

        public void Add(IStringToken info)
        {
            _storage.Add(info);
        }

        public void Upsert(IStringToken info)
        {
            _storage.Upsert(info);
        }
    }
}