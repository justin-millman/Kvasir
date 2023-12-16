using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Diagnostics;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   The base class for all annotations that restrict the value for the Field backing a particular numeric-type
        ///   property relative to zero.
        /// </summary>
        public abstract class SignednessAttribute : Attribute, INestableAnnotation {
            /// <inheritdoc/>
            public string Path { get; init; } = "";

            /// <summary>
            ///   The operator of the comparison restriction.
            /// </summary>
            internal ComparisonOperator Operator { get; private init; }

            /// <summary>
            ///   Constructs a new <see cref="SignednessAttribute"/> instance.
            /// </summary>
            /// <param name="op">
            ///   The <see cref="ComparisonAttribute.Operator"/> for the constraint.
            /// </param>
            private protected SignednessAttribute(ComparisonOperator op) {
                Debug.Assert(op.IsValid());
                Operator = op;
            }

            /// <summary>
            ///   Creates an exact copy of a <see cref="SignednessAttribute"/>, but with a different <see cref="Path"/>.
            /// </summary>
            /// <param name="path">
            ///   The new <see cref="Path"/>.
            /// </param>
            /// <returns>
            ///   A <see cref="SignednessAttribute"/> of the same most-derived type as <c>this</c>, whose
            ///   <see cref="Path"/> attribute is exactly <paramref name="path"/>.
            /// </returns>
            private protected abstract SignednessAttribute WithPath(string path);

            /// <inheritdoc/>
            INestableAnnotation INestableAnnotation.WithPath(string path) {
                return WithPath(path);
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property cannot be
        ///   <c>0</c>.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsNonZeroAttribute : SignednessAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNonZeroAttribute"/> class.
            /// </summary>
            public IsNonZeroAttribute()
                : base(ComparisonOperator.NE) {}

            /// <inheritdoc/>
            private protected sealed override SignednessAttribute WithPath(string path) {
                return new IsNonZeroAttribute() { Path = path };
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be positive
        ///   (i.e. <c>&gt;= 0</c>).
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsPositiveAttribute : SignednessAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsPositiveAttribute"/> class.
            /// </summary>
            public IsPositiveAttribute()
                : base(ComparisonOperator.GT) {}

            /// <inheritdoc/>
            private protected sealed override SignednessAttribute WithPath(string path) {
                return new IsPositiveAttribute() { Path = path };
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular property must be positive
        ///   (i.e. <c>&lt;= 0</c>).
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public class IsNegativeAttribute : SignednessAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNegativeAttribute"/> class.
            /// </summary>
            public IsNegativeAttribute()
                : base(ComparisonOperator.LT) {}

            /// <inheritdoc/>
            private protected sealed override SignednessAttribute WithPath(string path) {
                return new IsNegativeAttribute() { Path = path };
            }
        }
    }
}
