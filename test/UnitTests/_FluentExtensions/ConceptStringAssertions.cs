using Cybele.Core;

namespace FluentAssertions {
    internal static partial class AssertionExtensions {
        public static ConceptStringAssertions<T> Should<T>(this ConceptString<T> self) where T : ConceptString<T> {
            return new ConceptStringAssertions<T>(self);
        }


        public class ConceptStringAssertions<T> : Primitives.StringAssertions where T : ConceptString<T> {
            public new ConceptString<T> Subject { get; }
            public ConceptStringAssertions(ConceptString<T> subject)
                : base(subject.ToString()) {
            
                Subject = subject;
            }
        }
    }
}
