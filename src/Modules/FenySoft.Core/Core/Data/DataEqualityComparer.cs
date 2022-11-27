using System.Linq.Expressions;
using System.Reflection;

namespace FenySoft.Core.Data
{
    public class DataEqualityComparer : IEqualityComparer<ITData>
    {
        public readonly Func<ITData, ITData, bool> equals;
        public readonly Func<ITData, int> getHashCode;

        public readonly Type Type;
        public readonly Func<Type, MemberInfo, int> MembersOrder;
        public readonly CompareOption[] CompareOptions;

        public DataEqualityComparer(Type type, CompareOption[] compareOptions, Func<Type, MemberInfo, int> membersOrder = null)
        {
            Type = type;
            CompareOption.CheckCompareOptions(type, compareOptions, membersOrder);
            CompareOptions = compareOptions;
            MembersOrder = membersOrder;

            equals = CreateEqualsMethod().Compile();
            getHashCode = CreateGetHashCodeMethod().Compile();
        }

        public DataEqualityComparer(Type type, Func<Type, MemberInfo, int> membersOrder = null)
            : this(type, CompareOption.GetDefaultCompareOptions(type, membersOrder), membersOrder)
        {
        }

        public Expression<Func<ITData, ITData, bool>> CreateEqualsMethod()
        {
            var x = Expression.Parameter(typeof(ITData));
            var y = Expression.Parameter(typeof(ITData));
            var xValue = Expression.Variable(Type);
            var yValue = Expression.Variable(Type);

            var dataType = typeof(Data<>).MakeGenericType(Type);

            var body = Expression.Block(typeof(bool), new ParameterExpression[] { xValue, yValue },
                    Expression.Assign(xValue, Expression.Convert(x, dataType).Value()),
                    Expression.Assign(yValue, Expression.Convert(y, dataType).Value()),
                    EqualityComparerHelper.CreateEqualsBody(xValue, yValue, CompareOptions, MembersOrder)
                );
            var lambda = Expression.Lambda<Func<ITData, ITData, bool>>(body, x, y);

            return lambda;
        }

        public Expression<Func<ITData, int>> CreateGetHashCodeMethod()
        {
            var obj = Expression.Parameter(typeof(ITData));
            var objValue = Expression.Variable(Type);

            var dataType = typeof(Data<>).MakeGenericType(Type);

            var body = Expression.Block(typeof(int), new ParameterExpression[] { objValue },
                Expression.Assign(objValue, Expression.Convert(obj, dataType).Value()),
                EqualityComparerHelper.CreateGetHashCodeBody(objValue, MembersOrder)
                );
            var lambda = Expression.Lambda<Func<ITData, int>>(body, obj);

            return lambda;
        }

        public bool Equals(ITData x, ITData y)
        {
            return equals(x, y);
        }

        public int GetHashCode(ITData obj)
        {
            return getHashCode(obj);
        }
    }
}
