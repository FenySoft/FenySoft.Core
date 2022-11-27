namespace FenySoft.Core.Data
{
    public class Data<T> : ITData
    {
        public T Value;

        public Data()
        {
        }

        public Data(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
