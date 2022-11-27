using System.Linq.Expressions;

namespace FenySoft.Core.Data
{
    public static class DataExtensions
    {
        public static Expression Value(this Expression data)
        {
            return Expression.Field(data, "Value");
        }
    }
}
