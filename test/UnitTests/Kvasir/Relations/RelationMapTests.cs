using FluentAssertions;
using Kvasir.Relations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;

using KVP = System.Collections.Generic.KeyValuePair<string, string>;

namespace UT.Kvasir.Relations {
    [TestClass, TestCategory("RelationMap")]
    public class RelationMapTests {
        [TestMethod] public void DefaultConstruct() {
            // Arrange

            // Act
            var map = new RelationMap<string, string>();

            // Assert
            map.Count.Should().Be(0);
            map.Comparer.Should().Be(EqualityComparer<string>.Default);
            map.Keys.Should().BeEmpty();
            map.Values.Should().BeEmpty();
            map.Should().HaveConnectionType<KVP>();
            map.Should().HaveEntryCount(0);
        }

        [TestMethod] public void ConstructWithCustomComprarer() {
            // Arrange
            var comp = StringComparer.CurrentCultureIgnoreCase;

            // Act
            var map = new RelationMap<string, string>(comp);

            // Assert
            map.Count.Should().Be(0);
            map.Comparer.Should().Be(comp);
            map.Keys.Should().BeEmpty();
            map.Values.Should().BeEmpty();
            map.Should().HaveConnectionType<KVP>();
            map.Should().HaveEntryCount(0);
        }

        [TestMethod] public void ConstructFromCapacityDefaultComparer() {
            // Arrange

            // Act
            var map = new RelationMap<string, string>(2182);

            // Assert
            map.Count.Should().Be(0);
            map.Comparer.Should().Be(EqualityComparer<string>.Default);
            map.Keys.Should().BeEmpty();
            map.Values.Should().BeEmpty();
            map.Should().HaveConnectionType<KVP>();
            map.Should().HaveEntryCount(0);
        }

        [TestMethod] public void ConstructFromCapacityCustomComparer() {
            // Arrange
            var comp = StringComparer.CurrentCultureIgnoreCase;

            // Act
            var map = new RelationMap<string, string>(491, comp);

            // Assert
            map.Count.Should().Be(0);
            map.Comparer.Should().Be(comp);
            map.Keys.Should().BeEmpty();
            map.Values.Should().BeEmpty();
            map.Should().HaveConnectionType<KVP>();
            map.Should().HaveEntryCount(0);
        }

        [TestMethod] public void ConstructFromDictionaryDefaultComparer() {
            // Arrange
            var kvps = new List<KVP>() {
                new KVP("Guarhulos", "Brazil"),
                new KVP("Pristina", "Kosovo"),
                new KVP("Wollongong", "Australia"),
                new KVP("Nagoya", "Japan"),
            };
            var dict = new Dictionary<string, string>(kvps);

            // Act
            var map = new RelationMap<string, string>(dict);

            // Assert
            map.Count.Should().Be(4);
            map.Comparer.Should().Be(EqualityComparer<string>.Default);
            map[kvps[0].Key].Should().Be(kvps[0].Value);   (map as IDictionary)[kvps[0].Key].Should().Be(kvps[0].Value);
            map[kvps[1].Key].Should().Be(kvps[1].Value);   (map as IDictionary)[kvps[1].Key].Should().Be(kvps[1].Value);
            map[kvps[2].Key].Should().Be(kvps[2].Value);   (map as IDictionary)[kvps[2].Key].Should().Be(kvps[2].Value);
            map[kvps[3].Key].Should().Be(kvps[3].Value);   (map as IDictionary)[kvps[3].Key].Should().Be(kvps[3].Value);
            map.Should().HaveConnectionType<KVP>();
            map.Should().HaveEntryCount(dict.Count);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(kvps[0], Status.New);
            map.Should().ExposeEntry(kvps[1], Status.New);
            map.Should().ExposeEntry(kvps[2], Status.New);
            map.Should().ExposeEntry(kvps[3], Status.New);
        }

        [TestMethod] public void ConstructFromListOfKVPsDefaultComparer() {
            // Arrange
            var kvps = new List<KVP>() {
                new KVP("Malmö", "Sweden"),
                new KVP("Tbilisi", "Georgia"),
                new KVP("Casablanca", "Morocco")
            };

            // Act
            var map = new RelationMap<string, string>(kvps);

            // Assert
            map.Count.Should().Be(3);
            map.Comparer.Should().Be(EqualityComparer<string>.Default);
            map[kvps[0].Key].Should().Be(kvps[0].Value);
            map[kvps[1].Key].Should().Be(kvps[1].Value);
            map[kvps[2].Key].Should().Be(kvps[2].Value);
            map.Should().HaveConnectionType<KVP>();
            map.Should().HaveEntryCount(kvps.Count);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(kvps[0], Status.New);
            map.Should().ExposeEntry(kvps[1], Status.New);
            map.Should().ExposeEntry(kvps[2], Status.New);
        }

