using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;

namespace Test.Kvasir.Schema {
    internal sealed class CallSequence {
        public CallSequence() {
            setups_ = new List<(object, Queue<int>)>();
            numExpectedCalls_ = 0;
            numCallsMade_ = 0;
        }
        public Action AddCall(object setup) {
            var entry = FindEntryFor(setup);
            entry.indices.Enqueue(numExpectedCalls_++);

            return () => {
                var entry = FindEntryFor(setup);
                var indices = entry.indices;

                if (!indices.TryDequeue(out int expected)) {
                    Assert.Fail("An invocation on a method for which an in-sequence setup was created was called " +
                        "an extra time");
                }
                Assert.AreEqual(expected, numCallsMade_++, $"The mocked method invoked at index " +
                    $"#{numCallsMade_ - 1} in the sequence was not expected until index #{expected}");
            };
        }
        public void VerifyCompletedBy<U>(Mock<U> mock) where U : class {
            Assert.AreEqual(numExpectedCalls_, numCallsMade_, $"Expected {numExpectedCalls_} mocked function " +
                $"invocations but processed only {numCallsMade_}");

            foreach ((var derivedSetup, var _) in setups_) {
                var setup = (ISetup)derivedSetup.GetType().GetProperty("Setup")!.GetValue(derivedSetup)!;
                dynamic expr = setup.OriginalExpression;
                mock.Verify(expr, Times.AtLeastOnce());
            }
        }
        private (object setup, Queue<int> indices) FindEntryFor(object setup) {
            foreach (var entry in setups_) {
                if (setup.ToString() == entry.setup.ToString()) {
                    return entry;
                }
            }

            var newEntry = new ValueTuple<object, Queue<int>>(setup, new Queue<int>());
            setups_.Add(newEntry);
            return newEntry;
        }


        private readonly List<(object setup, Queue<int> indices)> setups_;
        private int numExpectedCalls_;
        private int numCallsMade_;
    }

    internal static partial class MoqExtensions {
        public static ICallbackResult NextIn<T>(this ISetup<T> self, CallSequence seq) where T : class {
            return self.Callback(seq.AddCall(self));
        }
        public static IReturnsThrows<T, U> NextIn<T, U>(this ISetup<T, U> self, CallSequence seq) where T : class {
            return self.Callback(seq.AddCall(self));
        }
    }
}
