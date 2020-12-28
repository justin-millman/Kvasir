using Kvasir.Transcription.Internal;
using Optional;
using System;
using System.Collections.Generic;

namespace Kvasir.Schema {
    /// <summary>
    ///   An <see cref="IKey"/> representing a <c>PRIMARY KEY</c> constraint on a Table in a back-end relational
    ///   database.
    /// </summary>
    public sealed class PrimaryKey : IKey {
        /// <inheritdoc/>
        public Option<KeyName> Name => impl_.Name;

        /// <summary>
        ///   Constructs a new nameless <see cref="CandidateKey"/>.
        /// </summary>
        public PrimaryKey() {
            impl_ = new CandidateKey();
        }

        /// <summary>
        ///   Constructs a new <see cref="CandidateKey"/> with a name.
        /// </summary>
        /// <param name="name">
        ///   The <see cref="Name"/> of the new <see cref="CandidateKey"/>.
        /// </param>
        public PrimaryKey(KeyName name) {
            impl_ = new CandidateKey(name);
        }

        /// <summary>
        ///   Adds a new <see cref="IField"/> to this <see cref="CandidateKey"/>.
        /// </summary>
        /// <param name="field">
        ///   The new <see cref="IField"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="field"/> is <see cref="IField.Nullability">nullable</see>.
        /// </exception>
        /// <remarks>
        ///   If <paramref name="field"/> has the same name (using as case-insensitive comparison) as any
        ///   <see cref="IField"/> that is already part of this <see cref="CandidateKey"/>, no action is performed.
        /// </remarks>
        public void AddField(IField field) {
            if (field.Nullability == IsNullable.Yes) {
                var msg = $"Field '{field.Name}' cannot be used in a Primary Key because it is nullable";
                throw new ArgumentException(msg);
            }

            impl_.AddField(field);
        }

        /// <inheritdoc/>
        public IEnumerator<IField> GetEnumerator() {
            return impl_.GetEnumerator();
        }

        /// <inheritdoc/>
        /// <pre>
        ///   At least one <see cref="IField"/> has been <see cref="AddField(IField)">added</see> to this
        ///   <see cref="PrimaryKey"/>.
        /// </pre>
        SqlSnippet IKey.GenerateDeclaration(IBuilderCollection syntax) {
            // This code is copy-pasted from the similar function on CandidateKey, which sucks (DRY) but I'm not sure
            // the best way to eliminate the duplication without significantly complicating the class desig. One option
            // is to add an additional member function to CandidateKey in the vein of Clauses' "AddDeclarationTo", but
            // that would only be needed as an implementation detail and therefore feels wrong. Another option is to
            // inherit Primary Key from Candidate Key; that would probably work, but I'm trying to avoid inheriting
            // non-abstract classes.

            var builder = syntax.KeyDeclBuilder();
            builder.SetAsPrimary();
            Name.MatchSome(n => builder.SetName(n));
            foreach (var field in this) {
                builder.AddField(field.Name);
            }

            return builder.Build();
        }


        private readonly CandidateKey impl_;
    }
}
