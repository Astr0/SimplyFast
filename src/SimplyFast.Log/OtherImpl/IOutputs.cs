using System.Collections.Generic;

namespace SimplyFast.Log
{
    public interface IOutputs : IOutput, IEnumerable<IOutput>
    {
        int Count { get; }
        void Add(IOutput writer);
        void Remove(IOutput writer);
    }
}