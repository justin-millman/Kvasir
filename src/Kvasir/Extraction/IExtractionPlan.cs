using Kvasir.Schema;
using System;
using System.Collections.Generic;

namespace Kvasir.Extraction {
    /// <summary>
    ///   The interface describing how to extract one or more values from the properties of an input object.
    /// </summary>
    public interface IExtractionPlan {
        /// <summary>
        ///   The expected <see cref="Type"/> of the source object on which this <see cref="IExtractionPlan"/> can be
        ///   executed.
        /// </summary>
        public Type ExpectedSource { get; }

        /// <summary>
        ///   Execute this <see cref="IExtractionPlan"/> on an input object, extracting one or more values from its
        ///   properties.
        /// </summary>
        /// <param name="source">
        ///   The source object.
        /// </param>
        /// <pre>
        ///   <see cref="ExpectedSource"/> is the same as the type of <paramref name="source"/> or is a base class or
        ///   interface thereof.
        /// </pre>
        /// <returns>
        ///   An ordered sequence of values extracted from <paramref name="source"/> according to this
        ///   <see cref="IExtractionPlan"/>.
        /// </returns>
        IEnumerable<DBValue> Execute(object? source);
    }
}
