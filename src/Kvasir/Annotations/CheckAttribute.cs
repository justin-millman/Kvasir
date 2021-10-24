using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Diagnostics;
using System.Linq;

using FieldList = System.Collections.Generic.IEnumerable<Kvasir.Schema.IField>;
using ConverterList = System.Collections.Generic.IEnumerable<Cybele.Core.DataConverter>;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that imposes a <c>CHECK</c> constraint on the Field backing a particular property.
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
    /// <seealso cref="Check.ComplexAttribute"/>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CheckAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { internal get; init; } = "";

        /// <summary>
        ///   Creates a <c>CHECK</c> constraint <see cref="Clause"/> for one or more Fields using the generator type
        ///   specified in the annotation.
        /// </summary>
        /// <param name="fields">
        ///   The list of Fields to which the constraint applies.
        /// </param>
        /// <param name="converters">
        ///   The list of data converters that correspond to <paramref name="fields"/>, which are to be used as needed
        ///   to transform any user-provided input.
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
            Debug.Assert(fields is not null && fields.Count() >= 1);
            Debug.Assert(converters is not null && !converters.IsEmpty());
            Debug.Assert(fields.Count() == converters.Count());
            Debug.Assert(settings is not null);

            return generator_.MakeConstraint(fields, converters, settings);
        }

        /// <summary>
        ///   Constructs a new instance of the <see cref="CheckAttribute"/> class.
        /// </summary>
        /// <param name="constraint">
        ///   The <see cref="Type"/> of the constraint generator to be used to produce the <c>CHECK</c> constraint
        ///   clause for this annotation.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="constraint"/> is not a <see cref="Type"/> that implements the
        ///   <see cref="IConstraintGenerator"/> interface.
        /// </exception>
        /// <exception cref="MissingMethodException">
        ///   if <paramref name="constraint"/> does not have a default (i.e. no-argument) constructor.
        /// </exception>
        /// <exception cref="System.Reflection.TargetInvocationException">
        ///   if invoking the default (i.e. no-argument) constructor of <paramref name="constraint"/> results in an
        ///   exception.
        /// </exception>
        public CheckAttribute(Type constraint)
            : this(constraint, Array.Empty<object>()) {}

        /// <summary>
        ///   Constructs a new instance of the <see cref="CheckAttribute"/> class.
        /// </summary>
        /// <param name="constraint">
        ///   The <see cref="Type"/> of the constraint generator to be used to produce the <c>CHECK</c> constraint
        ///   clause for this annotation.
        /// </param>
        /// <param name="args">
        ///   The parameterization of the <c>CHECK</c> constraint.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="constraint"/> is not a <see cref="Type"/> that implements the
        ///   <see cref="IConstraintGenerator"/> interface.
        /// </exception>
        /// <exception cref="MissingMethodException">
        ///   if <paramref name="constraint"/> does not have a constructor that can be invoked with
        ///   <paramref name="args"/>.
        /// </exception>
        /// <exception cref="System.Reflection.TargetInvocationException">
        ///   if invoking the constructor of <paramref name="constraint"/> with <paramref name="args"/> results in an
        ///   exception.
        /// </exception>
        public CheckAttribute(Type constraint, params object?[] args) {
            Guard.Against.Null(constraint, nameof(constraint));
            Guard.Against.Null(args, nameof(args));
            Guard.Against.TypeOtherThan(constraint, nameof(constraint), typeof(IConstraintGenerator));

            generator_ = (IConstraintGenerator)Activator.CreateInstance(constraint, args)!;
        }


        private readonly IConstraintGenerator generator_;
    }

    /// <summary>
    ///   A pseudo-namespace within which annotations in which specific, predefined constraints are defined.
    /// </summary>
    public static partial class Check {}
}
