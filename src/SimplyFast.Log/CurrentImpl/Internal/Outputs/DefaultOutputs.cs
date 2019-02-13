using System.Collections.Generic;

namespace SimplyFast.Log.Internal.Outputs
{
    internal class DefaultOutputs : OutputsBase
    {
        private readonly HashSet<IOutput> _outputs = new HashSet<IOutput>();

        public override int Count => _outputs.Count;

        public override void Add(IOutput output)
        {
            _outputs.Add(output);
        }

        public override void Remove(IOutput output)
        {
            _outputs.Remove(output);
        }

        public override IEnumerator<IOutput> GetEnumerator()
        {
            return _outputs.GetEnumerator();
        }
    }
}