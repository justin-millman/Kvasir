namespace Kvasir.Annotations {
    /// <summary>
    ///   The base interface to be implemented by any Kvasir annotations that expose a <c>Path</c> property for nested
    ///   application.
    /// </summary>
    internal interface INestableAnnotation {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        string Path { get; init; }

        /// <summary>
        ///   Creates an exact copy of a <see cref="INestableAnnotation"/>, but with a different <see cref="Path"/>.
        /// </summary>
        /// <param name="path">
        ///   The new <see cref="Path"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="INestableAnnotation"/> of the same most-derived type as <c>this</c>, whose <see cref="Path"/>
        ///   attribute is exactly <paramref name="path"/>.
        /// </returns>
        INestableAnnotation WithPath(string path);
    }
}
