using FenySoft.Core.Data;

namespace FenySoft.Core.Collections
{
    public interface ITOrderedSetFactory
    {
        ITOrderedSet<ITData, ITData> Create();
    }
}
