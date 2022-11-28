namespace FenySoft.Core.Persist
{
    public interface IIndexerPersist
    {
    }

    public interface ITIndexerPersist<T> : IIndexerPersist
    {
        void Store(BinaryWriter writer, Func<int, T> values, int count);
        void Load(BinaryReader reader, Action<int, T> values, int count);
    }
}
