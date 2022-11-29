namespace FenySoft.Core.Comparers
{
    public class TKeyValuePairComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
    {
        public static readonly TKeyValuePairComparer<TKey, TValue> Instance = new TKeyValuePairComparer<TKey, TValue>(Comparer<TKey>.Default);

        public IComparer<TKey> Comparer { get; private set; }

        public TKeyValuePairComparer(IComparer<TKey> comparer)
        {
            Comparer = comparer;
        }

        public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return Comparer.Compare(x.Key, y.Key);
        }
    }
}
