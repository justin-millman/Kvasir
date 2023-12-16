using System;

namespace Kvasir.Annotations {
    public static partial class Check {
        /// <summary>
        ///   The base class for all annotations that restrict the length of the value for the Field backing a
        ///   particular string-type property.
        /// </summary>
        public abstract class StringLengthAttribute : Attribute, INestableAnnotation {
            /// <summary>
            ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
            ///   which the annotation actually applies.
            /// </summary>
            public string Path { get; init; } = "";

            /// <summary>
            ///   The (inclusive) lower bound on the string length imposed by the annotation. A value that cannot be
            ///   represented by an <see cref="int"/> indicates that there is no lower bound. Note that this value may
            ///   be negative, despite a natural boundary at 0.
            /// </summary>
            internal long Minimum { get; private init; }

            /// <summary>
            ///   The (inclusive) upper bound on the string length imposed by the annotation. A value that cannot be
            ///   represented by an <see cref="int"/> indicates that there is no upper bound. Note that this value may
            ///   be less than the <see cref="Minimum"/>, despite that being a natural boundary.
            /// </summary>
            internal long Maximum { get; private init;}

            /// <summary>
            ///   Constructs a new <see cref="StringLengthAttribute"/> instance.
            /// </summary>
            /// <param name="minimum">
            ///   The <see cref="Minimum"/> string length.
            /// </param>
            /// <param name="maximum">
            ///   The <see cref="Maximum"/> string length.
            /// </param>
            private protected StringLengthAttribute(int? minimum = null, int? maximum = null) {
                Minimum = minimum ?? long.MinValue;
                Maximum = maximum ?? long.MaxValue;
            }

            /// <summary>
            ///   Creates an exact copy of a <see cref="StringLengthAttribute"/>, but with a different
            ///   <see cref="Path"/>.
            /// </summary>
            /// <param name="path">
            ///   The new <see cref="Path"/>.
            /// </param>
            /// <returns>
            ///   A <see cref="StringLengthAttribute"/> of the same most-derived type as <c>this</c>, whose
            ///   <see cref="Path"/> attribute is exactly <paramref name="path"/>.
            /// </returns>
            private protected abstract StringLengthAttribute WithPath(string path);

            /// <inheritdoc/>
            INestableAnnotation INestableAnnotation.WithPath(string path) {
                return WithPath(path);
            }
        }


        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular string-type property must
        ///   be non-empty.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class IsNonEmptyAttribute : StringLengthAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="IsNonEmptyAttribute"/> class.
            /// </summary>
            public IsNonEmptyAttribute()
                : base(minimum: 1) {}

            /// <inheritdoc/>
            private protected sealed override StringLengthAttribute WithPath(string path) {
                return new IsNonEmptyAttribute() { Path = path };
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular string-type property must
        ///   be at least a certain length.
        /// </summary>///
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class LengthIsAtLeastAttribute : StringLengthAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="LengthIsAtLeastAttribute"/> class.
            /// </summary>
            /// <param name="lowerBound">
            ///   The length (<i>inclusive</i>) that the Field backing the annotated property must be no shorter than.
            /// </param>
            public LengthIsAtLeastAttribute(int lowerBound)
                : base(minimum: lowerBound) {}

            /// <inheritdoc/>
            private protected sealed override StringLengthAttribute WithPath(string path) {
                return new LengthIsAtLeastAttribute((int)Minimum) { Path = path };
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular string-type property can
        ///   be at most a certain length.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class LengthIsAtMostAttribute : StringLengthAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="LengthIsAtMostAttribute"/> class.
            /// </summary>
            /// <param name="upperBound">
            ///   The length (<i>inclusive</i>) that the Field backing the annotated property must be no shorter than.
            /// </param>
            public LengthIsAtMostAttribute(int upperBound)
                : base(maximum: upperBound) {}

            /// <inheritdoc/>
            private protected sealed override StringLengthAttribute WithPath(string path) {
                return new LengthIsAtMostAttribute((int)Maximum) { Path = path };
            }
        }

        /// <summary>
        ///   An annotation that specifies that the value for the Field backing a particular string-type property must
        ///   have a length within a certain range.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class LengthIsBetweenAttribute : StringLengthAttribute {
            /// <summary>
            ///   Constructs a new instance of the <see cref="LengthIsBetweenAttribute"/> class.
            /// </summary>
            /// <param name="lowerBound">
            ///   The length (<i>inclusive</i>) that the Field backing the annotated property must be no shorter than.
            /// </param>
            /// <param name="upperBound">
            ///   The length (<i>inclusive</i>) that the Field backing the annotated property must be no longer than.
            /// </param>
            public LengthIsBetweenAttribute(int lowerBound, int upperBound)
                : base(minimum: lowerBound, maximum: upperBound) {}

            /// <inheritdoc/>
            private protected sealed override LengthIsBetweenAttribute WithPath(string path) {
                return new LengthIsBetweenAttribute((int)Minimum, (int)Maximum) { Path = path };
            }
        }
    }
}
