using System;
using System.Collections.Generic;
using System.Text;

namespace SimplyFast.Strings
{
    public static class StringEx
    {
        public static string Left(this string str, int length)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string Right(this string str, int length)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return str.Substring(Math.Max(0, str.Length - length), Math.Min(length, str.Length));
        }


        public static string Skip(this string str, string start)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (start == null)
                return str;
            return str.StartsWith(start) ? str.Substring(start.Length) : str;
        }

        public static string SubstringSafe(this string str, int startIndex, int length = int.MaxValue)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            startIndex = startIndex.Clip(0, str.Length);
            length = length.Clip(0, str.Length - startIndex);
            return str.Substring(startIndex, length);
        }

        public static string RemoveAll(this string str, params char[] characters)
        {
            if (characters == null || characters.Length == 0 || string.IsNullOrEmpty(str))
                return str;
            var set = new HashSet<char>(characters);
            var sb = new StringBuilder(str.Length);
            foreach (var t in str)
                if (!set.Contains(t))
                    sb.Append(t);
            return sb.ToString();
        }

        public static string Quote(this string str, char quoteCharacter = '"')
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            var doubleQuote = quoteCharacter + quoteCharacter.ToString();
            return quoteCharacter + str.Replace(char.ToString(quoteCharacter), doubleQuote) + quoteCharacter;
        }

        public static string Repeat(string str, int count)
        {
            if (string.IsNullOrEmpty(str) || count <= 0)
                return string.Empty;

            if (count == 1)
                return str;

            if (str.Length == 1)
                return new string(str[0], count);

            var builder = new StringBuilder(str.Length * count);
            for (var i = 0; i < count; ++i)
                builder.Append(str);
            return builder.ToString();
        }

        public static string[] SplitQuoted(this string str, char delimiter = ',', char quoteCharacter = '"')
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (str == string.Empty)
                return new string[0];

            var array = str.ToCharArray();
            var resultIndex = 0;
            var len = str.Length;
            var result = new string[len + 1];
            var index = 0;
            while (index < len)
            {
                var current = array[index];
                if (current == delimiter)
                {
                    // Empty value
                    result[resultIndex++] = string.Empty;
                }
                else if (current == quoteCharacter)
                {
                    // Quoted value

                    // skip quote character
                    index++;
                    var inQuote = true;
                    var bufIndex = 0;

                    while (inQuote)
                    {
                        // Process inQuote part
                        var quoteIndex = Array.IndexOf(array, quoteCharacter, index, len - index);
                        if (quoteIndex < 0)
                            quoteIndex = len;
                        var length = quoteIndex - index;
                        if (length > 0)
                        {
                            // skip quote character
                            Array.Copy(array, index, array, bufIndex, length);
                            bufIndex += length;
                        }
                        inQuote = false;
                        index = quoteIndex + 1;


                        // Process out of quote part
                        if (index >= len)
                            break;
                        current = array[index];
                        if (current == quoteCharacter)
                        {
                            inQuote = true;
                            // Set quote character
                            array[bufIndex++] = quoteCharacter;
                            index++;
                        }
                        else if (current != delimiter)
                        {
                            var delimiterIndex = Array.IndexOf(array, delimiter, index, len - index);
                            if (delimiterIndex < 0)
                                delimiterIndex = len;
                            length = delimiterIndex - index;
                            Array.Copy(array, index, array, bufIndex, length);
                            bufIndex += length;
                            index = delimiterIndex;
                        }
                    }
                    result[resultIndex++] = new string(array, 0, bufIndex);
                }
                else
                {
                    // Normal value
                    var endIndex = Array.IndexOf(array, delimiter, index, len - index);
                    if (endIndex < 0)
                        endIndex = len;
                    result[resultIndex++] = new string(array, index, endIndex - index);
                    index = endIndex;
                }
                ++index;
            }

            if (array[len - 1] == delimiter)
                result[resultIndex++] = string.Empty;

            Array.Resize(ref result, resultIndex);
            return result;
        }
    }
}