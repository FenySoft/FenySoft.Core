using System.Linq.Expressions;
using System.Reflection;

namespace FenySoft.Core.Data
{
    public class TDataComparer : IComparer<ITData>
    {
        public readonly Func<ITData, ITData, int> compare;

        public readonly Type Type;
        public readonly Type DataType;
        public readonly TCompareOption[] CompareOptions;
        public readonly Func<Type, MemberInfo, int> MembersOrder;

        public TDataComparer(Type type, TCompareOption[] compareOptions, Func<Type, MemberInfo, int> membersOrder = null)
        {
            Type = type;
            DataType = typeof(TData<>).MakeGenericType(type);

            TCompareOption.CheckCompareOptions(type, compareOptions, membersOrder);
            CompareOptions = compareOptions;
            MembersOrder = membersOrder;

            compare = CreateCompareMethod().Compile();
        }

        public TDataComparer(Type type, Func<Type, MemberInfo, int> membersOrder = null)
            : this(type, TCompareOption.GetDefaultCompareOptions(type, membersOrder), membersOrder)
        {
        }

        public Expression<Func<ITData, ITData, int>> CreateCompareMethod()
        {
            var x = Expression.Parameter(typeof(ITData));
            var y = Expression.Parameter(typeof(ITData));

            List<Expression> list = new List<Expression>();
            List<ParameterExpression> parameters = new List<ParameterExpression>();

            var value1 = Expression.Variable(Type, "value1");
            parameters.Add(value1);
            list.Add(Expression.Assign(value1, Expression.Convert(x, DataType).Value()));

            var value2 = Expression.Variable(Type, "value2");
            parameters.Add(value2);
            list.Add(Expression.Assign(value2, Expression.Convert(y, DataType).Value()));

            return Expression.Lambda<Func<ITData, ITData, int>>(ComparerHelper.CreateComparerBody(list, parameters, value1, value2, CompareOptions, MembersOrder), x, y);
        }

        public int Compare(ITData x, ITData y)
        {
            return compare(x, y);
        }
    }
}
