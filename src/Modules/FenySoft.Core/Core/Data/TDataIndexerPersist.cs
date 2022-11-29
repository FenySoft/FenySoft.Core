using FenySoft.Core.Persist;

using System.Linq.Expressions;
using System.Reflection;

using FenySoft.Core.Extensions;

namespace FenySoft.Core.Data
{
    public class TDataIndexerPersist : ITIndexerPersist<ITData>
    {
        public readonly Action<BinaryWriter, Func<int, ITData>, int> store;
        public readonly Action<BinaryReader, Action<int, ITData>, int> load;

        public readonly Type Type;
        public readonly IIndexerPersist[] Persists;
        public readonly Func<Type, MemberInfo, int> MembersOrder;

        public TDataIndexerPersist(Type type, IIndexerPersist[] persists, Func<Type, MemberInfo, int> membersOrder = null)
        {
            Type = type;
            Persists = persists;
            MembersOrder = membersOrder;

            store = CreateStoreMethod().Compile();
            load = CreateLoadMethod().Compile();
        }

        public TDataIndexerPersist(Type T, Func<Type, MemberInfo, int> membersOrder = null)
            : this(T, IndexerPersistHelper.GetDefaultPersists(T, membersOrder), membersOrder)
        {
        }

        public Expression<Action<BinaryWriter, Func<int, ITData>, int>> CreateStoreMethod()
        {
            var writer = Expression.Parameter(typeof(BinaryWriter), "writer");
            var values = Expression.Parameter(typeof(Func<int, ITData>), "values");
            var count = Expression.Parameter(typeof(int), "count");

            var idx = Expression.Variable(typeof(int), "idx");
            var callValues = Expression.Convert(Expression.Call(values, values.Type.GetMethod("Invoke"), idx), typeof(TData<>).MakeGenericType(Type)).Value();

            var body = IndexerPersistHelper.CreateStoreBody(Type, Persists, writer, callValues, idx, count, MembersOrder);
            var lambda = Expression.Lambda<Action<BinaryWriter, Func<int, ITData>, int>>(body, new ParameterExpression[] { writer, values, count });

            return lambda;
        }

        public Expression<Action<BinaryReader, Action<int, ITData>, int>> CreateLoadMethod()
        {
            var reader = Expression.Parameter(typeof(BinaryReader), "reader");
            var values = Expression.Parameter(typeof(Action<int, ITData>), "func");
            var count = Expression.Parameter(typeof(int), "count");

            var array = Expression.Variable(typeof(TData<>).MakeGenericType(Type).MakeArrayType());

            var body = TDataType.IsPrimitiveType(Type) ?
                    IndexerPersistHelper.SingleSlotCreateLoadBody(Type, true, values, reader, count, Persists) :
                    Expression.Block(new ParameterExpression[] { array },
                        Expression.Assign(array, Expression.New(array.Type.GetConstructor(new Type[] { typeof(int) }), count)),
                        array.For(i =>
                        {
                            return Expression.Block(Expression.Assign(Expression.ArrayAccess(array, i), Expression.New(typeof(TData<>).MakeGenericType(Type).GetConstructor(new Type[] { }))),
                                  Expression.Assign(Expression.ArrayAccess(array, i).Value(), Expression.New(Type.GetConstructor(new Type[] { }))),
                                    Expression.Call(values, values.Type.GetMethod("Invoke"), i, Expression.ArrayAccess(array, i)));
                        }, Expression.Label(), count),
                        IndexerPersistHelper.CreateLoadBody(Type, true, reader, array, count, MembersOrder, Persists)
                    );

            return Expression.Lambda<Action<BinaryReader, Action<int, ITData>, int>>(body, new ParameterExpression[] { reader, values, count });
        }

        public void Store(BinaryWriter writer, Func<int, ITData> values, int count)
        {
            store(writer, values, count);
        }

        public void Load(BinaryReader reader, Action<int, ITData> values, int count)
        {
            load(reader, values, count);
        }

        #region Examples

        //public class Tick
        //{
        //    public string Symbol { get; set; }
        //    public DateTime Timestamp { get; set; }
        //    public double Bid { get; set; }
        //    public double Ask { get; set; }
        //    public long Volume { get; set; }
        //    public string Provider { get; set; }
        //}

        //public class TickIndexerPersist : ITIndexerPersist<ITData>
        //{
        //    public Type Type { get; private set; }
        //    public ITIndexerPersist[] Persists { get; private set; }

        //    public readonly Func<Type, MemberInfo, int> MembersOrder;

        //    public TickIndexerPersist(Type type, ITIndexerPersist[] persist, Func<Type, MemberInfo, int> membersOrder = null)
        //    {
        //        Persists = persist;
        //        Type = type;
        //        MembersOrder = membersOrder;
        //    }

        //    public void Store(BinaryWriter writer, Func<int, ITData> values, int count)
        //    {
        //        Action[] actions = new Action[6];
        //        MemoryStream[] streams = new MemoryStream[6];

