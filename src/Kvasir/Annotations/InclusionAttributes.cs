using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   The base class for all annotations that restrict the value for the Field backing a particular property
        ///   relative to a collection of anchor values.
        /// </summary>
        public abstract class InclusionAttribute : Attribute {
            /// <summary>
            ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
            ///   which the annotation actually applies.
            /// </summary>
            public string Path { get; init; } = "";

            /// <summary>
            ///   The operator of the comparison restriction.
            /// </summary>
            internal InclusionOperator Operator { get; private init; }

            /// <summary>
            ///   The collection of anchor values for the comparison restriction.
            /// </summary>
            /// <remarks>
            ///   The values are taken directly from the user, and no compatibility checks are (or, really, can be)
            ///   performed. The one exception is that a literal user-provided <see langword="null"/> will be converted
            ///   into <see cref="DBNull"/>, though that will also induce an error during Translation. The collection
            ///   will not be empty, but it may consist of only a single value.
            /// </remarks>
            internal IEnumerable<object> Anchor { get; private init; }

            /// <summary>
            ///   Constructs a new <see cref="ComparisonAttribute"/> instance.
            /// </summary>
            /// <param name="op">
            ///   The <see cref="Operator"/> for the constraint.
            /// </param>
            /// <param name="anchor">
            ///   The <see cref="Anchor"/> for the constraint.
            /// </param>
            private protected InclusionAttribute(InclusionOperator op, IEnumerable<object?> anchor) {
                Debug.Assert(op.IsValid());
                Debug.Assert(!anchor.IsEmpty());

                Operator = op;
                Anchor = anchor.Select(v => v ?? DBNull.Value).ToArray();
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be one of a
        ///   discrete set of options.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsOneOfAttribute : InclusionAttribute {
            /// <summary>
            ///   Construct a new instance of the <see cref="IsOneOfAttribute"/> class.
            /// </summary>
            /// <param name="first">
            ///   The first of the discrete set of options, of which the value that the Field backing the annotated
            ///   property must be one.
            /// </param>
            /// <param name="rest">
            ///   The remainder of the discrete set of options, of which the value that the Field backing the annotated
            ///   property must be one. It is valid for this array to be empty.
            /// </param>
            /// <remarks>
            ///   Each element of <paramref name="rest"/> should be the same exact type as <paramref name="first"/>,
            ///   though this is not checked or enforced until Translation.
            /// </remarks>
            public IsOneOfAttribute(object? first, params object?[] rest)
                : base(InclusionOperator.In, rest.Prepend(first)) {}
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property cannot be one of
        ///   a discrete set of options
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsNotOneOfAttribute : InclusionAttribute {
            /// <summary>
            ///   Construct a new instance of the <see cref="IsNotOneOfAttribute"/> class.
            /// </summary>
            /// <param name="first">
            ///   The first of the discrete set of options, of which the value that the Field backing the annotated
            ///   property cannot be one.
            /// </param>
            /// <param name="rest">
            ///   The remainder of the discrete set of options, of which the value that the Field backing the annotated
            ///   property cannot be one. It is valid for this array to be empty.
            /// </param>
            /// <remarks>
            ///   Each element of <paramref name="rest"/> should be the same exact type as <paramref name="first"/>,
            ///   though this is not checked or enforced until Translation.
            /// </remarks>
            public IsNotOneOfAttribute(object? first, params object?[] rest)
                : base(InclusionOperator.NotIn, rest.Prepend(first)) {}
        }
    }
}
