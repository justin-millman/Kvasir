using Cybele.Extensions;
using Kvasir.Exceptions;
using Kvasir.Transaction;
using Kvasir.Translation;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Logger = Microsoft.Extensions.Logging.ILogger;

namespace Kvasir.Providers.MySQL {
    /// <summary>
    ///   The set of credentials needed to connect to a MySQL database.
    /// </summary>
    /// <param name="Server">The MySQL database server.</param>
    /// <param name="Port">The MySQL database connection port.</param>
    /// <param name="Database">The name of the MySQL database.</param>
    /// <param name="Username">The username / ID with which to connect to <paramref name="Database"/>.</param>
    /// <param name="Password">The password for <paramref name="Username"/>.</param>
    public readonly record struct Credentials(string Server, uint Port, string Database, string Username, string Password);

    /// <summary>
    ///   A back-end MySQL database.
    /// </summary>
    /// <remarks>
    ///   This class is the user's entry point into Kvasir for the MySQL database provider. It is responsible for
    ///   establishing a connection to the back-end system (through user-provided credentials), translating the user's
    ///   CLR types into a data model, and creating the SQL queries/statements for manipulating the database. It exposes
    ///   a number of APIs for the user to execute selections, insertions, updates, and deletes, and it also keeps track
    ///   of the current state of the database to expose Entity sets.
    /// </remarks>
    public sealed class Database {
        /// <summary>
        ///   Creates a new <see cref="Database"/> with a model based on all types from a given namespace in the calling
        ///   assembly.
        /// </summary>
        /// <remarks>
        ///   This function must be called from the assembly in the user-space code that contains the type definitions
        ///   that are to make up the data model. Each non-abstract, non-generic, top-level <see langword="public"/>
        ///   class type that is not a <see cref="Delegate"/> and is defined in exactly the given namespace is included
        ///   in the model, as are any types thereby referenced. (Of course, exclusion annotations are honored.)
        /// </remarks>
        /// <param name="credentials">
        ///   The <see cref="Credentials"/> with which to establish the connection to the back-end database.
        /// </param>
        /// <param name="entityNamespace">
        ///   The target namespace.
        /// </param>
        /// <returns>
        ///   A new <see cref="Database"/>, with a connection established through <see cref="Credentials"/> and a model
        ///   consisting of the types defined in <paramref name="entityNamespace"/> in the calling assembly.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if any of the types in discovered in <paramref name="entityNamespace"/>, or any types referenced thereby,
        ///   are not valid Entity types or have an impermissible model (e.g. a field of unsupported type, an
        ///   indiscernable primary key, etc.).
        /// </exception>
        public static async Task<Database> New(Credentials credentials, string entityNamespace) {
            var entities = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(t => t.IsPublic && t.IsClass && !t.IsAbstract && !t.IsGenericType && !t.IsGenericTypeDefinition)
                .Where(t => !t.IsNested)
                .Where(t => t.IsInstanceOf(typeof(Delegate)))
                .Where(t => t.Namespace == entityNamespace);
            return await New(credentials, entities);
        }

        /// <summary>
        ///   Creates a new <see cref="Database"/> with a model based on a set of <see cref="Type">Entity types</see>.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The set of types provided is an initial starter set: additional Entity types may be discovered through
        ///     reflection, because of references and Relations. This overload can be used to exclude certain types from
        ///     the model that do not belong, either because they are not valid (and not annotated for exclusion) or
        ///     because they are auxiliaries/utilities.
        ///   </para>
        ///   <para>
        ///     This function must be called from the assembly in the user-space code that contains the type definitions
        ///     of <paramref name="entityTypes"/>.
        ///   </para>
        /// </remarks>
        /// <param name="credentials">
        ///   The <see cref="Credentials"/> with which to establish the connection to the back-end database.
        /// </param>
        /// <param name="entityTypes">
        ///   The set of Entity types with which to seed the model.
        /// </param>
        /// <returns>
        ///   A new <see cref="Database"/>, with a connection established through <see cref="Credentials"/> and a model
        ///   consisting of <i>at least</i> the types in <paramref name="entityTypes"/>.
        /// </returns>
        /// <exception cref="KvasirException">
        ///   if any of the types in <paramref name="entityTypes"/>, or any types referenced thereby, are not valid
        ///   Entity types or have an impermissible model (e.g. a field of unsupported type, an indiscernable primary
        ///   key, etc.).
        /// </exception>
        public static async Task<Database> New(Credentials credentials, IEnumerable<Type> entityTypes) {
            var db = new Database(credentials);

            foreach (var type in entityTypes) {
                if (!Translator.IsLocalizationType(type)) {
                    var _ = db.translator_[type];
                }
                else {
                    var _ = db.translator_[type, Translator.AsLocalzation];
                }
            }

            var regulars = db.translator_.GetEntityTranslations();
            var localizations = db.translator_.GetLocalizationTranslations();
            Action<object> storer = e => db.InternalLookup(e.GetType()).Add(e);
            db.transactor_ = await Transactor.New(regulars, localizations, db.connectionPool_, db.commandsFactory_, storer, db.logger_);

            return db;
        }

        /// <summary>
        ///   Gets all of the saved instances of a particular Entity type in the current <see cref="Database"/>.
        /// </summary>
        /// <typeparam name="T">
        ///   The Entity type.
        /// </typeparam>
        /// <returns>
        ///   A collection of all Entity instances of type <typeparamref name="T"/> that have been saved to the back-end
        ///   database. The order in which the Entities are returned is undefined.
        /// </returns>
        public IEnumerable<T> AllOf<T>() where T : class {
            if (entities_.TryGetValue(typeof(T), out var instances)) {
                return instances.Cast<T>();
            }
            else {
                return [];
            }
        }

