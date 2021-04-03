using Cybele.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UT.Cybele.Extensions {
    [TestClass, TestCategory("IEnumerable: All (w/ Index)")]
    public sealed class IEnumerable_AllIndexed : ExtensionTests {
        [TestMethod] public void EmptyEnumerableMeetsAll() {
            // Arrange
            var empty = new List<string>();

            // Act
            var meetsAll = empty.All((i, s) => throw new Exception());

            // Assert
            meetsAll.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyEnumerableMeetsAll() {
            // Arrange
            var cities = new List<string>() { "Chicago", "Philadelphia", "Houston", "Indianapolis", "Atlanta" };
            var firstLetters = new List<char>() { 'C', 'P', 'H', 'I', 'A' };

            // Act
            var meetsAll = cities.All((i, s) => s[0] == firstLetters[i]);

            // Assert
            meetsAll.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyEnumerableDoesNotMeetAll() {
            // Arrange
            var cities = new List<string>() { "Dallas", "San Diego", "Milwaukee", "Miami", "Detroit", "Albany" };
            var firstLetters = new List<char>() { 'D', 'S', 'M', 'M', 'D', 'Q' };

            // Act
            var meetsAll = cities.All((i, s) => s[0] == firstLetters[i]);

            // Assert
            meetsAll.Should().BeFalse();
        }
    }

    [TestClass, TestCategory("IEnumerable: Any (w/ Index)")]
    public sealed class IEnumerable_AnyIndexed : ExtensionTests {
        [TestMethod] public void EmptyEnumerableDoesNotMeetAny() {
            // Arrange
            var empty = new List<string>();

            // Act
            var meetsAny = empty.Any((i, s) => throw new Exception());

            // Assert
            meetsAny.Should().BeFalse();
        }

        [TestMethod] public void NonEmptyEnumerableMeetsAny() {
            // Arrange
            var cities = new List<string>() { "Topeka", "Cincinnati", "Pittsburgh", "Reno", "Seattle", "Des Moines" };
            var firstLetters = new List<char>() { 'A', 'H', 'P', 'C', 'S', 'L' };

            // Act
            var meetsAny = cities.Any((i, s) => s[0] == firstLetters[i]);

            // Assert
            meetsAny.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyEnumerableDoesNotMeetAny() {
            // Arrange
            var cities = new List<string>() { "Oklahoma City", "Boise", "Kansas City", "Boston", "Charleston" };
            var firstLetters = new List<char>() { 'F', 'P', 'D', 'E', 'L' };

            // Act
            var meetsAny = cities.Any((i, s) => s[0] == firstLetters[i]);

            // Assert
            meetsAny.Should().BeFalse();
        }
    }

    [TestClass, TestCategory("IEnumerable: ContainsNoDuplicates")]
    public sealed class IEnumerable_ContainsNoDuplicates : ExtensionTests {
        [TestMethod] public void EmptyNoDuplicatesDefaultComparer() {
            // Arrange
            var empty = new List<string>();

            // Act
            var noDuplicates = empty.ContainsNoDuplicates();

            // Assert
            noDuplicates.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyNoDuplicatesDefaultComparer() {
            // Arrange
            var cities = new List<string>() { "Cleveland", "Green Bay", "Jacksonville", "Biloxi", "New Orleans" };

            // Act
            var noDuplicates = cities.ContainsNoDuplicates();

            // Assert
            noDuplicates.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyDuplicatesDefaultComparer() {
            // Arrange
            var cities = new List<string>() { "Little Rock", "Wichita", "Las Vegas", "Wichita", "San Francisco" };

            // Act
            var noDuplicates = cities.ContainsNoDuplicates();

            // Assert
            noDuplicates.Should().BeFalse();
        }

        [TestMethod] public void EmptyNoDuplicatesCustomComparer() {
            // Arrange
            var empty = new List<string>();

            // Act
            var noDuplicates = empty.ContainsNoDuplicates(StringComparer.OrdinalIgnoreCase);

            // Assert
            noDuplicates.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyNoDuplicatesCustomComparer() {
            // Arrange
            var cities = new List<string>() { "Portland", "Tacoma", "Helena", "Montpelier", "St. Louis", "Ann Arbor" };

            // Act
            var noDuplicates = cities.ContainsNoDuplicates(StringComparer.OrdinalIgnoreCase);

            // Assert
            noDuplicates.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyDuplicatesCustomComparer() {
            // Arrange
            var cities = new List<string>() { "Montgomery", "Honolulu", "Fairbanks", "FaIrbaNKs", "New Haven" };

            // Act
            var noDuplicates = cities.ContainsNoDuplicates(StringComparer.OrdinalIgnoreCase);

            // Assert
            noDuplicates.Should().BeFalse();
        }
    }

    [TestClass, TestCategory("IEnumerable: IsEmpty")]
    public sealed class IEnumerable_IsEmpty : ExtensionTests {
        [TestMethod] public void EmptyIsEmpty() {
            // Arrange
            var cities = new List<string>();

            // Act
            var isEmpty = cities.IsEmpty();

            // Assert
            isEmpty.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyIsNotEmpty() {
            // Arrange
            var cities = new List<string>() { "Phoenix", "Fresno", "Denver", "Jersey City", "Louisville" };

            // Act
            var isEmpty = cities.IsEmpty();

            // Assert
            isEmpty.Should().BeFalse();
        }
    }

    [TestClass, TestCategory("IEnumerable: None")]
    public sealed class IEnumerable_None : ExtensionTests {
        [TestMethod] public void EmptyMeetsNone() {
            // Arrange
            var empty = new List<string>();

            // Act
            var meetsNone = empty.None(s => throw new Exception());

            // Assert
            meetsNone.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyMeetsNone() {
            // Arrange
            var cities = new List<string>() { "Nashville", "Atlantic City", "El Paso", "Los Angeles", "Buffalo" };

            // Act
            var meetsNone = cities.None(s => s.Length < 3);

            // Assert
            meetsNone.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyDoesNotMeetNone() {
            // Arrange
            var cities = new List<string>() { "Oakland", "Lansing", "Birmingham", "Memphis", "Baltimore" };

            // Act
            var meetsNone = cities.None(s => s.Contains('m'));

            // Assert
            meetsNone.Should().BeFalse();
        }

        [TestMethod] public void EmptyMeetsNoneWithIndex() {
            // Arrange
            var empty = new List<string>();

            // Act
            var meetsNone = empty.None((i, s) => throw new Exception());

            // Assert
            meetsNone.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyMeetsNoneWithIndex() {
            // Arrange
            var cities = new List<string>() { "Washington, D.C.", "Canton", "Virginia Beach", "Dover", "Trenton" };
            var firstLetters = new List<char>() { 'C', 'R', 'O', 'N', 'D' };

            // Act
            var meetsNone = cities.None((i, s) => s[0] == firstLetters[i]);

            // Assert
            meetsNone.Should().BeTrue();
        }

        [TestMethod] public void NonEmptyDoesNotMeetNoneWithIndex() {
            // Arrange
            var cities = new List<string>() { "Omaha", "Springfield", "Spokane", "San Antonio" };
            var firstLetters = new List<char>() { 'H', 'T', 'S', 'U' };

            // Act
            var meetsNone = cities.None((i, s) => s[0] == firstLetters[i]);

            // Assert
            meetsNone.Should().BeFalse();
        }
    }

    [TestClass, TestCategory("IEnumerable: AllSame")]
    public sealed class IEnumerable_AllSame : ExtensionTests {
        [TestMethod] public void EmptyAllSameDefaultComparer() {
            // Arrange
            var empty = new List<string>();

            // Act
            var allSame = empty.AllSame(s => s.Length);

            // Assert
            allSame.Should().BeTrue();
        }

        [TestMethod] public void SingleAllSameDefaultComparer() {
            // Arrange
            var cities = new List<string>() { "Anaheim" };

            // Act
            var allSame = cities.AllSame(s => s.ToUpper());

            // Assert
            allSame.Should().BeTrue();
        }

        [TestMethod] public void MultipleAllSameDefaultComparer() {
            // Arrange
            var cities = new List<string>() { "Baton Rouge", "Laredo", "Madison", "Santa Fe" };

            // Act
            var allSame = cities.AllSame(s => s[1]);

            // Assert
            allSame.Should().BeTrue();
        }

        [TestMethod] public void MultipleNotAllSameDefaultComparer() {
            // Arrange
            var cities = new List<string>() { "Shreveport", "Tallahassee", "Tampa", "Waco", "Tucson" };

            // Act
            var allSame = cities.AllSame(s => s.Length);

            // Assert
            allSame.Should().BeFalse();
        }

        [TestMethod] public void EmptyAllSameCustomComparer() {
            // Arrange
            var empty = new List<string>();

            // Act
            var allSame = empty.AllSame(s => s.ToLower(), StringComparer.OrdinalIgnoreCase);

            // Assert
            allSame.Should().BeTrue();
        }

        [TestMethod] public void SingleAllSameCustomComparer() {
            // Arrange
            var cities = new List<string>() { "Jackson" };

            // Act
            var allSame = cities.AllSame(s => s.ToUpper(), StringComparer.OrdinalIgnoreCase);

            // Assert
            allSame.Should().BeTrue();
        }

        [TestMethod] public void MultipleAllSameCustomComparer() {
            // Arrange
            var cities = new List<string>() { "Glendale", "PLANO" };

            // Act
            var allSame = cities.AllSame(s => s[1].ToString(), StringComparer.OrdinalIgnoreCase);

            // Assert
            allSame.Should().BeTrue();
        }

        [TestMethod] public void MultipleNotAllSameCustomComparer() {
            // Arrange
            var cities = new List<string>() { "Winston-Salem", "North Las Vegas", "Bismarck" };

            // Act
            var allSame = cities.AllSame(s => s, StringComparer.OrdinalIgnoreCase);

            // Assert
            allSame.Should().BeFalse();
        }
    }
}
