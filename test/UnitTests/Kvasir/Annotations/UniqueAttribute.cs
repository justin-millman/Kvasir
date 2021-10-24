using FluentAssertions;
using Kvasir.Annotations;
//using Kvasir.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;

namespace UT.Kvasir.Annotations {
    [TestClass, TestCategory("Unique Attribute")]
    public class UniqueAttributeTests : AnnotationTestBase {
        [TestMethod] public void UniqueWithName_Direct() {
            // Arrange
            var name = "UNIQUE_CONSTRAINT";

            // Act
            var attr = new UniqueAttribute(name);

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Name.ToString().Should().Be(name);
        }

        [TestMethod] public void UniqueWithName_Indirect() {
            // Arrange
            var path = "Nested.Path";
            var name = "UNIQUE_CONSTRAINT";

            // Act
            var attr = new UniqueAttribute(name) { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Name.ToString().Should().Be(name);
        }

        [TestMethod] public void UniqueWithoutName_Direct() {
            // Arrange

            // Act
            var attr = new UniqueAttribute();

            // Assert
            attr.Path.Should().BeEmpty();
            attr.Name.ToString().Should().StartWith("UNIQUE_");
        }

        [TestMethod] public void UniqueWithoutName_Indirect() {
            // Arrange
            var path = "Nested.Path";

            // Act
            var attr = new UniqueAttribute() { Path = path };

            // Assert
            attr.Path.Should().Be(path);
            attr.Name.ToString().Should().StartWith("UNIQUE_");
        }

        [TestMethod] public void Unique_UniqueId() {
            // Arrange
            var attr = new UniqueAttribute();

            // Act
            var isUnique = ids_.Add(attr.TypeId);

            // Assert
            isUnique.Should().BeTrue();
        }

        /*[TestMethod] public void Unique_Multithreaded() {
            // Arrange
            var count = 10;
            var threads = new List<Thread>();
            var names = new HashSet<KeyName>();
            bool go = false;
            void act() { while (!go) {} names!.Add(new UniqueAttribute().Name); }
            for (var i = 0; i < count; ++i) {
                var t = new Thread(act) { IsBackground = true };
                t.Start();
                threads.Add(t);
            }

            // Act
            go = true;
            while (threads.Any(t => t.IsAlive)) {}

            // Assert
            names.Count.Should().Be(count);
        }*/
    }
}
