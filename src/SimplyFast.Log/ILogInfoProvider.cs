using System.Collections.Generic;

namespace SimplyFast.Log
{
    public interface ILogInfoProvider: IEnumerable<ILogInfo>
    {
        ILogInfo Get(string name);
    }

    public interface ILogInfoStorage : ILogInfoProvider
    {
        void Add(ILogInfo info);
        void Upsert(ILogInfo info);
    }
}