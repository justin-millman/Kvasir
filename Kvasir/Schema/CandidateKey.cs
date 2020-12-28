using Kvasir.Transcription.Internal;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kvasir.Schema {
    /// <summary>
    ///   An <see cref="IKey"/> representing a <c>UNIQUE</c> constraint on a Table in a back-end relational database.
    /// </summary>
    public sealed class CandidateKey : IKey {
        /// <inheritdoc/>
        public Option<KeyName> Name { get; }

        /// <summary>
        ///   Constructs a new nameless <see cref="CandidateKey"/>.
        /// </summary>
        public CandidateKey() {
            Name = Option.None<KeyName>();
            members_ = new List<IField>();
        }

        /// <summary>
        ///   Constructs a new <see cref="CandidateKey"/> with a name.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name"/> of the new <see cref="CandidateKey"/>.
        /// </param>
        public CandidateKey(KeyName name) {
            Name = Option.Some<KeyName>(name);
            members_ = new List<IField>();
        }

        /// <summary>
        ///   Adds a new <see cref="IField"/> to this <see cref="CandidateKey"/>.
        /// </summary>
        /// <param name="field">
        ///   The new <see cref="IField"/>.
        /// </param>
        /// <remarks>
        ///   If <paramref name="field"/> has the same name (using as case-insensitive comparison) as any
        ///   <see cref="IField"/> that is already part of this <see cref="CandidateKey"/>, no action is performed.
        /// </remarks>
        public void AddField(IField field) {
            if (!members_.Any(f => f.Name == field.Name)) {
                members_.Add(field);
            }
        }

        /// <inheritdoc/>
        public IEnumerator<IField> GetEnumerator() {
            return members_.GetEnumerator();
        }

        /// <inheritdoc/>
        /// <pre>
        ///   At least one <see cref="IField"/> has been <see cref="AddField(IField)">added</see> to this
        ///   <see cref="CandidateKey"/>.
        /// </pre>
        SqlSnippet IKey.GenerateDeclaration(IBuilderCollection syntax) {
            var builder = syntax.KeyDeclBuilder();
            Name.MatchSome(n => builder.SetName(n));
            foreach (var field in this) {
                builder.AddField(field.Name);
            }

            return builder.Build();
        }


        // I considered making this some kind of ISet as an optimization for the duplication checking, but this was
        // difficult due to the need to have a sort order (order of insertion) independent of the duplicate checking
        // predicate (Field name). The idea as to use an ISet<(int, IField)> with a comparer that returned 0 for pairs
        // with same-named second items and the comparison value of the first item otherwise, but any kind of binary
        // search using such a comparer could be made to fail. I don't anticipate Keys being that large, so using a
        // simple IList with a linear search on insert should be fine.
        private readonly List<IField> members_;
    }
}
