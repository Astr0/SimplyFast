using SimplyFast.Log.Messages;

namespace SimplyFast.Log.Internal
{
    public class StringOutputPattern : IOutputPattern
    {
        private readonly string _str;

        public StringOutputPattern(string str)
        {
            _str = str;
        }

        public string GetValue(IMessage message)
        {
            return _str;
        }

        public override string ToString()
        {
            return _str;
        }
    }
}