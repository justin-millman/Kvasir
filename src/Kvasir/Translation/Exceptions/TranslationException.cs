using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Exceptions;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Translation {
    /// <summary>
    ///   The base class for all exceptions that can arise during Translation.
    /// </summary>
    internal abstract class TranslationException : KvasirException {
        /// <summary>
        ///   Constructs a new <see cref="TranslationException"/> that describes a problem.
        /// </summary>
        /// <param name="loc">
        ///   The location at which the problem arose.
        /// </param>
        /// <param name="problem">
        ///   The problem.
        /// </param>
        protected TranslationException(Location loc, Problem problem)
            : base(
                MakeMessage(
                  $"Location: {loc}",
                  $"Problem: {problem}"
                )
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="TranslationException"/> that describes a problem caused by a single
        ///   annotation.
        /// </summary>
        /// <param name="loc">
        ///   The location at which the problem arose.
        /// </param>
        /// <param name="problem">
        ///   The problem.
        /// </param>
        /// <param name="annotation">
        ///   The annotation that caused the problem.
        /// </param>
        protected TranslationException(Location loc, Problem problem, Annotation annotation)
            : base(
                MakeMessage(
                  $"Location: {loc}",
                  $"Annotation: [{annotation}]",
                  $"Problem: {problem}"
                )
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="TranslationException"/> that describes a problem caused by a multiple
        ///   annotations that may apply to a nested property.
        /// </summary>
        /// <param name="loc">
        ///   The location at which the problem arose.
        /// </param>
        /// <param name="path">
        ///   The path to the nested property to which the annotations apply; if this is the empty string, then the
        ///   annotation is understood to apply to the property described by <paramref name="loc"/> directly.
        /// </param>
        /// <param name="problem">
        ///   The problem.
        /// </param>
        protected TranslationException(Location loc, Path path, Problem problem)
            : base(
                path.ToString() != "" ?
                  MakeMessage(
                    $"Location: {loc}",
                    $"Applied To: nested property @ \"{path}\"",
                    $"Problem: {problem}"
                  )
                /* else */ :
                  MakeMessage(
                    $"Location: {loc}",
                    $"Problem: {problem}"
                  )
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="TranslationException"/> that describes a problem caused by a single
        ///   annotation that may apply to a nested property.
        /// </summary>
        /// <param name="loc">
        ///   The location at which the problem arose.
        /// </param>
        /// <param name="path">
        ///   The path to the nested property to which <paramref name="annotation"/> applies; if this is the empty
        ///   string, then the annotation is understood to apply to the property described by <paramref name="loc"/>
        ///   directly.
        /// </param>
        /// <param name="problem">
        ///   The problem.
        /// </param>
        /// <param name="annotation">
        ///   The annotation that caused the problem.
        /// </param>
        protected TranslationException(Location loc, Path path, Problem problem, Annotation annotation)
            : base(
                path.ToString() != "" ?
                  MakeMessage(
                    $"Location: {loc}",
                    $"Annotation: [{annotation}]",
                    $"Applied To: nested property @ \"{path}\"",
                    $"Problem: {problem}"
                  )
                /* else */ :
                  MakeMessage(
                    $"Location: {loc}",
                    $"Annotation: [{annotation}]",
                    $"Problem: {problem}"
                  )
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="TranslationException"/> that describes a problem caused by a single
        ///   annotation that may apply to a nested property and may affect a further nested property.
        /// </summary>
        /// <param name="loc">
        ///   The location at which the problem arose.
        /// </param>
        /// <param name="path">
        ///   The path to the nested property to which <paramref name="annotation"/> applies; if this is the empty
        ///   string, then the annotation is understood to apply to the property described by <paramref name="loc"/>
        ///   directly.
        /// </param>
        /// <param name="cascade">
        ///   The path to the nested property that <paramref name="annotation"/> actually affects, relative to
        ///   <paramref name="path"/>; if this is the empty string, then the annotation is understood to affect the
        ///   property indicated by <paramref name="path"/>.
        /// </param>
        /// <param name="problem">
        ///   The problem.
        /// </param>
        /// <param name="annotation">
        ///   The annotation that caused the problem.
        /// </param>
        protected TranslationException(Location loc, Path path, Cascade cascade, Problem problem, Annotation annotation)
            : base(
                path.ToString() != "" && cascade.ToString() != "" ?
                  MakeMessage(
                    $"Location: {loc}",
                    $"Annotation: [{annotation}]",
                    $"Applied To: nested property @ \"{path}\"",
                    $"Affecting: further nested property @ \"{cascade}\"",
                    $"Problem: {problem}"
                  )
                /* else if */ :
                path.ToString() != "" ?
                  MakeMessage(
                    $"Location: {loc}",
                    $"Annotation: [{annotation}]",
                    $"Applied To: nested property @ \"{path}\"",
                    $"Problem: {problem}"
                  )
                /* else if */ :
                cascade.ToString() != "" ?
                  MakeMessage(
                    $"Location: {loc}",
                    $"Annotation: [{annotation}]",
                    $"Affecting: nested property @ \"{cascade}\"",
                    $"Problem: {problem}"
                  )
                /* else */ :
                  MakeMessage(
                    $"Location: {loc}",
                    $"Annotation: [{annotation}]",
                    $"Problem: {problem}"
                  )
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="TranslationException"/> that describes a problem caused by two annotations.
        /// </summary>
        /// <param name="loc">
        ///   The location at which the problem arose.
        /// </param>
        /// <param name="problem">
        ///   The problem.
        /// </param>
        /// <param name="first">
        ///   The first of the two annotations that, collectively, caused the problem.
        /// </param>
        /// <param name="second">
        ///   The second of the two annotations that, collectively, caused the problem.
        /// </param>
        protected TranslationException(Location loc, Problem problem, Annotation first, Annotation second)
            : base(
                MakeMessage(
                  $"Location: {loc}",
                  $"Annotations: [{first}], [{second}]",
                  $"Problem: {problem}"
                )
              )
        {}

        /// <summary>
        ///   Builds an error message for a <see cref="TranslationException"/> from one or more rows' information.
        /// </summary>
        /// <param name="msgs">
        ///   The information that makes up the body of the error message.
        /// </param>
        /// <returns>
        ///   A properly formatted error message based on <paramref name="msgs"/>.
        /// </returns>
        private static string MakeMessage(params string[] msgs) {
            Debug.Assert(!msgs.IsEmpty());
            Debug.Assert(msgs.None(m => m is null));

            var header = "Error Performing Translation";
            return header + "\n" + string.Join("\n", msgs.Select(m => $"  • {m}")) + "\n";
        }

        // These "strong string" types are used for overload resolution on constructors, since otherwise there would be
        // no good way to discriminate. Technically we could just define the constructor to take a bunch of strings
        // (e.g. `params string[] msgs`) but this is more self-documenting, ensures that we don't just get a bunch, and
        // means that the derived classes don't have to include row headings manually. We do this instead on the helper
        // function that builds up the full error message, since the constructors have already guarded us against
        // abuse and can apply the row headers themselves.
        protected sealed class Annotation : ConceptString<Annotation> { public Annotation(string msg) : base(msg) {} }
        protected sealed class Cascade : ConceptString<Cascade> { public Cascade(string msg) : base(msg) {} }
        protected sealed class Location : ConceptString<Location> { public Location(string msg) : base(msg) {} }
        protected sealed class Path : ConceptString<Path> { public Path(string msg) : base(msg) {} }
        protected sealed class Problem : ConceptString<Problem> { public Problem(string msg) : base(msg) {} }
    }
}
