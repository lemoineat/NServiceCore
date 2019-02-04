using System;
using System.Collections.Generic;
using System.Text;
using static NServiceCore.Middleware.Contracts.DataTypeConverters.DefaultConverters;

namespace NServiceCore.Middleware.Contracts.DataTypeConverters
{
    public class ConverterRegistrar
    {
        private Dictionary<Type, StringConversionDelegate> Delegates;

        public ConverterRegistrar()
        {
            Delegates = new Dictionary<Type, StringConversionDelegate>();
            RegisterDefaultConveters();
        }

        private void RegisterDefaultConveters()
        {
            Delegates[typeof(string)] = StringConverter;
            Delegates[typeof(sbyte)] = SByteConverter;
            Delegates[typeof(short)] = ShortConverter;
            Delegates[typeof(int)] = IntConverter;
            Delegates[typeof(long)] = LongConverter;
            Delegates[typeof(byte)] = ByteConverter;
            Delegates[typeof(ushort)] = UShortConverter;
            Delegates[typeof(uint)] = UIntConverter;
            Delegates[typeof(ulong)] = ULongConverter;
            Delegates[typeof(float)] = FloatConverter;
            Delegates[typeof(double)] = DoubleFloatConverter;
            Delegates[typeof(bool)] = BoolConverter;
        }

        public StringConversionDelegate GetConverter(Type t)
        {
            if (Delegates.ContainsKey(t))
            {
                return Delegates[t];
            }
            throw new NotSupportedException($"No converter exists for type {t.Name}");
        }

        public void AddRegistration<T>(StringConversionDelegate converter)
        {
            AddRegistration(typeof(T), converter);
        }

        public void AddRegistration(Type t, StringConversionDelegate converter)
        {
            Delegates[t] = converter;
        }
    }
}
