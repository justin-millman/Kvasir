using Atropos.Moq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UT.Atropos.Moq {
    [TestClass, TestCategory("Matchers: IsSameSequence")]
    public class Matchers_IsSameSequence {
        [TestMethod] public void BothEmptyDefaultComparer() {
            // Arrange
            var sequence = new List<string>();
            var mock = new Mock<ICollection<string>>();
            mock.Setup(list => list.GetEnumerator()).Returns(sequence.GetEnumerator());
            mock.Setup(list => list.Equals(It.IsAny<object>())).Returns(true);

            // Act
            mock.Object.Equals(sequence);

            // Assert
            mock.Verify(collection => collection.Equals(Arg.IsSameSequence<ICollection<string>>(sequence)));
        }

        [TestMethod] public void NonEmptyAndEqualDefaultComparer() {
            // Arrange
            var sequence = new List<string>() { "San Diego", "Milwaukee", "Chicago", "Houston" };
            var mock = new Mock<IList<string>>();
            mock.Setup(list => list.GetEnumerator()).Returns(sequence.GetEnumerator());
            mock.Setup(list => list.Equals(It.IsAny<object>())).Returns(true);

            // Act
            mock.Object.Equals(sequence);

            // Assert
            mock.Verify(list => list.Equals(Arg.IsSameSequence<IList<string>>(sequence)));
        }

        [TestMethod] public void NonEmptyAndEqualCustomComparer() {
            // Arrange
            var actual = new List<string>() { "Flagstaff", "Akron", "Peoria", "Tulsa" };
            var expected = new List<string>() { "FLagStaFf", "akrOn", "peOrIA", "TULSa" };
            var mock = new Mock<IList<string>>();
            mock.Setup(list => list.GetEnumerator()).Returns(actual.GetEnumerator());
            mock.Setup(list => list.Equals(It.IsAny<object>())).Returns(true);

            // Act
            mock.Object.Equals(expected);

            // Assert
            var comparer = StringComparer.OrdinalIgnoreCase;
            mock.Verify(list => list.Equals(Arg.IsSameSequence<IList<string>>(expected, comparer)));
        }

        [TestMethod] public void SameLengthNotEqualDefaultComparer() {
            // Arrange
            var actual = new List<string>() { "Juneau", "Laramie", "Raleigh", "Ithaca", "Albuquerque" };
            var expected = new List<string>() { "Juneau", "Laraie", "Charlotte", "Ithaca", "Albuquerque" };
            var mock = new Mock<IReadOnlyList<string>>();
            mock.Setup(list => list.GetEnumerator()).Returns(actual.GetEnumerator());
            mock.Setup(list => list.Equals(It.IsAny<object>())).Returns(true);

            // Act
            mock.Object.Equals(expected);
            Action action = () => mock.Verify(list => list.Equals(Arg.IsSameSequence<IReadOnlyList<string>>(actual)));

            // Assert
            action.Should().ThrowExactly<MockException>()
                .WithAnyMessage();
        }

        [TestMethod] public void ActualShorterThanExpectedDefaultComparer() {
            // Arrange
            var actual = new List<string>() { "Hartford", "Orlando", "Atlanta" };
            var expected = new List<string>() { "Hartford", "Orlando", "Atlanta", "Fort Wayne", "Myrtle Beach" };
            var mock = new Mock<IReadOnlySet<string>>();
            mock.Setup(set => set.GetEnumerator()).Returns(actual.GetEnumerator());
            mock.Setup(set => set.Equals(It.IsAny<object>())).Returns(true);

            // Act
            mock.Object.Equals(expected);
            Action action = () => mock.Verify(list => list.Equals(Arg.IsSameSequence<IReadOnlySet<string>>(actual)));

            // Assert
            action.Should().ThrowExactly<MockException>()
                .WithAnyMessage();
        }

        [TestMethod] public void ActualLongerThanExpectedDefaultComparer() {
            // Arrange
            var actual = new List<string>() { "Hartford", "Orlando", "Atlanta", "Fort Wayne", "Myrtle Beach" };
            var expected = new List<string>() { "Hartford", "Orlando", "Atlanta" };
            var mock = new Mock<IReadOnlySet<string>>();
            mock.Setup(set => set.GetEnumerator()).Returns(actual.GetEnumerator());
            mock.Setup(set => set.Equals(It.IsAny<object>())).Returns(true);

            // Act
            mock.Object.Equals(expected);
            Action action = () => mock.Verify(list => list.Equals(Arg.IsSameSequence<IReadOnlySet<string>>(actual)));

            // Assert
            action.Should().ThrowExactly<MockException>()
                .WithAnyMessage();
        }
    }
}
