using System.Runtime.InteropServices;

namespace FenySoft.Core.Comparers
{
    [StructLayout(LayoutKind.Explicit)]
    public struct TCommonArray
    {
        [FieldOffset(0)]
        public byte[] ByteArray;

        [FieldOffset(0)]
        public short[] Int16Array;

        [FieldOffset(0)]
        public ushort[] UInt16Array;

        [FieldOffset(0)]
        public int[] Int32Array;

        [FieldOffset(0)]
        public uint[] UInt32Array;

        [FieldOffset(0)]
        public long[] Int64Array;

        [FieldOffset(0)]
        public ulong[] UInt64Array;

        [FieldOffset(0)]
        public float[] SingleArray;

        [FieldOffset(0)]
        public double[] DoubleArray;
    }
}
