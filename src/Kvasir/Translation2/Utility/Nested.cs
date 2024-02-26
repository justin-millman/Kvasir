using Kvasir.Annotations;
using System.Diagnostics;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   A wrapper around a "nestable" annotation can track the applied-to path as translation descends.
    /// </summary>
    /// <remarks>
    ///   The <see cref="Nested{TAttribute}"/> serves as kind of façade over a "nestable" annotation. A "nestable"
    ///   annotation carries with it a dot-separated path that describes the property to which it ultimately applies.
    ///   Rather than attempt to resolve the entire path at once, it is easier (for both processing and error checking)
    ///   to "step" one nested property at a time. The API of the <see cref="Nested{TAttribute}"/> class allows for this
    ///   pattern, exposing only the next portion of the overall application path.
    /// </remarks>
    internal readonly struct Nested<TAttribute> where TAttribute : INestableAnnotation {
        /// <summary>
        ///   Whether or not the annotation applies to the current scope.
        /// </summary>
        public bool AppliesHere => NextPath == CURRENT_PROPERTY;

        /// <summary>
        ///   The next path to which the annotation applies.
        /// </summary>
        public string NextPath { get; }

        /// <summary>
        ///   The "nestable" annotation.
        /// </summary>
        public TAttribute Annotation { get; }

        /// <summary>
        ///   Implicitly creates a <see cref="Nested{TAttribute}"/> from a <typeparamref name="TAttribute"/>"/>.
        /// </summary>
        /// <param name="annotation">
        ///   The "nestable" annotation.
        /// </param>
        /// <returns>
        ///   A new <see cref="Nested{TAttribute}"/> wrapping <paramref name="annotation"/>.
        /// </returns>
        public static implicit operator Nested<TAttribute>(TAttribute annotation) {
            return new Nested<TAttribute>(annotation);
        }

        /// <summary>
        ///   Advance the <see cref="Nested{TAttribute}"/> to the next portion of its application path.
        /// </summary>
        /// <returns>
        ///   A new <see cref="Nested{TAttribute}"/> that has discarded its <see cref="NextPath">"next path"</see>.
        /// </returns>
        public Nested<TAttribute> Step() {
            Debug.Assert(NextPath != CURRENT_PROPERTY);
            return new Nested<TAttribute>(Annotation, restPath_);
        }

        /// <summary>
        ///   Constructs a new <see cref="Nested{TAttribute}"/>.
        /// </summary>
        /// <param name="annotation">
        ///   The "nestable" annotation.
        /// </param>
        private Nested(TAttribute annotation)
            : this(annotation, annotation.Path) {}

        /// <summary>
        ///   Constructs a new <see cref="Nested{TAttribute}"/>, which behaves as if the path on the "nestable"
        ///   annotation it represents were a specific value.
        /// </summary>
        /// <param name="annotation">
        ///   The "nestable" annotation.
        /// </param>
        /// <param name="path">
        ///   The value to use as the <see cref="INestableAnnotation.Path">path</see> of <paramref name="annotation"/>.
        /// </param>
        private Nested(TAttribute annotation, string path) {
            Debug.Assert(annotation is not null);
            Debug.Assert(annotation.Path is not null);
            Debug.Assert(path is not null);
            Debug.Assert(path == CURRENT_PROPERTY || annotation.Path == path || annotation.Path.EndsWith(PATH_SEPARATOR + path));

            Annotation = annotation;

            var pos = path.IndexOf(PATH_SEPARATOR);
            if (pos == -1) {
                NextPath = path;
                restPath_ = CURRENT_PROPERTY;
            }
            else {
                NextPath = path[..pos];
                restPath_ = path[(pos + 1)..];
            }
        }


        private readonly string restPath_;
        private const char PATH_SEPARATOR = '.';
        private const string CURRENT_PROPERTY = "";
    }
}
