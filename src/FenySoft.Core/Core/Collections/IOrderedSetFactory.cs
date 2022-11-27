using FenySoft.Core.Data;

namespace FenySoft.Core.Collections
{
    public interface IOrderedSetFactory
    {
        IOrderedSet<IData, IData> Create();
    }
}
