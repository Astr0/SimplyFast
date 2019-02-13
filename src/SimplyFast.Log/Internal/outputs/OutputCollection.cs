using System.Collections;
using System.Collections.Generic;
using SimplyFast.Log.Messages;

namespace SimplyFast.Log.Internal
{
    internal class OutputCollection : IOutputs
    {
        private readonly List<IOutput> _outputs = new List<IOutput>();

        public int Count => _outputs.Count;

        public void Add(IOutput writer)
        {
            _outputs.Add(writer);
        }

        public void Remove(IOutput writer)
        {
            _outputs.Remove(writer);
        }

        public void Dispose()
        {
            foreach (var output in _outputs) output.Dispose();
            _outputs.Clear();
        }

        public void Log(IMessage message)
        {
            foreach (var output in _outputs)
                try
                {
                    output.Log(message);
                }
                catch
                {
                    // we can't do something when writer fails
                    // bu we should at least ignore errors and don't let app fail cause of stupid logs
                }
        }

        public IEnumerator<IOutput> GetEnumerator()
        {
            return _outputs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}