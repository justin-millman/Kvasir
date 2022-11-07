using Kvasir.Core;
using System;

namespace UT.Kvasir.Translation {
    internal static partial class TestComponents {
        public class BoolToInt : IDataConverter<bool, int> {
            public int Convert(bool source) { return source ? 1 : 0; }
            public bool Revert(int result) { return result != 0; }
        }
        public class ByteModulo16 : IDataConverter<byte?, byte?> {
            public byte? Convert(byte? source) { return source is null ? null : (byte)(source % 16); }
            public byte? Revert(byte? result) { return result; }
        }
        public class CharToInt : IDataConverter<char, int> {
            public int Convert(char source) { return source; }
            public char Revert(int result) { return (char)result; }
        }
        public class DateToError : IDataConverter<DateTime, ArgumentException> {
            public ArgumentException Convert(DateTime source) { return new ArgumentException(source.ToString()); }
            public DateTime Revert(ArgumentException result) { return DateTime.Parse(result.Message); }
        }
        public class DeNullify<T> : IDataConverter<T?, T> where T : notnull {
            public T Convert(T? source) { return source!; }
            public T? Revert(T result) { return result; }
        }
        public class Identity<T> : IDataConverter<T, T> {
            public T Convert(T source) { return source; }
            public T Revert(T result) { return result; }
        }
        public class InvertBoolean : IDataConverter<bool, bool> {
            public bool Convert(bool source) { return !source; }
            public bool Revert(bool result) { return !result; }
        }
        public class Nullify<T> : IDataConverter<T, T?> where T : notnull {
            public T? Convert(T source) { return source; }
            public T Revert(T? source) { return source ?? default!; }
        }
        public class RoundDownDouble : IDataConverter<double, int> {
            public int Convert(double source) { return (int)source; }
            public double Revert(int result) { return result + 0.5; }
        }
    }
}
