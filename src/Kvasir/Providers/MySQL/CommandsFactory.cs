using Kvasir.Schema;
using Kvasir.Transaction;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   An implementation of the <see cref="ICommandsFactory"/> interface for a MySQL provider.
    /// </summary>
    internal sealed class CommandsFactory : ICommandsFactory {
        /// <inheritdoc/>
        public ICommands CreateCommands(ITable table, bool isPrincipalTable) {
            return new Commands(table, isPrincipalTable);
        }
    }
}
