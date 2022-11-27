using System.Linq.Expressions;
using System.Reflection;

using FenySoft.Core.Extensions;

namespace FenySoft.Core.Data
{
    public class DataToObjects : IToObjects<ITData>
    {
        public readonly Func<ITData, object[]> to;
        public readonly Func<object[], ITData> from;

        public readonly Type Type;
        public readonly Func<Type, MemberInfo, int> MembersOrder;

        public DataToObjects(Type type, Func<Type, MemberInfo, int> membersOrder = null)
        {
            if (!TDataType.IsPrimitiveType(type) && !type.HasDefaultConstructor())
                throw new NotSupportedException("No default constructor.");

            bool isSupported = DataTypeUtils.IsAllPrimitive(type);
            if (!isSupported)
                throw new NotSupportedException("Not all types are primitive.");

            Type = type;
            MembersOrder = membersOrder;

            to = CreateToMethod().Compile();
            from = CreateFromMethod().Compile();
        }

        public Expression<Func<ITData, object[]>> CreateToMethod()
        {
            var data = Expression.Parameter(typeof(ITData), "data");

            var d = Expression.Variable(typeof(Data<>).MakeGenericType(Type), "d");
            var body = Expression.Block(new ParameterExpression[] { d }, Expression.Assign(d, Expression.Convert(data, d.Type)), ValueToObjectsHelper.ToObjects(d.Value(), MembersOrder));

            return Expression.Lambda<Func<ITData, object[]>>(body, data);
        }

        public Expression<Func<object[], ITData>> CreateFromMethod()
        {
            var objectArray = Expression.Parameter(typeof(object[]), "item");
            var data = Expression.Variable(typeof(Data<>).MakeGenericType(Type));

            List<Expression> list = new List<Expression>();
            list.Add(Expression.Assign(data, Expression.New(data.Type.GetConstructor(new Type[] { }))));

            if (!TDataType.IsPrimitiveType(Type))
                list.Add(Expression.Assign(data.Value(), Expression.New(data.Value().Type.GetConstructor(new Type[] { }))));

            list.Add(ValueToObjectsHelper.FromObjects(data.Value(), objectArray, MembersOrder));
            list.Add(Expression.Label(Expression.Label(typeof(ITData)), data));

            var body = Expression.Block(typeof(ITData), new ParameterExpression[] { data }, list);

            return Expression.Lambda<Func<object[], ITData>>(body, objectArray);
        }

        public object[] To(ITData value1)
        {
            return to(value1);
        }

        public ITData From(object[] value2)
        {
            return from(value2);
        }
    }
}
