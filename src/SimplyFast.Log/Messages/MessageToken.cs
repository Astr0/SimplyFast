using System;

namespace SimplyFast.Log.Messages
{
    public struct MessageToken : IEquatable<MessageToken>
    {
        internal MessageToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            Token = token;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public readonly string Token;

        public bool Equals(MessageToken other)
        {
            return string.Equals(Token, other.Token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MessageToken other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Token != null ? Token.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return Token;
        }
    }
}