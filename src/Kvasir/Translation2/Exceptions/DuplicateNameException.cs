using Kvasir.Schema;
using System;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is thrown when a name is duplicated.
    /// </summary>
    internal sealed class DuplicateNameException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="DuplicateNameException"/> caused by a Field name being duplicated on an
        ///   Entity.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the duplicate Table name was encountered.
        /// </param>
        /// <param name="name">
        ///   The duplicated Field name.
        /// </param>
        public DuplicateNameException(Context context, Schema.FieldName name)
             : base(
                 new Location(context.ToString()),
                 new Problem($"there are two or more Fields with the name \"{name}\"")
               )
        {}

        /// <summary>
        ///   Constructs a new <see cref="DuplicateNameException"/> caused by a Table name being duplicated when it is
        ///   already in use by an Entity's Principal Table or by a Relation Table.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the duplicate Table name was encountered.
        /// </param>
        /// <param name="name">
        ///   The duplicated Table name.
        /// </param>
        /// <param name="otherType">
        ///   The Entity or Synthetic type that has already claimed <paramref name="name"/> for its Principal or
        ///   Relation Table.
        /// </param>
        public DuplicateNameException(Context context, TableName name, Type otherType)
            : base(
                new Location(context.ToString()),
                new Problem(
                    $"Table name \"{name}\" is already in use for the " +
                    (otherType is SyntheticType ? "Relation " : "Principal ") +
                    $"Table of {otherType.DisplayName()}"
                )
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="DuplicateNameException"/> caused by a Table name being duplicated when it is
        ///   already in use by an Relation Table.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the duplicate Table name was encountered.
        /// </param>
        /// <param name="name">
        ///   The duplicated Table name.
        /// </param>
        public DuplicateNameException(Context context, TableName name)
            : base(
                new Location(context.ToString()),
                new Problem($"Table name \"{name}\" is already in use for a Relation Table")
              )
        {}
    }
}
