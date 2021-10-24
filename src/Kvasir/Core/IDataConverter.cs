using Cybele.Core;
using Kvasir.Exceptions;
using Kvasir.Schema;
using System;

namespace Kvasir.Core {
    /// <summary>
    ///   The untyped interface for a user-defined data converter.
    /// </summary>
    /// <remarks>
    ///   This is the interface that is leveraged internally, in a type-erased context, by the Kvasir framework.
    ///   Clients are expected to implement the <see cref="IDataConverter{TSource, TResult}"/>, which inherits from
    ///   this one, to produce their custom data converters. This extra level of indirection allows for clients to
    ///   operate in a type-safe manner, a luxury that the framework does not have due to the need to use reflection.
    /// </remarks>
    public interface IDataConverter {
        /// <summary>
        ///   The bidirectional <see cref="DataConverter"/> that wraps the user-defined conversion and reversion
        ///   methods.
        /// </summary>
        internal DataConverter ConverterImpl { get; }
    }

    /// <summary>
    ///   The interface for a user-defined data converter, which is used to convert from in-source values to database
    ///   values and vice-versa.
    /// </summary>
    /// <typeparam name="TSource">
    ///   The <see cref="Type"/> of the in-source values on which this <see cref="IDataConverter{TSource, TResult}"/>
    ///   operates.
    /// </typeparam>
    /// <typeparam name="TResult">
    ///   The <see cref="Type"/> of the database values on which this <see cref="IDataConverter{TSource, TResult}"/>
    ///   operates.
    /// </typeparam>
    /// <see cref="DataConverter"/>
    public interface IDataConverter<TSource, TResult> : IDataConverter {
        /// <summary>
        ///   Converts an instance of <typeparamref name="TSource"/> into an instance of
        ///   <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="source">
        ///   The source object.
        /// </param>
        /// <returns>
        ///   The coversion of <paramref name="source"/> into an instance of <typeparamref name="TResult"/>.
        /// </returns>
        TResult Convert(TSource source);

        /// <summary>
        ///   Converts an instance of <typeparamref name="TResult"/> back into an instance of
        ///   <see typeparamref="TSource"/>.
        /// </summary>
        /// <remarks>
        ///   Because the underlying conversion mechanism need not be bijective (that is, multiple different source
        ///   objects can convert into the same result object), the <see cref="Revert(TResult)"/> API does not
        ///   necessarily present a true inversion. For a given result object <c>R</c>, the only guarantees are that
        ///   repeated calls to the API will yield the same source object and that <c>Convert(Revert(R)) == R</c>.
        /// </remarks>
        /// <param name="result">
        ///   The result object.
        /// </param>
        /// <returns>
        ///   The reversion of <paramref name="result"/> into an instance of <typeparamref name="TSource"/>.
        /// </returns>
        TSource Revert(TResult result);

        /// <inheritdoc/>
        DataConverter IDataConverter.ConverterImpl {
            get {
                if (!DBType.IsSupported(typeof(TResult))) {
                    throw new KvasirException($"IDataConverter result type '{typeof(TResult).Name}' is not supported");
                }

                return DataConverter.Create<TSource, TResult>(s => Convert(s), r => Revert(r));
            }
        }
    }
}
