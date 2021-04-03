using Ardalis.GuardClauses;
using Optional;
using Optional.Unsafe;
using System;

using ConvFn = System.Converter<object?, object?>;

namespace Kvasir.Core {
    /// <summary>
    ///   A utility for converting objects of one type into objects of another type, and possibly vice-versa.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A <see cref="DataConverter"/> represents a possibly reversible function that maps instance of one CLR type
    ///     into instances of another. This mapping can be relatively simple (i.e. formatting a string to be
    ///     <c>ALL CAPS</c>) or relatively complex (establishing a database connection from an user-defined structure).
    ///     This allows flexibility in the definition of the conversion mechanism without burdening the user on the
    ///     potential complexities that underlie that logic. The client is presented with a simple conversion interface
    ///     with which they can perform transformations.
    ///   </para>
    ///   <para>
    ///     A <see cref="DataConverter"/>, at a minimum, must support conversion of every instance of some "source
    ///     type" into instances of some "result type." This forward conversion can only fail when the source instance
    ///     is malformed or in some other invalid state: conversion of a valid source instance can never result in an
    ///     exception. Optionally, DataConverts can provide for converting <i>back</i> from instances of the "result"
    ///     type into instances of the "source" type; this is known as a "bidirectional" converter, and the specifics
    ///     of the reversal mechanism (e.g. what happens when two or more source instances map to the same result
    ///     instance, which is then reversed) are defined by the underlying conversion mechanic. Clients should thus be
    ///     careful when using the <see cref="Revert(object?)"/> API, as <c>Revert(Convert(X))</c> is not required to
    ///     yield <c>X</c>.
    ///   </para>
    ///   <para>
    ///     Both the conversion and reversion APIs operate in type-erased contexts. <see cref="DataConverter"/> will
    ///     throw a runtime exception if an input object does not match the expected type; the class further guarantees
    ///     that the returned value, though type-erased, can be safely cast.
    ///   </para>
    /// </remarks>
    public sealed class DataConverter {
        /// <summary>
        ///   The <see cref="Type"/> of the input objects in the forward conversion supported by this
        ///   <see cref="DataConverter"/>.
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        ///   The <see cref="Type"/> of the output objects in the forward conversion supported by this
        ///   <see cref="DataConverter"/>.
        /// </summary>
        public Type ResultType { get; }

        /// <summary>
        ///   Whether or not this <see cref="DataConverter"/> supports
        ///   <see cref="Revert(object?)">reverse conversion</see>.
        /// </summary>
        public bool IsBidirectional => revert_.HasValue;

        /// <summary>
        ///   Creates a new <see cref="DataConverter"/> that is not <see cref="IsBidirectional">bidirectional</see>.
        /// </summary>
        /// <typeparam name="TSource">
        ///   The <see cref="SourceType"/> of the new <see cref="DataConverter"/>.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///   The <see cref="ResultType"/> of the new <see cref="DataConverter"/>.
        /// </typeparam>
        /// <param name="convert">
        ///   The function by which the new <see cref="DataConverter"/> is to convert instances of
        ///   <see cref="SourceType"/> (i.e. <typeparamref name="TSource"/>) into instances of <see cref="ResultType"/>
        ///   (i.e. <typeparamref name="TResult"/>).
        /// </param>
        /// <returns>
        ///   A new unidirectional <see cref="DataConverter"/> that converts from instances of
        ///   <typeparamref name="TSource"/> into instances of <typeparamref name="TResult"/> using
        ///   <paramref name="convert"/>.
        /// </returns>
        public static DataConverter Create<TSource, TResult>(Converter<TSource, TResult> convert) {
            #pragma warning disable CS8604          // Possible null reference argument.
            ConvFn fwd = o => convert((TSource?)o);
            var bwd = Option.None<ConvFn>();
            #pragma warning restore CS8604          // Possible null reference argument.

            return new DataConverter(typeof(TSource), typeof(TResult), fwd, bwd);
        }

        /// <summary>
        ///   Creates a new <see cref="DataConverter"/> that is <see cref="IsBidirectional">bidirectional</see>.
        /// </summary>
        /// <typeparam name="TSource">
        ///   The <see cref="SourceType"/> of the new <see cref="DataConverter"/>.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///   The <see cref="ResultType"/> of the new <see cref="DataConverter"/>.
        /// </typeparam>
        /// <param name="convert">
        ///   The function by which the new <see cref="DataConverter"/> is to convert instances of
        ///   <see cref="SourceType"/> (i.e. <typeparamref name="TSource"/>) into instances of <see cref="ResultType"/>
        ///   (i.e. <typeparamref name="TResult"/>).
        /// </param>
        /// <param name="revert">
        ///   The function by which the new <see cref="DataConverter"/> is to convert instances of
        ///   <see cref="ResultType"/> (i.e. <typeparamref name="TResult"/>) into instances of <see cref="SourceType"/>
        ///   (i.e. <typeparamref name="TSource"/>).
        /// </param>
        /// <returns>
        ///   A new bidirectional <see cref="DataConverter"/> that converts from instances of
        ///   <typeparamref name="TSource"/> into instances of <typeparamref name="TResult"/> using
        ///   <paramref name="convert"/> and vice-versa using <paramref name="revert"/>.
	/// </returns>
        public static DataConverter Create<TSource, TResult>(Converter<TSource, TResult> convert,
            Converter<TResult, TSource> revert) {

            ConvFn fwd = o => o is null ? null : convert((TSource)o);
            var bwd = Option.Some<ConvFn>(o => o is null ? null : revert((TResult)o));

            return new DataConverter(typeof(TSource), typeof(TResult), fwd, bwd);
        }

