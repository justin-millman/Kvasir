using Cybele.Core;
using System;

namespace Kvasir.Schema {
    /// <summary>
    ///   A strongly typed <see cref="string"/> that represents the name of a Schema component.
    /// </summary>
    /// <typeparam name="TComponent">
    ///   The tag used to differentiate a <see cref="ComponentName{TComponent}"/> for one Schema component from one for
    ///   another. Two <see cref="ComponentName{TComponent}"/> types instantiated with different
    ///   <typeparamref name="TComponent"/> types are not interoperable.
    /// </typeparam>
    public abstract class ComponentName<TComponent> : ConceptString<ComponentName<TComponent>> {
        /// <summary>
        ///   Constructs a new <see cref="ComponentName{TComponent}"/>.
        /// </summary>
        /// <param name="name">
        ///   The name. Leading and trailing whitespace are discarded.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="name"/> is zero-length
        ///     --or--
        ///   if <paramref name="name"/> consists only of whitespace.
        /// </exception>
        public ComponentName(string name)
            : base(name.Trim()) {

            if (string.IsNullOrWhiteSpace(name)) {
                var msg = $"The name of a {typeof(TComponent).Name} must have non-zero length after leading and " +
                    "trailing whitespace are trimmed";
                throw new ArgumentException(msg, nameof(name));
            }
        }
    }
}
