using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SimplyFast.Collections;
using SimplyFast.Log.Messages.Internal;

namespace SimplyFast.Log.Messages
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class MessageTokens
    {
        public static readonly MessageToken Message = Token("Message");
        public static readonly MessageToken Severity = Token("Severity");
        public static readonly MessageToken Date = Token("Date");
        public static readonly MessageToken Logger = Token("Logger");
        public static readonly MessageToken Thread = Token("Thread");
        public static readonly MessageToken NewLine = Token("NewLine");
        public static readonly MessageToken AppData = Token("AppData");
        public static readonly MessageToken AppRoot = Token("AppRoot");

        private static readonly Dictionary<string, MessageToken> _allTokens = new[]
        {
            Message,
            Severity,
            Date,
            Logger,
            Thread,
            NewLine,
            AppData,
            AppRoot
        }.ToDictionary(x => x.Token);

        private static readonly GlobalTokenResolvers _resolvers = new GlobalTokenResolvers
        {
            {Date, f => DateTime.Now.ToString(f)},
            {Thread, GetThreadValue},
            {NewLine, f => Environment.NewLine},
            {AppData, f => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)},
            {AppRoot, f => AppEx.ExecutableDirectory}
        };

        private static MessageToken Token(string token)
        {
            token = Normalize(token);
            return new MessageToken(token);
        }

        private static string Normalize(string token)
        {
            return token.ToUpperInvariant();
        }

        private static string GetThreadValue(string format)
        {
            var current = System.Threading.Thread.CurrentThread;
            return current.Name ?? current.ManagedThreadId.ToString();
        }

        public static void MapToken(string token, GlobalTokenResolver resolver)
        {
            MapToken(Get(token), resolver);
        }

        public static void MapToken(MessageToken token, GlobalTokenResolver resolver)
        {
            _resolvers.Add(token, resolver);
        }

        public static string GetTokenValue(MessageToken token, string format)
        {
            return _resolvers.GetTokenValue(token, format);
        }

        public static MessageToken Get(string token)
        {
            token = Normalize(token);
            var result = _allTokens.GetOrDefault(token);
            return result.IsEmpty ? new MessageToken(token) : result;
        }
    }
}