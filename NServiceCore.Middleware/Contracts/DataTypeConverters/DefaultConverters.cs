using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceCore.Middleware.Contracts.DataTypeConverters
{
    public static class DefaultConverters
    {
        public delegate object StringConversionDelegate(string str);

        public delegate TConverted StringConversionDelegate<TConverted>(string str) where TConverted : struct;

        internal static object StringConverter(string str)
        {
            return str;
        }

        #region Integral Types

        internal static object SByteConverter(string str)
        {
            return sbyte.Parse(str);
        }

        internal static object ShortConverter(string str)
        {
            return short.Parse(str);
        }

        internal static object IntConverter(string str)
        {
            return int.Parse(str);
        }

        internal static object LongConverter(string str)
        {
            return long.Parse(str);
        }

        internal static object ByteConverter(string str)
        {
            return byte.Parse(str);
        }

        internal static object UShortConverter(string str)
        {
            return ushort.Parse(str);
        }

        internal static object UIntConverter(string str)
        {
            return uint.Parse(str);
        }

        internal static object ULongConverter(string str)
        {
            return ulong.Parse(str);
        }

        #endregion Integral Types

        internal static object FloatConverter(string str)
        {
            return float.Parse(str);
        }

        internal static object DoubleFloatConverter(string str)
        {
            return double.Parse(str);
        }

        internal static object BoolConverter(string str)
        {
            string[] BooleanTrueStrings = { bool.TrueString, "1" };
            string[] BooleanFalseStrings = { bool.FalseString, "0" };

            if (string.IsNullOrEmpty(str))
                throw new FormatException($"String \"{str}\" does not convert into a boolean.");

            if (BooleanTrueStrings.Contains(str, StringComparer.InvariantCultureIgnoreCase))
                return true;
            if (BooleanFalseStrings.Contains(str, StringComparer.InvariantCultureIgnoreCase))
                return false;

            throw new FormatException($"String \"{str}\" does not convert into a boolean.");
        }

        internal static T EnumConverter<T>(string str) where T : struct
        {
            if(Enum.TryParse(str, true, out T result))
            {
                return result;
            }
            throw new FormatException($"String \"{str}\" does not convert into Enum of type {typeof(T).Name}.");
        }
    }
}
