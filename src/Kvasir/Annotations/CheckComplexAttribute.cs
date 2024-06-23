using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   A non-generic view of a <see cref="Check.ComplexAttribute{TConstraintGenerator}"/>.
        /// </summary>
        /// <seealso cref="Check.ComplexAttribute{TConstraintGenerator}"/>
        public abstract class ComplexAttribute : Attribute {
            /// <summary>
            ///   The names of the Fields to which the <c>CHECK</c> constraint imposed by this attribute apply.
            /// </summary>
            internal abstract IEnumerable<FieldName> FieldNames { get; }

            /// <summary>
            ///   The <see cref="IConstraintGenerator"/> instance specified in the annotation.
            /// </summary>
            internal abstract IConstraintGenerator ConstraintGenerator { get; }

            /// <summary>
            ///   The error message explaining why the <see cref="ConstraintGenerator"/> specified in the annotation is
            ///   invalid, for example if it throws an error during construction. (This value will be
            ///   <see langword="null"/> if the <see cref="ConstraintGenerator"/> is, in fact, valid.)
            /// </summary>
            internal abstract string? UserError { get; }
        }

        /// <summary>
        ///   An annotation that imposes a <c>CHECK</c> constraint on the Fields backing a particular set of property.
        /// </summary>
        /// <typeparam name="TConstraintGenerator">
        ///   The type of <see cref="IConstraintGenerator"/>.
        /// </typeparam>
        /// <seealso cref="Check.IsNotAttribute"/>
        /// <seealso cref="Check.IsLessThanAttribute"/>
        /// <seealso cref="Check.IsLessOrEqualToAttribute"/>
        /// <seealso cref="Check.IsGreaterThanAttribute"/>
        /// <seealso cref="Check.IsGreaterOrEqualToAttribute"/>
        /// <seealso cref="Check.IsNonZeroAttribute"/>
        /// <seealso cref="Check.IsPositiveAttribute"/>
        /// <seealso cref="Check.IsNegativeAttribute"/>
        /// <seealso cref="Check.IsNonEmptyAttribute"/>
        /// <seealso cref="Check.LengthIsAtLeastAttribute"/>
        /// <seealso cref="Check.LengthIsAtMostAttribute"/>
        /// <seealso cref="Check.LengthIsBetweenAttribute"/>
        /// <seealso cref="Check.IsOneOfAttribute"/>
        /// <seealso cref="Check.IsNotOneOfAttribute"/>
        /// <seealso cref="CheckAttribute{TConstraintGenerator}"/>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
        public sealed class ComplexAttribute<TConstraintGenerator> : ComplexAttribute
            where TConstraintGenerator : IConstraintGenerator {

            /// <inheritdoc/>
            internal sealed override IEnumerable<FieldName> FieldNames => fieldNames_;

            /// <inheritdoc/>
            internal sealed override IConstraintGenerator ConstraintGenerator => impl_.ConstraintGenerator;

            /// <inheritdoc/>
            internal sealed override string? UserError => impl_.UserError;

            /// <summary>
            ///   Constructs a new instance of the <see cref="ComplexAttribute{TConstraintGenerator}"/> class.
            /// </summary>
            /// <param name="fieldNames">
            ///   The names of the backing Fields (<i>NOT</i> the C# properties) to which the <c>CHECK</c> constraint
            ///   imposed by this annotation applies.
            /// </param>
            public ComplexAttribute(string[] fieldNames)
                : this(fieldNames, Array.Empty<object?>()) {}

            /// <summary>
            ///   Constructs a new instance of the <see cref="ComplexAttribute{TConstraintGenerator}"/> class.
            /// </summary>
            /// <param name="fieldNames">
            ///   The names of the backing Fields (<i>NOT</i> the C# properties) to which the <c>CHECK</c> constraint
            ///   imposed by this annotation applies.
            /// </param>
            /// <param name="args">
            ///   The parameterization of the <c>CHECK</c> constraint.
            /// </param>
            public ComplexAttribute(string[] fieldNames, params object?[] args) {
                impl_ = new CheckAttribute<TConstraintGenerator>(args);
                fieldNames_ = fieldNames.Select(n => new FieldName(n)).ToList();
            }


            private readonly CheckAttribute impl_;
            private readonly IReadOnlyList<FieldName> fieldNames_;
        }
    }
}
