using Cybele;
using Kvasir.Schema.Constraints;
using Kvasir.Transcription.Internal;
using Optional;
using System;
using System.Collections.Generic;

namespace Kvasir.Schema {
    /// <summary>
    ///   An arbitrary <c>CHECK</c> constraint that applies to the values of one or more Fields.
    /// </summary>
    public sealed class CheckConstraint {
        /// <summary>
        ///   The name of this <see cref="CheckConstraint"/>.
        /// </summary>
        public Option<ConstraintName> Name { get; }

        /// <summary>
        ///   Constructs a new nameless <see cref="CheckConstraint"/>.
        /// </summary>
        /// <param name="condition">
        ///   The condition body of the new <see cref="CheckConstraint"/>.
        /// </param>
        internal CheckConstraint(Clause condition) {
            Name = Option.None<ConstraintName>();
            condition_ = condition;
        }

        /// <summary>
        ///   Constructs a new <see cref="CheckConstraint"/> with a name.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name"/> of the new <see cref="CheckConstraint"/>.
        /// </param>
        /// <param name="condition">
        ///   The condition body of the new <see cref="CheckConstraint"/>.
        /// </param>
        internal CheckConstraint(ConstraintName name, Clause condition) {
            Name = Option.Some(name);
            condition_ = condition;
        }

        /// <summary>
        ///   Gets a collection of the <see cref="IField">Fields</see> on whose value this
        ///   <see cref="CheckConstraint"/> is at least partially dependent.
        /// </summary>
        /// <returns>
        ///   A collection of the <see cref="IField">Fields</see> on whose value this <see cref="CheckConstraint"/> is
        ///   at least partially dependent. The order of the Fields is not defined, and Fields may be (but are not
        ///   necessarily) duplicated.
        /// </returns>
        public IEnumerable<IField> GetDependentFields() {
            return condition_.GetDependentFields();
        }

        /// <summary>
        ///   Generates a <see cref="SqlSnippet"/> that declares this <see cref="CheckConstraint"/> as part of a
        ///   <c>CREATE TABLE</c> statement.
        /// </summary>
        /// <param name="syntax">
        ///   The <see cref="IBuilderCollection"/> exposing the <see cref="IDeclBuilder">IDeclBuilders</see> that this
        ///   <see cref="CheckConstraint"/> should use to generate its declaratory <see cref="SqlSnippet"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="SqlSnippet"/> that declares this <see cref="CheckConstraint"/> within a <c>CREATE TABLE</c>
        ///   statement according to the syntax rules of <paramref name="syntax"/>.
        /// </returns>
        internal SqlSnippet GenerateDeclaration(IBuilderCollection syntax) {
            var builder = syntax.ConstraintDeclBuilder();
            Name.MatchSome(n => builder.SetName(n));
            condition_.AddDeclarationTo(builder);

            return builder.Build();
        }


        private readonly Clause condition_;
    }

    /// <summary>
    ///   The name of an <see cref="CheckConstraint"/>.
    /// </summary>
    public sealed class ConstraintName : ConceptString<ConstraintName> {
        /// <summary>
        ///   Constructs a new <see cref="ConstraintName"/>.
        /// </summary>
        /// <param name="name">
        ///   The name. Any leading and trailing whitespace is trimmed.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="name"/> is the empty string or consists only of whitespace characters.
        /// </exception>
        public ConstraintName(string name)
            : base(name.Trim()) {

            if (string.IsNullOrWhiteSpace(Contents)) {
                var msg = "The name of a CHECK constraint must have non-zero length when leading/trailing " +
                    "whitespace is ignored";
                throw new ArgumentException(msg);
            }
        }
    }
}
