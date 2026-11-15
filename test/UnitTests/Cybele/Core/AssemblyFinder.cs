using Cybele.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace UT.Cybele.Core {
    [TestClass, TestCategory("AssemblyFinder")]
    public sealed class AssemblyFinderTests {
        [TestMethod] public void BasicFunction() {
            // Arrange
            static Assembly? exec() {
                return AssemblyFinder.CallerNotFrom(typeof(int).Assembly);
            }

            // Act
            var result = exec();

            // Assert
            result.Should().BeSameAs(typeof(AssemblyFinderTests).Assembly);
        }

        [TestMethod] public async Task AsyncFunction() {
            // Arrange
            static async Task<Assembly?> exec() {
                return AssemblyFinder.CallerNotFrom(typeof(int).Assembly);
            }

            // Act
            var result = await exec();

            // Assert
            result.Should().BeSameAs(typeof(AssemblyFinderTests).Assembly);
        }

        [TestMethod] public void InvokedThroughSystemLibrary() {
            // Arrange
            static Assembly? exec() {
                return AssemblyFinder.CallerNotFrom(typeof(int).Assembly);
            }

            // Act
            var result = Enumerable.Repeat(true, 1).Select(b => b ? exec() : null).First();

            // Assert
            result.Should().BeSameAs(typeof(AssemblyFinderTests).Assembly);
        }
    }
}
