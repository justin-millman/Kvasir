﻿using Kvasir.Core;
using System;

namespace UT.Kvasir.Translation {
    internal static class TestConverters {
        public class ChangeBase : IDataConverter<int, int> {
            public ChangeBase(int _) {}
            public int Convert(int source) { return source; }
            public int Revert(int result) { return result; }
        }
        public class DeNullify<T> : IDataConverter<T?, T> where T : notnull {
            public T Convert(T? source) { return source!; }
            public T? Revert(T result) { return result; }
        }
        public class Invert : IDataConverter<bool, bool> {
            public bool Convert(bool source) { return !source; }
            public bool Revert(bool result) { return !result; }
        }
        public class Nullify<T> : IDataConverter<T, T?> where T : notnull {
            public T? Convert(T source) { return source; }
            public T Revert(T? result) { return result!; }
        }
        public class RoundDown : IDataConverter<double, long> {
            public long Convert(double source) { return (long)source; }
            public double Revert(long result) { return (double)result + 0.25; }
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


        public static readonly string CANNOT_CONSTRUCT_MSG = "This converter type is not constructible";
    }
}
