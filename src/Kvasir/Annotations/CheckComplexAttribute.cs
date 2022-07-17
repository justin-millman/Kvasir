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
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
        public sealed class ComplexAttribute : Attribute {
            /// <summary>
            ///   The names of the Fields to which the <c>CHECK</c> constraint imposed by this attribute apply.
            /// </summary>
            internal IEnumerable<FieldName> FieldNames { get; }

            /// <summary>
            ///   Creates a <c>CHECK</c> constraint <see cref="Clause"/> for one or more Fields using the generator
            ///   type specified in the annotation.
            /// </summary>
            /// <param name="fields">
            ///   The list of Fields to which the constraint applies.
            /// </param>
            /// <param name="converters">
            ///   The list of data converters that correspond to <paramref name="fields"/>, which are to be used as
            ///   needed to transform any user-provided input.
            /// </param>
            /// <param name="settings">
            ///   The Kvasir framework settings.
            /// </param>
            /// <pre>
            ///   <paramref name="fields"/> has at least one element
            ///     --and--
            ///   <paramref name="converters"/> is the same length as <paramref name="fields"/>
            /// </pre>
            /// <returns>
            ///   A new <c>CHECK</c> constraint clause that applies to <paramref name="fields"/>.
            /// </returns>
            internal Clause MakeConstraint(FieldList fields, ConverterList converters, Settings settings) {
                return impl_.MakeConstraint(fields, converters, settings);
            }

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
            /// <exception cref="ArgumentException">
            ///   if <paramref name="constraint"/> is not a <see cref="Type"/> that implements the
            ///   <see cref="IConstraintGenerator"/> interface
            ///     --or--
            ///   if <paramref name="fieldNames"/> is empty.
            /// </exception>
            /// <exception cref="MissingMethodException">
            ///   if <paramref name="constraint"/> does not have a default (i.e. no-argument) constructor.
            /// </exception>
            /// <exception cref="System.Reflection.TargetInvocationException">
            ///   if invoking the default (i.e. no-argument) constructor of <paramref name="constraint"/> results in an
            ///   exception.
            /// </exception>
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
            /// <exception cref="ArgumentException">
            ///   if <paramref name="constraint"/> is not a <see cref="Type"/> that implements the
            ///   <see cref="IConstraintGenerator"/> interface
            ///     --or--
            ///   if <paramref name="fieldNames"/> is empty.
            /// </exception>
            /// <exception cref="MissingMethodException">
            ///   if <paramref name="constraint"/> does not have a constructor that can be invoked with
            ///   <paramref name="args"/>.
            /// </exception>
            /// <exception cref="System.Reflection.TargetInvocationException">
            ///   if invoking the constructor of <paramref name="constraint"/> with <paramref name="args"/> results in
            ///   an exception.
            /// </exception>
            public ComplexAttribute(Type constraint, string[] fieldNames, params object?[] args) {
                Guard.Against.NullOrEmpty(fieldNames, nameof(FieldName));
                impl_ = new CheckAttribute(constraint, args);
                FieldNames = fieldNames.Select(n => new FieldName(n));
            }


            private readonly CheckAttribute impl_;
        }
    }
}
