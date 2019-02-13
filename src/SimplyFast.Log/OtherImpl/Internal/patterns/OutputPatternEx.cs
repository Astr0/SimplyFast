using System.Collections.Generic;
using System.Text;
using SimplyFast.Log.Messages;

namespace SimplyFast.Log.Internal
{
    public static class OutputPatternEx
    {
        public static string ToString(this IEnumerable<IOutputPattern> patterns, IMessage message)
        {
            var sb = new StringBuilder();
            foreach (var pattern in patterns)
                sb.Append(pattern.GetValue(message));
            return sb.ToString();
        }
    }
}