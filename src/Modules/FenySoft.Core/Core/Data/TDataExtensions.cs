using System.Linq.Expressions;

namespace FenySoft.Core.Data
{
    public static class TDataExtensions
    {
        public static Expression Value(this Expression AData)
        {
            return Expression.Field(AData, "Value");
        }
    }
}
