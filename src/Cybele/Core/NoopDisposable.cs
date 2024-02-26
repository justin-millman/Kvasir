using System;

namespace Cybele.Core {
    /// <summary>
    ///   An <see cref="IDisposable"/> whose <see cref="IDisposable.Dispose"/> method does nothing, enabling the
    ///   null-object pattern.
    /// </summary>
    public sealed class NoopDisposable : IDisposable {
        /// <inheritdoc/>
        void IDisposable.Dispose() {}
    }
}