        /// <summary>
        ///   Converts an instance of <see cref="SourceType"/> into an instance of <see cref="ResultType"/>.
        /// </summary>
        /// <param name="source">
        ///   The source object.
        /// </param>
        /// <returns>
        ///   The coversion of <paramref name="source"/> into an instance of <see cref="ResultType"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="source"/> is neither an instance of <see cref="SourceType"/> nor a derived class or an
        ///   implementation thereof
        ///     --or--
        ///   if <paramref name="source"/> cannot be converted.
        /// </exception>
        public object? Convert(object? source) {
            if (source is not null && !SourceType.IsInstanceOfType(source)) {
                var msg = $"Cannot convert object of type {source.GetType()}: expected {SourceType}";
                throw new ArgumentException(msg, nameof(source));
            }

            return convert_(source);
        }

        /// <summary>
        ///   Converts an instance of <see cref="ResultType"/> back into an instance of <see cref="SourceType"/>.
        /// </summary>
        /// <remarks>
        ///   Because the underlying conversion mechanism need not be bijective (that is, multiple different source
        ///   objects can convert into the same result object), the <see cref="Revert(object?)"/> API does not
        ///   necessarily present a true inversion. For a given result object <c>R</c>, the only guarantees are that
        ///   repeated calls to the API will yield the same source object and that <c>Convert(Revert(R)) == R</c>.
        /// </remarks>
        /// <param name="result">
        ///   The result object.
        /// </param>
        /// <returns>
        ///   The reversion of <paramref name="result"/> into an instance of <see cref="SourceType"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        ///   if this <see cref="DataConverter"/> is not <see cref="IsBidirectional">bidirectional</see>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="result"/> is neither an instance of <see cref="ResultType"/> nor a derived class or an
        ///   implementation thereof
        ///     --or--
        ///   if <paramref name="result"/> cannot be reverted.
        /// </exception>
        public object? Revert(object? result) {
            if (!IsBidirectional) {
                var msg = $"Reverse conversion is not supported by this {nameof(DataConverter)}";
                throw new NotSupportedException(msg);
            }
            else if (result is not null && !ResultType.IsInstanceOfType(result)) {
                var msg = $"Cannot revert object of type {result.GetType()}: expected {ResultType}";
                throw new ArgumentException(msg, nameof(result));
            }

            var fn = revert_.ValueOrFailure();
            return fn(result);
        }

        /// <summary>
        ///   Creates a new <see cref="DataConverter"/> that represents an bidirectional identity conversion.
        /// </summary>
        /// <typeparam name="T">
        ///   The type on which the identity converter is to operate.
        /// </typeparam>
        /// <returns>
        ///   A new bidirectional <see cref="DataConverter"/> that performs identity conversion.
        /// </returns>
        public static DataConverter Identity<T>() {
            return Create<T, T>(t => t, t => t);
        }

        /// <summary>
        ///   Constructs a new <see cref="DataConverter"/>.
        /// </summary>
        /// <param name="sourceType">
        ///   The <see cref="SourceType"/> of the new <see cref="DataConverter"/>.
        /// </param>
        /// <param name="resultType">
        ///   The <see cref="ResultType"/> of the new <see cref="DataConverter"/>.
        /// </param>
        /// <param name="fwd">
        ///   The function by which the new <see cref="DataConverter"/> is to convert instances of
        ///   <see cref="SourceType"/> into instances of <see cref="ResultType"/>.
        /// </param>
        /// <param name="bwd">
        ///   The function by which the new <see cref="DataConverter"/> is to convert instances of
        ///   <see cref="ResultType"/> into instances of <see cref="SourceType"/>, if such a function is supported.
        /// </param>
        private DataConverter(Type sourceType, Type resultType, ConvFn fwd, Option<ConvFn> bwd) {
            Guard.Against.Null(sourceType, nameof(sourceType));
            Guard.Against.Null(resultType, nameof(resultType));
            Guard.Against.Null(fwd, nameof(fwd));
            Guard.Against.Null(bwd, nameof(bwd));

            SourceType = sourceType;
            ResultType = resultType;
            convert_ = fwd;
            revert_ = bwd;
        }


        private readonly ConvFn convert_;
        private readonly Option<ConvFn> revert_;
    }
}
