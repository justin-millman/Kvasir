using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Schema;
using System.Linq;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when the name of a Primary Key for a table matches that of a Candidate Key.
    /// </summary>
    internal sealed class ConflictingKeyNameException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="ConflictingKeyNameException"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the name conflict was encountered.
        /// </param>
        /// <param name="conflict">
        ///   The Candidate Key with the same name as the table's Primary Key.
        /// </param>
        public ConflictingKeyNameException(Context context, CandidateKey conflict)
            : base(
                new Location(context.ToString()),
                new Problem(
                    $"name \"{conflict.Name.Unwrap()}\" is already taken by a candidate key consisting of " +
                    "{" + string.Join(", ", conflict.Fields.Select(f => f.Name.ToString())) + "}"
                ),
                new Annotation(Display.AnnotationDisplayName(typeof(NamedPrimaryKeyAttribute)))
              )
        {}
    }
}
