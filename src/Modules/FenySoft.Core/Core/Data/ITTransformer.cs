namespace FenySoft.Core.Data
{
    public interface ITTransformer<T1, T2>
    {
        T2 To(T1 value1);
        T1 From(T2 value2);
    }
}