        [TestMethod] public void ConstructFromDictionaryCustomComparer() {
            // Arrange
            var kvps = new List<KVP>() {
                new KVP("Maracaibo", "Venezuela"),
                new KVP("Majuro", "Marshall Islands"),
                new KVP("Rotterdam", "Netherlands"),
                new KVP("Kingstown", "St. Vincent & the Grenadines"),
            };
            var dict = new Dictionary<string, string>(kvps);
            var comp = StringComparer.OrdinalIgnoreCase;

            // Act
            var map = new RelationMap<string, string>(dict, comp);

            // Assert
            map.Count.Should().Be(4);
            map.Comparer.Should().Be(comp);
            map[kvps[0].Key.ToUpper()].Should().Be(kvps[0].Value);
            map[kvps[1].Key.ToUpper()].Should().Be(kvps[1].Value);
            map[kvps[2].Key.ToUpper()].Should().Be(kvps[2].Value);
            map[kvps[3].Key.ToUpper()].Should().Be(kvps[3].Value);
            map.Should().HaveConnectionType<KVP>();
            map.Should().HaveEntryCount(dict.Count);
            map.Should().ExposeDeletesFirst();

            map.Should().ExposeEntry(kvps[0], Status.New);
            map.Should().ExposeEntry(kvps[1], Status.New);
            map.Should().ExposeEntry(kvps[2], Status.New);
            map.Should().ExposeEntry(kvps[3], Status.New);
        }

        [TestMethod] public void ConstructFromListOfKVPsCustomComparer() {
            // Arrange
            var kvps = new List<KVP>() {
                new KVP("Adwa", "Ethiopia"),
                new KVP("Fallujah", "Iraq"),
                new KVP("Aguascalientes", "Mexico")
            };
            var comp = StringComparer.CurrentCultureIgnoreCase;

            // Act
            var map = new RelationMap<string, string>(kvps, comp);

            // Assert
            map.Count.Should().Be(3);
            map.Comparer.Should().Be(comp);
            map[kvps[0].Key.ToUpper()].Should().Be(kvps[0].Value);
            map[kvps[1].Key.ToUpper()].Should().Be(kvps[1].Value);
            map[kvps[2].Key.ToUpper()].Should().Be(kvps[2].Value);
            map.Should().HaveConnectionType<KVP>();
            map.Should().HaveEntryCount(kvps.Count);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(kvps[0], Status.New);
            map.Should().ExposeEntry(kvps[1], Status.New);
            map.Should().ExposeEntry(kvps[2], Status.New);
        }

        [TestMethod] public void RemoveExistingNewItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Perth", "Australia"),
                new KVP("Puerto Vallarta", "Mexico"),
                new KVP("Windsor", "Canada"),
                new KVP("Voronezh", "Russia"),
                new KVP("Ahmedabad", "India"),
            };
            var map = new RelationMap<string, string>(pairs);

            // Act
            var first_success = map.Remove(pairs[1].Key);
            var second_success = (map as ICollection<KVP>).Remove(pairs[3]);
            (map as IDictionary).Remove(pairs[4].Key);

