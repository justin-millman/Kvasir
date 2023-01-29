using Cybele.Core;
using Kvasir.Schema;

namespace Kvasir.Core {
    /// <summary>
    ///   The interface for tying a user-defined <c>CHECK</c> constraint to one or more Fields.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The Kvasir framework provides a collection of basic constraints natively as part of its annotation system.
    ///     These constraints can be applied to a single Field by placing the relevant attribute directly on the target
    ///     property, with minimal parametrization available. The set of pre-defined constraints is by no means
    ///     exhaustive, and this mechanic is also limited: it cannot be used to apply a constraint that takes into
    ///     account the values of multiple Fields.
    ///   </para>
    ///   <para>
    ///     To provide users with the power to define their own constraints outside of the library, Kvasir's annotation
    ///     system allows for the specification of a <i>Type</i> that encapsulates the custom constraint logic. When
    ///     used, the provided Type must implement this interface, which exposes the method necessary to convert one
    ///     or more Fields into the actual constraint. This pattern allows for the re-use of custom constraint classes
    ///     on multiple Fields in the same Entity or in wholly separate Entities altogether; it also permits the use
    ///     of non-compile-time-constants (which are forbidden in C# attributes).
    ///   </para>
    /// </remarks>
    /// <seealso cref="Kvasir.Annotations.CheckAttribute"/>
    /// <seealso cref="Kvasir.Annotations.Check.ComplexAttribute"/>
    public interface IConstraintGenerator {
        /// <summary>
        ///   Produces a constraint <see cref="Clause"/> that restricts the values of one or more Fields.
        /// </summary>
        /// <param name="fields">
        ///   The collection of <see cref="IField">Fields</see> on whose values the <c>CHECK</c> constraint depends.
        ///   The order of the Fields reflects the order in which they were listed in the annotation and should be used
        ///   to fill in the <see cref="FieldExpression">FieldExpressions</see> of the resultant <see cref="Clause"/>.
        /// </param>
        /// <param name="converters">
        ///   The <see cref="DataConverter">DataConverters</see> relevant to the <paramref name="fields"/>. The
        ///   <i>i<sup>th</sup></i> item in this list corresponds to the <i>i<sup>th</sup></i> item in
        ///   <paramref name="fields"/>. These should be used to transform any parameterized values, if present.
        /// </param>
        /// <param name="settings">
        ///   The system <see cref="Settings"/>.
        /// </param>
        /// <pre>
        ///   Both <paramref name="fields"/> and <paramref name="converters"/> are non-empty and of equal length. None
        ///   of the arguments, nor any of the collection items, is <see langword="null"/>.
        /// </pre>
        /// <returns>
        ///   A <c>CHECK</c> constraint <see cref="Clause"/> that is configured to constrain <paramref name="fields"/>.
        /// </returns>
        Clause MakeConstraint(FieldSeq fields, ConverterSeq converters, Settings settings);
    }
}
