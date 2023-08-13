using Atropos.NSubstitute;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Exceptions;
using System;
using System.Collections.Generic;

namespace UT.Atropos.NSubstitute {
    [TestClass, TestCategory("Matchers: IsSameSequence")]
    public class Matchers_IsSameSequence {
        [TestMethod] public void BothEmptyDefaultComparer() {
            // Arrange
            var sequence = new List<string>();
            var mock = Substitute.For<ISequence<string>>();
            mock.GetEnumerator().Returns(sequence.GetEnumerator());

            // Act
            mock.OperateOn(sequence);

            // Assert
            mock.Received().OperateOn(NArg.IsSameSequence<ICollection<string>>(sequence));
        }

        [TestMethod] public void NonEmptyAndEqualDefaultComparer() {
            // Arrange
            var sequence = new List<string>() { "San Diego", "Milwaukee", "Chicago", "Houston" };
            var mock = Substitute.For<ISequence<string>>();
            mock.GetEnumerator().Returns(sequence.GetEnumerator());

            // Act
            mock.OperateOn(sequence);

            // Assert
            mock.Received().OperateOn(NArg.IsSameSequence<IList<string>>(sequence));
        }

        [TestMethod] public void NonEmptyAndEqualCustomComparer() {
            // Arrange
            var actual = new List<string>() { "Flagstaff", "Akron", "Peoria", "Tulsa" };
            var expected = new List<string>() { "FLagStaFf", "akrOn", "peOrIA", "TULSa" };
            var mock = Substitute.For<ISequence<string>>();
            mock.GetEnumerator().Returns(actual.GetEnumerator());

            // Act
            mock.OperateOn(expected);

            // Assert
            var comparer = StringComparer.OrdinalIgnoreCase;
            mock.Received().OperateOn(NArg.IsSameSequence<IList<string>>(expected, comparer));
        }

        [TestMethod] public void SameLengthNotEqualDefaultComparer() {
            // Arrange
            var actual = new List<string>() { "Juneau", "Laramie", "Raleigh", "Ithaca", "Albuquerque" };
            var expected = new List<string>() { "Juneau", "Laraie", "Charlotte", "Ithaca", "Albuquerque" };
            var mock = Substitute.For<ISequence<string>>();
            mock.GetEnumerator().Returns(actual.GetEnumerator());

            // Act
            mock.OperateOn(expected);
            Action action = () => mock.Received().OperateOn(NArg.IsSameSequence<IReadOnlyList<string>>(actual));

            // Assert
            action.Should().ThrowExactly<ReceivedCallsException>()
                .WithAnyMessage();
        }

        [TestMethod] public void ActualShorterThanExpectedDefaultComparer() {
            // Arrange
            var actual = new List<string>() { "Hartford", "Orlando", "Atlanta" };
            var expected = new List<string>() { "Hartford", "Orlando", "Atlanta", "Fort Wayne", "Myrtle Beach" };
            var mock = Substitute.For<ISequence<string>>();
            mock.GetEnumerator().Returns(actual.GetEnumerator());

            // Act
            mock.OperateOn(expected);
            Action action = () => mock.Received().OperateOn(NArg.IsSameSequence<IReadOnlySet<string>>(actual));

            // Assert
            action.Should().ThrowExactly<ReceivedCallsException>()
                .WithAnyMessage();
        }

        [TestMethod] public void ActualLongerThanExpectedDefaultComparer() {
            // Arrange
            var actual = new List<string>() { "Hartford", "Orlando", "Atlanta", "Fort Wayne", "Myrtle Beach" };
            var expected = new List<string>() { "Hartford", "Orlando", "Atlanta" };
            var mock = Substitute.For<ISequence<string>>();
            mock.GetEnumerator().Returns(actual.GetEnumerator());

            // Act
            mock.OperateOn(expected);
            Action action = () => mock.Received().OperateOn(NArg.IsSameSequence<IReadOnlySet<string>>(actual));

            // Assert
            action.Should().ThrowExactly<ReceivedCallsException>()
                .WithAnyMessage();
        }


        public interface ISequence<T> : IEnumerable<T> {
            void OperateOn(IEnumerable<T> other);
        }
    }
}
