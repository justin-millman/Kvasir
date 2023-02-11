using Ardalis.GuardClauses;
using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   An annotation that imposes a <c>CHECK</c> constraint on the Fields backing a particular set of property.
        /// </summary>
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
        /// <seealso cref="CheckAttribute"/>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
        public sealed class ComplexAttribute : Attribute {
            /// <summary>
            ///   The names of the Fields to which the <c>CHECK</c> constraint imposed by this attribute apply.
            /// </summary>
            internal IEnumerable<FieldName> FieldNames { get; }

            /// <summary>
            ///   The <see cref="IConstraintGenerator"/> instance specified in the annotation.
            /// </summary>
            internal IConstraintGenerator ConstraintGenerator => impl_.ConstraintGenerator;

            /// <summary>
            ///   The error message explaining why a viable <see cref="IConstraintGenerator"/> could not be created from the
            ///   user input provided to the <see cref="CheckAttribute"/> constructor. (This value will be
            ///   <see langword="null"/> if no such error occurred.)
            /// </summary>
            internal string? UserError => impl_.UserError;

            /// <summary>
            ///   Constructs a new instance of the <see cref="ComplexAttribute"/> class.
            /// </summary>
            /// <param name="constraint">
            ///   The <see cref="Type"/> of the constraint generator to be used to produce the <c>CHECK</c> constraint
            ///   clause for this annotation.
            /// </param>
            /// <param name="fieldNames">
            ///   The names of the backing Fields (<i>NOT</i> the C# properties) to which the <c>CHECK</c> constraint
            ///   imposed by this annotation applies.
            /// </param>
            public ComplexAttribute(Type constraint, string[] fieldNames)
                : this(constraint, fieldNames, Array.Empty<object?>()) {}

            /// <summary>
            ///   Constructs a new instance of the <see cref="ComplexAttribute"/> class.
            /// </summary>
            /// <param name="constraint">
            ///   The <see cref="Type"/> of the constraint generator to be used to produce the <c>CHECK</c> constraint
            ///   clause for this annotation.
            /// </param>
            /// <param name="fieldNames">
            ///   The names of the backing Fields (<i>NOT</i> the C# properties) to which the <c>CHECK</c> constraint
            ///   imposed by this annotation applies.
            /// </param>
            /// <param name="args">
            ///   The parameterization of the <c>CHECK</c> constraint.
            /// </param>
            public ComplexAttribute(Type constraint, string[] fieldNames, params object?[] args) {
                impl_ = new CheckAttribute(constraint, args);
                FieldNames = fieldNames.Select(n => new FieldName(n));
            }


            private readonly CheckAttribute impl_;
        }
    }
}
