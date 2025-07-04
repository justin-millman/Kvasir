using FluentAssertions;
using Kvasir.Localization;
using Kvasir.Relations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;

using KVP = System.Collections.Generic.KeyValuePair<string, string>;
using Triplet = System.Tuple<string, string, string>;

namespace UT.Kvasir.Localization {
    [TestClass, TestCategory("Localization")]
    public class LocalizationTests {
        [TestMethod] public void DefaultConstruct() {
            // Arrange
            var key = "key";

            // Act
            var loc = new Localization<string, string, string>(key);

            // Assert
            loc.Count.Should().Be(0);
            loc.Should().HaveConnectionType<Triplet>();
            (loc as ILocalization).LocalizationKey.Should().Be(key);
            (loc as IReadOnlyLocalization<string, string, string>).Should().HaveConnectionType<Triplet>();
            loc.Should().HaveEntryCount(0);
        }

        [TestMethod] public void ConstructWithCustomComparator() {
            // Arrange
            var key = "key";

            // Act
            var loc = new Localization<string, string, string>(key, StringComparer.OrdinalIgnoreCase);

            // Assert
            loc.Count.Should().Be(0);
            loc.Should().HaveConnectionType<Triplet>();
            (loc as ILocalization).LocalizationKey.Should().Be(key);
            loc.Should().HaveEntryCount(0);
        }

        [TestMethod] public void ConstructFromCapacityDefaultComparator() {
            // Arrange
            var key = "key";

            // Act
            var loc = new Localization<string, string, string>(key, 809);

            // Assert
            loc.Count.Should().Be(0);
            loc.Should().HaveConnectionType<Triplet>();
            (loc as ILocalization).LocalizationKey.Should().Be(key);
            loc.Should().HaveEntryCount(0);
        }

        [TestMethod] public void ConstructFromCapacityCustomComparator() {
            // Arrange
            var key = "key";

            // Act
            var loc = new Localization<string, string, string>(key, 809, StringComparer.CurrentCulture);

            // Assert
            loc.Count.Should().Be(0);
            loc.Should().HaveConnectionType<Triplet>();
            (loc as ILocalization).LocalizationKey.Should().Be(key);
            loc.Should().HaveEntryCount(0);
        }

