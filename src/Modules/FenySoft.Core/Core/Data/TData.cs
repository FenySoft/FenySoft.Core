namespace FenySoft.Core.Data
{
  public class TData<T> : ITData
  {
    #region Fields..

    public T Value;

    #endregion

    #region Constructors..

    public TData()
    {
    }

    public TData(T value)
    {
      Value = value;
    }

    #endregion

    #region Methods..

    public override string ToString()
    {
      return Value.ToString();
    }

    #endregion
  }
}