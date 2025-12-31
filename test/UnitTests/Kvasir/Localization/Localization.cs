using Atropos;
using FluentAssertions;
using Kvasir.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.Kvasir.Localization {
    [TestClass, TestCategory("Localization")]
    public class LocalizationTests {
        [TestMethod] public void Construction() {
            // Arrange
            var key = 1827003;

            // Act
            var localization = new LocalizedText(key);

            // Assert
            localization.Key.Should().Be(key);
            localization.Localizations.Should().BeSameAs(localization.Relation);
            localization.Localizations.Should().BeEmpty();
        }

        [TestMethod] public void LocalizeValue() {
            // Arrange
            var locale0 = "English";
            var value0 = "Drogheda";
            var locale1 = "Spanish";
            var value1 = "Benin City";

            // Act
            var localization = new LocalizedText(8901134);
            localization[locale0] = value0;
            localization[locale1] = value1;

            // Assert
            localization[locale0].Should().Be(value0);
            localization[locale1].Should().Be(value1);
            localization.Localizations.Should().HaveCount(2);
            localization.Localizations[locale0].Should().Be(value0);
            localization.Localizations[locale1].Should().Be(value1);
        }

        [TestMethod] public void RelocalizeValue() {
            // Arrange
            var locale = "French";
            var valueA = "Bydgoszcz";
            var valueB = "Kristiansand";

            // Act
            var localization = new LocalizedText(-306);
            localization[locale] = valueA;
            localization[locale] = valueB;

            // Assert
            localization[locale].Should().Be(valueB);
            localization.Localizations.Should().HaveCount(1);
            localization.Localizations[locale].Should().Be(valueB);
        }

        [TestMethod] public void DelocalizeValue() {
            // Arrange
            var locale = "Portuguese";

            // Act
            var localization = new LocalizedText(-7);
            localization[locale] = "Arusha City";
            localization.Delocalize(locale);

            // Assert
            localization.Localizations.Should().BeEmpty();
        }

        [TestMethod] public void StrongEquality() {
            // Arrange
            var localization0 = new LocalizedText(8751);
            var localization1 = new LocalizedText(-1834124555);
            LocalizedText? nullLocalization = null;

            // Act
            var areEqual = FullCheck.ExpectEqual(localization0, localization0);
            var areNotEqual = FullCheck.ExpectNotEqual(localization0, localization1);
            var nullAreEqual = FullCheck.ExpectEqual(nullLocalization, nullLocalization);
            var nullAreNotEqual = FullCheck.ExpectNotEqual(localization0, nullLocalization);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqual.Should().NotHaveValue();
        }

        [TestMethod] public void WeakEquality() {
            // Arrange
            var localization = new LocalizedText(43333);
            var nonLocalization = "Carrefour";
            object? nll = null;

            // Act
            var areNotEqual = FullCheck.ExpectNotEqual<object>(localization, nonLocalization);
            var nullAreNotEqual = FullCheck.ExpectNotEqual(localization, nll);

            // Assert
            areNotEqual.Should().NotHaveValue();
            nullAreNotEqual.Should().NotHaveValue();
        }

        [TestMethod] public void Stringification() {
            // Arrange
            var key = 60314;
            var localization = new LocalizedText(key);

            // Act
            var stringifiation = localization.ToString();

            // Assert
            stringifiation.Should().Be($"LOC[String:String].{key}");
        }
    }


    internal class LocalizedText : Localization<int, string, string> {
        public LocalizedText(int key) : base(key) {}
        public new string this[string locale] {
            get { return base[locale]; }
            set { base[locale] = value; }
        }
        public void Delocalize(string locale) { base.RemoveLocalizationFor(locale); }
    }
}
