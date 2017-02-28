using System.Collections.Generic;

namespace SimplyFast.Strings.Tokens
{
    public interface IStringTokenProvider: IEnumerable<IStringToken>
    {
        IStringToken Get(string name);
    }

    public interface IStringTokenStorage : IStringTokenProvider
    {
        void Add(IStringToken info);
        void Upsert(IStringToken info);
    }
}