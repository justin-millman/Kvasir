using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Core;
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
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class CheckAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { get; init; } = "";

        /// <summary>
        ///   The <see cref="IConstraintGenerator"/> instance specified in the annotation.
        /// </summary>
        internal IConstraintGenerator ConstraintGenerator {
            get {
                Debug.Assert(generator_ is not null);
                return generator_;
            }
        }

        /// <summary>
        ///   The error message explaining why a viable <see cref="IConstraintGenerator"/> could not be created from the
        ///   user input provided to the <see cref="CheckAttribute"/> constructor. (This value will be
        ///   <see langword="null"/> if no such error occurred.)
        /// </summary>
        internal string? UserError { get; private init; }

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
                var argString = args.Length == 0 ? "<none>" : string.Join(", ", args.Select(a => a.ForDisplay()));
                UserError = $"{constraint.FullName!} cannot be constructed from arguments: {argString}";
                generator_ = null;
            }
            catch (TargetInvocationException ex) {
                var argString = args.Length == 0 ? "<none>" : string.Join(", ", args.Select(a => a.ForDisplay()));
                var reason = ex.InnerException?.Message ?? "<reason unknown>";
                UserError = $"Error constructing {constraint.FullName!} from arguments: {argString} ({reason})";
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
