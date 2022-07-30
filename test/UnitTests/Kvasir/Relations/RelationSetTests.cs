using FluentAssertions;
using Kvasir.Relations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UT.Kvasir.Relations {
    [TestClass, TestCategory("RelationSet")]
    public class RelationSetTests {
        [TestMethod] public void DefaultConstruct() {
            // Arrange

            // Act
            var set = new RelationSet<int>();

            // Assert
            set.Count.Should().Be(0);
            set.Comparer.Should().Be(EqualityComparer<int>.Default);
            set.Should().HaveEntryCount(0);
            (set as IRelation).ConnectionType.Should().Be<int>();
        }

        [TestMethod] public void ConstructFromElements() {
            // Arrange
            var elements = new string[] { "Bangkok", "Zagreb", "Ypres", "Nice", "Frankfurt" };

            // Act
            var set = new RelationSet<string>(elements);

            // Assert
            set.Count.Should().Be(5);
            set.Comparer.Should().Be(EqualityComparer<string>.Default);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeTrue();
            set.Contains(elements[4]).Should().BeTrue();
            set.Should().HaveEntryCount(5);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.New);
            set.Should().ExposeEntry(elements[1], Status.New);
            set.Should().ExposeEntry(elements[2], Status.New);
            set.Should().ExposeEntry(elements[3], Status.New);
            set.Should().ExposeEntry(elements[4], Status.New);
        }

        [TestMethod] public void ConstructFromCapacity() {
            // Arrange
            var capacity = 57;

            // Act
            var set = new RelationSet<DateTime>(capacity);

            // Assert
            set.Count.Should().Be(0);
            set.Comparer.Should().Be(EqualityComparer<DateTime>.Default);
            set.Should().HaveEntryCount(0);
            (set as IRelation).ConnectionType.Should().Be<DateTime>();
        }

        [TestMethod] public void ConstructCustomComparer() {
            // Arrange
            var comparer = StringComparer.InvariantCulture;

            // Act
            var set = new RelationSet<string>(comparer);

            // Assert
            set.Count.Should().Be(0);
            set.Comparer.Should().Be(comparer);
            set.Should().HaveEntryCount(0);
            (set as IRelation).ConnectionType.Should().Be<string>();
        }

        [TestMethod] public void ConstructCustomComparerFromElements() {
            // Arrange
            var elements = new string[] { "Minsk", "Vatican City", "Valletta", "Timbuktu" };
            var comparer = StringComparer.OrdinalIgnoreCase;

            // Act
            var set = new RelationSet<string>(elements, comparer);

            // Assert
            set.Count.Should().Be(4);
            set.Comparer.Should().Be(comparer);
            set.Contains(elements[0].ToUpper()).Should().BeTrue();
            set.Contains(elements[1].ToLower()).Should().BeTrue();
            set.Contains(elements[2].ToUpperInvariant()).Should().BeTrue();
            set.Contains(elements[3].ToLowerInvariant()).Should().BeTrue();
            set.Should().HaveEntryCount(4);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.New);
            set.Should().ExposeEntry(elements[1], Status.New);
            set.Should().ExposeEntry(elements[2], Status.New);
            set.Should().ExposeEntry(elements[3], Status.New);
        }

        [TestMethod] public void ConstructCustomComparerFromCapacity() {
            // Arrange
            var capacity = 114;
            var comparer = StringComparer.InvariantCulture;

            // Act
            var set = new RelationSet<string>(capacity, comparer);

            // Assert
            set.Count.Should().Be(0);
            set.Comparer.Should().Be(comparer);
            set.Should().HaveEntryCount(0);
            (set as IRelation).ConnectionType.Should().Be<string>();
        }

        [TestMethod] public void CanonicalizeNoneDeleted() {
            // Arrange
            var elements = new string[] { "Nagasaki", "Málaga", "Budapest", "Bangalore", "Sana'a", "Guayaquil" };
            var set = new RelationSet<string>(elements);

            // Act
            (set as IRelation).Canonicalize();

            // Assert
            set.Count.Should().Be(6);
            set.Should().HaveEntryCount(6);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.Saved);
            set.Should().ExposeEntry(elements[4], Status.Saved);
            set.Should().ExposeEntry(elements[5], Status.Saved);
        }

        [TestMethod] public void RemoveExistingNewItem() {
            // Arrange
            var elements = new string[] { "Abuja", "Yamoussoukro", "Leipzig", "Taipei" };
            var set = new RelationSet<string>(elements);

            // Act
            var success = set.Remove(elements[0]);

            // Assert
            success.Should().BeTrue();
            set.Count.Should().Be(3);
            set.Contains(elements[0]).Should().BeFalse();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeTrue();
            set.Should().HaveEntryCount(3);
            set.Should().ExposeDeletesFirst();
            set.Should().NotExposeEntryFor(elements[0]);
            set.Should().ExposeEntry(elements[1], Status.New);
            set.Should().ExposeEntry(elements[2], Status.New);
            set.Should().ExposeEntry(elements[3], Status.New);
        }

        [TestMethod] public void RemoveExistingSavedItem() {
            // Arrange
            var elements = new string[] { "Bujumbura", "Cape Town", "Ankara", "Vladivostok" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();

            // Act
            var success = set.Remove(elements[2]);

            // Assert
            success.Should().BeTrue();
            set.Count.Should().Be(3);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeFalse();
            set.Contains(elements[3]).Should().BeTrue();
            set.Should().HaveEntryCount(4);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Deleted);
            set.Should().ExposeEntry(elements[3], Status.Saved);
        }

        [TestMethod] public void RemoveNonexistingItem() {
            // Arrange
            var elements = new string[] { "Chennai", "Cork", "Lithuania", "Kigali" };
            var set = new RelationSet<string>() { elements[0], elements[1] };
            (set as IRelation).Canonicalize();
            set.Add(elements[2]);
            set.Add(elements[3]);

            // Act
            var succes = set.Remove("La Paz");

            // Assert
            succes.Should().BeFalse();
            set.Count.Should().Be(4);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeTrue();
            set.Should().HaveEntryCount(4);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.New);
            set.Should().ExposeEntry(elements[3], Status.New);
        }

        [TestMethod] public void RemoveWhere() {
            // Arrange
            var elements = new string[] { "Kampala", "Dodoma", "Jalisco", "Asmara", "Antwerp", "Halifax" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();

            // Act
            var count = set.RemoveWhere(s => s.ToUpperInvariant().Contains("A"));

            // Assert
            count.Should().Be(6);
            set.Count.Should().Be(0);
            set.Contains(elements[0]).Should().BeFalse();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeFalse();
            set.Contains(elements[3]).Should().BeFalse();
            set.Contains(elements[4]).Should().BeFalse();
            set.Contains(elements[5]).Should().BeFalse();
            set.Should().HaveEntryCount(6);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Deleted);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Deleted);
            set.Should().ExposeEntry(elements[3], Status.Deleted);
            set.Should().ExposeEntry(elements[4], Status.Deleted);
            set.Should().ExposeEntry(elements[5], Status.Deleted);
        }

        [TestMethod] public void Clear() {
            // Arrange
            var elements = new string[] { "Vaduz", "Phnom Penh", "Xi'an", "Lusaka"  };
            var moreElements = new string[] { "Funafuti", "Reykjavík", "Edinburgh" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            set.Add(moreElements[0]);
            set.Add(moreElements[1]);
            set.Add(moreElements[2]);

            // Act
            set.Clear();

            // Assert
            set.Count.Should().Be(0);
            set.Contains(elements[0]).Should().BeFalse();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeFalse();
            set.Contains(elements[3]).Should().BeFalse();
            set.Contains(moreElements[0]).Should().BeFalse();
            set.Contains(moreElements[1]).Should().BeFalse();
            set.Contains(moreElements[2]).Should().BeFalse();
            set.Should().HaveEntryCount(4);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Deleted);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Deleted);
            set.Should().ExposeEntry(elements[3], Status.Deleted);
            set.Should().NotExposeEntryFor(moreElements[0]);
            set.Should().NotExposeEntryFor(moreElements[1]);
            set.Should().NotExposeEntryFor(moreElements[2]);
        }

        [TestMethod] public void CanonicalizeSomeDeleted() {
            // Arrange
            var elements = new string[] { "Gdańsk", "Quezon City", "Padua", "Tallinn", "Kilkenny" };
            var moreElements = new string[] { "Dhaka", "Ytterby", "Munich" };
            var set = new RelationSet<string>(elements);

            // Act
            (set as IRelation).Canonicalize();
            set.Add(moreElements[0]);
            set.Add(moreElements[1]);
            set.Add(moreElements[2]);
            set.Remove(elements[1]);
            set.Remove(elements[4]);
            set.Remove(moreElements[0]);

            // Assert
            set.Count.Should().Be(5);
            set.Should().HaveEntryCount(7);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.Saved);
            set.Should().ExposeEntry(elements[4], Status.Deleted);
            set.Should().NotExposeEntryFor(moreElements[0]);
            set.Should().ExposeEntry(moreElements[1], Status.New);
            set.Should().ExposeEntry(moreElements[2], Status.New);
        }

        [TestMethod] public void AddNewItem() {
            // Arrange
            var elements = new string[] { "Quebec City", "Beirut", "Florence", "Hamburg" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            var first_single = "Moscow";
            var second_single = "Fukuoka";

            // Act
            set.Add(first_single);
            (set as ICollection<string>).Add(second_single);

            // Assert
            set.Count.Should().Be(6);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeTrue();
            set.Contains(first_single).Should().BeTrue();
            set.Contains(second_single).Should().BeTrue();
            set.Should().HaveEntryCount(6);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.Saved);
            set.Should().ExposeEntry(first_single, Status.New);
            set.Should().ExposeEntry(second_single, Status.New);
        }

        [TestMethod] public void AddExistingNewItem() {
            // Arrange
            var firstHalf = new string[] { "Algiers", "Tripoli", "Nur-Sultan" };
            var secondHalf = new string[] { "Hong Kong", "Warsaw", "Berlin" };
            var set = new RelationSet<string>(firstHalf);
            (set as IRelation).Canonicalize();
            set.Add(secondHalf[0]);
            set.Add(secondHalf[1]);
            set.Add(secondHalf[2]);

            // Act
            var success = set.Add(secondHalf[1]);
            (set as ICollection<string>).Add(secondHalf[2]);

            // Assert
            success.Should().BeFalse();
            set.Count.Should().Be(6);
            set.Contains(firstHalf[0]).Should().BeTrue();
            set.Contains(firstHalf[1]).Should().BeTrue();
            set.Contains(firstHalf[2]).Should().BeTrue();
            set.Contains(secondHalf[0]).Should().BeTrue();
            set.Contains(secondHalf[1]).Should().BeTrue();
            set.Contains(secondHalf[2]).Should().BeTrue();
            set.Should().HaveEntryCount(6);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(firstHalf[0], Status.Saved);
            set.Should().ExposeEntry(firstHalf[1], Status.Saved);
            set.Should().ExposeEntry(firstHalf[2], Status.Saved);
            set.Should().ExposeEntry(secondHalf[0], Status.New);
            set.Should().ExposeEntry(secondHalf[1], Status.New);
            set.Should().ExposeEntry(secondHalf[2], Status.New);
        }

        [TestMethod] public void AddExistingSavedItem() {
            // Arrange
            var elements = new string[] { "Bratislava", "Isfahan", "Brisbane" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();

            // Act
            var success = set.Add(elements[0]);
            (set as ICollection<string>).Add(elements[1]);

            // Assert
            success.Should().BeFalse();
            set.Count.Should().Be(3);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Should().HaveEntryCount(3);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
        }

        [TestMethod] public void AddExistingDeletedItem() {
            // Arrange
            var elements = new string[] { "Ljubljana", "Bandar Seri Begawan", "Hyderabad", "Podgorica" };
            var single = "Salzburg";
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            set.Add(single);
            set.Remove(elements[1]);
            set.Remove(single);

            // Act
            var success0 = set.Add(elements[1]);
            (set as ICollection<string>).Add(single);

            // Assert
            success0.Should().BeTrue();
            set.Count.Should().Be(5);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeTrue();
            set.Contains(single).Should().BeTrue();
            set.Should().HaveEntryCount(5);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.Saved);
            set.Should().ExposeEntry(single, Status.New);
        }

        [TestMethod] public void ExceptWithDisjoint() {
            // Arrange
            var lhsElements = new string[] { "Amman", "Melbourne", "Buenos Aires", "Cartagena" };
            var rhsElements = new string[] { "London", "Auckland", "Shanghai", "Cairo", "St. Petersburg" };
            var set = new RelationSet<string>(lhsElements);

            // Act
            set.ExceptWith(rhsElements);

            // Assert
            set.Count.Should().Be(4);
            set.Contains(lhsElements[0]).Should().BeTrue();
            set.Contains(lhsElements[1]).Should().BeTrue();
            set.Contains(lhsElements[2]).Should().BeTrue();
            set.Contains(lhsElements[3]).Should().BeTrue();
            set.Should().HaveEntryCount(4);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(lhsElements[0], Status.New);
            set.Should().ExposeEntry(lhsElements[1], Status.New);
            set.Should().ExposeEntry(lhsElements[2], Status.New);
            set.Should().ExposeEntry(lhsElements[3], Status.New);
        }

        [TestMethod] public void ExceptWithSelf() {
            // Arrange
            var elements = new string[] { "Paris", "Mexico City", "Kinshasa", "Tel Aviv", "São Paolo", "Seoul" };
            var set = new RelationSet<string>() { elements[0], elements[1], elements[2] };
            (set as IRelation).Canonicalize();
            set.Add(elements[3]);
            set.Add(elements[4]);
            set.Add(elements[5]);

            // Act
            set.ExceptWith(elements);

            // Assert
            set.Count.Should().Be(0);
            set.Contains(elements[0]).Should().BeFalse();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeFalse();
            set.Contains(elements[3]).Should().BeFalse();
            set.Contains(elements[4]).Should().BeFalse();
            set.Contains(elements[5]).Should().BeFalse();
            set.Should().HaveEntryCount(3);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Deleted);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Deleted);
            set.Should().NotExposeEntryFor(elements[3]);
            set.Should().NotExposeEntryFor(elements[4]);
            set.Should().NotExposeEntryFor(elements[5]);
        }

        [TestMethod] public void ExceptWithSubset() {
            // Arrange
            var elements = new string[] { "Mumbai", "Pretoria", "Havana", "Athens", "Barcelona" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();

            // Act
            set.ExceptWith(elements.Where(s => s.Contains("e")));

            // Assert
            set.Count.Should().Be(2);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeFalse();
            set.Contains(elements[4]).Should().BeFalse();
            set.Should().HaveEntryCount(5);
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.Deleted);
            set.Should().ExposeEntry(elements[4], Status.Deleted);
        }

        [TestMethod] public void ExceptWithSuperset() {
            // Arrange
            var elements = new string[] { "Istanbul", "Helsinki", "Dublin" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            var single = "Brussels";
            set.Add(single);

            // Act
            set.ExceptWith(elements.Append(single).Append("Venice").Append("Libson").Append("Tokyo"));

            // Assert
            set.Count.Should().Be(0);
            set.Contains(elements[0]).Should().BeFalse();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeFalse();
            set.Contains(single).Should().BeFalse();
            set.Should().HaveEntryCount(3);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Deleted);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Deleted);
            set.Should().NotExposeEntryFor(single);
        }

        [TestMethod] public void ExceptWithOverlap() {
            // Arrange
            var elements = new string[] { "Beijing", "Sydney", "Geneva", "Pyongyang" };
            var moreElements = new string[] { "Madrid", "Vienna" };
            var otherElements = new string[] { elements[1], "Prague", elements[2], "Santo Domingo", moreElements[1] };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            set.Add(moreElements[0]);
            set.Add(moreElements[1]);

            // Act
            set.ExceptWith(otherElements);

            // Assert
            set.Count.Should().Be(3);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeFalse();
            set.Contains(elements[3]).Should().BeTrue();
            set.Contains(moreElements[0]).Should().BeTrue();
            set.Contains(moreElements[1]).Should().BeFalse();
            set.Should().ExposeDeletesFirst();
            set.Should().HaveEntryCount(5);
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Deleted);
            set.Should().ExposeEntry(elements[3], Status.Saved);
            set.Should().ExposeEntry(moreElements[0], Status.New);
            set.Should().NotExposeEntryFor(moreElements[1]);
        }

        [TestMethod] public void IntersectWithDisjoint() {
            // Arrange
            var lhsElements = new string[] { "Kyiv", "Johannesburg", "Toronto", "Belmopan" };
            var rhsElements = new string[] { "Amsterdam", "Glasgow", "Abu Dhabi", "Calcutta", "Harare" };
            var set = new RelationSet<string>(lhsElements);

            // Act
            set.IntersectWith(rhsElements);

            // Assert
            set.Count.Should().Be(0);
            set.Contains(lhsElements[0]).Should().BeFalse();
            set.Contains(lhsElements[1]).Should().BeFalse();
            set.Contains(lhsElements[2]).Should().BeFalse();
            set.Contains(lhsElements[3]).Should().BeFalse();
            set.Should().HaveEntryCount(0);
            set.Should().ExposeDeletesFirst();
            set.Should().NotExposeEntryFor(lhsElements[0]);
            set.Should().NotExposeEntryFor(lhsElements[1]);
            set.Should().NotExposeEntryFor(lhsElements[2]);
            set.Should().NotExposeEntryFor(lhsElements[3]);
        }

        [TestMethod] public void IntersectWithSelf() {
            // Arrange
            var elements = new string[] { "Nuku'alofa", "Tehran", "Hanoi", "Stockholm", "Odessa", "Marseille" };
            var set = new RelationSet<string>() { elements[0], elements[1], elements[2] };
            (set as IRelation).Canonicalize();
            set.Add(elements[3]);
            set.Add(elements[4]);
            set.Add(elements[5]);

            // Act
            set.IntersectWith(elements);

            // Assert
            set.Count.Should().Be(6);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeTrue();
            set.Contains(elements[4]).Should().BeTrue();
            set.Contains(elements[5]).Should().BeTrue();
            set.Should().HaveEntryCount(6);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.New);
            set.Should().ExposeEntry(elements[4], Status.New);
            set.Should().ExposeEntry(elements[5], Status.New);
        }

        [TestMethod] public void IntersectWithSubset() {
            // Arrange
            var elements = new string[] { "Quito", "Palermo", "Rio de Janeiro", "N'Djamena", "Addis Ababa" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();

            // Act
            set.IntersectWith(elements.Where(s => s.Contains("n")));

            // Assert
            set.Count.Should().Be(2);
            set.Contains(elements[0]).Should().BeFalse();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeTrue();
            set.Contains(elements[4]).Should().BeFalse();
            set.Should().HaveEntryCount(5);
            set.Should().ExposeEntry(elements[0], Status.Deleted);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.Saved);
            set.Should().ExposeEntry(elements[4], Status.Deleted);
        }

        [TestMethod] public void IntersectWithSuperset() {
            // Arrange
            var elements = new string[] { "Antananarivo", "Bogotá", "Haifa" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            var single = "Vientiane";
            set.Add(single);

            // Act
            set.IntersectWith(elements.Append(single).Append("Kaliningrad").Append("Riyadh").Append("Dar es Salaam"));

            // Assert
            set.Count.Should().Be(4);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(single).Should().BeTrue();
            set.Should().HaveEntryCount(4);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(single, Status.New);
        }

        [TestMethod] public void IntersectWithOverlap() {
            // Arrange
            var elements = new string[] { "Guangzhou", "Kyoto", "Oslo", "Copenhagen" };
            var moreElements = new string[] { "Tunis", "Montréal" };
            var otherElements = new string[] { elements[1], "Santiago", elements[2], "Lviv", moreElements[1] };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            set.Add(moreElements[0]);
            set.Add(moreElements[1]);

            // Act
            set.IntersectWith(otherElements);

            // Assert
            set.Count.Should().Be(3);
            set.Contains(elements[0]).Should().BeFalse();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeFalse();
            set.Contains(moreElements[0]).Should().BeFalse();
            set.Contains(moreElements[1]).Should().BeTrue();
            set.Should().ExposeDeletesFirst();
            set.Should().HaveEntryCount(5);
            set.Should().ExposeEntry(elements[0], Status.Deleted);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.Deleted);
            set.Should().ExposeEntry(moreElements[1], Status.New);
            set.Should().NotExposeEntryFor(moreElements[0]);
        }

        [TestMethod] public void UnionWithDisjoint() {
            // Arrange
            var lhsElements = new string[] { "Skopje", "Sofia", "Yerevan", "Muscat" };
            var rhsElements = new string[] { "Liverpool", "Port Moresby", "Jakarta", "Kabul", "Baku" };
            var set = new RelationSet<string>(lhsElements);

            // Act
            set.UnionWith(rhsElements);

            // Assert
            set.Count.Should().Be(9);
            set.Contains(lhsElements[0]).Should().BeTrue();
            set.Contains(lhsElements[1]).Should().BeTrue();
            set.Contains(lhsElements[2]).Should().BeTrue();
            set.Contains(lhsElements[3]).Should().BeTrue();
            set.Contains(rhsElements[0]).Should().BeTrue();
            set.Contains(rhsElements[1]).Should().BeTrue();
            set.Contains(rhsElements[2]).Should().BeTrue();
            set.Contains(rhsElements[3]).Should().BeTrue();
            set.Contains(rhsElements[4]).Should().BeTrue();
            set.Should().HaveEntryCount(9);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(lhsElements[0], Status.New);
            set.Should().ExposeEntry(lhsElements[1], Status.New);
            set.Should().ExposeEntry(lhsElements[2], Status.New);
            set.Should().ExposeEntry(lhsElements[3], Status.New);
            set.Should().ExposeEntry(rhsElements[0], Status.New);
            set.Should().ExposeEntry(rhsElements[1], Status.New);
            set.Should().ExposeEntry(rhsElements[2], Status.New);
            set.Should().ExposeEntry(rhsElements[3], Status.New);
            set.Should().ExposeEntry(rhsElements[4], Status.New);
        }

        [TestMethod] public void UnionWithSelf() {
            // Arrange
            var elements = new string[] { "Bucharest", "Mogadishu", "New Delhi", "Jerusalem", "Manchester", "Dubai" };
            var set = new RelationSet<string>() { elements[0], elements[1], elements[2] };
            (set as IRelation).Canonicalize();
            set.Add(elements[3]);
            set.Add(elements[4]);
            set.Add(elements[5]);

            // Act
            set.UnionWith(elements);

            // Assert
            set.Count.Should().Be(6);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeTrue();
            set.Contains(elements[4]).Should().BeTrue();
            set.Contains(elements[5]).Should().BeTrue();
            set.Should().HaveEntryCount(6);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.New);
            set.Should().ExposeEntry(elements[4], Status.New);
            set.Should().ExposeEntry(elements[5], Status.New);
        }

        [TestMethod] public void UnionWithSubset() {
            // Arrange
            var elements = new string[] { "Seville", "Managua", "San José", "Juba", "Nicosia" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();

            // Act
            set.UnionWith(elements.Where(s => s.Contains("a")));

            // Assert
            set.Count.Should().Be(5);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeTrue();
            set.Contains(elements[4]).Should().BeTrue();
            set.Should().HaveEntryCount(5);
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.Saved);
            set.Should().ExposeEntry(elements[4], Status.Saved);
        }

        [TestMethod] public void UnionWithSuperset() {
            // Arrange
            var elements = new string[] { "Rome", "Damascus", "Wellington" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            var single = "Singapore";
            set.Add(single);
            var moreElements = new string[] { "Kuala Lumpur", "Brazzaville", "Riga" };

            // Act
            set.UnionWith(elements.Append(single).Concat(moreElements));

            // Assert
            set.Count.Should().Be(7);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(single).Should().BeTrue();
            set.Contains(moreElements[0]).Should().BeTrue();
            set.Contains(moreElements[1]).Should().BeTrue();
            set.Contains(moreElements[2]).Should().BeTrue();
            set.Should().HaveEntryCount(7);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(single, Status.New);
            set.Should().ExposeEntry(moreElements[0], Status.New);
            set.Should().ExposeEntry(moreElements[1], Status.New);
            set.Should().ExposeEntry(moreElements[2], Status.New);
        }

        [TestMethod] public void UnionWithOverlap() {
            // Arrange
            var elements = new string[] { "Canberra", "Alexandria", "Lagos", "Islamabad" };
            var moreElements = new string[] { "Honiara", "Vancouver" };
            var otherElements = new string[] { elements[1], "Cancún", elements[2], "Manila", moreElements[1] };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            set.Add(moreElements[0]);
            set.Add(moreElements[1]);

            // Act
            set.UnionWith(otherElements);

            // Assert
            set.Count.Should().Be(8);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeTrue();
            set.Contains(elements[2]).Should().BeTrue();
            set.Contains(elements[3]).Should().BeTrue();
            set.Contains(moreElements[0]).Should().BeTrue();
            set.Contains(moreElements[1]).Should().BeTrue();
            set.Contains(otherElements[1]).Should().BeTrue();
            set.Contains(otherElements[3]).Should().BeTrue();
            set.Should().ExposeDeletesFirst();
            set.Should().HaveEntryCount(8);
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Saved);
            set.Should().ExposeEntry(elements[2], Status.Saved);
            set.Should().ExposeEntry(elements[3], Status.Saved);
            set.Should().ExposeEntry(moreElements[0], Status.New);
            set.Should().ExposeEntry(moreElements[1], Status.New);
            set.Should().ExposeEntry(otherElements[1], Status.New);
            set.Should().ExposeEntry(otherElements[3], Status.New);
        }

        [TestMethod] public void SymmetricExceptWithDisjoint() {
            // Arrange
            var lhsElements = new string[] { "Ottawa", "Baghdad", "Belgrade", "Montevideo" };
            var rhsElements = new string[] { "Tijuana", "Dakar", "Gaborone", "Medina", "Panama City" };
            var set = new RelationSet<string>(lhsElements);

            // Act
            set.SymmetricExceptWith(rhsElements);

            // Assert
            set.Count.Should().Be(9);
            set.Contains(lhsElements[0]).Should().BeTrue();
            set.Contains(lhsElements[1]).Should().BeTrue();
            set.Contains(lhsElements[2]).Should().BeTrue();
            set.Contains(lhsElements[3]).Should().BeTrue();
            set.Contains(rhsElements[0]).Should().BeTrue();
            set.Contains(rhsElements[1]).Should().BeTrue();
            set.Contains(rhsElements[2]).Should().BeTrue();
            set.Contains(rhsElements[3]).Should().BeTrue();
            set.Contains(rhsElements[4]).Should().BeTrue();
            set.Should().HaveEntryCount(9);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(lhsElements[0], Status.New);
            set.Should().ExposeEntry(lhsElements[1], Status.New);
            set.Should().ExposeEntry(lhsElements[2], Status.New);
            set.Should().ExposeEntry(lhsElements[3], Status.New);
            set.Should().ExposeEntry(rhsElements[0], Status.New);
            set.Should().ExposeEntry(rhsElements[1], Status.New);
            set.Should().ExposeEntry(rhsElements[2], Status.New);
            set.Should().ExposeEntry(rhsElements[3], Status.New);
            set.Should().ExposeEntry(rhsElements[4], Status.New);
        }

        [TestMethod] public void SymmetricExceptWithSelf() {
            // Arrange
            var elements = new string[] { "Sarajevo", "Nassau", "The Hague", "Ulaanbaatar", "Maseru", "Saskatoon" };
            var set = new RelationSet<string>() { elements[0], elements[1], elements[2] };
            (set as IRelation).Canonicalize();
            set.Add(elements[3]);
            set.Add(elements[4]);
            set.Add(elements[5]);

            // Act
            set.SymmetricExceptWith(elements);

            // Assert
            set.Count.Should().Be(0);
            set.Contains(elements[0]).Should().BeFalse();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeFalse();
            set.Contains(elements[3]).Should().BeFalse();
            set.Contains(elements[4]).Should().BeFalse();
            set.Contains(elements[5]).Should().BeFalse();
            set.Should().HaveEntryCount(3);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Deleted);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Deleted);
            set.Should().NotExposeEntryFor(elements[3]);
            set.Should().NotExposeEntryFor(elements[4]);
            set.Should().NotExposeEntryFor(elements[5]);
        }

        [TestMethod] public void SymmetricExceptWithSubset() {
            // Arrange
            var elements = new string[] { "Belfast", "Hobart", "Karachi", "Nairobi", "Suva" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();

            // Act
            set.SymmetricExceptWith(elements.Where(s => s.Contains("r")));

            // Assert
            set.Count.Should().Be(2);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeFalse();
            set.Contains(elements[3]).Should().BeFalse();
            set.Contains(elements[4]).Should().BeTrue();
            set.Should().HaveEntryCount(5);
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Deleted);
            set.Should().ExposeEntry(elements[3], Status.Deleted);
            set.Should().ExposeEntry(elements[4], Status.Saved);
        }

        [TestMethod] public void SymmetricExceptWithSuperset() {
            // Arrange
            var elements = new string[] { "Thessaloniki", "Monrovia", "Benghazi" };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            var single = "Asunción";
            set.Add(single);
            var moreElements = new string[] { "Turin", "Pamplona", "San Salvador" };

            // Act
            set.SymmetricExceptWith(elements.Append(single).Concat(moreElements));

            // Assert
            set.Count.Should().Be(3);
            set.Contains(elements[0]).Should().BeFalse();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeFalse();
            set.Contains(single).Should().BeFalse();
            set.Contains(moreElements[0]).Should().BeTrue();
            set.Contains(moreElements[1]).Should().BeTrue();
            set.Contains(moreElements[2]).Should().BeTrue();
            set.Should().HaveEntryCount(6);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(elements[0], Status.Deleted);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Deleted);
            set.Should().NotExposeEntryFor(single);
            set.Should().ExposeEntry(moreElements[0], Status.New);
            set.Should().ExposeEntry(moreElements[1], Status.New);
            set.Should().ExposeEntry(moreElements[2], Status.New);
        }

        [TestMethod] public void SymmetricExceptWithOverlap() {
            // Arrange
            var elements = new string[] { "Niamey", "Ramallah", "Djibouti", "Providenciales" };
            var moreElements = new string[] { "Tenerife", "Paramaribo" };
            var otherElements = new string[] { elements[1], "Recife", elements[2], "Incheon", moreElements[1] };
            var set = new RelationSet<string>(elements);
            (set as IRelation).Canonicalize();
            set.Add(moreElements[0]);
            set.Add(moreElements[1]);

            // Act
            set.SymmetricExceptWith(otherElements);

            // Assert
            set.Count.Should().Be(5);
            set.Contains(elements[0]).Should().BeTrue();
            set.Contains(elements[1]).Should().BeFalse();
            set.Contains(elements[2]).Should().BeFalse();
            set.Contains(elements[3]).Should().BeTrue();
            set.Contains(moreElements[0]).Should().BeTrue();
            set.Contains(moreElements[1]).Should().BeFalse();
            set.Contains(otherElements[1]).Should().BeTrue();
            set.Contains(otherElements[3]).Should().BeTrue();
            set.Should().ExposeDeletesFirst();
            set.Should().HaveEntryCount(7);
            set.Should().ExposeEntry(elements[0], Status.Saved);
            set.Should().ExposeEntry(elements[1], Status.Deleted);
            set.Should().ExposeEntry(elements[2], Status.Deleted);
            set.Should().ExposeEntry(elements[3], Status.Saved);
            set.Should().ExposeEntry(moreElements[0], Status.New);
            set.Should().NotExposeEntryFor(moreElements[1]);
            set.Should().ExposeEntry(otherElements[1], Status.New);
            set.Should().ExposeEntry(otherElements[3], Status.New);
        }

        [TestMethod] public void Repopulate() {
            // Arrange
            var set = new RelationSet<string>();
            var item0 = "Sri Jayawardenepura Kotte";
            var item1 = "Aachen";
            var item2 = "Banjul";
            var item3 = "Zanzibar City";

            // Act
            (set as IRelation).Repopulate(item0);
            (set as IRelation).Repopulate(item1);
            (set as IRelation).Repopulate(item2);
            (set as IRelation).Repopulate(item3);

            // Assert
            set.Count.Should().Be(4);
            set.Contains(item0).Should().BeTrue();
            set.Contains(item1).Should().BeTrue();
            set.Contains(item2).Should().BeTrue();
            set.Contains(item3).Should().BeTrue();
            set.Should().HaveEntryCount(4);
            set.Should().ExposeDeletesFirst();
            set.Should().ExposeEntry(item0, Status.Saved);
            set.Should().ExposeEntry(item1, Status.Saved);
            set.Should().ExposeEntry(item2, Status.Saved);
            set.Should().ExposeEntry(item3, Status.Saved);
        }
    }
}
