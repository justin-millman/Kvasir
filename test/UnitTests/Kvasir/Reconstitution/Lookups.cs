using Cybele.Core;
using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("LookupByKey")]
    public class ByKeyLookupTests {
        [TestMethod] public void MatchesFirstPossibleEntity() {
            // Arrange
            var conv = DataConverter.Identity<string>();
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>(), conv);
            var plan = new DataExtractionPlan(new IExtractionStep[] { step }, new DataConverter[] { conv });
            var data = new string[] { "Juárez", "Tarawa", "Leeds" };
            var target = data[0];
            var key = new List<DBValue>() { DBValue.Create(target) };

            // Act
            var lookup = Lookup.ByKey(key, data, plan);

            // Assert
            lookup.Should().Be(target);
        }

        [TestMethod] public void MatchesIntermediatePossibleEntity() {
            // Arrange
            var conv = DataConverter.Identity<string>();
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>(), conv);
            var plan = new DataExtractionPlan(new IExtractionStep[] { step }, new DataConverter[] { conv });
            var data = new string[] { "Düsseldorf", "Naypyidaw", "Tirana" };
            var target = data[1];
            var key = new List<DBValue>() { DBValue.Create(target) };

            // Act
            var lookup = Lookup.ByKey(key, data, plan);

            // Assert
            lookup.Should().Be(target);
        }

        [TestMethod] public void MatchesLastPossibleEntity() {
            // Arrange
            var conv = DataConverter.Identity<string>();
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>(), conv);
            var plan = new DataExtractionPlan(new IExtractionStep[] { step }, new DataConverter[] { conv });
            var data = new string[] { "Siem Reap", "Caesarea", "Bissau" };
            var target = data[2];
            var key = new List<DBValue>() { DBValue.Create(target) };

            // Act
            var lookup = Lookup.ByKey(key, data, plan);

            // Assert
            lookup.Should().Be(target);
        }

        [TestMethod] public void NoMatchingEntity() {
            // Arrange
            var conv = DataConverter.Identity<string>();
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>(), conv);
            var plan = new DataExtractionPlan(new IExtractionStep[] { step }, new DataConverter[] { conv });
            var data = new string[] { "Corfu", "Cologne", "Victoria" };
            var target = "Monterrey";
            var key = new List<DBValue>() { DBValue.Create(target) };

            // Act
            Action act = () => Lookup.ByKey(key, data, plan);

            // Assert
            act.Should().Throw<Exception>().WithAnyMessage();
        }

        [TestMethod] public void StopReflectingAfterFirstPositionalMismatch() {
            // Arrange
            var conv = DataConverter.Identity<string>();
            var data = new string[] { "Bamako", "Monte Carlo", "Genoa" };
            var target = data[^1];
            var key = new List<DBValue>() { DBValue.Create(target), DBValue.Create("!!!") };
            var step0 = new PrimitiveExtractionStep(new IdentityExtractor<string>(), conv);
            var mockStep = new Mock<IExtractionStep>();
            mockStep.Setup(e => e.ExpectedSource).Returns(typeof(string));
            mockStep.Setup(e => e.Execute(It.Is<string>(s => s == target))).Returns(new List<DBValue>() { key[1] });
            mockStep.Setup(e => e.Execute(It.Is<string>(s => s != target))).Throws<NotSupportedException>();
            var step1 = mockStep.Object;
            var plan = new DataExtractionPlan(new IExtractionStep[] { step0, step1 }, new DataConverter[] { conv, conv });

            // Act
            var lookup = Lookup.ByKey(key, data, plan);

            // Assert
            lookup.Should().Be(target);
        }
    }
}
