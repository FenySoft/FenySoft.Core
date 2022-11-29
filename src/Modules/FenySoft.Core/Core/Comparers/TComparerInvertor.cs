namespace FenySoft.Core.Comparers
{
    public class TComparerInvertor<T> : IComparer<T>
    {
        public readonly IComparer<T> Comparer;

        public TComparerInvertor(IComparer<T> comparer)
        {
            Comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return -Comparer.Compare(x, y);
        }
    }
}
