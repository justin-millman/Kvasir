using Cybele.Extensions;
using Kvasir.Core;
using Kvasir.Translation;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Annotations {
    /// <summary>
    ///   A non-generic view of a <see cref="CheckAttribute{TConstraintGenerator}"/>.
    /// </summary>
    /// <seealso cref="CheckAttribute{TConstraintGenerator}"/>
    public abstract class CheckAttribute : Attribute, INestableAnnotation {
        /// <inheritdoc/>
        public abstract string Path { get; init; }

        /// <summary>
        ///   The <see cref="IConstraintGenerator"/> instance specified in the annotation.
        /// </summary>
        internal abstract IConstraintGenerator ConstraintGenerator { get; }

        /// <summary>
        ///   The error message explaining why the <see cref="ConstraintGenerator"/> specified in the annotation is
        ///   invalid, for example if it throws an error during construction. (This value will be <see langword="null"/>
        ///   if the <see cref="ConstraintGenerator"/> is, in fact, valid.)
        /// </summary>
        internal abstract string? UserError { get; }

        /// <see cref="INestableAnnotation.WithPath(string)"/>
        protected abstract CheckAttribute WithPath(string path);

        /// <inheritdoc/>
        INestableAnnotation INestableAnnotation.WithPath(string path) {
            return WithPath(path);
        }
    }

    /// <summary>
    ///   An annotation that imposes a <c>CHECK</c> constraint on the Field backing a particular property.
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
    /// <seealso cref="Check.ComplexAttribute{TConstraintGenerator}"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class CheckAttribute<TConstraintGenerator> : CheckAttribute
        where TConstraintGenerator : IConstraintGenerator {

        /// <inheritdoc/>
        public sealed override string Path { get; init; } = "";

        /// <inheritdoc/>
        internal sealed override IConstraintGenerator ConstraintGenerator {
            get {
                Debug.Assert(generator_ is not null);
                return generator_;
            }
        }

        /// <inheritdoc/>
        internal sealed override string? UserError => userError_;

        /// <summary>
        ///   Constructs a new instance of the <see cref="CheckAttribute{TConstraintGenerator}"/> class.
        /// </summary>
        public CheckAttribute()
            : this(Array.Empty<object>()) {}

        /// <summary>
        ///   Constructs a new instance of the <see cref="CheckAttribute{TConstraintGenerator}"/> class.
        /// </summary>
        /// <param name="args">
        ///   The parameterization of the <c>CHECK</c> constraint.
        /// </param>
        public CheckAttribute(params object?[] args) {
            var constraintGenType = typeof(TConstraintGenerator);

            try {
                generator_ = (IConstraintGenerator)Activator.CreateInstance(constraintGenType, args)!;
                userError_ = null;
            }
            catch (MissingMethodException) {
                var argString = args.Length == 0 ? "<none>" : string.Join(", ", args.Select(a => a.ForDisplay()));
                userError_ = $"{constraintGenType.DisplayName()} cannot be constructed from arguments {{{argString}}}";
                generator_ = null;
            }
            catch (TargetInvocationException ex) {
                var argString = args.Length == 0 ? "<none>" : string.Join(", ", args.Select(a => a.ForDisplay()));
                var reason = ex.InnerException?.Message ?? "<reason unknown>";
                userError_ = $"error constructing {constraintGenType.DisplayName()} from arguments {{{argString}}} ({reason})";
                generator_ = null;
            }
        }

        /// <summary>
        ///   Constructs a new instance of the <see cref="CheckAttribute{TConstraintGenerator}"/> class.
        /// </summary>
        /// <param name="generator">
        ///   The already-constructed <see cref="IConstraintGenerator"/> to be used to produce the <c>CHECK</c>
        ///   constraint clause for this annotation.
        /// </param>
        private CheckAttribute(IConstraintGenerator generator) {
            generator_ = generator;
        }

        /// <inheritdoc/>
        protected sealed override CheckAttribute WithPath(string path) {
            Debug.Assert(generator_ is not null);
            return new CheckAttribute<TConstraintGenerator>(generator_) { Path = path };
        }


        private readonly IConstraintGenerator? generator_;
        private readonly string? userError_;
    }

    /// <summary>
    ///   A pseudo-namespace within which annotations in which specific, predefined constraints are defined.
    /// </summary>
    public static partial class Check {}
}