        [TestMethod] public void ConstructFromDictionaryDefaultComparer() {
            // Arrange
            var kvps = new List<KVP>() {
                new KVP("English", "strawberry"),
                new KVP("Spanish", "fresa"),
                new KVP("French", "fraise"),
                new KVP("Hebrew", "תות שדה")
            };
            var dict = new Dictionary<string, string>(kvps);
            var key = "STRAWBERRY_NAME_LOC";

            // Act
            var loc = new Localization<string, string, string>(key, dict);

            // Assert
            loc.Count.Should().Be(4);
            loc[kvps[0].Key].Should().Be(kvps[0].Value); (loc as IDictionary)[kvps[0].Key].Should().Be(kvps[0].Value);
            loc[kvps[1].Key].Should().Be(kvps[1].Value); (loc as IDictionary)[kvps[1].Key].Should().Be(kvps[1].Value);
            loc[kvps[2].Key].Should().Be(kvps[2].Value); (loc as IDictionary)[kvps[2].Key].Should().Be(kvps[2].Value);
            loc[kvps[3].Key].Should().Be(kvps[3].Value); (loc as IDictionary)[kvps[3].Key].Should().Be(kvps[3].Value);
            loc.Should().HaveConnectionType<Triplet>();
            (loc as ILocalization).LocalizationKey.Should().Be(key);
            loc.Should().HaveEntryCount(dict.Count);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, kvps[0].Key, kvps[0].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[1].Key, kvps[1].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[2].Key, kvps[2].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[3].Key, kvps[3].Value), Status.New);
        }

        [TestMethod] public void ConstructFromListOfKVPsDefaultComparer() {
            // Arrange
            var kvps = new List<KVP>() {
                new KVP("English", "apple"),
                new KVP("Spanish", "manzana"),
                new KVP("French", "pomme"),
                new KVP("Hebrew", "​ת​פוח")
            };
            var key = "APPLE_NAME_LOC";

            // Act
            var loc = new Localization<string, string, string>(key, kvps);

            // Assert
            loc.Count.Should().Be(4);
            loc[kvps[0].Key].Should().Be(kvps[0].Value); (loc as IDictionary)[kvps[0].Key].Should().Be(kvps[0].Value);
            loc[kvps[1].Key].Should().Be(kvps[1].Value); (loc as IDictionary)[kvps[1].Key].Should().Be(kvps[1].Value);
            loc[kvps[2].Key].Should().Be(kvps[2].Value); (loc as IDictionary)[kvps[2].Key].Should().Be(kvps[2].Value);
            loc[kvps[3].Key].Should().Be(kvps[3].Value); (loc as IDictionary)[kvps[3].Key].Should().Be(kvps[3].Value);
            loc.Should().HaveConnectionType<Triplet>();
            (loc as ILocalization).LocalizationKey.Should().Be(key);
            loc.Should().HaveEntryCount(kvps.Count);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, kvps[0].Key, kvps[0].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[1].Key, kvps[1].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[2].Key, kvps[2].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[3].Key, kvps[3].Value), Status.New);
        }

        [TestMethod] public void ConstructFromDictionaryCustomComparer() {
            // Arrange
            var kvps = new List<KVP>() {
                new KVP("English", "banana"),
                new KVP("Spanish", "banana"),
                new KVP("French", "bananier"),
                new KVP("Hebrew", "בננה")
            };
            var dict = new Dictionary<string, string>(kvps);
            var key = "BANANA_NAME_LOC";

            // Act
            var loc = new Localization<string, string, string>(key, dict, StringComparer.Ordinal);

            // Assert
            loc.Count.Should().Be(4);
            loc[kvps[0].Key].Should().Be(kvps[0].Value); (loc as IDictionary)[kvps[0].Key].Should().Be(kvps[0].Value);
            loc[kvps[1].Key].Should().Be(kvps[1].Value); (loc as IDictionary)[kvps[1].Key].Should().Be(kvps[1].Value);
            loc[kvps[2].Key].Should().Be(kvps[2].Value); (loc as IDictionary)[kvps[2].Key].Should().Be(kvps[2].Value);
            loc[kvps[3].Key].Should().Be(kvps[3].Value); (loc as IDictionary)[kvps[3].Key].Should().Be(kvps[3].Value);
            loc.Should().HaveConnectionType<Triplet>();
            (loc as ILocalization).LocalizationKey.Should().Be(key);
            loc.Should().HaveEntryCount(dict.Count);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, kvps[0].Key, kvps[0].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[1].Key, kvps[1].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[2].Key, kvps[2].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[3].Key, kvps[3].Value), Status.New);
        }

        [TestMethod] public void ConstructFromListOfKVPsCustomComparer() {
            // Arrange
            var kvps = new List<KVP>() {
                new KVP("English", "mango"),
                new KVP("Spanish", "mango"),
                new KVP("French", "mangue"),
                new KVP("Hebrew", "​מנגו")
            };
            var key = "MANGO_NAME_LOC";

            // Act
            var loc = new Localization<string, string, string>(key, kvps);

            // Assert
            loc.Count.Should().Be(4);
            loc[kvps[0].Key].Should().Be(kvps[0].Value); (loc as IDictionary)[kvps[0].Key].Should().Be(kvps[0].Value);
            loc[kvps[1].Key].Should().Be(kvps[1].Value); (loc as IDictionary)[kvps[1].Key].Should().Be(kvps[1].Value);
            loc[kvps[2].Key].Should().Be(kvps[2].Value); (loc as IDictionary)[kvps[2].Key].Should().Be(kvps[2].Value);
            loc[kvps[3].Key].Should().Be(kvps[3].Value); (loc as IDictionary)[kvps[3].Key].Should().Be(kvps[3].Value);
            loc.Should().HaveConnectionType<Triplet>();
            (loc as ILocalization).LocalizationKey.Should().Be(key);
            loc.Should().HaveEntryCount(kvps.Count);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, kvps[0].Key, kvps[0].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[1].Key, kvps[1].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[2].Key, kvps[2].Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, kvps[3].Key, kvps[3].Value), Status.New);
        }

        [TestMethod] public void RemoveExistingNewItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "grapefruit"),
                new KVP("Spanish", "pomelo"),
                new KVP("French", "pamplemousse"),
                new KVP("German", "Grapefruit"),
                new KVP("Vietnamese", "bưởi chùm"),
            };
            var key = "GRAPEFRUIT_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);

            // Act
            var first_success = loc.Remove(pairs[1].Key);
            var second_success = (loc as ICollection<KVP>).Remove(pairs[3]);
            (loc as IDictionary).Remove(pairs[4].Key);

            // Assert
            loc.Count.Should().Be(2);
            first_success.Should().BeTrue();
            second_success.Should().BeTrue();
            loc[pairs[0].Key].Should().Be(pairs[0].Value);
            loc[pairs[2].Key].Should().Be(pairs[2].Value);
            loc.Should().HaveEntryCount(2);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.New);
            loc.Should().NotExposeEntryFor(new Triplet(key, pairs[1].Key, pairs[1].Value));
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.New);
        }

        [TestMethod] public void RemoveExistingSavedItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "coconut"),
                new KVP("French", "noix de coco"),
                new KVP("Hebrew", "קוקוס")
            };
            var key = "COCONUT_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();

            // Act
            var success = loc.Remove(pairs[0].Key);

            // Assert
            loc.Count.Should().Be(2);
            success.Should().BeTrue();
            loc[pairs[1].Key].Should().Be(pairs[1].Value);
            loc[pairs[2].Key].Should().Be(pairs[2].Value);
            loc.Should().HaveEntryCount(3);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.Deleted);
            loc.Should().ExposeEntry(new Triplet(key, pairs[1].Key, pairs[1].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.Saved);
        }

        [TestMethod] public void RemoveNonexistingItem() {
            // Arrange
            var key = "ORANGE_NAME_LOC";
            var loc = new Localization<string, string, string>(key) { { "English", "orange" }, { "Spanish", "naranja" } };

            // Act
            var first_success = loc.Remove("Portuguese");
            var second_success = (loc as ICollection<KVP>).Remove(new KVP("Greek", "πορτοκάλι"));

            // Assert
            loc.Count.Should().Be(2);
            first_success.Should().BeFalse();
            second_success.Should().BeFalse();
        }

        [TestMethod] public void RemoveExistingNewItemObtainValue() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "cherry"),
                new KVP("Greek", "κεράσι"),
                new KVP("Arabic", "كرز")
            };
            var key = "CHERRY_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);

            // Act
            var success = loc.Remove(pairs[1].Key, out string result);

            // Assert
            loc.Count.Should().Be(2);
            success.Should().BeTrue();
            result.Should().Be(pairs[1].Value);
            loc[pairs[0].Key].Should().Be(pairs[0].Value);
            loc[pairs[2].Key].Should().Be(pairs[2].Value);
            loc.Should().HaveEntryCount(2);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.New);
            loc.Should().NotExposeEntryFor(new Triplet(key, pairs[1].Key, pairs[1].Value));
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.New);
        }

        [TestMethod] public void RemoveExistingSavedItemObtainValue() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "avocado"),
                new KVP("French", "avocat"),
                new KVP("Finnish", "avokado")
            };
            var key = "AVOCADO_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();

            // Act
            var success = loc.Remove(pairs[0].Key, out string result);

            // Assert
            loc.Count.Should().Be(2);
            success.Should().BeTrue();
            result.Should().Be(pairs[0].Value);
            loc[pairs[1].Key].Should().Be(pairs[1].Value);
            loc[pairs[2].Key].Should().Be(pairs[2].Value);
            loc.Should().HaveEntryCount(3);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.Deleted);
            loc.Should().ExposeEntry(new Triplet(key, pairs[1].Key, pairs[1].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.Saved);
        }

        [TestMethod] public void RemoveNonexistingItemObtainValue() {
            // Arrange
            var key = "LEMON_NAME_LOC";
            var loc = new Localization<string, string, string>(key) { { "English", "lemon" }, { "Portuguese", "limão" } };

            // Act
            var success = loc.Remove("Farsi", out string? result);

            // Assert
            loc.Count.Should().Be(2);
            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [TestMethod] public void Clear() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "blueberry"),
                new KVP("Hindi", "ब्लूबेरी"),
                new KVP("Romanian", "afină")
            };
            var key = "BLUEBERRY_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();
            var single = new KVP("Russian", "черника");
            loc.Add(single.Key, single.Value);

            // Act
            loc.Clear();

            // Assert
            loc.Count.Should().Be(0);
            loc.Should().HaveEntryCount(3);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.Deleted);
            loc.Should().ExposeEntry(new Triplet(key, pairs[1].Key, pairs[1].Value), Status.Deleted);
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.Deleted);
            loc.Should().NotExposeEntryFor(new Triplet(key, single.Key, single.Value));
        }

        [TestMethod] public void CanonicalizeSomeDeleted() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "guava"),
                new KVP("French", "goyave"),
                new KVP("Hebrew", "גויאבה"),
                new KVP("Italian", "guaiava"),
                new KVP("Japanese", "グアバ"),
            };
            var key = "GUAVA_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();
            loc.Remove(pairs[1].Key);
            loc.Remove(pairs[3].Key);

            // Act
            (loc as IRelation).Canonicalize();

            // Assert
            loc.Count.Should().Be(3);
            loc.Should().HaveEntryCount(3);
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.Saved);
            loc.Should().NotExposeEntryFor(new Triplet(key, pairs[1].Key, pairs[1].Value));
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.Saved);
            loc.Should().NotExposeEntryFor(new Triplet(key, pairs[3].Key, pairs[3].Value));
            loc.Should().ExposeEntry(new Triplet(key, pairs[4].Key, pairs[4].Value), Status.Saved);
        }

        [TestMethod] public void AddNewItem() {
            // Arrange
            var key = "KIWI_NAME_LOC";
            var loc = new Localization<string, string, string>(key);
            var pair0 = new KVP("English", "kiwi");
            var pair1 = new KVP("Greek", "ακτινίδιο");
            var pair2 = new KVP("Lao", "ໝາກກີວີ");

            // Act
            loc.Add(pair0.Key, pair0.Value);
            (loc as IDictionary).Add(pair1.Key, pair1.Value);
            (loc as ICollection<KVP>).Add(pair2);

            // Assert
            loc.Count.Should().Be(3);
            loc[pair0.Key].Should().Be(pair0.Value);
            loc[pair1.Key].Should().Be(pair1.Value);
            loc[pair2.Key].Should().Be(pair2.Value);
            loc.Should().HaveEntryCount(3);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pair0.Key, pair0.Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, pair1.Key, pair1.Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, pair2.Key, pair2.Value), Status.New);
        }

        [TestMethod] public void AddExistingDeletedItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "apricot"),
                new KVP("Spanish", "albaricoque"),
                new KVP("Korean", "살구")
            };
            var key = "APRICOT_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();
            var deleted = pairs[1];
            loc.Remove(deleted.Key);

            // Act
            loc.Add(deleted.Key, deleted.Value);

            // Assert
            loc.Count.Should().Be(3);
            loc[pairs[0].Key].Should().Be(pairs[0].Value);
            loc[pairs[1].Key].Should().Be(pairs[1].Value);
            loc[pairs[2].Key].Should().Be(pairs[2].Value);
            loc.Should().HaveEntryCount(3);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, deleted.Key, deleted.Value), Status.Saved);
        }

        [TestMethod] public void AddExistingItem_IsError() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "watermelon"),
                new KVP("Spanish", "sandía"),
                new KVP("Hebrew", "אבטיח"),
                new KVP("Portuguese", "melancia"),
                new KVP("Klingon", "qo'")
            };
            var key = "WATERMELON_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);

            // Act
            Action act = () => loc.Add(pairs[3].Key, pairs[3].Value);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void TryAddNewItem() {
            var key = "OLIVE_NAME_LOC";
            var loc = new Localization<string, string, string>(key);
            var pair0 = new KVP("English", "olive");
            var pair1 = new KVP("Croatian", "maslina");

            // Act
            var success0 = loc.TryAdd(pair0.Key, pair0.Value);
            var success1 = loc.TryAdd(pair1.Key, pair1.Value);

            // Assert
            loc.Count.Should().Be(2);
            success0.Should().BeTrue();
            success1.Should().BeTrue();
            loc[pair0.Key].Should().Be(pair0.Value);
            loc[pair1.Key].Should().Be(pair1.Value);
            loc.Should().HaveEntryCount(2);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pair0.Key, pair0.Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, pair1.Key, pair1.Value), Status.New);
        }

        [TestMethod] public void TryAddExistingDeletedItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "pomegranate"),
                new KVP("Greek", "Stρόδι"),
                new KVP("Spanish", "granada")
            };
            var key = "POMEGRANATE_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();
            var deleted = pairs[1];
            loc.Remove(deleted.Key);

            // Act
            var success = loc.TryAdd(deleted.Key, deleted.Value);

            // Assert
            loc.Count.Should().Be(3);
            success.Should().BeTrue();
            loc[pairs[0].Key].Should().Be(pairs[0].Value);
            loc[pairs[1].Key].Should().Be(pairs[1].Value);
            loc[pairs[2].Key].Should().Be(pairs[2].Value);
            loc.Should().HaveEntryCount(3);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, deleted.Key, deleted.Value), Status.Saved);
        }

        [TestMethod] public void TryAddExistingItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "pumpkin"),
                new KVP("Hebrew", "דלעת"),
                new KVP("Finnish", "kurpitsa"),
                new KVP("Filipino", "kalabasa"),
                new KVP("Italian", "zucca")
            };
            var key = "PUMPKIN_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);

            // Act
            var success = loc.TryAdd(pairs[3].Key, pairs[3].Value);

            // Assert
            success.Should().BeFalse();
        }

        [TestMethod] public void AddNewItemViaIndexer() {
            // Arrange
            var key = "PASSIONFRUIT_NAME_LOC";
            var loc = new Localization<string, string, string>(key);
            var pair0 = new KVP("English", "passionfruit");
            var pair1 = new KVP("Welsh", "ffrwythau angerdd");

            // Act
            loc[pair0.Key] = pair0.Value;
            loc[pair1.Key] = pair1.Value;

            // Assert
            loc.Count.Should().Be(2);
            loc[pair0.Key].Should().Be(pair0.Value);
            loc[pair1.Key].Should().Be(pair1.Value);
            loc.Should().HaveEntryCount(2);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pair0.Key, pair0.Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, pair1.Key, pair1.Value), Status.New);
        }

        [TestMethod] public void OverwriteValueOfNewKey() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "plum"),
                new KVP("Spanish", "ciruela"),
                new KVP("French", "prune"),
                new KVP("Hebrew", "שזיף")
            };
            var key = "PLUM_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();
            var single0 = new KVP("German", "Pflaume");
            var single1 = new KVP("Ukrainian", "слива");
            loc.Add(single0.Key, single0.Value);
            loc.Add(single1.Key, single1.Value);

            // Act
            var newValue = "~~~SPANISH~~~";
            loc[single1.Key] = newValue;
            (loc as IDictionary)[single1.Key] = newValue;

            // Assert
            loc.Count.Should().Be(6);
            loc[pairs[0].Key].Should().Be(pairs[0].Value);
            loc[pairs[1].Key].Should().Be(pairs[1].Value);
            loc[pairs[2].Key].Should().Be(pairs[2].Value);
            loc[pairs[3].Key].Should().Be(pairs[3].Value);
            loc.Should().HaveEntryCount(6);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[1].Key, pairs[1].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[3].Key, pairs[3].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, single0.Key, single0.Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, single1.Key, newValue), Status.New);
        }

        [TestMethod] public void OverwriteValueOfSavedKey() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "cucumber"),
                new KVP("Spanish", "pepino"),
                new KVP("French", "concombre"),
                new KVP("Thai", "แตงกวา")
            };
            var key = "CUCUMBER_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();
            var single0 = new KVP("Armenian", "վարունգ");
            var single1 = new KVP("Samoan", "kukama");
            loc.Add(single0.Key, single0.Value);
            loc.Add(single1.Key, single1.Value);

            // Act
            var newValue = "concombre (Français)";
            loc[pairs[2].Key] = newValue;
            (loc as IDictionary)[pairs[2].Key] = newValue;

            // Assert
            loc.Count.Should().Be(6);
            loc[pairs[0].Key].Should().Be(pairs[0].Value);
            loc[pairs[1].Key].Should().Be(pairs[1].Value);
            loc[pairs[2].Key].Should().Be(newValue);
            loc[pairs[3].Key].Should().Be(pairs[3].Value);
            loc.Should().HaveEntryCount(7);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[1].Key, pairs[1].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.Deleted);
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, newValue), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, pairs[3].Key, pairs[3].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, single0.Key, single0.Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, single1.Key, single1.Value), Status.New);
        }

        [TestMethod] public void OverwriteValueOfSavedKeyWithSelf() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "durian"),
                new KVP("Spanish", "durián"),
                new KVP("Traditional Chinese", "榴槤"),
                new KVP("Dutch", "Sandoerian")
            };
            var key = "DURIAN_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();
            var single0 = new KVP("Hebrew", "דוריאן");
            var single1 = new KVP("Nepali", "ड्यूरियन");
            loc.Add(single0.Key, single0.Value);
            loc.Add(single1.Key, single1.Value);

            // Act
            loc[pairs[0].Key] = pairs[0].Value;

            // Assert
            loc.Count.Should().Be(6);
            loc[pairs[0].Key].Should().Be(pairs[0].Value);
            loc[pairs[1].Key].Should().Be(pairs[1].Value);
            loc[pairs[2].Key].Should().Be(pairs[2].Value);
            loc[pairs[3].Key].Should().Be(pairs[3].Value);
            loc.Should().HaveEntryCount(6);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[1].Key, pairs[1].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[3].Key, pairs[3].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, single0.Key, single0.Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, single1.Key, single1.Value), Status.New);
        }

        [TestMethod] public void IndexAddDeletedItemWithSameValue() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "fig"),
                new KVP("Spanish", "higo"),
                new KVP("French", "figue"),
                new KVP("Latvian", "Vīģes")
            };
            var key = "FIG_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();
            var single0 = new KVP("Swedish", "fikon");
            var single1 = new KVP("Hindi", "आलंकारिक रूप");
            loc.Add(single0.Key, single0.Value);
            loc.Add(single1.Key, single1.Value);
            var removal = pairs[3];
            loc.Remove(removal.Key);

            // Act
            loc[removal.Key] = removal.Value;

            // Assert
            loc.Count.Should().Be(6);
            loc[pairs[0].Key].Should().Be(pairs[0].Value);
            loc[pairs[1].Key].Should().Be(pairs[1].Value);
            loc[pairs[2].Key].Should().Be(pairs[2].Value);
            loc[pairs[3].Key].Should().Be(pairs[3].Value);
            loc.Should().HaveEntryCount(6);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[1].Key, pairs[1].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[3].Key, pairs[3].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, single0.Key, single0.Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, single1.Key, single1.Value), Status.New);
        }

        [TestMethod] public void IndexAddDeletedItemWithNewValue() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("English", "papaya"),
                new KVP("Hebrew", "פפאיה"),
                new KVP("Thai", "มะละกอ"),
                new KVP("Japanese", "パパイヤ")
            };
            var key = "PAPAYA_NAME_LOC";
            var loc = new Localization<string, string, string>(key, pairs);
            (loc as IRelation).Canonicalize();
            var single0 = new KVP("Khmer", "ផ្លែល្ហុង");
            var single1 = new KVP("Arabic", "بابايا");
            loc.Add(single0.Key, single0.Value);
            loc.Add(single1.Key, single1.Value);
            var removal = pairs[3];
            loc.Remove(removal.Key);

            // Act
            var newValue = "## -> Japanese";
            loc[removal.Key] = newValue;

            // Assert
            loc.Count.Should().Be(6);
            loc[pairs[0].Key].Should().Be(pairs[0].Value);
            loc[pairs[1].Key].Should().Be(pairs[1].Value);
            loc[pairs[2].Key].Should().Be(pairs[2].Value);
            loc[pairs[3].Key].Should().Be(newValue);
            loc.Should().HaveEntryCount(7);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(new Triplet(key, pairs[0].Key, pairs[0].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[1].Key, pairs[1].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[2].Key, pairs[2].Value), Status.Saved);
            loc.Should().ExposeEntry(new Triplet(key, pairs[3].Key, pairs[3].Value), Status.Deleted);
            loc.Should().ExposeEntry(new Triplet(key, removal.Key, newValue), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, single0.Key, single0.Value), Status.New);
            loc.Should().ExposeEntry(new Triplet(key, single1.Key, single1.Value), Status.New);
        }

        [TestMethod] public void Repopulate() {
            // Arrange
            var key = "GRAPE_NAME_LOC";
            var loc = new Localization<string, string, string>(key);
            var trip0 = new Triplet(key, "English", "grape");
            var trip1 = new Triplet(key, "Spanish", "uva");
            var trip2 = new Triplet(key, "French", "raisin");

            // Act
            (loc as IRelation).Repopulate(trip0);
            (loc as IRelation).Repopulate(trip1);
            (loc as IRelation).Repopulate(trip2);

            // Assert
            loc.Count.Should().Be(3);
            loc[trip0.Item2].Should().Be(trip0.Item3);
            loc[trip1.Item2].Should().Be(trip1.Item3);
            loc[trip2.Item2].Should().Be(trip2.Item3);
            loc.Should().HaveEntryCount(3);
            loc.Should().ExposeDeletesFirst();
            loc.Should().ExposeEntry(trip0, Status.Saved);
            loc.Should().ExposeEntry(trip1, Status.Saved);
            loc.Should().ExposeEntry(trip2, Status.Saved);
        }
    }
}
