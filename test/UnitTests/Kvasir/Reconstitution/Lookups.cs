using Cybele.Core;
using FluentAssertions;
using Kvasir.Extraction;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("LookupByKey")]
    public class ByKeyLookupTests {
        [TestMethod] public void MatchesFirstPossibleEntity() {
            // Arrange
            var conv = DataConverter.Identity<string>();
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>());
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
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>());
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
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>());
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
            var step = new PrimitiveExtractionStep(new IdentityExtractor<string>());
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
            var step0 = new PrimitiveExtractionStep(new IdentityExtractor<string>());
            var mockStep = Substitute.For<IExtractionStep>();
            mockStep.ExpectedSource.Returns(typeof(string));
            mockStep.Execute(target).Returns(new List<DBValue>() { key[1] });
            mockStep.Execute(Arg.Is<string>(s => s != target)).Returns(_ => { throw new NotSupportedException(); });
            var step1 = mockStep;
            var plan = new DataExtractionPlan(new IExtractionStep[] { step0, step1 }, new DataConverter[] { conv, conv });

            // Act
            var lookup = Lookup.ByKey(key, data, plan);

            // Assert
            lookup.Should().Be(target);
        }
    }
}