        //        actions[0] = () =>
        //        {
        //            streams[0] = new MemoryStream();
        //            ((TStringIndexerPersist)Persists[0]).Store(new BinaryWriter(streams[0]), (idx) => ((Data2<Tick>)values.Invoke(idx)).Value.Symbol, count);
        //        };

        //        actions[1] = () =>
        //        {
        //            streams[1] = new MemoryStream();
        //            ((TDateTimeIndexerPersist)Persists[1]).Store(new BinaryWriter(streams[1]), (idx) => ((Data2<Tick>)values.Invoke(idx)).Value.Timestamp, count);
        //        };

        //        actions[2] = () =>
        //        {
        //            streams[2] = new MemoryStream();
        //            ((TDoubleIndexerPersist)Persists[2]).Store(new BinaryWriter(streams[2]), (idx) => ((Data2<Tick>)values.Invoke(idx)).Value.Ask, count);
        //        };

        //        actions[3] = () =>
        //        {
        //            streams[3] = new MemoryStream();
        //            ((TDoubleIndexerPersist)Persists[3]).Store(new BinaryWriter(streams[3]), (idx) => ((Data2<Tick>)values.Invoke(idx)).Value.Bid, count);
        //        };

        //        actions[4] = () =>
        //        {
        //            streams[4] = new MemoryStream();
        //            ((TInt64IndexerPersist)Persists[4]).Store(new BinaryWriter(streams[4]), (idx) => ((Data2<Tick>)values.Invoke(idx)).Value.Volume, count);
        //        };

        //        actions[5] = () =>
        //        {
        //            streams[5] = new MemoryStream();
        //            ((TStringIndexerPersist)Persists[5]).Store(new BinaryWriter(streams[5]), (idx) => ((Data2<Tick>)values.Invoke(idx)).Value.Provider, count);
        //        };

        //        Parallel.Invoke(actions);

        //        for (int i = 0; i < actions.Length; i++)
        //        {
        //            var stream = streams[i];
        //            using (stream)
        //            {
        //                TCountCompression.Serialize(writer, (ulong)stream.Length);
        //                writer.Write(stream.GetBuffer(), 0, (int)stream.Length);
        //            }
        //        }
        //    }

        //    public void Load(BinaryReader reader, Action<int, ITData> values, int count)
        //    {
        //        Data2<Tick>[] array = new Data2<Tick>[count];
        //        for (int i = 0; i < count; i++)
        //        {
        //            var item = new Data2<Tick>();
        //            item.Value = new Tick();
        //            array[i] = item;
        //            values(i, item);
        //        }

        //        Action[] actions = new Action[6];
        //        byte[][] buffers = new byte[6][];

        //        for (int i = 0; i < 6; i++)
        //            buffers[i] = reader.ReadBytes((int)TCountCompression.Deserialize(reader));

        //        actions[0] = () =>
        //        {
        //            using (MemoryStream ms = new MemoryStream(buffers[0]))
        //                ((ITIndexerPersist<String>)Persists[0]).Load(new BinaryReader(ms), (idx, value) => { ((Data2<Tick>)array[idx]).Value.Symbol = value; }, count);
        //        };

        //        actions[1] = () =>
        //        {
        //            using (MemoryStream ms = new MemoryStream(buffers[1]))
        //                ((ITIndexerPersist<DateTime>)Persists[1]).Load(new BinaryReader(ms), (idx, value) => { ((Data2<Tick>)array[idx]).Value.Timestamp = value; }, count);
        //        };

        //        actions[2] = () =>
        //        {
        //            using (MemoryStream ms = new MemoryStream(buffers[2]))
        //                ((ITIndexerPersist<Double>)Persists[2]).Load(new BinaryReader(ms), (idx, value) => { ((Data2<Tick>)array[idx]).Value.Bid = value; }, count);
        //        };

        //        actions[3] = () =>
        //        {
        //            using (MemoryStream ms = new MemoryStream(buffers[3]))
        //                ((ITIndexerPersist<Double>)Persists[3]).Load(new BinaryReader(ms), (idx, value) => { ((Data2<Tick>)array[idx]).Value.Ask = value; }, count);
        //        };

        //        actions[4] = () =>
        //        {
        //            using (MemoryStream ms = new MemoryStream(buffers[4]))
        //                ((ITIndexerPersist<Int64>)Persists[4]).Load(new BinaryReader(ms), (idx, value) => { ((Data2<Tick>)array[idx]).Value.Volume = value; }, count);
        //        };

        //        actions[5] = () =>
        //        {
        //            using (MemoryStream ms = new MemoryStream(buffers[5]))
        //                ((ITIndexerPersist<String>)Persists[5]).Load(new BinaryReader(ms), (idx, value) => { ((Data2<Tick>)array[idx]).Value.Provider = value; }, count);
        //        };

        //        Parallel.Invoke(actions);
        //    }
        //}
        #endregion

    }
}
