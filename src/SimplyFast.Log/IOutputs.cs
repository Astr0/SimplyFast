using System.Collections.Generic;

namespace SimplyFast.Log
{
    public interface IOutputs: IReadOnlyCollection<IOutput>, IOutput
    {
        void Add(IOutput output);
        void Remove(IOutput output);
    }
}