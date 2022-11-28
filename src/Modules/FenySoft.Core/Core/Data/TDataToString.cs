using System.Linq.Expressions;
using System.Reflection;

namespace FenySoft.Core.Data
{
    public class TDataToString : ITToString<ITData>
    {
        public readonly Func<ITData, string> to;
        public readonly Func<string, ITData> from;

        public readonly Type Type;
        public readonly int StringBuilderCapacity;
        public readonly IFormatProvider[] Providers;
        public readonly char[] Delimiters;
        public readonly Func<Type, MemberInfo, int> MembersOrder;

        public TDataToString(Type type, int stringBuilderCapacity, IFormatProvider[] providers, char[] delimiters, Func<Type, MemberInfo, int> membersOrder = null)
        {
            Type = type;
            StringBuilderCapacity = stringBuilderCapacity;
            var typeCount = TDataType.IsPrimitiveType(type) ? 1 : TDataTypeUtils.GetPublicMembers(type, membersOrder).Count();
            if (providers.Length != typeCount)
                throw new ArgumentException("providers.Length != dataType.Length");

            Providers = providers;
            Delimiters = delimiters;
            MembersOrder = membersOrder;

            to = CreateToMethod().Compile();
            from = CreateFromMethod().Compile();
        }

        public TDataToString(Type type, int stringBuilderCapacity, char[] delimiters, Func<Type, MemberInfo, int> membersOrder = null)
            : this(type, stringBuilderCapacity, ValueToStringHelper.GetDefaultProviders(type, membersOrder), delimiters, membersOrder)
        {
        }

        public TDataToString(Type type, Func<Type, MemberInfo, int> membersOrder = null)
            : this(type, 16, new char[] { ';' }, membersOrder)
        {
        }

        public Expression<Func<ITData, string>> CreateToMethod()
        {
            var data = Expression.Parameter(typeof(ITData), "data");
            var d = Expression.Variable(typeof(TData<>).MakeGenericType(Type), "d");

            List<Expression> list = new List<Expression>();
            list.Add(Expression.Assign(d, Expression.Convert(data, typeof(TData<>).MakeGenericType(Type))));
            list.Add(ValueToStringHelper.CreateToStringBody(d.Value(), StringBuilderCapacity, Providers, Delimiters[0], MembersOrder));

            var body = Expression.Block(new ParameterExpression[] { d }, list);

            return Expression.Lambda<Func<ITData, string>>(body, data);
        }

        public Expression<Func<string, ITData>> CreateFromMethod()
        {
            var stringParam = Expression.Parameter(typeof(string), "item");
            List<Expression> list = new List<Expression>();

            var data = Expression.Variable(typeof(TData<>).MakeGenericType(Type), "d");

            list.Add(Expression.Assign(data, Expression.New(data.Type.GetConstructor(new Type[] { }))));

            if (!TDataType.IsPrimitiveType(Type))
                list.Add(Expression.Assign(data.Value(), Expression.New(Type.GetConstructor(new Type[] { }))));

            list.Add(ValueToStringHelper.CreateParseBody(data.Value(), stringParam, Providers, Delimiters, MembersOrder));
            list.Add(Expression.Label(Expression.Label(typeof(TData<>).MakeGenericType(Type)), data));

            var body = Expression.Block(new ParameterExpression[] { data }, list);

            return Expression.Lambda<Func<string, ITData>>(body, new ParameterExpression[] { stringParam });
        }

        public string To(ITData value1)
        {
            return to(value1);
        }

        public ITData From(string value2)
        {
            return from(value2);
        }
    }
}
