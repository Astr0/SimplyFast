using System.Collections;
using System.Collections.Generic;

namespace SimplyFast.Log.Internal.Outputs
{
    internal abstract class OutputsBase : IOutputs
    {
        public abstract IEnumerator<IOutput> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract int Count { get; }

        public void Log(IMessage message)
        {
            foreach (var output in this)
                try
                {
                    output.Log(message);
                }
                catch
                {
                    // we can't do anything in case of failure, but should not throw! Logs are not that important
                }
        }

        public abstract void Add(IOutput output);

        public abstract void Remove(IOutput output);
    }
}