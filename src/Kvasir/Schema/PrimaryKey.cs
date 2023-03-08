using Kvasir.Transcription;
using Optional;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kvasir.Schema {
    /// <summary>
    ///   A collection of Fields that uniquely identifies a row in a Table of a relational database and can be
    ///   referenced by Foreign Keys in another Table.
    /// </summary>
    public sealed record PrimaryKey : IKey {
        /// <inheritdoc/>
        public Option<KeyName> Name => impl_.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IField> Fields => impl_.Fields;

        /// <summary>
        ///   Constructs a new <see cref="PrimaryKey"/> with no name.
        /// </summary>
        /// <param name="fields">
        ///   The <see cref="Fields">Fields</see> that make up the new <see cref="PrimaryKey"/>.
        /// </param>
        internal PrimaryKey(FieldSeq fields) {
            impl_ = new CandidateKey(fields);
        }

        /// <summary>
        ///   Constructs a new <see cref="PrimaryKey"/> with no name.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name">name</see> of the new <see cref="PrimaryKey"/>.
        /// </param>
        /// <param name="fields">
        ///   The <see cref="Fields">Fields</see> that make up the new <see cref="PrimaryKey"/>.
        /// </param>
        internal PrimaryKey(KeyName name, FieldSeq fields) {
            impl_ = new CandidateKey(name, fields);
        }

        /// <inheritdoc/>
        public IEnumerator<IField> GetEnumerator() {
            return Fields.GetEnumerator();
        }

        /* Because PrimaryKey is record type, the following methods are synthesized automatically by the compiler:
         *   > public PrimaryKey(PrimaryKey rhs)
         *   > public bool Equals(PrimaryKey? rhs)
         *   > public sealed override bool Equals(object? rhs)
         *   > public sealed override int GetHashCode()
         *   > public sealed override string ToString()
         *   > public static bool operator==(PrimaryKey? lhs, PrimaryKey? rhs)
         *   > public static bool operator!=(PrimaryKey? lhs, PrimaryKey? rhs)
         */

        /// <inheritdoc/>
        TDecl IKey.GenerateDeclaration<TDecl>(IKeyDeclBuilder<TDecl> builder) {
            Debug.Assert(builder is not null);

            Name.MatchSome(n => builder.SetName(n));
            builder.SetFields(Fields);
            builder.SetAsPrimaryKey();

            return builder.Build();
        }


        private readonly CandidateKey impl_;
    }
}
