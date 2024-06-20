using Kvasir.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace UT.Kvasir.Translation {
    internal static class TestConverters {
        public class AllCaps : IDataConverter<string, string> {
            public string Convert(string source) { return source.ToUpper(); }
            public string Revert(string result) { return new CultureInfo("en-US", false).TextInfo.ToTitleCase(result); }
        }
        public class ChangeBase : IDataConverter<int, int> {
            public ChangeBase(int _) {}
            public int Convert(int source) { return source; }
            public int Revert(int result) { return result; }
        }
        public class DeNullify<T> : IDataConverter<T?, T> where T : notnull {
            public T Convert(T? source) { return source!; }
            public T? Revert(T result) { return result; }
        }
        public class Enumify<T, E> : IDataConverter<T, E> where T : notnull where E : Enum {
            public E Convert(T source) { return (E)new EnumToNumericConverter(typeof(E)).ConverterImpl.Revert(source)!; }
            public T Revert(E result) { return (T)new EnumToNumericConverter(typeof(E)).ConverterImpl.Convert(result)!; }
        }
        public class Identity<T> : IDataConverter<T, T> {
            public T Convert(T source) { return source; }
            public T Revert(T result) { return result; }
        }
        public class Invert : IDataConverter<bool, bool> {
            public bool Convert(bool source) { return !source; }
            public bool Revert(bool result) { return !result; }
        }
        public class MakeDate<T> : IDataConverter<T, DateTime> where T : notnull {
            public MakeDate() {
                conversions_ = new Dictionary<T, DateTime>();
                reversions_ = new Dictionary<DateTime, T>();
                lastDate_ = new DateTime(2000, 1, 1);
            }
            public DateTime Convert(T source) {
                conversions_.TryAdd(source, lastDate_);
                lastDate_ = lastDate_.AddDays(1);
                return conversions_[source];
            }
            public T Revert(DateTime result) {
                return reversions_[result];
            }


            public DateTime lastDate_;
            public readonly Dictionary<T, DateTime> conversions_;
            public readonly Dictionary<DateTime, T> reversions_;
        }
        public class Nullify<T> : IDataConverter<T, T?> where T : notnull {
            public T? Convert(T source) { return source; }
            public T Revert(T? result) { return result!; }
        }
        public class RoundDown : IDataConverter<double, ulong> {
            public ulong Convert(double source) { return (ulong)source; }
            public double Revert(ulong result) { return (double)result + 0.25; }
        }
        public class SwapEnums<A, B> : IDataConverter<A, B> where A : notnull, Enum where B : Enum {
            public B Convert(A source) { return (B)System.Convert.ChangeType(source, typeof(int)); }
            public A Revert(B result) { return (A)System.Convert.ChangeType(result, typeof(int)); }
        }
        public class ToError<T> : IDataConverter<T?, Exception?> {
            public Exception? Convert(T? source) { return default; }
            public T? Revert(Exception? result) { return default; }
        }
        public class ToInt<T> : IDataConverter<T, int> {
            public int Convert(T source) { return System.Convert.ToInt32(source); }
            public T Revert(int result) { return (T)System.Convert.ChangeType(result, typeof(T)); }
        }
        public class ToString<T> : IDataConverter<T, string> where T : notnull {
            public string Convert(T source) { return source.ToString()!; }
            public T Revert(string result) { return (T)System.Convert.ChangeType(result, typeof(T)); }
        }
        public class Unconstructible<T> : IDataConverter<T, T> {
            public Unconstructible() { throw new InvalidOperationException(CANNOT_CONSTRUCT_MSG); }
            public T Convert(T source) { return source; }
            public T Revert(T result) { return result; }
        }
        public class Unconvertible<T> : IDataConverter<T, T> {
            public T Convert(T source) { throw new InvalidOperationException(CANNOT_CONVERT_MSG); }
            public T Revert(T result) { return result; }
        }


        public static readonly string CANNOT_CONSTRUCT_MSG = "This converter type is not constructible";
        public static readonly string CANNOT_CONVERT_MSG = "This value cannot be converted";
    }
}
