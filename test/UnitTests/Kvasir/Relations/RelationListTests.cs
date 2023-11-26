using FluentAssertions;
using Kvasir.Relations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace UT.Kvasir.Relations {
    [TestClass, TestCategory("RelationList")]
    public class RelationListTests {
        [TestMethod] public void DefaultConstruct() {
            // Arrange

            // Act
            var list = new RelationList<string>();

            // Assert
            list.Count.Should().Be(0);
            list.Should().HaveConnectionType<string>();
            (list as IReadOnlyRelationList<string>).Should().HaveConnectionType<string>();
            list.Should().HaveEntryCount(0);
        }

        [TestMethod] public void SetCapacity() {
            // Arrange
            var list = new RelationList<string>();
            var initialCapacity = list.Capacity;
            var newCapacity = initialCapacity + 200;

            // Act
            list.Capacity = newCapacity;

            // Assert
            list.Capacity.Should().Be(newCapacity);
        }

        [TestMethod] public void ConstructFromElements() {
            // Arrange
            var elements = new string[] { "Kalispell", "Bethesda", "Yuma", "Oregon City", "Unalaska" };

            // Act
            var list = new RelationList<string>(elements);

            // Assert
            list.Count.Should().Be(5);
            list[0].Should().Be(elements[0]);   (list as IList)[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);   (list as IList)[1].Should().Be(elements[1]);
            list[2].Should().Be(elements[2]);   (list as IList)[2].Should().Be(elements[2]);
            list[3].Should().Be(elements[3]);   (list as IList)[3].Should().Be(elements[3]);
            list[4].Should().Be(elements[4]);   (list as IList)[4].Should().Be(elements[4]);
            list.Capacity.Should().BeGreaterOrEqualTo(5);
            list.Should().HaveConnectionType<string>();
            list.Should().HaveEntryCount(elements.Length);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.New);
            list.Should().ExposeEntry(elements[1], Status.New);
            list.Should().ExposeEntry(elements[2], Status.New);
            list.Should().ExposeEntry(elements[3], Status.New);
            list.Should().ExposeEntry(elements[4], Status.New);
        }

        [TestMethod] public void ConstructFromCapacity() {
            // Arrange
            var capacity = 37;

            // Act
            var list = new RelationList<string>(capacity);

            // Assert
            list.Count.Should().Be(0);
            list.Capacity.Should().BeGreaterOrEqualTo(capacity);
            list.Should().HaveConnectionType<string>();
            list.Should().HaveEntryCount(0);
        }

        [TestMethod] public void CanonicalizeNoneDeleted() {
            // Arrange
            var elements = new string[] { "Evansville", "Dubuque", "Newport News", "Sitka" };
            var list = new RelationList<string>(elements);

            // Act
            (list as IRelation).Canonicalize();

            // Assert
            list.Count.Should().Be(4);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().ExposeEntry(elements[3], Status.Saved);
        }

        [TestMethod] public void RemoveExistingNewItem() {
            // Arrange
            var elements = new string[] { "Burlington", "Stockton", "College Station" };
            var list = new RelationList<string>(elements);

            // Act
            var success = list.Remove(elements[1]);
            (list as IList).Remove(elements[0]);

            // Assert
            list.Count.Should().Be(1);
            success.Should().BeTrue();
            list[0].Should().Be(elements[2]);
            list.Should().HaveEntryCount(1);
            list.Should().ExposeDeletesFirst();
            list.Should().NotExposeEntryFor(elements[0]);
            list.Should().NotExposeEntryFor(elements[1]);
            list.Should().ExposeEntry(elements[2], Status.New);
        }

        [TestMethod] public void RemoveExistingSavedItem() {
            // Arrange
            var elements = new string[] { "Hardin", "Kailua", "Youngstown" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();

            // Act
            var success = list.Remove(elements[0]);
            (list as IList).Remove(elements[2]);

            // Assert
            list.Count.Should().Be(1);
            success.Should().BeTrue();
            list[0].Should().Be(elements[1]);
            list.Should().HaveEntryCount(3);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Deleted);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Deleted);
        }

        [TestMethod] public void RemoveTwiceThenReplace() {
            // Arrange
            var elements = new string[] { "Morgantown", "Oxnard", "Daytona Beach", "Carmel" };
            var list = new RelationList<string>(elements);
            var duplicate = elements[1];
            list.Add(duplicate);
            (list as IRelation).Canonicalize();

            // Act
            var removals = list.RemoveAll(s => s == duplicate);
            list.Add(duplicate);

            // Assert
            removals.Should().Be(2);
            list.Count.Should().Be(4);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[2]);
            list[2].Should().Be(elements[3]);
            list[3].Should().Be(duplicate);
            list.Should().HaveEntryCount(5);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Deleted);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().ExposeEntry(elements[3], Status.Saved);
        }

        [TestMethod] public void RemoveNonexistingItem() {
            // Arrange
            var list = new RelationList<string>() { "Lake Charles", "Nashua" };

            // Act
            var success = list.Remove("Auburn");

            // Assert
            list.Count.Should().Be(2);
            success.Should().BeFalse();
        }

        [TestMethod] public void RemoveItemByIndex() {
            // Arrange
            var elements = new string[] { "Flint", "Kirkland", "Norman", "Alexandria" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();

            // Act
            list.RemoveAt(2);

            // Assert
            list.Count.Should().Be(3);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(elements[3]);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Deleted);
            list.Should().ExposeEntry(elements[3], Status.Saved);
        }

        [TestMethod] public void RemoveAtNegativeIndex() {
            // Arrange
            var list = new RelationList<string>() { "West Covina", "Palatine" };

            // Act
            Action act = () => list.RemoveAt(-3);

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void RemoveAtOverlargeIndex() {
            // Arrange
            var list = new RelationList<string>() { "Gulfport" };

            // Act
            Action act = () => list.RemoveAt(list.Count * 17);

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void RemoveRangeOfItems() {
            // Arrange
            var elements = new string[] { "Reston", "Rancho Cucamonga", "Lake Placid", "Eau Claire", "Worcester" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();

            // Act
            list.RemoveRange(1, 3);

            // Assert
            list.Count.Should().Be(2);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[4]);
            list.Should().HaveEntryCount(5);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Deleted);
            list.Should().ExposeEntry(elements[2], Status.Deleted);
            list.Should().ExposeEntry(elements[3], Status.Deleted);
            list.Should().ExposeEntry(elements[4], Status.Saved);
        }

        [TestMethod] public void RemoveRangeZeroItems() {
            // Arrange
            var elements = new string[] { "Waterloo", "Ketchikan", "Waimanalo", "North Platte" };
            var list = new RelationList<string>(elements);

            // Act
            list.RemoveRange(2, 0);

            // Assert
            list.Count.Should().Be(4);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(elements[2]);
            list[3].Should().Be(elements[3]);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.New);
            list.Should().ExposeEntry(elements[1], Status.New);
            list.Should().ExposeEntry(elements[2], Status.New);
            list.Should().ExposeEntry(elements[3], Status.New);
        }

        [TestMethod] public void RemoveInvalidRange() {
            // Arrange
            var list = new RelationList<string>() { "Fort Hood", "Manassas", "Narragansett Pier" };

            // Act
            Action act0 = () => list.RemoveRange(-3, 7);
            Action act1 = () => list.RemoveRange(1, -6);
            Action act2 = () => list.RemoveRange(2, 4);

            // Assert
            act0.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            act1.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            act2.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void Clear() {
            // Arrange
            var elements = new string[] { "Macon", "Brownsville", "Naperville" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var single = "Hollywood";
            list.Add(single);

            // Act
            list.Clear();

            // Assert
            list.Count.Should().Be(0);
            list.Should().HaveEntryCount(3);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Deleted);
            list.Should().ExposeEntry(elements[1], Status.Deleted);
            list.Should().ExposeEntry(elements[2], Status.Deleted);
            list.Should().NotExposeEntryFor(single);
        }

        [TestMethod] public void CanonicalizeSomeDeleted() {
            // Arrange
            var elements = new string[] { "Huntsville", "Pembroke Pines", "Champaign", "Bowling Green", "Casper" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            list.Remove(elements[1]);
            list.Remove(elements[3]);

            // Act
            (list as IRelation).Canonicalize();

            // Assert
            list.Count.Should().Be(3);
            list.Should().HaveEntryCount(3);
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().NotExposeEntryFor(elements[1]);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().NotExposeEntryFor(elements[3]);
            list.Should().ExposeEntry(elements[4], Status.Saved);
        }

        [TestMethod] public void AddNewItem() {
            // Arrange
            var list = new RelationList<string>();
            var element0 = "High Point";
            var element1 = "Aynor";

            // Act
            list.Add(element0);
            var position = (list as IList).Add(element1);

            // Assert
            position.Should().Be(1);
            list.Count.Should().Be(2);
            list[0].Should().Be(element0);
            list[1].Should().Be(element1);
            list.Should().HaveEntryCount(2);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(element0, Status.New);
            list.Should().ExposeEntry(element1, Status.New);
        }

        [TestMethod] public void AddExistingNewItem() {
            // Arrange
            var elements = new string[] { "Pocatello", "Wheeling" };
            var list = new RelationList<string>(elements);
            var repeat = list[0];

            // Act
            list.Add(repeat);
            var position = (list as IList).Add(repeat);

            // Assert
            position.Should().Be(3);
            list.Count.Should().Be(4);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(repeat);
            list[3].Should().Be(repeat);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(repeat, Status.New, 3);
        }

        [TestMethod] public void AddExitingSavedItem() {
            // Arrange
            var elements = new string[] { "Pawtucket", "Kettering", "Joplin" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var repeat = list[2];

            // Act
            list.Add(repeat);
            var position = (list as IList).Add(repeat);

            // Assert
            position.Should().Be(4);
            list.Count.Should().Be(5);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(elements[2]);
            list[3].Should().Be(repeat);
            list[4].Should().Be(repeat);
            list.Should().HaveEntryCount(5);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(repeat, Status.Saved);
            list.Should().ExposeEntry(repeat, Status.New, 2);
        }

        [TestMethod] public void AddExistingDeletedItem() {
            // Arrange
            var elements = new string[] { "Berkeley", "Erie", "Altoona" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var deleted = elements[1];
            list.Remove(deleted);

            // Act
            list.Add(deleted);

            // Assert
            list.Count.Should().Be(3);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[2]);
            list[2].Should().Be(deleted);
            list.Should().HaveEntryCount(3);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(deleted, Status.Saved);
        }

        [TestMethod] public void AddRangeNewItems() {
            // Arrange
            var elements = new string[] { "Fullerton", "Texas City" };
            var list = new RelationList<string>();

            // Act
            list.AddRange(elements);

            // Assert
            list.Count.Should().Be(2);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list.Should().HaveEntryCount(2);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.New);
            list.Should().ExposeEntry(elements[1], Status.New);
        }

        [TestMethod] public void AddRangeExistingNewItems() {
            // Arrange
            var elements = new string[] { "Normal", "Northridge", "Mount Pleasant" };
            var list = new RelationList<string>(elements);

            // Act
            list.AddRange(elements);

            // Assert
            list.Count.Should().Be(6);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(elements[2]);
            list[3].Should().Be(elements[0]);
            list[4].Should().Be(elements[1]);
            list[5].Should().Be(elements[2]);
            list.Should().HaveEntryCount(6);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.New, 2);
            list.Should().ExposeEntry(elements[1], Status.New, 2);
            list.Should().ExposeEntry(elements[2], Status.New, 2);
        }

        [TestMethod] public void AddRangeExistingSavedItems() {
            // Arrange
            var elements = new string[] { "Fort Collins", "Lynchburg" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();

            // Act
            list.AddRange(elements);

            // Assert
            list.Count.Should().Be(4);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(elements[0]);
            list[3].Should().Be(elements[1]);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[0], Status.New);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.New);
        }

        [TestMethod] public void AddRangeExistingDeletedItems() {
            // Arrange
            var elements = new string[] { "Storrs", "Cedar Falls", "Bayamón" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            list.Clear();

            // Act
            list.AddRange(elements);

            // Assert
            list.Count.Should().Be(3);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(elements[2]);
            list.Should().ExposeDeletesFirst();
            list.Should().HaveEntryCount(3);
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Saved);
        }

        [TestMethod] public void InsertNewItem() {
            // Arrange
            var elements = new string[] { "Farmington Hills", "Terre Haute", "Tupelo" };
            var list = new RelationList<string>(elements);
            var first_insertion = "Kaneohe";
            var second_insertion = "Guaynabo";

            // Act
            list.Insert(1, first_insertion);
            (list as IList).Insert(1, second_insertion);

            // Assert
            list.Count.Should().Be(5);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(second_insertion);
            list[2].Should().Be(first_insertion);
            list[3].Should().Be(elements[1]);
            list[4].Should().Be(elements[2]);
            list.Should().HaveEntryCount(5);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(first_insertion, Status.New);
            list.Should().ExposeEntry(second_insertion, Status.New);
        }

        [TestMethod] public void InsertExistingNewItem() {
            // Arrange
            var elements = new string[] { "Whitefish", "Fayetteville" };
            var list = new RelationList<string>(elements);
            var repeat = list[0];

            // Act
            list.Insert(0, repeat);
            (list as IList).Insert(0, repeat);

            // Assert
            list.Count.Should().Be(4);
            list[0].Should().Be(repeat);
            list[1].Should().Be(repeat);
            list[2].Should().Be(elements[0]);
            list[3].Should().Be(elements[1]);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(repeat, Status.New, 3);
        }

        [TestMethod] public void InsertExitingSavedItem() {
            // Arrange
            var elements = new string[] { "Waterbury", "Toms River", "Decatur" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var repeat = list[2];

            // Act
            list.Insert(2, repeat);
            (list as IList).Insert(2, repeat);

            // Assert
            list.Count.Should().Be(5);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(repeat);
            list[3].Should().Be(repeat);
            list[4].Should().Be(elements[2]);
            list.Should().HaveEntryCount(5);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(repeat, Status.Saved);
            list.Should().ExposeEntry(repeat, Status.New, 2);
        }

        [TestMethod] public void InsertExistingDeletedItem() {
            // Arrange
            var elements = new string[] { "Rochester", "Stamford", "Kennewick" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var deleted = elements[1];
            list.Remove(deleted);

            // Act
            list.Insert(0, deleted);

            // Assert
            list.Count.Should().Be(3);
            list[0].Should().Be(elements[1]);
            list[1].Should().Be(elements[0]);
            list[2].Should().Be(elements[2]);
            list.Should().HaveEntryCount(3);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(deleted, Status.Saved);
        }

        [TestMethod] public void InsertNegativeIndex() {
            // Arrange
            var list = new RelationList<string>() { "Lander", "East Hartford" };

            // Act
            Action first_act = () => list.Insert(-4, "Shaker Heights");
            Action second_act = () => (list as IList).Insert(-182, "Vicksburg");

            // Assert
            first_act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            second_act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void InsertOverlargeIndex() {
            // Arrange
            var list = new RelationList<string>();

            // Act
            Action act = () => list.Insert(9, "Danbury");

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void InsertRangeOfItemsAllNew() {
            // Arrange
            var elements = new string[] { "Wasilla", "Pine Bluff" };
            var list = new RelationList<string>(elements);
            var insertions = new string[] { "Minnetonka", "Bristol" };

            // Act
            list.InsertRange(1, insertions);

            // Assert
            list.Count.Should().Be(4);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(insertions[0]);
            list[2].Should().Be(insertions[1]);
            list[3].Should().Be(elements[1]);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.New);
            list.Should().ExposeEntry(elements[1], Status.New);
            list.Should().ExposeEntry(insertions[0], Status.New);
            list.Should().ExposeEntry(insertions[1], Status.New);
        }

        [TestMethod] public void InsertRangeOfItemsSomeDuplicates() {
            // Arrange
            var elements = new string[] { "Lima", "Germantown", "Cairo" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var single = "Zzyzx";
            list.Add(single);
            var insertions = new string[] { "Cape Girardeau", "Yonkers" };

            // Act
            list.InsertRange(2, insertions);

            // Assert
            list.Count.Should().Be(6);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(insertions[0]);
            list[3].Should().Be(insertions[1]);
            list[4].Should().Be(elements[2]);
            list[5].Should().Be(single);
            list.Should().HaveEntryCount(6);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().ExposeEntry(single, Status.New);
            list.Should().ExposeEntry(insertions[0], Status.New);
            list.Should().ExposeEntry(insertions[1], Status.New);
        }

        [TestMethod] public void InsertRangeBringBackDeleted() {
            // Arrange
            var elements = new string[] { "Burbank", "Kenosha", "Levittown" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            list.Remove(elements[1]);
            var insertions = new string[] { "Spearfish", elements[1], "Grand Forks" };

            // Act
            list.InsertRange(1, insertions);

            // Assert
            list.Count.Should().Be(5);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(insertions[0]);
            list[2].Should().Be(insertions[1]);
            list[3].Should().Be(insertions[2]);
            list[4].Should().Be(elements[2]);
            list.Should().HaveEntryCount(5);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().ExposeEntry(insertions[0], Status.New);
            list.Should().ExposeEntry(insertions[2], Status.New);
        }

        [TestMethod] public void InsertNullRange() {
            // Arrange
            var list = new RelationList<string>();

            // Act
            Action act = () => list.InsertRange(0, null!);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>().WithAnyMessage();
        }

        [TestMethod] public void InsertInvalidRange() {
            // Arrange
            var list = new RelationList<string>() { "Pontiac", "Quincy", "Tafuna" };

            // Act
            var insertion = new string[] { "Christiansted", "Miramar" };
            Action act0 = () => list.InsertRange(-189, insertion);
            Action act1 = () => list.InsertRange(list.Count * 3, insertion);

            // Assert
            act0.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            act1.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void OvewriteNewSelfWithSelf() {
            // Arrange
            var elements = new string[] { "Carbondale", "Charlottesville", "Port St. Lucie" };
            var list = new RelationList<string>(elements);
            var single = "Fort Myers";
            var replacement = elements[1];
            (list as IRelation).Canonicalize();
            list.Add(single);

            // Act
            list[^1] = single;
            list[1] = replacement;

            // Assert
            list.Count.Should().Be(4);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(elements[2]);
            list[3].Should().Be(single);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().ExposeEntry(single, Status.New);
        }

        [TestMethod] public void OverwriteSavedSelfWithSelf() {
            // Arrange
            var elements = new string[] { "Hagåtña", "Dededo", "Charlotte Amalie" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();

            // Act
            list[0] = elements[0];
            list[1] = elements[1];
            list[2] = elements[2];

            // Assert
            list.Count.Should().Be(3);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(elements[2]);
            list.Should().HaveEntryCount(3);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Saved);
        }

        [TestMethod] public void OverwriteNewWithNonexistent() {
            // Arrange
            var elements = new string[] { "Malibu", "Hattiesburg", "Macomb" };
            var list = new RelationList<string>(elements);
            var replacement = "Iowa City";

            // Act
            list[2] = replacement;

            // Assert
            list.Count.Should().Be(3);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(replacement);
            list.Should().HaveEntryCount(3);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.New);
            list.Should().ExposeEntry(elements[1], Status.New);
            list.Should().ExposeEntry(replacement, Status.New);
            list.Should().NotExposeEntryFor(elements[2]);
        }

        [TestMethod] public void OverwriteNewWithNew() {
            // Arrange
            var elements = new string[] { "Urbana", "Missoula" };
            var list = new RelationList<string>(elements);
            var replacement = "West Des Moines";

            // Act
            list[0] = elements[1];
            (list as IList)[1] = replacement;

            // Assert
            list.Count.Should().Be(2);
            list[0].Should().Be(elements[1]);
            list[1].Should().Be(replacement);
            list.Should().HaveEntryCount(2);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[1], Status.New);
            list.Should().ExposeEntry(replacement, Status.New);
            list.Should().NotExposeEntryFor(elements[0]);
        }

        [TestMethod] public void OverwriteNewWithSaved() {
            // Arrange
            var elements = new string[] { "Oxford", "New Rochelle", "Des Plaines" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var single = "San Luis Obispo";
            list.Add(single);

            // Act
            list[^1] = list[1];

            // Assert
            list.Count.Should().Be(4);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(elements[2]);
            list[3].Should().Be(elements[1]);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.New);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().NotExposeEntryFor(single);
        }

        [TestMethod] public void OverwriteNewWithDeleted() {
            // Arrange
            var elements = new string[] { "West Point", "Chapel Hill" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var single = "Santa Barbara";
            list.RemoveAt(0);
            list.Add(single);

            // Act
            list[^1] = elements[0];

            // Assert
            list.Count.Should().Be(2);
            list[0].Should().Be(elements[1]);
            list[1].Should().Be(elements[0]);
            list.Should().HaveEntryCount(2);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().NotExposeEntryFor(single);
        }

        [TestMethod] public void OverwriteSavedWithNonexistent() {
            // Arrange
            var elements = new string[] { "Eureka" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var single = "Coral Gables";

            // Act
            list[0] = single;

            // Assert
            list.Count.Should().Be(1);
            list[0].Should().Be(single);
            list.Should().HaveEntryCount(2);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Deleted);
            list.Should().ExposeEntry(single, Status.New);
        }

        [TestMethod] public void OverwriteSavedWithNew() {
            // Arrange
            var elements = new string[] { "San Juan", "Pago Pago" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var single = "Saipan";
            list.Add(single);

            // Act
            list[0] = single;

            // Assert
            list.Count.Should().Be(3);
            list[0].Should().Be(single);
            list[1].Should().Be(elements[1]);
            list[2].Should().Be(single);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Deleted);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(single, Status.New, 2);
        }

        [TestMethod] public void OverwriteSavedWithSaved() {
            // Arrange
            var elements = new string[] { "Andersonville", "Nauvoo" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();

            // Act
            list[1] = list[0];

            // Assert
            list.Count.Should().Be(2);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(elements[0]);
            list.Should().HaveEntryCount(3);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[0], Status.New);
            list.Should().ExposeEntry(elements[1], Status.Deleted);
        }

        [TestMethod] public void OverwriteSavedWithDeleted() {
            // Arrange
            var elements = new string[] { "Key West", "Wabaunsee" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            list.Remove(elements[0]);

            // Act
            list[0] = elements[0];

            // Assert
            list.Count.Should().Be(1);
            list[0].Should().Be(elements[0]);
            list.Should().HaveEntryCount(2);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Deleted);
        }

        [TestMethod] public void Sort() {
            // Arrange
            var elements = new string[] { "Jackson Hole", "Irving", "Kokomo" };
            var list = new RelationList<string>(elements);
            var single0 = "Seward";
            var single1 = "Lahaina";
            (list as IRelation).Canonicalize();
            list.Insert(0, single0);
            list.Add(single1);

            // Act
            list.Remove(elements[1]);
            list.Sort();

            // Assert
            list.Count.Should().Be(4);
            list[0].Should().Be("Jackson Hole");
            list[1].Should().Be("Kokomo");
            list[2].Should().Be("Lahaina");
            list[3].Should().Be("Seward");
            list.Should().HaveEntryCount(5);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Deleted);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().ExposeEntry(single0, Status.New);
            list.Should().ExposeEntry(single1, Status.New);
        }

        [TestMethod] public void SortCustomComparer() {
            // Arrange
            var elements = new string[] { "paterson", "Owensboro", "deKalb" };
            var list = new RelationList<string>(elements);
            var single0 = "Smyrna";
            var single1 = "buena Vista";
            (list as IRelation).Canonicalize();
            list.Add(single0);
            list.Add(single1);

            // Act
            list.Sort(StringComparer.OrdinalIgnoreCase);

            // Assert
            list.Count.Should().Be(5);
            list[0].Should().Be("buena Vista");
            list[1].Should().Be("deKalb");
            list[2].Should().Be("Owensboro");
            list[3].Should().Be("paterson");
            list[4].Should().Be("Smyrna");
            list.Should().HaveEntryCount(5);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().ExposeEntry(single0, Status.New);
            list.Should().ExposeEntry(single1, Status.New);
        }

        [TestMethod] public void SortSubrange() {
            // Arrange
            var elements = new string[] {
                "Metairie", "Georgetown", "Schenectady", "La Crosse", "Alomogordo", "Frankfort", "Langley"
            };
            var list = new RelationList<string>(elements);

            // Act
            list.Sort(2, 4, StringComparer.Ordinal);

            // Assert
            list.Count.Should().Be(7);
            list[0].Should().Be("Metairie");
            list[1].Should().Be("Georgetown");
            list[2].Should().Be("Alomogordo");
            list[3].Should().Be("Frankfort");
            list[4].Should().Be("La Crosse");
            list[5].Should().Be("Schenectady");
            list[6].Should().Be("Langley");
            list.Should().HaveEntryCount(7);
            list.Should().ExposeEntry(elements[0], Status.New);
            list.Should().ExposeEntry(elements[1], Status.New);
            list.Should().ExposeEntry(elements[2], Status.New);
            list.Should().ExposeEntry(elements[3], Status.New);
            list.Should().ExposeEntry(elements[4], Status.New);
            list.Should().ExposeEntry(elements[5], Status.New);
            list.Should().ExposeEntry(elements[6], Status.New);
        }

        [TestMethod] public void Reverse() {
            // Arrange
            var elements = new string[] { "Butte", "Appleton", "Novi", };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var single = "Oshkosh";
            list.Add(single);

            // Act
            list.Reverse();

            // Assert
            list.Count.Should().Be(4);
            list[0].Should().Be(single);
            list[1].Should().Be(elements[^1]);
            list[2].Should().Be(elements[^2]);
            list[3].Should().Be(elements[^3]);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().ExposeEntry(single, Status.New);
        }

        [TestMethod] public void ReverseSubsequence() {
            // Arrange
            var elements = new string[] { "Racine", "Tahlequa", "St. Cloud" };
            var list = new RelationList<string>(elements);
            (list as IRelation).Canonicalize();
            var single = "Florida City";
            list.Insert(2, single);

            // Act
            list.Reverse(1, 2);

            // Assert
            list.Count.Should().Be(4);
            list[0].Should().Be(elements[0]);
            list[1].Should().Be(single);
            list[2].Should().Be(elements[1]);
            list[3].Should().Be(elements[2]);
            list.Should().HaveEntryCount(4);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(elements[0], Status.Saved);
            list.Should().ExposeEntry(elements[1], Status.Saved);
            list.Should().ExposeEntry(elements[2], Status.Saved);
            list.Should().ExposeEntry(single, Status.New);
        }

        [TestMethod] public void RepopulateSingleItem() {
            // Arrange
            var list = new RelationList<string>();
            var item = "Bridgetown";

            // Act
            (list as IRelation).Repopulate(item);

            // Assert
            list.Count.Should().Be(1);
            list[0].Should().Be(item);
            list.Should().HaveEntryCount(1);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(item, Status.Saved);
        }

        [TestMethod] public void RepopulateMultipleItems() {
            // Arrange
            var list = new RelationList<string>();
            var item0 = "Lomé";
            var item1 = "Acapulco";
            var item2 = "Yaren";

            // Act
            (list as IRelation).Repopulate(item0);
            (list as IRelation).Repopulate(item1);
            (list as IRelation).Repopulate(item2);

            // Assert
            list.Count.Should().Be(3);
            list[0].Should().Be(item0);
            list[1].Should().Be(item1);
            list[2].Should().Be(item2);
            list.Should().HaveEntryCount(3);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(item0, Status.Saved);
            list.Should().ExposeEntry(item1, Status.Saved);
            list.Should().ExposeEntry(item2, Status.Saved);
        }

        [TestMethod] public void RepopulateMultipleIdenticalItems() {
            // Arrange
            var list = new RelationList<string>();
            var item = "Tegucigalpa";

            // Act
            (list as IRelation).Repopulate(item);
            (list as IRelation).Repopulate(item);

            // Assert
            list.Count.Should().Be(2);
            list[0].Should().Be(item);
            list[1].Should().Be(item);
            list.Should().ExposeDeletesFirst();
            list.Should().ExposeEntry(item, Status.Saved, 2);
        }
    }
}
