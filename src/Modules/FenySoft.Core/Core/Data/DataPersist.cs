using FenySoft.Core.Persist;

using System.Linq.Expressions;
using System.Reflection;

namespace FenySoft.Core.Data
{
    public class DataPersist : IPersist<ITData>
    {
        public readonly Action<BinaryWriter, ITData> write;
        public readonly Func<BinaryReader, ITData> read;

        public readonly Type Type;
        public readonly Func<Type, MemberInfo, int> MembersOrder;
        public readonly AllowNull AllowNull;

        public DataPersist(Type type, Func<Type, MemberInfo, int> membersOrder = null, AllowNull allowNull = AllowNull.None)
        {
            Type = type;
            MembersOrder = membersOrder;
            AllowNull = allowNull;

            write = CreateWriteMethod().Compile();
            read = CreateReadMethod().Compile();
        }

        public void Write(BinaryWriter writer, ITData item)
        {
            write(writer, item);
        }

        public ITData Read(BinaryReader reader)
        {
            return read(reader);
        }

        public Expression<Action<BinaryWriter, ITData>> CreateWriteMethod()
        {
            var writer = Expression.Parameter(typeof(BinaryWriter), "writer");
            var idata = Expression.Parameter(typeof(ITData), "idata");

            var dataType = typeof(Data<>).MakeGenericType(Type);
            var dataValue = Expression.Variable(Type, "dataValue");

            var assign = Expression.Assign(dataValue, Expression.Convert(idata, dataType).Value());

            return Expression.Lambda<Action<BinaryWriter, ITData>>(Expression.Block(new ParameterExpression[] { dataValue }, assign, PersistHelper.CreateWriteBody(dataValue, writer, MembersOrder, AllowNull)), writer, idata);
        }

        public Expression<Func<BinaryReader, ITData>> CreateReadMethod()
        {
            var reader = Expression.Parameter(typeof(BinaryReader), "reader");

            var dataType = typeof(Data<>).MakeGenericType(Type);

            return Expression.Lambda<Func<BinaryReader, ITData>>(
                    Expression.Label(Expression.Label(dataType), Expression.New(dataType.GetConstructor(new Type[] { Type }), PersistHelper.CreateReadBody(reader, Type, MembersOrder, AllowNull))),
                    reader
                );
        }
    }
}