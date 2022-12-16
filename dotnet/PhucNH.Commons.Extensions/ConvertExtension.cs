using System;

namespace PhucNH.Commons.Extensions
{
    public static class ConvertExtension
    {
        public static ulong ToULong(this object obj)
        {
            try
            {
                return Convert.ToUInt64(obj);
            }
            catch
            {
                return ulong.MinValue;
            }
        }

        public static long ToLong(this object obj)
        {
            try
            {
                return Convert.ToInt64(obj);
            }
            catch
            {
                return long.MinValue;
            }
        }

        public static uint ToUInt(this object obj)
        {
            try
            {
                return Convert.ToUInt32(obj);
            }
            catch
            {
                return uint.MinValue;
            }
        }

        public static int ToInt(this object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return int.MinValue;
            }
        }

        public static ushort ToUShort(this object obj)
        {
            try
            {
                return Convert.ToUInt16(obj);
            }
            catch
            {
                return ushort.MinValue;
            }
        }

        public static short ToShort(this object obj)
        {
            try
            {
                return Convert.ToInt16(obj);
            }
            catch
            {
                return short.MinValue;
            }
        }

        public static byte ToByte(this object obj)
        {
            try
            {
                return Convert.ToByte(obj);
            }
            catch
            {
                return byte.MinValue;
            }
        }

        public static sbyte ToSbyte(this object obj)
        {
            try
            {
                return Convert.ToSByte(obj);
            }
            catch
            {
                return sbyte.MinValue;
            }
        }

        public static bool ToBoolean(this object obj)
        {
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return false;
            }
        }
    }
}