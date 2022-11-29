namespace FenySoft.Core.Persist
{
    public class TDateTimeIndexerPersist : ITIndexerPersist<DateTime>
    {
        public const byte VERSION = 40;
        
        private static readonly long MILLISECOND = 10000;
        private static readonly long SECOND = 1000 * MILLISECOND;
        private static readonly long MINUTE = 60 * SECOND;
        private static readonly long HOUR = 60 * MINUTE;
        private static readonly long DAY = 24 * HOUR;

        private readonly TInt64IndexerPersist persist = new TInt64IndexerPersist(new long[] { MILLISECOND, SECOND, MINUTE, HOUR, DAY });

        public void Store(BinaryWriter writer, Func<int, DateTime> values, int count)
        {
            writer.Write(VERSION);

            persist.Store(writer, (i) => { return values(i).Ticks; }, count);
        }

        public void Load(BinaryReader reader, Action<int, DateTime> values, int count)
        {
            if (reader.ReadByte() != VERSION)
                throw new Exception("Invalid TDateTimeIndexerPersist version.");

            persist.Load(reader, (i, v) => { values(i, new DateTime(v)); }, count);
        }
    }
}
