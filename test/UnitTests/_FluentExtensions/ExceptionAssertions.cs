using Cybele.Extensions;
using FluentAssertions.Execution;
using FluentAssertions.Specialized;
using Kvasir.Translation;
using System;
using System.Collections.Generic;
using System.Linq;

// Unicode code points are used in this file because otherwise
// the Ubuntu unit tests fail on string comparisons for expected
// errors. I'm not really sure why, but that's the deal.

namespace FluentAssertions {
    internal static partial class AssertionExtensions {
        public static TranslationAssertions Should(this Func<EntityTranslation> self) {
            return new TranslationAssertions(self);
        }


        public class TranslationAssertions : FunctionAssertions<EntityTranslation> {
            public new Func<EntityTranslation> Subject { get; }
            public TranslationAssertions(Func<EntityTranslation> subject)
                : base(subject, new AggregateExceptionExtractor()) {

                Subject = subject;
            }

            public TranslationExceptionAssertions<TEx> FailWith<TEx>() where TEx : TranslationException {
                var caught = this.ThrowExactly<TEx>();
                return new TranslationExceptionAssertions<TEx>(caught.Subject.First());
            }
        }


        public class TranslationExceptionAssertions<TEx> : ExceptionAssertions<TEx> where TEx : TranslationException {
            public new TEx Subject { get; }
            public TranslationExceptionAssertions(TEx subject)
                : base(Enumerable.Repeat(subject, 1)) {

                Subject = subject;
                locationAsserted_ = false;
                pathAsserted_ = false;
                applicationAsserted_ = false;
                problemAsserted_ = false;
                annotationsAsserted_ = false;
            }

            [CustomAssertion]
            public TranslationExceptionAssertions<TEx> WithLocation(string location) {
                this.WithMessageContaining($"\n  \u2022 Location: {location}\n");
                locationAsserted_ = true;
                return this;
            }

            [CustomAssertion]
            public TranslationExceptionAssertions<TEx> WithPath(string path) {
                this.WithMessageContaining($"\n  \u2022 Applied To: nested property @ \"{path}\"\n");
                pathAsserted_ = true;
                return this;
            }

            [CustomAssertion]
            public TranslationExceptionAssertions<TEx> WithApplicationTo(string path) {
                if (pathAsserted_) {
                    this.WithMessageContaining($"\n  \u2022 Affecting: further nested property @ \"{path}\"\n");
                }
                else {
                    this.WithMessageContaining($"\n  \u2022 Affecting: nested property @ \"{path}\"\n");
                }

                applicationAsserted_ = true;
                return this;
            }

            [CustomAssertion]
            public TranslationExceptionAssertions<TEx> WithProblem(string problem) {
                this.WithMessageContaining($"\n  \u2022 Problem: *{problem}*\n");
                problemAsserted_ = true;
                return this;
            }

            [CustomAssertion]
            public TranslationExceptionAssertions<TEx> WithAnnotations(params string[] annotations) {
                var intro = annotations.Length == 1 ? "Annotation" : "Annotations";
                this.WithMessageContaining($"\n  \u2022 {intro}: {string.Join(", ", annotations)}\n");
                annotationsAsserted_ = true;
                return this;
            }

            [CustomAssertion]
            public void EndMessage() {
                var extraneous = new List<string>();

                if (!locationAsserted_ && Subject.Message.Contains("\n  \u2022 Location: ")) {
                    extraneous.Add("Location");
                }
                if (!pathAsserted_ && Subject.Message.Contains("\n  \u2022 Applied To: ")) {
                    extraneous.Add("Path");
                }
                if (!applicationAsserted_ && Subject.Message.Contains("\n  \u2022 Affecting: ")) {
                    extraneous.Add("Application");
                }
                if (!problemAsserted_ && Subject.Message.Contains("\n  \u2022 Problem: ")) {
                    extraneous.Add("Problem");
                }
                if (!annotationsAsserted_ && (Subject.Message.Contains("\n  \u2022 Annotation: ") || Subject.Message.Contains("\n  \u2022 Annotations: "))) {
                    extraneous.Add("Annotations");
                }

                Execute.Assertion
                    .ForCondition(extraneous.IsEmpty())
                    .FailWith($"{{context:Exception}} has message lines for: {string.Join(", ", extraneous)}");
            }


            private bool locationAsserted_;
            private bool pathAsserted_;
            private bool applicationAsserted_;
            private bool problemAsserted_;
            private bool annotationsAsserted_;
        }


        [CustomAssertion]
        public static ExceptionAssertions<TEx> WithAnyMessage<TEx>(this ExceptionAssertions<TEx> self)
            where TEx : Exception {

            return self.WithMessage("*");
        }

        [CustomAssertion]
        public static ExceptionAssertions<TEx> WithMessageContaining<TEx>(this ExceptionAssertions<TEx> self, string msg)
            where TEx : Exception {

            return self.WithMessage($"*{msg}*");
        }
    }
}