        /// <summary>
        ///   Saves one or more Entities of any type to the back-end database.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Saving an Entity results in either an <c>INSERT</c> operation or an <c>UPDATE</c> operation. The
        ///     <see cref="Database"/> keeps track of all Entities that have already been inserted to determine which
        ///     operation is to be undertaken. Identification is done by object ID rather than by extracting the
        ///     Entity's Primary Key fields.
        ///   </para>
        ///   <para>
        ///     A new Entity can produce only <c>INSERT</c> operations: one for the Entity into its Principal Table, and
        ///     potentially many into the connection Relation Tables. However, an <c>UPDATE</c> operation can produce
        ///     <c>INSERT</c>s, <c>UPDATE</c>s, and <c>DELETE</c>s, depending on the state of the Entity's relations.
        ///     At the conclusion of the save operation, the back-end database reflects the state of the CLR objects.
        ///   </para>
        /// </remarks>
        /// <param name="entities">
        ///   The Entities to save.
        /// </param>
        public async Task Save(IEnumerable<object> entities) {
            var existing = entities.Where(e => entities_[e.GetType()].Contains(e));
            var brandNew = entities.Except(existing);

            // Insertion of new entities must come first, because the updates may cause existing entities to refer to
            // those new ones. If we don't do things in this order, we're liable to get referential integrity errors
            // from the back-end database.
            await transactor_.Insert(brandNew);
            await transactor_.Update(existing);

            foreach (var entity in brandNew) {
                entities_[entity.GetType()].Add(entity);
            }
        }

        /// <summary>
        ///   Deletes one or more Entities of any type from the back-end database.
        /// </summary>
        /// <remarks>
        ///   It is the user's responsibility to ensure that any references to the deleted Entities have been redirected
        ///   prior to the <c>DELETE</c> operation. The framework will take care of the Relations attached to the
        ///   various Entities, but the back-end database's <c>ON DELETE</c> behavior can cause the CLR objects to
        ///   become out of sync if referential integrity is not maintained directly.
        /// </remarks>
        /// <param name="entities">
        ///   The Entities to delete.
        /// </param>
        public async Task Delete(IEnumerable<object> entities) {
            await transactor_.Delete(entities);

            foreach (var entity in entities) {
                entities_[entities.GetType()].Remove(entity);
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="Database"/>.
        /// </summary>
        /// <param name="credentials">
        ///   The <see cref="Credentials"/> with which to establish the connection to the back-end database.
        /// </param>
        private Database(Credentials credentials) {
            logger_ = MakeLogger();
            connectionPool_ = new ConnectionPool(credentials.Server, credentials.Port, credentials.Database, credentials.Username, credentials.Password);
            commandsFactory_ = new CommandsFactory();
            entities_ = [];
            translator_ = new Translator(t => InternalLookup(t), logger_);

            // Constructing a `Transactor` is an `async` operation, which can only be done in an `async` function.
            // Constructors cannot be `async`, so we can't do so here.
            transactor_ = null!;
        }

        /// <summary>
        ///   Looks up the set of known Entities of a given type.
        /// </summary>
        /// <remarks>
        ///   There are two use cases for looking up Entities: the public <see cref="AllOf{T}"/> API exposed to users
        ///   (essentially equivalent to a <c>SELECT *</c> query) and the storage callback for when objects are
        ///   reconstituted from the back-end database. The former exposes Entities via an <see cref="IEnumerable{T}"/>,
        ///   because it is intended to be read-only; however, the latter needs to be able to actually update the
        ///   collection. This function fulfills both, since a <see cref="HashSet{T}"/> implements the
        ///   <see cref="IEnumerable{T}"/> interface.
        /// </remarks>
        /// <param name="type">
        ///   The probe Entity type.
        /// </param>
        /// <returns>
        ///   The set of known, saved Entities of type <paramref name="type"/>, in no particular order.
        /// </returns>
        private HashSet<object> InternalLookup(Type type) {
            entities_.TryAdd(type, []);
            return entities_[type];
        }

        /// <summary>
        ///   Creates a logger to trace the operations of the <see cref="Database"/>.
        /// </summary>
        /// <remarks>
        ///   The logger is connected to a file in the <c>%APPDATA%</c> directory for Kvasir, nested within a
        ///   subdirectory for the running application. The log files are set up to rotate every day, and are retained
        ///   for two weeks (14 days).
        /// </remarks>
        /// <returns>
        ///   A new logger for Kvasir.
        /// </returns>
        private static Logger MakeLogger() {
            string localAppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string logsDir = Path.Combine(localAppDataDir, "JSM", "Kvasir", "Logs");
            string logFile = Path.Combine(logsDir, $"{System.AppDomain.CurrentDomain.FriendlyName}.log");

            var logger = new LoggerConfiguration()
                .WriteTo.File(
                    logFile,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14
                ).CreateLogger();

            return new SerilogLoggerFactory(logger).CreateLogger("Kvasir");
        }


        private readonly Logger logger_;
        private readonly ConnectionPool connectionPool_;
        private readonly CommandsFactory commandsFactory_;
        private readonly ConcurrentDictionary<Type, HashSet<object>> entities_;
        private readonly Translator translator_;
        private Transactor transactor_;
    }
}
