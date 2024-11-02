using Kvasir.Schema;

namespace Kvasir.Transaction {
    /// <summary>
    ///   The interface describing how to create <see cref="ICommands"/> instances.
    /// </summary>
    /// <remarks>
    ///   The purpose of this interface is to provide a mechanism for the framework to create instances of the
    ///   <see cref="ICommands"/> interface for constituent without knowing anything about the particular
    ///   implementation details. An implementation of the <see cref="ICommandsFactory"/> interface should generally
    ///   correspond to a particular back-end database provider (e.g. MySQL) and produce instances that can operate
    ///   against that RDBMS.
    /// </remarks>
    public interface ICommandsFactory {
        /// <summary>
        ///   Get the <see cref="ICommands"/> that defines database interactions against a particular
        ///   <see cref="ITable">Table</see>.
        /// </summary>
        /// <param name="table">
        ///   The <see cref="ITable">Table</see>.
        /// </param>
        ICommands CreateCommands(ITable table);
    }
}
