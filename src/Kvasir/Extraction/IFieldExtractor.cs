using System;

namespace Kvasir.Extraction {
    /// <summary>
    ///   The interface describing a unit of logic that extracts a single, undecomposed value out of a source object.
    /// </summary>
    /// <remarks>
    ///   The <see cref="IFieldExtractor"/> interface is a companion to the <see cref="IExtractionStep"/> interface,
    ///   separating the logic for how to extract data from a source object from what to do with the extracting content
    ///   afterward. Because the <see cref="IFieldExtractor"/> interface is not responsible for decomposition or any
    ///   form of data transformation, it cannot guarantee that the singular value produced by an execution can be
    ///   stored in a back-end database as is.
    /// </remarks>
    public interface IFieldExtractor {
        /// <summary>
        ///   The <see cref="Type"/> of source object on which this <see cref="IExtractionStep"/> is capable of being
        ///   executed.
        /// </summary>
        Type ExpectedSource { get; }

        /// <summary>
        ///   The <see cref="Type"/> of the field value produced by executing this <see cref="IExtractionStep"/>.
        /// </summary>
        Type FieldType { get; }

        /// <summary>
        ///   Execute this <see cref="IFieldExtractor"/> on a source object, producing a single value of unspecified
        ///   type and structure.
        /// </summary>
        /// <param name="sourceObject">
        ///   The source object on which to execute this <see cref="IFieldExtractor"/>.
        /// </param>
        /// <pre>
        ///   If <paramref name="sourceObject"/> is not <see langword="null"/>, then <see cref="ExpectedSource"/> is
        ///   its dynamic type or is a base class or interface thereof.
        /// </pre>
        /// <returns>
        ///   The singular value extracted from <paramref name="sourceObject"/>, which is guaranteed to be
        ///   <see langword="null"/> if <paramref name="sourceObject"/> itself is <see langword="null"/>.
        /// </returns>
        object? Execute(object? sourceObject);
    }
}
