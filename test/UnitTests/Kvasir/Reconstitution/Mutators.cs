using FluentAssertions;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UT.Kvasir.Reconstitution {
    [TestClass, TestCategory("WritePropertyMutator")]
    public class WritePropertyMutatorTests {
        [TestMethod] public void MutateNull() {
            // Arrange
            var property = typeof(Pair).GetProperty(nameof(Pair.Value0))!;
            var value = "Hafnarfjörður";
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(string));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(value);

            // Act
            Pair? pair = null;
            var row = new DBValue[] { DBValue.Create(-984L), DBValue.Create('?') };
            var mutator = new WritePropertyMutator(property, creator);
            mutator.Mutate(pair, row);

            // Assert
            mutator.SourceType.Should().Be(property.ReflectedType);
            pair.Should().BeNull();
            creator.DidNotReceive().CreateFrom(Arg.Any<IReadOnlyList<DBValue>>());
        }

        [TestMethod] public void MutateExact() {
            // Arrange
            var property = typeof(Pair).GetProperty(nameof(Pair.Value0))!;
            var value = "Liege";
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(string));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(value);

            // Act
            var pair = new Pair();
            var row = new DBValue[] { DBValue.Create(true), DBValue.Create("Utrecht"), DBValue.Create(DateTime.Now) };
            var mutator = new WritePropertyMutator(property, creator);
            mutator.Mutate(pair, row);

            // Assert
            mutator.SourceType.Should().Be(property.ReflectedType);
            pair.Value0.Should().Be(value);
            pair.Value1.Should().Be("");
            pair.GetPrivateValue().Should().Be("");
            Pair.Value3.Should().Be("");
            creator.Received().CreateFrom(row);
        }

        [TestMethod] public void MutateDerived() {
            // Arrange
            var property = typeof(Base).GetProperty(nameof(Base.Value1))!;
            var value = "Verona";
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(string));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(value);

            // Act
            var pair = new Pair();
            var row = new DBValue[] { DBValue.Create(new Guid()), DBValue.Create(-77.331M) };
            var mutator = new WritePropertyMutator(property, creator);
            mutator.Mutate(pair, row);

            // Assert
            mutator.SourceType.Should().Be(property.ReflectedType);
            pair.Value0.Should().Be("");
            pair.Value1.Should().Be(value);
            pair.GetPrivateValue().Should().Be("");
            Pair.Value3.Should().Be("");
            creator.Received().CreateFrom(row);
        }

        [TestMethod] public void MutateImplementation() {
            // Arrange
            var property = typeof(IInterface).GetProperty(nameof(IInterface.Value0))!;
            var value = "A Coruña";
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(string));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(value);

            // Act
            var pair = new Pair();
            var row = new DBValue[] { DBValue.Create(1.666606f) };
            var mutator = new WritePropertyMutator(property, creator);
            mutator.Mutate(pair, row);

            // Assert
            mutator.SourceType.Should().Be(property.ReflectedType);
            pair.Value0.Should().Be(value);
            pair.Value1.Should().Be("");
            pair.GetPrivateValue().Should().Be("");
            Pair.Value3.Should().Be("");
            creator.Received().CreateFrom(row);
        }

        [TestMethod] public void MutateNonPublicProperty() {
            // Arrange
            var property = typeof(Pair).GetProperty("Value2", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var value = "Santiago de los Caballeros";
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(string));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(value);

            // Act
            var pair = new Pair();
            var row = new DBValue[] { DBValue.Create(1.666606f) };
            var mutator = new WritePropertyMutator(property, creator);
            mutator.Mutate(pair, row);

            // Assert
            mutator.SourceType.Should().Be(property.ReflectedType);
            pair.Value0.Should().Be("");
            pair.Value1.Should().Be("");
            pair.GetPrivateValue().Should().Be(value);
            Pair.Value3.Should().Be("");
            creator.Received().CreateFrom(row);
        }

        [TestMethod] public void MutateStaticProperty() {
            // Arrange
            var property = typeof(Pair).GetProperty(nameof(Pair.Value3))!;
            var value = "Palembang";
            var creator = Substitute.For<ICreator>();
            creator.ResultType.Returns(typeof(string));
            creator.CreateFrom(Arg.Any<IReadOnlyList<DBValue>>()).Returns(value);

            // Act
            var pair = new Pair();
            var row = new DBValue[] { DBValue.Create(1.666606f) };
            var mutator = new WritePropertyMutator(property, creator);
            mutator.Mutate(pair, row);

            // Assert
            mutator.SourceType.Should().Be(property.ReflectedType);
            pair.Value0.Should().Be("");
            pair.Value1.Should().Be("");
            pair.GetPrivateValue().Should().Be("");
            Pair.Value3.Should().Be(value);
            creator.Received().CreateFrom(row);

            // Reset (for parallel tests)
            Pair.Value3 = "";
        }


        private interface IInterface {
            string Value0 { get; set; }
        }
        private abstract class Base {
            public abstract string Value1 { get; set; }
        }
        private sealed class Pair : Base, IInterface {
            public string Value0 { get; set; } = "";
            public sealed override string Value1 { get; set; } = "";
            private string Value2 { get; set; } = "";
            public static string Value3 { get; set; } = "";

            public string GetPrivateValue() {
                return Value2;
            }
        }
    }
}
