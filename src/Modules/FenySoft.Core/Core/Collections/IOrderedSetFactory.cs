using FenySoft.Core.Data;

namespace FenySoft.Core.Collections
{
    public interface IOrderedSetFactory
    {
        IOrderedSet<ITData, ITData> Create();
    }
}
