using FluentAssertions;
using Kvasir.Relations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

using KVP = System.Collections.Generic.KeyValuePair<uint, string>;

namespace UT.Kvasir.Relations {
    [TestClass, TestCategory("RelationOrderedList")]
    public class RelationOrderedListTests {
        [TestMethod] public void DefaultConstruct() {
            // Arrange

            // Act
            var orderedList = new RelationOrderedList<string>();

            // Assert
            orderedList.Count.Should().Be(0);
            orderedList.Should().HaveConnectionType<KVP>();
            (orderedList as IReadOnlyRelationOrderedList<string>).Should().HaveConnectionType<KVP>();
            orderedList.Should().HaveEntryCount(0);
        }

        [TestMethod] public void SetCapacity() {
            // Arrange
            var orderedList = new RelationOrderedList<string>();
            var initialCapacity = orderedList.Capacity;
            var newCapacity = initialCapacity + 200;

            // Act
            orderedList.Capacity = newCapacity;

            // Assert
            orderedList.Capacity.Should().Be(newCapacity);
        }

        [TestMethod] public void ConstructFromElements() {
            // Arrange
            var elements = new string[] { "Ba Sing Se", "Minas Tirith", "Tar Valon", "Springfield", "Dimmsdale" };

            // Act
            var orderedList = new RelationOrderedList<string>(elements);

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(elements[0]); (orderedList as IList)[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]); (orderedList as IList)[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]); (orderedList as IList)[2].Should().Be(elements[2]);
            orderedList[3].Should().Be(elements[3]); (orderedList as IList)[3].Should().Be(elements[3]);
            orderedList[4].Should().Be(elements[4]); (orderedList as IList)[4].Should().Be(elements[4]);
            orderedList.Capacity.Should().BeGreaterOrEqualTo(5);
            orderedList.Should().HaveConnectionType<KVP>();
            orderedList.Should().HaveEntryCount(elements.Length);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(3, elements[3]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(4, elements[4]), Status.New);
        }

        [TestMethod] public void ConstructFromCapacity() {
            // Arrange
            var capacity = 1116;

            // Act
            var orderedList = new RelationOrderedList<string>(capacity);

            // Assert
            orderedList.Count.Should().Be(0);
            orderedList.Capacity.Should().BeGreaterOrEqualTo(capacity);
            orderedList.Should().HaveConnectionType<KVP>();
            orderedList.Should().HaveEntryCount(0);
        }

        [TestMethod] public void CanonicalizeAllNew() {
            // Arrange
            var elements = new string[] { "Camelot", "Sunnydale", "Theed", "Zoombiniville" };
            var orderedList = new RelationOrderedList<string>(elements);

            // Act
            (orderedList as IRelation).Canonicalize();

            // Assert
            orderedList.Count.Should().Be(4);
            orderedList.Should().HaveEntryCount(4);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, elements[3]), Status.Saved);
        }

        [TestMethod] public void OverwriteNewWithSelf() {
            // Arrange
            var elements = new string[] { "Faar", "Ravnica", "Emon", "South Park", "R'lyeh" };
            var orderedList = new RelationOrderedList<string>(elements);

            // Act
            orderedList[1] = elements[1];
            (orderedList as IList)[3] = elements[3];

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList[3].Should().Be(elements[3]);
            orderedList[4].Should().Be(elements[4]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(3, elements[3]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(4, elements[4]), Status.New);
        }

        [TestMethod] public void OverwiteNewWithNew() {
            // Arrange
            var elements = new string[] { "Quahog", "Luthadel", "Hogsmeade" };
            var orderedList = new RelationOrderedList<string>(elements);
            var single = "Emerald City";

            // Act
            orderedList[2] = single;

            // Assert
            orderedList.Count.Should().Be(3);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(single);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(2, single), Status.New);
        }

        [TestMethod] public void OverwiteSavedWithSelf() {
            // Arrange
            var elements = new string[] { "King's Landing", "Ankh Morpork" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList[0] = elements[0];

            // Assert
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList.Should().HaveEntryCount(2);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
        }

        [TestMethod] public void OverwriteSavedWithNew() {
            // Arrange
            var elements = new string[] { "Amity Park", "Zootopia", "Duloc", "Katolis" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var single = "Republic City";

            // Act
            orderedList[1] = single;

            // Assert
            orderedList.Count.Should().Be(4);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(single);
            orderedList[2].Should().Be(elements[2]);
            orderedList[3].Should().Be(elements[3]);
            orderedList.Should().HaveEntryCount(4);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, single), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, elements[3]), Status.Saved);
        }

        [TestMethod] public void OvewriteSavedThenRevert() {
            // Arrange
            var elements = new string[] { "Haven City", "Elysium", "Bikini Bottom" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var single = "White London";

            // Act
            orderedList[0] = single;
            orderedList[0] = elements[0];

            // Assert
            orderedList.Count.Should().Be(3);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
        }

        [TestMethod] public void OverwriteModifiedWithSelf() {
            // Arrange
            var elements = new string[] { "Cair Paravel", "Gotham City" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var single = "Tír na nÓg";

            // Act
            orderedList[1] = single;
            orderedList[1] = single;

            // Assert
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(single);
            orderedList.Should().HaveEntryCount(2);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, single), Status.Modified);
        }

        [TestMethod] public void OvewriteModifiedWithNew() {
            // Arrange
            var elements = new string[] { "Atlantis", "Whoville", "Ellesméra" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var first = "Danville";
            var second = "Bedrock";

            // Act
            orderedList[1] = first;
            orderedList[1] = second;

            // Assert
            orderedList.Count.Should().Be(3);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(second);
            orderedList[2].Should().Be(elements[2]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, second), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
        }

        [TestMethod] public void CanonicalizeSomeModified() {
            // Arrange
            var elements = new string[] { "Retroville", "Zion", "Castle Rock", "Cerulean City", "Demonreach" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var single = "Middlemarch";

            // Act
            orderedList[3] = single;
            (orderedList as IRelation).Canonicalize();

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList[3].Should().Be(single);
            orderedList[4].Should().Be(elements[4]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, single), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(4, elements[4]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
        }

        [TestMethod] public void RemoveExistingNewFromEnd() {
            // Arrange
            var elements = new string[] { "El Dorado", "Pawnee", "Smallville", "Hohman" };
            var orderedList = new RelationOrderedList<string>(elements);

            // Act
            orderedList.Remove(elements[^1]);
            (orderedList as IList).Remove(elements[^2]);

            // Assert
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList.Should().HaveEntryCount(2);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.New);
        }

        [TestMethod] public void RemoveExistingSavedFromEnd() {
            // Arrange
            var elements = new string[] { "Stars Hollow", "Elantris", "Stain'd-by-the-Sea" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.Remove(elements[^1]);

            // Assert
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Deleted);
        }

        [TestMethod] public void RemoveExistingModifiedFromEnd() {
            // Arrange
            var elements = new string[] { "Camp Half-Blood", "Duckburg", "Asgard" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var single = "Neverwinter";

            // Act
            orderedList[^1] = single;
            orderedList.Remove(single);

            // Assert
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Deleted);
        }

        [TestMethod] public void RemoveExistingFromFrontAllShiftedItemsNew() {
            // Arrange
            var elements = new string[] { "Kamar-Taj", "Orbit City", "The Citadel", "Cloud Cuckoo Land" };
            var orderedList = new RelationOrderedList<string>(elements);

            // Act
            orderedList.Remove(elements[0]);

            // Assert
            orderedList.Count.Should().Be(3);
            orderedList[0].Should().Be(elements[1]);
            orderedList[1].Should().Be(elements[2]);
            orderedList[2].Should().Be(elements[3]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[1]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, elements[2]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(2, elements[3]), Status.New);
        }

        [TestMethod] public void RemoveExistingFromFrontAllShiftedItemsSaved() {
            // Arrange
            var elements = new string[] { "Elwood City", "Arborlon", "Motonui", "Alta Base", "Nicodranas" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.Remove(elements[0]);

            // Assert
            orderedList.Count.Should().Be(4);
            orderedList[0].Should().Be(elements[1]);
            orderedList[1].Should().Be(elements[2]);
            orderedList[2].Should().Be(elements[3]);
            orderedList[3].Should().Be(elements[4]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[1]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(1, elements[2]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, elements[3]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, elements[4]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(4, elements[4]), Status.Deleted);
        }

        [TestMethod] public void RemoveExistingFromMiddleAllShiftedItemsNew() {
            // Arrange
            var elements = new string[] { "Bridgehead", "Aquari-Yum", "Kakariko Village", "Domino City" };
            var orderedList = new RelationOrderedList<string>(elements);

            // Act
            orderedList.Remove(elements[1]);

            // Assert
            orderedList.Count.Should().Be(3);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[2]);
            orderedList[2].Should().Be(elements[3]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, elements[2]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(2, elements[3]), Status.New);
        }

        [TestMethod] public void RemoveExistingFromMiddleAllShiftedItemsSaved() {
            // Arrange
            var elements = new string[] { "Twin Peaks", "Nalhalla", "Fraggle Rock", "Vice City", "SimCity", "Gravity Falls" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.Remove(elements[2]);

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[3]);
            orderedList[3].Should().Be(elements[4]);
            orderedList[4].Should().Be(elements[5]);
            orderedList.Should().HaveEntryCount(6);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[3]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, elements[4]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(4, elements[5]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(5, elements[5]), Status.Deleted);
        }

        [TestMethod] public void RemoveExistingNotAllShiftedItemsChanged() {
            // Arrange
            var elements = new string[] { "Middleton", "Monstropolis", "West Egg", "West Egg", "West Egg", "Tilted Towers", "Riverdale" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.Remove("West Egg");

            // Assert
            orderedList.Count.Should().Be(6);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[3]);
            orderedList[3].Should().Be(elements[4]);
            orderedList[4].Should().Be(elements[5]);
            orderedList[5].Should().Be(elements[6]);
            orderedList.Should().HaveEntryCount(7);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[3]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, elements[4]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(4, elements[5]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(5, elements[6]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(6, elements[6]), Status.Deleted);
        }

        [TestMethod] public void RemoveByIndexNewFromEnd() {
            // Arrange
            var elements = new string[] { "Goldenrod City", "Knapford", "Cittàgazze" };
            var orderedList = new RelationOrderedList<string>(elements);

            // Act
            orderedList.RemoveAt(orderedList.Count - 1);

            // Assert
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList.Should().HaveEntryCount(2);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.New);
        }

        [TestMethod] public void RemoveByIndexSavedFromEnd() {
            // Arrange
            var elements = new string[] { "Elendel", "Király Szek", "Hawkins" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.RemoveAt(orderedList.Count - 1);

            // Assert
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Deleted);
        }

        [TestMethod] public void RemoveByIndexModifiedFromEnd() {
            // Arrange
            var elements = new string[] { "Sootopolis City", "Brigadoon", "Ard Carraigh" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var single = "Coolsville";

            // Act
            orderedList[^1] = single;
            orderedList.RemoveAt(orderedList.Count - 1);

            // Assert
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Deleted);
        }

        [TestMethod] public void RemoveByIndexFromFrontAllShiftedItemsNew() {
            // Arrange
            var elements = new string[] { "Celeseteville", "Point Place", "New New York City", "Arrakeen" };
            var orderedList = new RelationOrderedList<string>(elements);

            // Act
            orderedList.RemoveAt(0);

            // Assert
            orderedList.Count.Should().Be(3);
            orderedList[0].Should().Be(elements[1]);
            orderedList[1].Should().Be(elements[2]);
            orderedList[2].Should().Be(elements[3]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[1]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, elements[2]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(2, elements[3]), Status.New);
        }

        [TestMethod] public void RemoveByIndexFromFrontAllShiftedItemsSaved() {
            // Arrange
            var elements = new string[] { "Verdansk", "Omi Town", "Amaravati", "Shangri-La", "Zuldazar" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.RemoveAt(0);

            // Assert
            orderedList.Count.Should().Be(4);
            orderedList[0].Should().Be(elements[1]);
            orderedList[1].Should().Be(elements[2]);
            orderedList[2].Should().Be(elements[3]);
            orderedList[3].Should().Be(elements[4]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[1]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(1, elements[2]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, elements[3]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, elements[4]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(4, elements[4]), Status.Deleted);
        }

        [TestMethod] public void RemoveByIndexFromMiddleAllShiftedItemsNew() {
            // Arrange
            var elements = new string[] { "Halloween Town", "Konohagakure", "Townsville", "Klow" };
            var orderedList = new RelationOrderedList<string>(elements);

            // Act
            orderedList.RemoveAt(1);

            // Assert
            orderedList.Count.Should().Be(3);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[2]);
            orderedList[2].Should().Be(elements[3]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, elements[2]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(2, elements[3]), Status.New);
        }

        [TestMethod] public void RemoveByIndexFromMiddleAllShiftedItemsSaved() {
            // Arrange
            var elements = new string[] { "Bitanga", "Hill Valley", "Metroville", "Arlen", "Bedford Falls", "Tarbean" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.RemoveAt(2);

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[3]);
            orderedList[3].Should().Be(elements[4]);
            orderedList[4].Should().Be(elements[5]);
            orderedList.Should().HaveEntryCount(6);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[3]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, elements[4]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(4, elements[5]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(5, elements[5]), Status.Deleted);
        }

        [TestMethod] public void RemoveAtNegativeIndex_IsError() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() { "Capeside", "ShiKahr" };

            // Act
            Action act = () => orderedList.RemoveAt(-92);

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void RemoveAtOverlargeIndex_IsError() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() { "Jrusar" };

            // Act
            Action act = () => orderedList.RemoveAt(orderedList.Count * 451902);

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void RemoveRangeOfItems() {
            // Arrange
            var elements = new string[] { "Kokaua Town", "Urithiru", "Spook City", "Florin City", "Mildendo" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.RemoveRange(1, 3);

            // Assert
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[4]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[4]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Deleted);
            orderedList.Should().ExposeEntry(new KVP(3, elements[3]), Status.Deleted);
            orderedList.Should().ExposeEntry(new KVP(4, elements[4]), Status.Deleted);
        }

        [TestMethod] public void RemoveRangeZeroItems() {
            // Arrange
            var elements = new string[] { "Seahaven", "Hundred Acre Wood", "Canalave City" };
            var orderedList = new RelationOrderedList<string>(elements);

            // Act
            orderedList.RemoveRange(0, 0);

            // Assert
            orderedList.Count.Should().Be(3);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.New);
        }

        [TestMethod] public void RemoveInvalidRange_IsError() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() { "Alicante", "Caprica City", "Ocean Shores" };

            // Act
            Action act0 = () => orderedList.RemoveRange(-3, 7);
            Action act1 = () => orderedList.RemoveRange(1, -6);
            Action act2 = () => orderedList.RemoveRange(2, 4);

            // Assert
            act0.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            act1.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            act2.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void RemoveAll() {
            // Arrange
            var elements = new string[] {
                "Casterbridge", "Bayport", "Belvedere", "Gimmerton", "San Narciso", "Novokribirsk", "Trantor"
            };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.RemoveAll(s => s.Contains('o'));

            // Assert
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[2]);
            orderedList.Should().HaveEntryCount(7);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[2]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Deleted);
            orderedList.Should().ExposeEntry(new KVP(3, elements[3]), Status.Deleted);
            orderedList.Should().ExposeEntry(new KVP(4, elements[4]), Status.Deleted);
            orderedList.Should().ExposeEntry(new KVP(5, elements[5]), Status.Deleted);
            orderedList.Should().ExposeEntry(new KVP(6, elements[6]), Status.Deleted);
        }

        [TestMethod] public void Clear() {
            // Arrange
            var elements = new string[] { "Kingsbury", "Vysnia", "Toontown" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var single = "Autobot City";
            orderedList.Add(single);

            // Act
            orderedList.Clear();

            // Assert
            orderedList.Count.Should().Be(0);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Deleted);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Deleted);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Deleted);
        }

        [TestMethod] public void CanonicalizeSomeDeleted() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() { "Greendale", "Ascalon", "Smurf Village", "Opelucid City" };
            (orderedList as IRelation).Canonicalize();
            orderedList.Clear();

            // Act
            (orderedList as IRelation).Canonicalize();

            // Assert
            orderedList.Count.Should().Be(0);
            orderedList.Should().HaveEntryCount(0);
        }

        [TestMethod] public void AddNewItem() {
            // Arrange
            var orderedList = new RelationOrderedList<string>();
            var element0 = "Maycomb";
            var element1 = "Antillus";

            // Act
            orderedList.Add(element0);
            var position = (orderedList as IList).Add(element1);

            // Assert
            position.Should().Be(1);
            orderedList.Count.Should().Be(2);
            orderedList[0].Should().Be(element0);
            orderedList[1].Should().Be(element1);
            orderedList.Should().HaveEntryCount(2);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, element0), Status.New);
            orderedList.Should().ExposeEntry(new KVP(1, element1), Status.New);
        }

        [TestMethod] public void AddRemovedSavedItemBack() {
            // Arrange
            var elements = new string[] { "San Fransokyo", "Alamode", "Marienburg" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            orderedList.RemoveAt(orderedList.Count - 1);

            // Act
            orderedList.Add(elements[^1]);

            // Assert
            orderedList.Count.Should().Be(3);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
        }

        [TestMethod] public void AddNewItemAfterRemoval() {
            // Arrange
            var elements = new string[] { "Walkerville", "Anistar City", "Gomorrah", "Haddonfield" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            orderedList.RemoveAt(orderedList.Count - 1);
            var replacement = "Radiator Springs";

            // Act
            orderedList.Add(replacement);

            // Assert
            orderedList.Count.Should().Be(4);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList[3].Should().Be(replacement);
            orderedList.Should().HaveEntryCount(4);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, replacement), Status.Modified);
        }

        [TestMethod] public void AddRangeOfItems() {
            // Arrange
            var elements = new string[] { "Raccoon City", "Bellwood", "City of Frank" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            orderedList.Clear();
            var replacements = new string[] { "Isla Nublar", elements[1], "Woodsboro", "Kilahito", "Serenity View" };

            // Act
            orderedList.AddRange(replacements);

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(replacements[0]);
            orderedList[1].Should().Be(replacements[1]);
            orderedList[2].Should().Be(replacements[2]);
            orderedList[3].Should().Be(replacements[3]);
            orderedList[4].Should().Be(replacements[4]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, replacements[0]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(1, replacements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, replacements[2]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, replacements[3]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(4, replacements[4]), Status.New);
        }

        [TestMethod] public void InsertItemAtEnd() {
            // Arrange
            var elements = new string[] { "Macondo", "Yumenes", "Canterlot" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var single = "Grover's Corners";

            // Act
            orderedList.Insert(orderedList.Count, single);

            // Assert
            orderedList.Count.Should().Be(4);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList[3].Should().Be(single);
            orderedList.Should().HaveEntryCount(4);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, single), Status.New);
        }

        [TestMethod] public void InsertItemAtFront() {
            // Arrange
            var elements = new string[] { "Mayapore", "Bluffington", "Flavortown" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var single = "Medford";

            // Act
            orderedList.Insert(0, single);

            // Assert
            orderedList.Count.Should().Be(4);
            orderedList[0].Should().Be(single);
            orderedList[1].Should().Be(elements[0]);
            orderedList[2].Should().Be(elements[1]);
            orderedList[3].Should().Be(elements[2]);
            orderedList.Should().HaveEntryCount(4);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, single), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(1, elements[0]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, elements[1]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, elements[2]), Status.New);
        }

        [TestMethod] public void InsertItemAtMiddle() {
            // Arrange
            var elements = new string[] { "Berk", "Urinetown", "Springwood", "Gulfhaven" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var addendum = "Ten Alders";
            orderedList.Add(addendum);
            var single = "Paniola Town";

            // Act
            orderedList.Insert(3, single);

            // Assert
            orderedList.Count.Should().Be(6);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList[3].Should().Be(single);
            orderedList[4].Should().Be(elements[3]);
            orderedList[5].Should().Be(addendum);
            orderedList.Should().HaveEntryCount(6);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, single), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(4, elements[3]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(5, addendum), Status.New);
        }

        [TestMethod] public void InsertItemSelf() {
            // Arrange
            var elements = new string[] { "Westwood", "Santa Cecilia", "Avalor City", "Paikang" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.Insert(1, orderedList[1]);

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[1]);
            orderedList[3].Should().Be(elements[2]);
            orderedList[4].Should().Be(elements[3]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[1]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, elements[2]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(4, elements[3]), Status.New);
        }

        [TestMethod] public void InsertItemReplaceDeleted() {
            // Arrange
            var elements = new string[] { "Newtopia", "Kingston Falls", "Elmville", "Dillon", "Hammerlocke" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            orderedList.RemoveAt(1);

            // Act
            orderedList.Insert(1, elements[1]);

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList[3].Should().Be(elements[3]);
            orderedList[4].Should().Be(elements[4]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, elements[3]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(4, elements[4]), Status.Saved);
        }

        [TestMethod] public void InsertNegativeIndex_IsError() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() { "Zodanga", "Ilium" };

            // Act
            Action first_act = () => orderedList.Insert(-4, "Endsville");
            Action second_act = () => (orderedList as IList).Insert(-182, "Man-Village");

            // Assert
            first_act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            second_act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void InsertOverlageIndex_IsError() {
            // Arrange
            var orderedList = new RelationOrderedList<string>();

            // Act
            Action act = () => orderedList.Insert(9, "Orchid Bay City");

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void InsertRangeAtEnd() {
            // Arrange
            var elements = new string[] { "Anatevka", "Longneck Crater", "Winesburg" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var insertions = new string[] { "Durnsville", "Langley Falls" };

            // Act
            orderedList.InsertRange(orderedList.Count, insertions);

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList[3].Should().Be(insertions[0]);
            orderedList[4].Should().Be(insertions[1]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, insertions[0]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(4, insertions[1]), Status.New);
        }

        [TestMethod] public void InsertRangeAtFront() {
            // Arrange
            var elements = new string[] { "Free City", "Metro City" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var insertions = new string[] { "Wayward Pines" };

            // Act
            orderedList.InsertRange(0, insertions);

            // Assert
            orderedList.Count.Should().Be(3);
            orderedList[0].Should().Be(insertions[0]);
            orderedList[1].Should().Be(elements[0]);
            orderedList[2].Should().Be(elements[1]);
            orderedList.Should().HaveEntryCount(3);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, insertions[0]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(1, elements[0]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, elements[1]), Status.New);
        }

        [TestMethod] public void InsertRangeAtMiddle() {
            // Arrange
            var elements = new string[] { "Westview", "River City", "Peach Creek", "Fairytale Land" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            var insertions = new string[] { "O-Town", "Iram of the Pillars", "Lei Chen" };

            // Act
            orderedList.InsertRange(2, insertions);

            // Assert
            orderedList.Count.Should().Be(7);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(insertions[0]);
            orderedList[3].Should().Be(insertions[1]);
            orderedList[4].Should().Be(insertions[2]);
            orderedList[5].Should().Be(elements[2]);
            orderedList[6].Should().Be(elements[3]);
            orderedList.Should().HaveEntryCount(7);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, insertions[0]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, insertions[1]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(4, insertions[2]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(5, elements[2]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(6, elements[3]), Status.New);
        }

        [TestMethod] public void InsertRangeSelf() {
            // Arrange
            var elements = new string[] { "Tremorton", "Woodland", "Element City" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.InsertRange(1, elements[1..]);

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(elements[1]);
            orderedList[2].Should().Be(elements[2]);
            orderedList[3].Should().Be(elements[1]);
            orderedList[4].Should().Be(elements[2]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, elements[1]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, elements[2]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, elements[1]), Status.New);
            orderedList.Should().ExposeEntry(new KVP(4, elements[2]), Status.New);
        }

        [TestMethod] public void InsertRangeReplaceDeleted() {
            // Arrange
            var elements = new string[] { "Gujaareh", "Mictlān", "Cascarrafa", "Care-a-Lot", "Bomont" };
            var orderedList = new RelationOrderedList<string>(elements);
            (orderedList as IRelation).Canonicalize();
            orderedList.RemoveAt(1);
            orderedList.RemoveAt(1);
            orderedList.RemoveAt(1);
            var insertions = new string[] { elements[1], "", elements[3] };

            // Act
            orderedList.InsertRange(1, insertions);

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(elements[0]);
            orderedList[1].Should().Be(insertions[0]);
            orderedList[2].Should().Be(insertions[1]);
            orderedList[3].Should().Be(insertions[2]);
            orderedList[4].Should().Be(elements[4]);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, elements[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, insertions[0]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, insertions[1]), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, insertions[2]), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(4, elements[4]), Status.Saved);
        }

        [TestMethod] public void InsertNullRange_IsError() {
            // Arrange
            var orderedList = new RelationOrderedList<string>();

            // Act
            Action act = () => orderedList.InsertRange(0, null!);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>().WithAnyMessage();
        }

        [TestMethod] public void InsertInvalidRange_IsError() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() { "Agrabah", "Peyton Place", "Beach City" };

            // Act
            var insertion = new string[] { "Bushwood", "Nowhere", "Swallow Falls", "Avonlea" };
            Action act0 = () => orderedList.InsertRange(-14, insertion);
            Action act1 = () => orderedList.InsertRange(orderedList.Count * 681, insertion);

            // Assert
            act0.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
            act1.Should().ThrowExactly<ArgumentOutOfRangeException>().WithAnyMessage();
        }

        [TestMethod] public void Sort() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() {
                "Green Hills", "Dinsford", "Camp Wawanakwa", "Fansville", "Eternal City", "Thneedville"
            };
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.Sort();

            // Assert
            orderedList.Count.Should().Be(6);
            orderedList[0].Should().Be("Camp Wawanakwa");
            orderedList[1].Should().Be("Dinsford");
            orderedList[2].Should().Be("Eternal City");
            orderedList[3].Should().Be("Fansville");
            orderedList[4].Should().Be("Green Hills");
            orderedList[5].Should().Be("Thneedville");
            orderedList.Should().HaveEntryCount(6);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, "Camp Wawanakwa"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(1, "Dinsford"), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, "Eternal City"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, "Fansville"), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(4, "Green Hills"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(5, "Thneedville"), Status.Saved);
        }

        [TestMethod] public void SortCustomComparer() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() {
                "irKallA", "GREEnfiEld", "gUmBaLdIa", "Waterfall City"
            };
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.Sort(StringComparer.OrdinalIgnoreCase);

            // Assert
            orderedList.Count.Should().Be(4);
            orderedList[0].Should().Be("GREEnfiEld");
            orderedList[1].Should().Be("gUmBaLdIa");
            orderedList[2].Should().Be("irKallA");
            orderedList[3].Should().Be("Waterfall City");
            orderedList.Should().HaveEntryCount(4);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, "GREEnfiEld"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(1, "gUmBaLdIa"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, "irKallA"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, "Waterfall City"), Status.Saved);
        }

        [TestMethod] public void SortSubrange() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() {
                "Fort Chicken",
                "Ratropolis",
                "Mystic Falls",
                "Southside Reef",
                "Megasaki",
                "San Madrigal"
            };
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.Sort(1, 4, StringComparer.Ordinal);

            // Assert
            orderedList.Count.Should().Be(6);
            orderedList[0].Should().Be("Fort Chicken");
            orderedList[1].Should().Be("Megasaki");
            orderedList[2].Should().Be("Mystic Falls");
            orderedList[3].Should().Be("Ratropolis");
            orderedList[4].Should().Be("Southside Reef");
            orderedList[5].Should().Be("San Madrigal");
            orderedList.Should().HaveEntryCount(6);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, "Fort Chicken"), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, "Megasaki"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, "Mystic Falls"), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, "Ratropolis"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(4, "Southside Reef"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(5, "San Madrigal"), Status.Saved);
        }

        [TestMethod] public void Reverse() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() {
                "New Mushroomton", "Textopolis", "Highland", "Petropolis", "Textopolis", "Royal Woods"
            };
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.Reverse();

            // Assert
            orderedList.Count.Should().Be(6);
            orderedList[0].Should().Be("Royal Woods");
            orderedList[1].Should().Be("Textopolis");
            orderedList[2].Should().Be("Petropolis");
            orderedList[3].Should().Be("Highland");
            orderedList[4].Should().Be("Textopolis");
            orderedList[5].Should().Be("New Mushroomton");
            orderedList.Should().HaveEntryCount(6);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, "Royal Woods"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(1, "Textopolis"), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, "Petropolis"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, "Highland"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(4, "Textopolis"), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(5, "New Mushroomton"), Status.Modified);
        }

        [TestMethod] public void ReverseSubsequence() {
            // Arrange
            var orderedList = new RelationOrderedList<string>() {
                "Cabot Cove", "New Mombasa", "Whiterun", "Hollywoob", "Rapture", "LazyTown", "Erinsborough"
            };
            (orderedList as IRelation).Canonicalize();

            // Act
            orderedList.Reverse(1, 5);

            // Assert
            orderedList.Count.Should().Be(7);
            orderedList[0].Should().Be("Cabot Cove");
            orderedList[1].Should().Be("LazyTown");
            orderedList[2].Should().Be("Rapture");
            orderedList[3].Should().Be("Hollywoob");
            orderedList[4].Should().Be("Whiterun");
            orderedList[5].Should().Be("New Mombasa");
            orderedList[6].Should().Be("Erinsborough");
            orderedList.Should().HaveEntryCount(7);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, "Cabot Cove"), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, "LazyTown"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(2, "Rapture"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(3, "Hollywoob"), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(4, "Whiterun"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(5, "New Mombasa"), Status.Modified);
            orderedList.Should().ExposeEntry(new KVP(6, "Erinsborough"), Status.Saved);
        }

        [TestMethod] public void RepopulateSingleItem() {
            // Arrange
            var orderedList = new RelationOrderedList<string>();
            var item = "Walkabout Creek";

            // Act
            (orderedList as IRelation).Repopulate(new KVP(0, item));

            // Assert
            orderedList.Count.Should().Be(1);
            orderedList[0].Should().Be(item);
            orderedList.Should().HaveEntryCount(1);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, item), Status.Saved);
        }

        [TestMethod] public void RepopulateMultipleItems() {
            // Arrange
            var orderedList = new RelationOrderedList<string>();
            var item0 = "Okern";
            var item1 = "Yomi-no-kuni";
            var item2 = "Bon Temps";
            var item3 = "Cicely";
            var item4 = "Xibalba";

            // Act
            (orderedList as IRelation).Repopulate(new KVP(0, item0));
            (orderedList as IRelation).Repopulate(new KVP(1, item1));
            (orderedList as IRelation).Repopulate(new KVP(2, item2));
            (orderedList as IRelation).Repopulate(new KVP(3, item3));
            (orderedList as IRelation).Repopulate(new KVP(4, item4));

            // Assert
            orderedList.Count.Should().Be(5);
            orderedList[0].Should().Be(item0);
            orderedList[1].Should().Be(item1);
            orderedList[2].Should().Be(item2);
            orderedList[3].Should().Be(item3);
            orderedList[4].Should().Be(item4);
            orderedList.Should().HaveEntryCount(5);
            orderedList.Should().ExposeDeletesFirst();
            orderedList.Should().ExposeEntry(new KVP(0, item0), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(1, item1), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(2, item2), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(3, item3), Status.Saved);
            orderedList.Should().ExposeEntry(new KVP(4, item4), Status.Saved);
        }
    }
}
