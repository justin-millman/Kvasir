using Cybele.Core;
using Kvasir.Core;
using Kvasir.Translation;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Annotations {
    /// <summary>
    ///   A non-generic view of a <see cref="DataConverterAttribute{TConverter}"/>.
    /// </summary>
    /// <seealso cref="DataConverterAttribute{TConverter}"/>.
    public abstract class DataConverterAttribute : Attribute {
        /// <summary>
        ///   The <see cref="DataConverter"/> instance specified in the annotation.
        /// </summary>
        internal abstract DataConverter DataConverter { get; }

        /// <summary>
        ///   The error message explaining why the <see cref="DataConverter"/> specified in the annotation is invalid,
        ///   for example if it throws an error during construction. (This value will be <see langword="null"/> if the
        ///   <see cref="DataConverter"/> is, in fact, valid.)
        /// </summary>
        internal abstract string? UserError { get; }
    }

    /// <summary>
    ///   An annotation that defines the data converter to be used to convert between C# values for a particular
    ///   property and values of the backing Field.
    /// </summary>
    /// <typeparam name="TConverter">
    ///   The type of <see cref="IDataConverter"/>.
    /// </typeparam>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DataConverterAttribute<TConverter> : DataConverterAttribute
        where TConverter : IDataConverter, new() {
        
        /// <inheritdoc/>
        internal sealed override DataConverter DataConverter {
            get {
                Debug.Assert(converter_ is not null);
                return converter_;
            }
        }

        /// <inheritdoc/>
        internal sealed override string? UserError => userError_;

        /// <summary>
        ///   Constructs a new instance of the <see cref="DataConverterAttribute{TConverter}"/> class.
        /// </summary>
        public DataConverterAttribute() {
            try {
                converter_ = new TConverter().ConverterImpl;
                userError_ = null;
            }
            catch (TargetInvocationException ex) {
                var reason = ex.InnerException?.Message ?? "<reason unknown>";
                userError_ = $"error constructing {typeof(TConverter).DisplayName()} ({reason})";
                converter_ = null;
            }
        }


        private readonly DataConverter? converter_;
        private readonly string? userError_;
    }
}
