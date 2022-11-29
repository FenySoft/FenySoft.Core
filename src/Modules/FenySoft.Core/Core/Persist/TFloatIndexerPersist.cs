﻿using FenySoft.Core.Compression;
using FenySoft.Core.Mathematics;

namespace FenySoft.Core.Persist
{
    public class TSingleIndexerPersist : ITIndexerPersist<Single>
    {
        public const byte VERSION = 40;

        private int GetMaxDigits(Func<int, float> values, int count)
        {
            int maxDigits = 0;
            for (int i = 0; i < count; i++)
            {
                float value = values(i);
                int digits = TMathUtils.GetDigits(value);
                if (digits < 0)
                    return -1;

                if (digits > maxDigits)
                    maxDigits = digits;
            }

            return maxDigits;
        }

        public void Store(BinaryWriter writer, Func<int, float> values, int count)
        {
            writer.Write(VERSION);

            TDeltaCompression.Helper helper = null;
            long[] array = null;
            int digits;

            try
            {
                digits = GetMaxDigits(values, count);
                if (digits >= 0)
                {
                    helper = new TDeltaCompression.Helper();
                    array = new long[count];

                    double koef = Math.Pow(10, digits);
                    for (int i = 0; i < count; i++)
                    {
                        float value = values(i);
                        long v = checked((long)Math.Round(value * koef));

                        array[i] = v;
                        helper.Add(v);
                    }
                }
            }
            catch (OverflowException)
            {
                digits = -1;
            }

            writer.Write((sbyte)digits);
            if (digits >= 0)
                TDeltaCompression.Compress(writer, array, 0, count, helper);
            else
            {
                for (int i = 0; i < count; i++)
                    writer.Write(values(i));
            }
        }

        public void Load(BinaryReader reader, Action<int, float> values, int count)
        {
            if (reader.ReadByte() != VERSION)
                throw new Exception("Invalid TSingleIndexerPersist version.");

            int digits = reader.ReadSByte();

            if (digits >= 0)
            {
                double koef = Math.Pow(10, digits);
                TDeltaCompression.Decompress(reader, (idx, val) => values(idx, (float)Math.Round(val / koef, digits)), count);
            }
            else //native read
            {
                for (int i = 0; i < count; i++)
                    values(i, reader.ReadSingle());
            }
        }
    }

    public class TDoubleIndexerPersist : ITIndexerPersist<Double>
    {
        public const byte VERSION = 40;

        private int GetMaxDigits(Func<int, double> values, int count)
        {
            int maxDigits = 0;
            for (int i = 0; i < count; i++)
            {
                double value = values(i);
                int digits = TMathUtils.GetDigits(value);
                if (digits < 0)
                    return -1;

                if (digits > maxDigits)
                    maxDigits = digits;
            }

            return maxDigits;
        }

        public void Store(BinaryWriter writer, Func<int, double> values, int count)
        {
            writer.Write(VERSION);
            
            TDeltaCompression.Helper helper = null;
            long[] array = null;
            int digits;

            try
            {
                digits = GetMaxDigits(values, count);
                if (digits >= 0)
                {
                    helper = new TDeltaCompression.Helper();
                    array = new long[count];

                    double koef = Math.Pow(10, digits);
                    for (int i = 0; i < count; i++)
                    {
                        double value = values(i);
                        long v = checked((long)Math.Round(value * koef));

                        array[i] = v;
                        helper.Add(v);
                    }
                }
            }
            catch (OverflowException)
            {
                digits = -1;
            }

            writer.Write((sbyte)digits);
            if (digits >= 0)
                TDeltaCompression.Compress(writer, array, 0, count, helper);
            else
            {
                for (int i = 0; i < count; i++)
                    writer.Write(values(i));
            }
        }

        public void Load(BinaryReader reader, Action<int, double> values, int count)
        {
            if (reader.ReadByte() != VERSION)
                throw new Exception("Invalid TDoubleIndexerPersist version.");
            
            int digits = reader.ReadSByte();
            if (digits >= 0)
            {
                double koef = Math.Pow(10, digits);
                TDeltaCompression.Decompress(reader, (idx, val) => values(idx, (double)Math.Round(val / koef, digits)), count);
            }
            else //native read
            {
                for (int i = 0; i < count; i++)
                    values(i, reader.ReadDouble());
            }
        }
    }

    public class TDecimalIndexerPersist : ITIndexerPersist<Decimal>
    {
        public const byte VERSION = 40;

        private int GetMaxDigits(Func<int, decimal> values, int count)
        {
            int maxDigits = 0;
            for (int i = 0; i < count; i++)
            {
                decimal value = values(i);
                int digits = TMathUtils.GetDigits(value);
                if (digits > maxDigits)
                    maxDigits = digits;
            }

            return maxDigits;
        }

        #region ITIndexerPersist<decimal> Members

        public void Store(BinaryWriter writer, Func<int, decimal> values, int count)
        {
            writer.Write(VERSION);

            TDeltaCompression.Helper helper = null;
            long[] array = null;
            int digits;

            try
            {
                digits = GetMaxDigits(values, count);
                if (digits <= 15)
                {
                    helper = new TDeltaCompression.Helper();
                    array = new long[count];

                    decimal koef = (decimal)Math.Pow(10, digits);
                    for (int i = 0; i < count; i++)
                    {
                        decimal value = values(i);
                        long v = checked((long)Math.Round(value * koef));

                        array[i] = v;
                        helper.Add(v);
                    }
                }
                else
                    digits = -1;
            }
            catch (OverflowException)
            {
                digits = -1;
            }

            writer.Write((sbyte)digits);
            if (digits >= 0)
                TDeltaCompression.Compress(writer, array, 0, count, helper);
            else
            {
                for (int i = 0; i < count; i++)
                    writer.Write(values(i));
            }
        }

        public void Load(BinaryReader reader, Action<int, decimal> values, int count)
        {
            if (reader.ReadByte() != VERSION)
                throw new Exception("Invalid TDecimalIndexerPersist version.");

            int digits = reader.ReadSByte();

            if (digits >= 0)
            {
                double koef = Math.Pow(10, digits);
                TDeltaCompression.Decompress(reader, (idx, val) => values(idx, (decimal)Math.Round(val / koef, digits)), count);
            }
            else //native read
            {
                for (int i = 0; i < count; i++)
                    values(i, reader.ReadDecimal());
            }
        }
        
        #endregion
    }
}
