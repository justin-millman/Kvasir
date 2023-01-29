using Ardalis.GuardClauses;
using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Core;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that defines the data converter to be used to convert between C# values for a particular
    ///   property and values of the backing Field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class DataConverterAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { get; init; } = "";

        /// <summary>
        ///   The <see cref="DataConverter"/> instance specified in the annotation.
        /// </summary>
        internal DataConverter DataConverter {
            get {
                Debug.Assert(converter_ is not null);
                return converter_;
            }
        }

        /// <summary>
        ///   The error message explaining why a viable <see cref="DataConverter"/> could not be created from the user
        ///   input provided to the <see cref="DataConverterAttribute"/> constructor. (This value will be
        ///   <see langword="null"/> if no such error occurred.)
        /// </summary>
        internal string? UserError { get; private init; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="DataConverterAttribute"/> class.
        /// </summary>
        /// <param name="converter">
        ///   The <see cref="Type"/> of the data converter to be used to convert between C# values of the annotated
        ///   property and values of the backing Field.
        /// </param>
        public DataConverterAttribute(Type converter) {
            Guard.Against.Null(converter, nameof(converter));

            if (!converter.IsInstanceOf(typeof(IDataConverter))) {
                converter_ = null;
                UserError = $"{converter.FullName!} does not implement the {nameof(IDataConverter)} interface";
                return;
            }

            try {
                converter_ = ((IDataConverter)Activator.CreateInstance(converter)!).ConverterImpl;
                UserError = null;
            }
            catch (MissingMethodException) {
                UserError = $"{converter.FullName!} does not have a default (i.e. no-parameter) constructor";
                converter_ = null;
            }
            catch (TargetInvocationException ex) {
                var reason = ex.InnerException?.Message ?? "<reason unknown>";
                UserError = $"Error constructing {converter.FullName!}: {reason}";
                converter_ = null;
            }
        }


        private readonly DataConverter? converter_;
    }
}
