using Cybele.Extensions;
using Kvasir.Transcription;
using Optional;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   A collection of Fields that must be tuple-wise unique for each row in a Table of a relational database.
    /// </summary>
    public sealed record CandidateKey : IKey {
        /// <inheritdoc/>
        public Option<KeyName> Name { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IField> Fields { get; }

        /// <summary>
        ///   Constructs a new <see cref="CandidateKey"/> with no name.
        /// </summary>
        /// <param name="fields">
        ///   The <see cref="Fields">Fields</see> that make up the new <see cref="CandidateKey"/>.
        /// </param>
        internal CandidateKey(FieldSeq fields)
            : this(Option.None<KeyName>(), fields) {}

        /// <summary>
        ///   Constructs a new <see cref="CandidateKey"/> with no name.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name"/> of the new <see cref="CandidateKey"/>.
        /// </param>
        /// <param name="fields">
        ///   The <see cref="Fields">Fields</see> that make up the new <see cref="CandidateKey"/>.
        /// </param>
        internal CandidateKey(KeyName name, FieldSeq fields)
            : this(Option.Some(name), fields) {}

        /// <inheritdoc/>
        public IEnumerator<IField> GetEnumerator() {
            return Fields.GetEnumerator();
        }

        /* Because CandidateKey is record type, the following methods are synthesized automatically by the compiler:
         *   > public CandidateKey(CandidateKey rhs)
         *   > public bool Equals(CandidateKey? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(CandidateKey? lhs, CandidateKey? rhs)
         *   > public static bool operator!=(CandidateKey? lhs, CandidateKey? rhs)
         */

        /// <summary>
        ///   Constructs a new <see cref="CandidateKey"/> with no name.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name">name</see> of the new <see cref="CandidateKey"/>.
        /// </param>
        /// <param name="fields">
        ///   The <see cref="Fields">Fields</see> that make up the new <see cref="CandidateKey"/>.
        /// </param>
        private CandidateKey(Option<KeyName> name, FieldSeq fields) {
            Debug.Assert(!name.Exists(n => n is null));
            Debug.Assert(fields is not null);
            Debug.Assert(!fields.IsEmpty());

            Name = name;
            Fields = new List<IField>(fields);

            // We have to use a List as opposed to, e.g., a HashSet because the order of the Fields within a Key is
            // important. Even though the order doesn't have any impact on the generated SQL (i.e. the Key [A,B] is
            // logically equivalent to the Key [B,A]), the reference order of a Foreign Key depends on the Field order
            // of the targeted Primary Key. HashSet does not preserve order of insertion; plus, this is just a
            // defensive check.
            Debug.Assert(Fields.ContainsNoDuplicates());
        }

        /// <inheritdoc/>
        TDecl IKey.GenerateDeclaration<TDecl>(IKeyDeclBuilder<TDecl> builder) {
            Debug.Assert(builder is not null);

            Name.MatchSome(n => builder.SetName(n));
            builder.SetFields(Fields);

            return builder.Build();
        }
    }
}