            // Assert
            map.Count.Should().Be(2);
            first_success.Should().BeTrue();
            second_success.Should().BeTrue();
            map[pairs[0].Key].Should().Be(pairs[0].Value);
            map[pairs[2].Key].Should().Be(pairs[2].Value);
            map.Should().HaveEntryCount(2);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pairs[0], Status.New);
            map.Should().NotExposeEntryFor(pairs[1]);
            map.Should().ExposeEntry(pairs[2], Status.New);
        }

        [TestMethod] public void RemoveExistingSavedItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Brasília", "Brazil"),
                new KVP("Doha", "Qatar"),
                new KVP("Veracruz", "Mexico")
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();

            // Act
            var success = map.Remove(pairs[0].Key);

            // Assert
            map.Count.Should().Be(2);
            success.Should().BeTrue();
            map[pairs[1].Key].Should().Be(pairs[1].Value);
            map[pairs[2].Key].Should().Be(pairs[2].Value);
            map.Should().HaveEntryCount(3);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pairs[0], Status.Deleted);
            map.Should().ExposeEntry(pairs[1], Status.Saved);
            map.Should().ExposeEntry(pairs[2], Status.Saved);
        }

        [TestMethod] public void RemoveNonexistingItem() {
            // Arrange
            var map = new RelationMap<string, string>() { { "Tianjin", "China" }, { "Manama", "Bahrain" } };

            // Act
            var first_success = map.Remove("Bangui");
            var second_success = (map as ICollection<KVP>).Remove(new KVP("Abidjan", "Côte d'Ivoire"));

            // Assert
            map.Count.Should().Be(2);
            first_success.Should().BeFalse();
            second_success.Should().BeFalse();
        }

        [TestMethod] public void RemoveExistingNewItemObtainValue() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Ho Chi Minh City", "Vietnam"),
                new KVP("Kathmandu", "Nepal"),
                new KVP("Kingston", "Jamaica")
            };
            var map = new RelationMap<string, string>(pairs);

            // Act
            var success = map.Remove(pairs[1].Key, out string result);

            // Assert
            map.Count.Should().Be(2);
            success.Should().BeTrue();
            result.Should().Be(pairs[1].Value);
            map[pairs[0].Key].Should().Be(pairs[0].Value);
            map[pairs[2].Key].Should().Be(pairs[2].Value);
            map.Should().HaveEntryCount(2);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pairs[0], Status.New);
            map.Should().NotExposeEntryFor(pairs[1]);
            map.Should().ExposeEntry(pairs[2], Status.New);
        }

        [TestMethod] public void RemoveExistingSavedItemObtainValue() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Windhoek", "Namibia"),
                new KVP("Yellowknife", "Canada"),
                new KVP("Dushanbe", "Tajikistan")
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();

            // Act
            var success = map.Remove(pairs[0].Key, out string result);

            // Assert
            map.Count.Should().Be(2);
            success.Should().BeTrue();
            result.Should().Be(pairs[0].Value);
            map[pairs[1].Key].Should().Be(pairs[1].Value);
            map[pairs[2].Key].Should().Be(pairs[2].Value);
            map.Should().HaveEntryCount(3);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pairs[0], Status.Deleted);
            map.Should().ExposeEntry(pairs[1], Status.Saved);
            map.Should().ExposeEntry(pairs[2], Status.Saved);
        }

        [TestMethod] public void RemoveNonexistingItemObtainValue() {
            // Arrange
            var map = new RelationMap<string, string>() { { "Apia", "Samoa" }, { "Yokohama", "Japan" } };

            // Act
            var success = map.Remove("Malabo", out string? result);

            // Assert
            map.Count.Should().Be(2);
            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [TestMethod] public void Clear() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Cardiff", "United Kingdom (Wales)"),
                new KVP("Jaipur", "India"),
                new KVP("Adelaide", "Australia")
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();
            var single = new KVP("Mecca", "Saudi Arabia");
            map.Add(single.Key, single.Value);

            // Act
            map.Clear();

            // Assert
            map.Count.Should().Be(0);
            map.Should().HaveEntryCount(3);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pairs[0], Status.Deleted);
            map.Should().ExposeEntry(pairs[1], Status.Deleted);
            map.Should().ExposeEntry(pairs[2], Status.Deleted);
            map.Should().NotExposeEntryFor(single);
        }

        [TestMethod] public void CanonicalizeSomeDeleted() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Osaka", "Japan"),
                new KVP("Lahore", "Pakistan"),
                new KVP("Milan", "Italy"),
                new KVP("Khartoum", "Sudan"),
                new KVP("Lima", "Peru"),
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();
            map.Remove(pairs[1].Key);
            map.Remove(pairs[3].Key);

            // Act
            (map as IRelation).Canonicalize();

            // Assert
            map.Count.Should().Be(3);
            map.Should().HaveEntryCount(3);
            map.Should().ExposeEntry(pairs[0], Status.Saved);
            map.Should().NotExposeEntryFor(pairs[1]);
            map.Should().ExposeEntry(pairs[2], Status.Saved);
            map.Should().NotExposeEntryFor(pairs[3]);
            map.Should().ExposeEntry(pairs[4], Status.Saved);
        }

        [TestMethod] public void AddNewItem() {
            // Arrange
            var map = new RelationMap<string, string>();
            var pair0 = new KVP("Yangon", "Myanmar");
            var pair1 = new KVP("Port Louis", "Mauritius");
            var pair2 = new KVP("Port of Spain", "Trinidad and Tobago");

            // Act
            map.Add(pair0.Key, pair0.Value);
            (map as IDictionary).Add(pair1.Key, pair1.Value);
            (map as ICollection<KVP>).Add(pair2);

            // Assert
            map.Count.Should().Be(3);
            map[pair0.Key].Should().Be(pair0.Value);
            map[pair1.Key].Should().Be(pair1.Value);
            map[pair2.Key].Should().Be(pair2.Value);
            map.Should().HaveEntryCount(3);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pair0, Status.New);
            map.Should().ExposeEntry(pair1, Status.New);
            map.Should().ExposeEntry(pair2, Status.New);
        }

        [TestMethod] public void AddExistingDeletedItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Guatemala City", "Guatemala"),
                new KVP("Nouakchott", "Mauritania"),
                new KVP("Naples", "Italy")
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();
            var deleted = pairs[1];
            map.Remove(deleted.Key);

            // Act
            map.Add(deleted.Key, deleted.Value);

            // Assert
            map.Count.Should().Be(3);
            map[pairs[0].Key].Should().Be(pairs[0].Value);
            map[pairs[1].Key].Should().Be(pairs[1].Value);
            map[pairs[2].Key].Should().Be(pairs[2].Value);
            map.Should().HaveEntryCount(3);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(deleted, Status.Saved);
        }

        [TestMethod] public void AddExistingItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Christchurch", "New Zealand"),
                new KVP("Málaga", "Spain"),
                new KVP("Mainz", "Germany"),
                new KVP("Yaoundé", "Cameroon"),
                new KVP("Roseau", "Dominica")
            };
            var map = new RelationMap<string, string>(pairs);

            // Act
            Action act = () => map.Add(pairs[3].Key, pairs[3].Value);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithAnyMessage();
        }

        [TestMethod] public void TryAddNewItem() {
            var map = new RelationMap<string, string>();
            var pair0 = new KVP("Inverness", "Scotland (UK)");
            var pair1 = new KVP("Castries", "St. Lucia");

            // Act
            var success0 = map.TryAdd(pair0.Key, pair0.Value);
            var success1 = map.TryAdd(pair1.Key, pair1.Value);

            // Assert
            map.Count.Should().Be(2);
            success0.Should().BeTrue();
            success1.Should().BeTrue();
            map[pair0.Key].Should().Be(pair0.Value);
            map[pair1.Key].Should().Be(pair1.Value);
            map.Should().HaveEntryCount(2);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pair0, Status.New);
            map.Should().ExposeEntry(pair1, Status.New);
        }

        [TestMethod] public void TryAddExistingDeletedItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Lyon", "France"),
                new KVP("Basseterre", "St. Kitts & Nevis"),
                new KVP("Luxor", "Egypt")
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();
            var deleted = pairs[1];
            map.Remove(deleted.Key);

            // Act
            var success = map.TryAdd(deleted.Key, deleted.Value);

            // Assert
            map.Count.Should().Be(3);
            success.Should().BeTrue();
            map[pairs[0].Key].Should().Be(pairs[0].Value);
            map[pairs[1].Key].Should().Be(pairs[1].Value);
            map[pairs[2].Key].Should().Be(pairs[2].Value);
            map.Should().HaveEntryCount(3);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(deleted, Status.Saved);
        }

        [TestMethod] public void TryAddExistingItem() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("San Luis Potosí City", "Mexico"),
                new KVP("Guadalajara", "Mexico"),
                new KVP("Nuuk", "Greenland (Denmark)"),
                new KVP("Bastogne", "Belgium"),
                new KVP("Ouagadougou", "Burkina Faso")
            };
            var map = new RelationMap<string, string>(pairs);

            // Act
            var success = map.TryAdd(pairs[3].Key, pairs[3].Value);

            // Assert
            success.Should().BeFalse();
        }

        [TestMethod] public void AddNewItemViaIndexer() {
            // Arrange
            var map = new RelationMap<string, string>();
            var pair0 = new KVP("Caracas", "Venezuela");
            var pair1 = new KVP("Winnipeg", "Canada");

            // Act
            map[pair0.Key] = pair0.Value;
            map[pair1.Key] = pair1.Value;

            // Assert
            map.Count.Should().Be(2);
            map[pair0.Key].Should().Be(pair0.Value);
            map[pair1.Key].Should().Be(pair1.Value);
            map.Should().HaveEntryCount(2);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pair0, Status.New);
            map.Should().ExposeEntry(pair1, Status.New);
        }

        [TestMethod] public void OverwriteValueOfNewKey() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Accra", "Ghana"),
                new KVP("Mbabane", "eSwatini"),
                new KVP("Rabat", "Morocco"),
                new KVP("Edmonton", "Canada")
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();
            var single0 = new KVP("Camagüey", "Cuba");
            var single1 = new KVP("Porto", "Portugal");
            map.Add(single0.Key, single0.Value);
            map.Add(single1.Key, single1.Value);

            // Act
            var newValue = "~~~Portugal~~~";
            map[single1.Key] = newValue;
            (map as IDictionary)[single1.Key] = newValue;

            // Assert
            map.Count.Should().Be(6);
            map[pairs[0].Key].Should().Be(pairs[0].Value);
            map[pairs[1].Key].Should().Be(pairs[1].Value);
            map[pairs[2].Key].Should().Be(pairs[2].Value);
            map[pairs[3].Key].Should().Be(pairs[3].Value);
            map.Should().HaveEntryCount(6);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pairs[0], Status.Saved);
            map.Should().ExposeEntry(pairs[1], Status.Saved);
            map.Should().ExposeEntry(pairs[2], Status.Saved);
            map.Should().ExposeEntry(pairs[3], Status.Saved);
            map.Should().ExposeEntry(single0, Status.New);
            map.Should().ExposeEntry(new KVP(single1.Key, newValue), Status.New);
        }

        [TestMethod] public void OverwriteValueOfSavedKey() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Trondheim", "Norway"),
                new KVP("Jalalabad", "Afghanistan"),
                new KVP("Hebron", "Israel"),
                new KVP("Alice Springs", "Australia")
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();
            var single0 = new KVP("Maputo", "Mozambique");
            var single1 = new KVP("Jeju City", "South Korea");
            map.Add(single0.Key, single0.Value);
            map.Add(single1.Key, single1.Value);

            // Act
            var newValue = "Yisra'el";
            map[pairs[2].Key] = newValue;
            (map as IDictionary)[pairs[2].Key] = newValue;

            // Assert
            map.Count.Should().Be(6);
            map[pairs[0].Key].Should().Be(pairs[0].Value);
            map[pairs[1].Key].Should().Be(pairs[1].Value);
            map[pairs[2].Key].Should().Be(newValue);
            map[pairs[3].Key].Should().Be(pairs[3].Value);
            map.Should().HaveEntryCount(7);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pairs[0], Status.Saved);
            map.Should().ExposeEntry(pairs[1], Status.Saved);
            map.Should().ExposeEntry(pairs[2], Status.Deleted);
            map.Should().ExposeEntry(new KVP(pairs[2].Key, newValue), Status.New);
            map.Should().ExposeEntry(pairs[3], Status.Saved);
            map.Should().ExposeEntry(single0, Status.New);
            map.Should().ExposeEntry(single1, Status.New);
        }

        [TestMethod] public void OverwriteValueOfSavedKeyWithSelf() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Magdeburg", "Germany"),
                new KVP("Conakry", "Guinea"),
                new KVP("Aberdeen", "Scotland (UK)"),
                new KVP("San Marino", "San Marino")
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();
            var single0 = new KVP("Innsbruck", "Austria");
            var single1 = new KVP("Bern", "Switzerland");
            map.Add(single0.Key, single0.Value);
            map.Add(single1.Key, single1.Value);

            // Act
            map[pairs[0].Key] = pairs[0].Value;

            // Assert
            map.Count.Should().Be(6);
            map[pairs[0].Key].Should().Be(pairs[0].Value);
            map[pairs[1].Key].Should().Be(pairs[1].Value);
            map[pairs[2].Key].Should().Be(pairs[2].Value);
            map[pairs[3].Key].Should().Be(pairs[3].Value);
            map.Should().HaveEntryCount(6);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pairs[0], Status.Saved);
            map.Should().ExposeEntry(pairs[1], Status.Saved);
            map.Should().ExposeEntry(pairs[2], Status.Saved);
            map.Should().ExposeEntry(pairs[3], Status.Saved);
            map.Should().ExposeEntry(single0, Status.New);
            map.Should().ExposeEntry(single1, Status.New);
        }

        [TestMethod] public void IndexAddDeletedItemWithSameValue() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Bilbao", "Spain"),
                new KVP("Andorra la Vella", "Andorra"),
                new KVP("Porto Alegre", "Brazil"),
                new KVP("Marrakesh", "Morocco")
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();
            var single0 = new KVP("Lilongwe", "Malawi");
            var single1 = new KVP("Bloemfontein", "South Africa");
            map.Add(single0.Key, single0.Value);
            map.Add(single1.Key, single1.Value);
            var removal = pairs[3];
            map.Remove(removal.Key);

            // Act
            map[removal.Key] = removal.Value;

            // Assert
            map.Count.Should().Be(6);
            map[pairs[0].Key].Should().Be(pairs[0].Value);
            map[pairs[1].Key].Should().Be(pairs[1].Value);
            map[pairs[2].Key].Should().Be(pairs[2].Value);
            map[pairs[3].Key].Should().Be(pairs[3].Value);
            map.Should().HaveEntryCount(6);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pairs[0], Status.Saved);
            map.Should().ExposeEntry(pairs[1], Status.Saved);
            map.Should().ExposeEntry(pairs[2], Status.Saved);
            map.Should().ExposeEntry(pairs[3], Status.Saved);
            map.Should().ExposeEntry(single0, Status.New);
            map.Should().ExposeEntry(single1, Status.New);
        }

        [TestMethod] public void IndexAddDeletedItemWithNewValue() {
            // Arrange
            var pairs = new KVP[] {
                new KVP("Galway", "Ireland"),
                new KVP("Port-au-Prince", "Haiti"),
                new KVP("Ocho Rios", "Jamaica"),
                new KVP("Launceston", "Australia")
            };
            var map = new RelationMap<string, string>(pairs);
            (map as IRelation).Canonicalize();
            var single0 = new KVP("Rhodes", "Greece");
            var single1 = new KVP("Ashgabat", "Turkmenistan");
            map.Add(single0.Key, single0.Value);
            map.Add(single1.Key, single1.Value);
            var removal = pairs[3];
            map.Remove(removal.Key);

            // Act
            var newValue = "## -> Australia";
            map[removal.Key] = newValue;

            // Assert
            map.Count.Should().Be(6);
            map[pairs[0].Key].Should().Be(pairs[0].Value);
            map[pairs[1].Key].Should().Be(pairs[1].Value);
            map[pairs[2].Key].Should().Be(pairs[2].Value);
            map[pairs[3].Key].Should().Be(newValue);
            map.Should().HaveEntryCount(7);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(pairs[0], Status.Saved);
            map.Should().ExposeEntry(pairs[1], Status.Saved);
            map.Should().ExposeEntry(pairs[2], Status.Saved);
            map.Should().ExposeEntry(pairs[3], Status.Deleted);
            map.Should().ExposeEntry(new KVP(removal.Key, newValue), Status.New);
            map.Should().ExposeEntry(single0, Status.New);
            map.Should().ExposeEntry(single1, Status.New);
        }

        [TestMethod] public void Repopulate() {
            // Arrange
            var map = new RelationMap<string, string>();
            var kvp0 = new KVP("Łódź", "Poland");
            var kvp1 = new KVP("Toulouse", "France");
            var kvp2 = new KVP("Córdoba", "Spain");

            // Act
            (map as IRelation).Repopulate(kvp0);
            (map as IRelation).Repopulate(kvp1);
            (map as IRelation).Repopulate(kvp2);

            // Assert
            map.Count.Should().Be(3);
            map[kvp0.Key].Should().Be(kvp0.Value);
            map[kvp1.Key].Should().Be(kvp1.Value);
            map[kvp2.Key].Should().Be(kvp2.Value);
            map.Should().HaveEntryCount(3);
            map.Should().ExposeDeletesFirst();
            map.Should().ExposeEntry(kvp0, Status.Saved);
            map.Should().ExposeEntry(kvp1, Status.Saved);
            map.Should().ExposeEntry(kvp2, Status.Saved);
        }
    }
}
