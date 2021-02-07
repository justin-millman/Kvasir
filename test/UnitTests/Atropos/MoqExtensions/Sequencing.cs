using Atropos.Moq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UT.Atropos.Moq {
    [TestClass, TestCategory("CallSequence")]
    public class CallSequenceTests {
        [TestMethod] public void CreateWithName() {
            // Arrange
            var name = "Michelangelo";
            var mock = new Mock<IList<int>>();
            var sequence = mock.MakeSequence(name);

            // Act
            var seqName = sequence.Name;

            // Assert
            seqName.Should().Be(name);
        }

        [TestMethod] public void CreateWithoutName() {
            // Arrange
            var mock = new Mock<IList<int>>();
            var sequence1 = mock.MakeSequence();
            var sequence2 = mock.MakeSequence();

            // Act
            var seqName1 = sequence1.Name;
            var seqName2 = sequence2.Name;

            // Assert
            seqName1.Should().NotBeNullOrWhiteSpace();
            seqName2.Should().NotBeNullOrWhiteSpace();
            seqName1.Should().NotBe(seqName2);
        }

        [TestMethod] public void CallOutOfOrder() {
            // Arrange
            var mock = new Mock<IList<int>>();
            var sequence = mock.MakeSequence();
            sequence.Add(list => list.Add(100));
            sequence.Add(list => list.Contains(It.IsAny<int>()));

            // Act
            var mockObj = mock.Object;
            mockObj.Add(100);
            Action action = () => mockObj.Add(100);

            // Assert
            action.Should().ThrowExactly<Exception>()
                .WithMessage($"*{sequence.Name}*");
        }

        [TestMethod] public void CallAfterSequenceCompleted() {
            // Arrange
            var mock = new Mock<IList<int>>();
            var sequence = mock.MakeSequence();
            sequence.Add(list => list.Contains(3));
            sequence.Add(list => list.Clear());
            sequence.Add(list => list.Count);

            // Act
            var mockObj = mock.Object;
            mockObj.Contains(3);
            mockObj.Clear();
            _ = mockObj.Count;
            Action action = () => mockObj.Clear();

            // Assert
            action.Should().ThrowExactly<Exception>()
                .WithMessage($"*{sequence.Name}*");
        }

        [TestMethod] public void VerifyIncompleteSequence() {
            // Arrange
            var mock = new Mock<IList<int>>();
            var sequence = mock.MakeSequence();
            sequence.Add(list => list.Add(1));
            sequence.Add(list => list.Add(2));
            sequence.Add(list => list.Add(3));
            sequence.Add(list => list.Add(4));

            // Act
            var mockObj = mock.Object;
            mockObj.Add(1);
            mockObj.Add(2);
            Action action = () => sequence.VerifyCompleted();

            // Assert
            action.Should().ThrowExactly<MockException>()
                .WithAnyMessage();
        }

        [TestMethod] public void VerifyCompleteSequence() {
            // Arrange
            var mock = new Mock<IList<int>>();
            var sequence = mock.MakeSequence();
            sequence.Add(list => list.Add(1));
            sequence.Add(list => list.Add(2));
            sequence.Add(list => list.Add(3));
            sequence.Add(list => list.Add(4));

            // Act
            var mockObj = mock.Object;
            mockObj.Add(1);
            mockObj.Add(2);
            mockObj.Add(3);
            mockObj.Add(4);
            Action action = () => sequence.VerifyCompleted();

            // Assert
            action.Should().NotThrow();
        }

        [TestMethod] public void EmptySequenceIsAlwaysComplete() {
            // Arrange
            var mock = new Mock<IList<int>>();
            var sequence = mock.MakeSequence();

            // Act
            Action action = () => sequence.VerifyCompleted();

            // Assert
            action.Should().NotThrow();
        }

        [TestMethod] public void SingleExpressionMultipleCallbacks() {
            // Arrange
            var mock = new Mock<IList<int>>();
            var sequence = mock.MakeSequence();
            sequence.Add(list => list.Add(It.IsAny<int>()));
            sequence.Add(list => list.Count);
            sequence.Add(list => list.Add(It.IsAny<int>()));
            sequence.Add(list => list.Count);
            sequence.Add(list => list.Add(It.IsAny<int>()));

            // Act
            var mockObj = mock.Object;
            mockObj.Add(791);
            _ = mockObj.Count;
            mockObj.Add(-10);
            _ = mockObj.Count;
            mockObj.Add(888281);
            Action action = () => sequence.VerifyCompleted();

            // Assert
            action.Should().NotThrow();
        }

        [TestMethod] public void ExtendSequenceAfterStarting() {
            // Arrange
            var mock = new Mock<IList<int>>();
            var sequence = mock.MakeSequence();
            sequence.Add(list => list.Add(It.IsAny<int>()));
            sequence.Add(list => list.Clear());

            // Act
            var mockObj = mock.Object;
            mockObj.Add(0);
            sequence.Add(list => list.Remove(0));
            mockObj.Clear();
            mockObj.Remove(0);
            sequence.Add(list => list.Count);
            _ = mockObj.Count;
            Action action = () => sequence.VerifyCompleted();

            // Assert
            action.Should().NotThrow();
        }

        [TestMethod] public void MakeCallsNotTrackedBySequence() {
            // Arrange
            var mock = new Mock<IList<int>>();
            var sequence = mock.MakeSequence();
            sequence.Add(list => list.Clear());

            // Act
            var mockObj = mock.Object;
            mockObj.Add(7);
            mockObj.Add(122);
            mockObj.Clear();
            mockObj.Add(1);
            Action action = () => sequence.VerifyCompleted();

            // Assert
            action.Should().NotThrow();
        }
    }
}
