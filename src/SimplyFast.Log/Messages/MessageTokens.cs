using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.Collections;

namespace SimplyFast.Log.Messages
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    // Caches message tokens
    public static class MessageTokens
    {
        private static readonly Dictionary<string, MessageToken> _allTokens = new Dictionary<string, MessageToken>();

        public static readonly MessageToken Message = Get("Message");
        public static readonly MessageToken Severity = Get("Severity");
        public static readonly MessageToken Date = Get("Date");
        public static readonly MessageToken Logger = Get("Logger");
        public static readonly MessageToken Thread = Get("Thread");
        public static readonly MessageToken NewLine = Get("NewLine");
        public static readonly MessageToken AppData = Get("AppData");
        public static readonly MessageToken AppRoot = Get("AppRoot");

        public static MessageToken Get(string token)
        {
            token = Normalize(token);
            return _allTokens.GetOrAdd(token, t => new MessageToken(t));
        }

        private static string Normalize(string token)
        {
            return token.ToUpperInvariant();
        }
    }
}