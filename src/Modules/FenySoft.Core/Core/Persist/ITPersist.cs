namespace FenySoft.Core.Persist
{
    public interface IPersist
    {
    }

    public interface ITPersist<T> : IPersist
    {
        void Write(BinaryWriter writer, T item);
        T Read(BinaryReader reader);
    }
}
