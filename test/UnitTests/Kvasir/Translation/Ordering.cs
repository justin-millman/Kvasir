using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.TestComponents;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Column Ordering")]
    public class OrderingTests {
        [TestMethod] public void AllFieldsManuallyOrdered() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Fraction);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("IsNegative", 0).And
                .HaveField("Denominator", 1).And
                .HaveField("Numerator", 2);
        }

        [TestMethod] public void SomeFieldsManuallyOrdered() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Parashah);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("EndChapter", 2).And
                .HaveField("EndVerse", 3);
        }

        [TestMethod] public void DuplicateColumnIndices_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pizza);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Pizza.Veggie2)}*")              // source property
                .WithMessage("*[Column]*")                              // annotation
                .WithMessage("*index*is already occupied*");            // rationale
        }

        [TestMethod] public void GapsInColumnIndices_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PhoneNumber);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*[Column]*")                              // annotation
                .WithMessage("*indices are non-consecutive*");          // rationale
        }

        [TestMethod] public void NegativeColumnIndex_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NationalPark);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(NationalPark.Established)}*")   // source property
                .WithMessage("*[Column]*")                              // annotation
                .WithMessage("*index*is negative*")                     // rationale
                .WithMessage("*-196*");                                 // details
        }
    }
}
