using Kvasir.Transcription;
using Optional;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   An arbitrary logical restriction on the value of one or more Fields in a row of a Table in a relational
    ///   database.
    /// </summary>
    public sealed record CheckConstraint {
        /// <summary>
        ///   The name of this <c>CHECK</c> constraint.
        /// </summary>
        public Option<CheckName> Name { get; }

        /// <summary>
        ///   The logical condition that is enforced by this <c>CHECK</c> constraint.
        /// </summary>
        public Clause Condition { get; }

        /// <summary>
        ///   Constructs a new <see cref="CheckConstraint"/> with no name.
        /// </summary>
        /// <param name="condition">
        ///   The <see cref="Condition">condition</see> imposed by the new <see cref="CheckConstraint"/>
        /// </param>
        internal CheckConstraint(Clause condition)
            : this(Option.None<CheckName>(), condition) {}

        /// <summary>
        ///   Constructs a new <see cref="CheckConstraint"/> with no name.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name">name</see> of the new <see cref="CheckConstraint"/>.
        /// </param>
        /// <param name="condition">
        ///   The <see cref="Condition">condition</see> imposed by the new <see cref="CheckConstraint"/>
        /// </param>
        internal CheckConstraint(CheckName name, Clause condition)
            : this(Option.Some(name), condition) {}

        /* Because CheckConstraint is record type, the following methods are synthesized automatically by the compiler:
         *   > public CheckConstraint(CheckConstraint rhs)
         *   > public bool Equals(CheckConstraint? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(CheckConstraint? lhs, CheckConstraint? rhs)
         *   > public static bool operator!=(CheckConstraint? lhs, CheckConstraint? rhs)
         */

        /// <summary>
        ///   Produces a declaration that, when used as part of a larger Table-creating declaration, defines this
        ///   <c>CHECK</c> constraint as applying to the subject Table.
        /// </summary>
        /// <typeparam name="TDecl">
        ///   [deduced] The type of declaration produced by <paramref name="builder"/>.
        /// </typeparam>
        /// <param name="builder">
        ///   The <see cref="IConstraintDeclBuilder{TDecl}"/> to use to create the declaration.
        /// </param>
        /// <pre>
        ///   <paramref name="builder"/> is not <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A <typeparamref name="TDecl"/> that declares this <c>CHECK</c> constraint.
        /// </returns>
        internal TDecl GenerateDeclaration<TDecl>(IConstraintDeclBuilder<TDecl> builder) {
            Debug.Assert(builder is not null);

            Name.MatchSome(n => builder.SetName(n));
            Condition.AddDeclarationTo(builder);
            return builder.Build();
        }

        /// <summary>
        ///   Constructs a new <see cref="CheckConstraint"/> with no name.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name">name</see> of the new <see cref="CheckConstraint"/>.
        /// </param>
        /// <param name="condition">
        ///   The <see cref="Condition">condition</see> imposed by the new <see cref="CheckConstraint"/>
        /// </param>
        private CheckConstraint(Option<CheckName> name, Clause condition) {
            Debug.Assert(!name.Exists(n => n is null));
            Debug.Assert(condition is not null);

            Name = name;
            Condition = condition;
        }
    }

    /// <summary>
    ///   A strongly typed <see cref="string"/> representing the name of a <c>CHECK</c> constraint.
    /// </summary>
    public sealed class CheckName : ComponentName<CheckConstraint> {
        /// <summary>
        ///   Constructs a new <see cref="CheckName"/>.
        /// </summary>
        /// <param name="name">
        ///   The name. Leading and trailing whitespace will be discarded.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///   if <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///   if <paramref name="name"/> is zero-length
        ///     --or--
        ///   if <paramref name="name"/> consists only of whitespace.
        /// </exception>
        public CheckName(string name)
            : base(name) {}
    }
}
