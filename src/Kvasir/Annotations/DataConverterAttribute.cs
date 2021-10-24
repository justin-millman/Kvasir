using Ardalis.GuardClauses;
using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Core;
using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that defines the data converter to be used to convert between C# values for a particular
    ///   property and values of the backing Field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class DataConverterAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { internal get; init; } = "";

        /// <summary>
        ///   The <see cref="DataConverter"/> instance specified in the annotation.
        /// </summary>
        internal DataConverter DataConverter { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="DataConverterAttribute"/> class.
        /// </summary>
        /// <param name="converter">
        ///   The <see cref="Type"/> of the data converter to be used to convert between C# values of the annotated
        ///   property and values of the backing Field.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="converter"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="converter"/> is not a <see cref="Type"/> that implements the
        ///   <see cref="IDataConverter"/> interface.
        /// </exception>
        /// <exception cref="MissingMethodException">
        ///   if <paramref name="converter"/> does not have a default (i.e. no-argument) constructor.
        /// </exception>
        /// <exception cref="System.Reflection.TargetInvocationException">
        ///   if invoking the default (i.e. no-argument) constructor of <paramref name="converter"/> results in an
        ///   exception.
        /// </exception>
        public DataConverterAttribute(Type converter) {
            Guard.Against.TypeOtherThan(converter, nameof(converter), typeof(IDataConverter));
            DataConverter = ((IDataConverter)Activator.CreateInstance(converter)!).ConverterImpl;
        }
    }
}
