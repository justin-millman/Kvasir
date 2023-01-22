using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CheckAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { internal get; init; } = "";

        /// <summary>
        ///   The error message explaining why a viable <see cref="IConstraintGenerator"/> could not be created from the
        ///   user input provided to the <see cref="CheckAttribute"/> constructor. (This value will be
        ///   <see langword="null"/> if no such error occurred.)
        /// </summary>
        internal string? UserError { get; init; }

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
            Debug.Assert(generator_ is not null);
            Debug.Assert(fields is not null);
            Debug.Assert(converters is not null);
            Debug.Assert(settings is not null);
            Debug.Assert(!fields.IsEmpty());
            Debug.Assert(fields.Count() == converters.Count());

            return generator_.MakeConstraint(fields, converters, settings);
        }

        /// <summary>
        ///   Constructs a new instance of the <see cref="CheckAttribute"/> class.
        /// </summary>
        /// <param name="constraint">
        ///   The <see cref="Type"/> of the constraint generator to be used to produce the <c>CHECK</c> constraint
        ///   clause for this annotation.
        /// </param>
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
        public CheckAttribute(Type constraint, params object?[] args) {
            Guard.Against.Null(constraint, nameof(constraint));
            Guard.Against.Null(args, nameof(args));

            if (!constraint.IsInstanceOf(typeof(IConstraintGenerator))) {
                UserError = $"{constraint.FullName!} does not implement the {nameof(IConstraintGenerator)} interface";
                generator_ = null;
                return;
            }

            try {
                generator_ = (IConstraintGenerator)Activator.CreateInstance(constraint, args)!;
                UserError = null;
            }
            catch (MissingMethodException) {
                var argString = "(" + string.Join(", ", args) + ")";
                UserError = $"{constraint.FullName!} cannot be constructed from arguments: {argString}";
                generator_ = null;
            }
            catch (TargetInvocationException ex) {
                var argString = "(" + string.Join(", ", args) + ")";
                var reason = ex.InnerException?.Message ?? "<reason unknown>";
                UserError = $"Error constructing {constraint.FullName!} from arguments {argString}: {reason}";
                generator_ = null;
            }
        }


        private readonly IConstraintGenerator? generator_;
    }

    /// <summary>
    ///   A pseudo-namespace within which annotations in which specific, predefined constraints are defined.
    /// </summary>
    public static partial class Check {}
}
