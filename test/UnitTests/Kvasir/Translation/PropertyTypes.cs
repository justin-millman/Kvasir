using FluentAssertions;
using FluentAssertions.Execution;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

using static UT.Kvasir.Translation.PropertyTypes;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Property Types")]
    public class PropertyTypeTests {
        [TestMethod] public void NonNullableScalars() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Smorgasbord);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("Byte").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Char").OfTypeCharacter().BeingNonNullable().And
                .HaveField("DateTime").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Decimal").OfTypeDecimal().BeingNonNullable().And
                .HaveField("Double").OfTypeDouble().BeingNonNullable().And
                .HaveField("Float").OfTypeSingle().BeingNonNullable().And
                .HaveField("Guid").OfTypeGuid().BeingNonNullable().And
                .HaveField("Int").OfTypeInt32().BeingNonNullable().And
                .HaveField("Long").OfTypeInt64().BeingNonNullable().And
                .HaveField("SByte").OfTypeInt8().BeingNonNullable().And
                .HaveField("Short").OfTypeInt16().BeingNonNullable().And
                .HaveField("String").OfTypeText().BeingNonNullable().And
                .HaveField("UInt").OfTypeUInt32().BeingNonNullable().And
                .HaveField("ULong").OfTypeUInt64().BeingNonNullable().And
                .HaveField("UShort").OfTypeUInt16().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NullableScalars() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Plethora);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("Byte").OfTypeUInt8().BeingNullable().And
                .HaveField("Char").OfTypeCharacter().BeingNullable().And
                .HaveField("DateTime").OfTypeDateTime().BeingNullable().And
                .HaveField("Decimal").OfTypeDecimal().BeingNullable().And
                .HaveField("Double").OfTypeDouble().BeingNullable().And
                .HaveField("Float").OfTypeSingle().BeingNullable().And
                .HaveField("Guid").OfTypeGuid().BeingNullable().And
                .HaveField("Int").OfTypeInt32().BeingNullable().And
                .HaveField("Long").OfTypeInt64().BeingNullable().And
                .HaveField("SByte").OfTypeInt8().BeingNullable().And
                .HaveField("Short").OfTypeInt16().BeingNullable().And
                .HaveField("String").OfTypeText().BeingNullable().And
                .HaveField("UInt").OfTypeUInt32().BeingNullable().And
                .HaveField("ULong").OfTypeUInt64().BeingNullable().And
                .HaveField("UShort").OfTypeUInt16().BeingNullable().And
                .HaveField("PrimaryKey").OfTypeInt32().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void PropertyTypeIsDelegate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hurricane);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Hurricane` → Form")
                .WithProblem("type `HurricaneAction` is a delegate and cannot be the backing type of a property")
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsDynamic_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MonopolyProperty);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`MonopolyProperty` → HotelCost")
                .WithProblem("type `object` is `object` (or `dynamic`) and cannot be the backing type of a property")
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsObject_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(URL);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`URL` → NetLoc")
                .WithProblem("type `object` is `object` (or `dynamic`) and cannot be the backing type of a property")
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsSystemEnum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Enumeration);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Enumeration` → ZeroValue")
                .WithProblem("type `Enum` is a universal base class and cannot be the backing type of a property")
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsSystemValueType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(YouTubeVideo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`YouTubeVideo` → CommentCount")
                .WithProblem("type `ValueType` is a universal base class and cannot be the backing type of a property")
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsNonCollectionClassFromStandardLibrary_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Coin);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Coin` → CounterfeitResult")
                .WithProblem(
                    $"type `Exception` comes from assembly {typeof(Exception).Assembly.FullName!}, " +
                    $"not from user assembly {typeof(Coin).Assembly.FullName!}"
                )
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsCollectionFromStandardLibrary_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Eigenvector);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Eigenvector` → Vector")
                .WithProblem(
                    $"type `ArrayList` comes from assembly {typeof(ArrayList).Assembly.FullName!}, " +
                    $"not from user assembly {typeof(Eigenvector).Assembly.FullName!}"
                )
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsStructFromStandardLibrary_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Emoji);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Emoji` → Identity")
                .WithProblem(
                    $"type `Uri` comes from assembly {typeof(Uri).Assembly.FullName!}, " +
                    $"not from user assembly {typeof(Emoji).Assembly.FullName!}"
                )
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsClassFromNugetPackage_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(UUID);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`UUID` → GenerationScope")
                .WithProblem(
                    $"type `Continuation` comes from assembly {typeof(Continuation).Assembly.FullName!}, " +
                    $"not from user assembly {typeof(UUID).Assembly.FullName!}"
                )
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsInterface_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Painting);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Painting` → Artist")
                .WithProblem("type `IArtist` is an interface and cannot be the backing type of a property")
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsClosedGenericClass_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlackChannel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`SlackChannel` → NumMessages")
                .WithProblem("type `MessageCount<short>` is a closed generic type and cannot be the backing type of a property")
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsAbstractClass_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BotanicalGarden);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`BotanicalGarden` → OfficialFlower")
                .WithProblem("type `Flower` is an abstract class and cannot be the backing type of a property")
                .EndMessage();
        }

        [TestMethod] public void CodeOnly_PropertyOfUnsupportedType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DNDCharacter);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Charisma").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Constitution").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Dexterity").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Intelligence").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Strength").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Wisdom").OfTypeUInt8().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void Enumerations() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DNDWeapon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("AttackBonus").OfTypeUInt16().BeingNonNullable().And
                .HaveField("AverageDamage").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Type").OfTypeEnumeration(
                    DNDWeapon.WeaponType.Simple,
                    DNDWeapon.WeaponType.Martial,
                    DNDWeapon.WeaponType.Improvised
                ).BeingNonNullable().And
                .HaveField("Properties").OfTypeEnumeration(
                    DNDWeapon.WeaponProperty.Finesse,
                    DNDWeapon.WeaponProperty.Silvered,
                    DNDWeapon.WeaponProperty.Ranged,
                    DNDWeapon.WeaponProperty.TwoHanded,
                    DNDWeapon.WeaponProperty.Finesse | DNDWeapon.WeaponProperty.Silvered,
                    DNDWeapon.WeaponProperty.Finesse | DNDWeapon.WeaponProperty.Ranged,
                    DNDWeapon.WeaponProperty.Finesse | DNDWeapon.WeaponProperty.TwoHanded,
                    DNDWeapon.WeaponProperty.Silvered | DNDWeapon.WeaponProperty.Ranged,
                    DNDWeapon.WeaponProperty.Silvered | DNDWeapon.WeaponProperty.TwoHanded,
                    DNDWeapon.WeaponProperty.Ranged | DNDWeapon.WeaponProperty.TwoHanded,
                    DNDWeapon.WeaponProperty.Finesse | DNDWeapon.WeaponProperty.Silvered | DNDWeapon.WeaponProperty.Ranged,
                    DNDWeapon.WeaponProperty.Finesse | DNDWeapon.WeaponProperty.Silvered | DNDWeapon.WeaponProperty.TwoHanded,
                    DNDWeapon.WeaponProperty.Silvered | DNDWeapon.WeaponProperty.Ranged | DNDWeapon.WeaponProperty.TwoHanded,
                    DNDWeapon.WeaponProperty.Ranged | DNDWeapon.WeaponProperty.TwoHanded | DNDWeapon.WeaponProperty.Finesse,
                    DNDWeapon.WeaponProperty.Finesse | DNDWeapon.WeaponProperty.Silvered | DNDWeapon.WeaponProperty.Ranged | DNDWeapon.WeaponProperty.TwoHanded
                ).BeingNonNullable().And
                .HaveField("MostEffectiveOn").OfTypeEnumeration(
                    DayOfWeek.Sunday,
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday
                ).BeingNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NonNullableAggregates() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChineseDynasty);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Founder.Name").OfTypeText().BeingNonNullable().And
                .HaveField("Founder.ReignBegin").OfTypeInt16().BeingNonNullable().And
                .HaveField("Founder.ReignEnd").OfTypeInt16().BeingNonNullable().And
                .HaveField("Founder.Death").OfTypeText().BeingNullable().And
                .HaveField("MaxExtent").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Established").OfTypeInt16().BeingNonNullable().And
                .HaveField("Fell").OfTypeInt16().BeingNonNullable().And
                .HaveField("Population").OfTypeUInt64().BeingNonNullable().And
                .HaveField("Capital.Name").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NullableAggregates() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BarbecueSauce);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Brand").OfTypeText().BeingNonNullable().And
                .HaveField("PerServing.Calories").OfTypeUInt32().BeingNullable().And
                .HaveField("PerServing.Fat").OfTypeDouble().BeingNullable().And
                .HaveField("PerServing.Sugar").OfTypeDouble().BeingNullable().And
                .HaveField("PerServing.Carbohydrates").OfTypeDouble().BeingNullable().And
                .HaveField("KetchupBased").OfTypeBoolean().BeingNonNullable().And
                .HaveField("Style").OfTypeEnumeration(
                    BarbecueSauce.Kind.Sweet,
                    BarbecueSauce.Kind.Spicy,
                    BarbecueSauce.Kind.Tangy,
                    BarbecueSauce.Kind.Chocolatey
                ).BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NestedAggregates() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DNDMonster);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("Species").OfTypeText().BeingNonNullable().And
                .HaveField("Stats.STR").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.DEX").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.CON").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.INT").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.WIS").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.CHA").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.Saves.STR").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.Saves.DEX").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.Saves.CON").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.Saves.INT").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.Saves.WIS").OfTypeUInt8().BeingNullable().And
                .HaveField("Stats.Saves.CHA").OfTypeUInt8().BeingNullable().And
                .HaveField("Size").OfTypeEnumeration(
                    DNDMonster.BodySize.Tiny,
                    DNDMonster.BodySize.Small,
                    DNDMonster.BodySize.Medium,
                    DNDMonster.BodySize.Large,
                    DNDMonster.BodySize.Huge,
                    DNDMonster.BodySize.Gargantuan
                ).BeingNonNullable().And
                .HaveField("PhysicalSenses.PassivePerception").OfTypeUInt8().BeingNonNullable().And
                .HaveField("PhysicalSenses.Sight.Distance").OfTypeUInt16().BeingNonNullable().And
                .HaveField("PhysicalSenses.Sight.Darkness").OfTypeBoolean().BeingNonNullable().And
                .HaveField("PhysicalSenses.Sight.Trueness").OfTypeBoolean().BeingNonNullable().And
                .HaveField("CR").OfTypeUInt16().BeingNonNullable().And
                .HaveField("AC").OfTypeUInt32().BeingNonNullable().And
                .HaveField("HP").OfTypeUInt32().BeingNonNullable().And
                .HaveField("LegendaryActions").OfTypeUInt8().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NonNullableReferences() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Scorpion);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("CommonName").OfTypeText().BeingNonNullable().And
                .HaveField("Genus.Genus").OfTypeText().BeingNonNullable().And
                .HaveField("Species").OfTypeText().BeingNonNullable().And
                .HaveField("StingIndex").OfTypeDouble().BeingNonNullable().And
                .HaveField("AverageLength").OfTypeSingle().BeingNonNullable().And
                .HaveField("AverageWeight").OfTypeSingle().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Genus.Genus")
                    .Against(translator[typeof(Scorpion.TaxonomicGenus)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NullableReferences() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ferry);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("RegistrationNumber").OfTypeGuid().BeingNonNullable().And
                .HaveField("PassengerCapacity").OfTypeUInt64().BeingNullable().And
                .HaveField("Type").OfTypeEnumeration(
                    Ferry.Kind.Passenger,
                    Ferry.Kind.Cargo,
                    Ferry.Kind.State,
                    Ferry.Kind.Passenger | Ferry.Kind.Cargo,
                    Ferry.Kind.Passenger | Ferry.Kind.State,
                    Ferry.Kind.Cargo | Ferry.Kind.State,
                    Ferry.Kind.Passenger | Ferry.Kind.Cargo | Ferry.Kind.State
                ).BeingNonNullable().And
                .HaveField("Embarcation.PortID").OfTypeGuid().BeingNullable().And
                .HaveField("Embarcation.PortName").OfTypeText().BeingNullable().And
                .HaveField("Destination.PortID").OfTypeGuid().BeingNullable().And
                .HaveField("Destination.PortName").OfTypeText().BeingNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Embarcation.PortID", "Embarcation.PortName")
                    .Against(translator[typeof(Ferry.Port)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Destination.PortID", "Destination.PortName")
                    .Against(translator[typeof(Ferry.Port)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void ReferencesNestedWithinAggregates() {
            // Arrange
            var translator = new Translator();
            var source = typeof(WeekendUpdate);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Airing.Month").OfTypeEnumeration(
                    WeekendUpdate.Date.MonthOfYear.JAN,
                    WeekendUpdate.Date.MonthOfYear.FEB,
                    WeekendUpdate.Date.MonthOfYear.MAR,
                    WeekendUpdate.Date.MonthOfYear.APR,
                    WeekendUpdate.Date.MonthOfYear.MAY,
                    WeekendUpdate.Date.MonthOfYear.JUN,
                    WeekendUpdate.Date.MonthOfYear.JUL,
                    WeekendUpdate.Date.MonthOfYear.AUG,
                    WeekendUpdate.Date.MonthOfYear.SEP,
                    WeekendUpdate.Date.MonthOfYear.OCT,
                    WeekendUpdate.Date.MonthOfYear.NOV,
                    WeekendUpdate.Date.MonthOfYear.DEC
                ).BeingNonNullable().And
                .HaveField("Airing.Day").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Airing.Year").OfTypeInt16().BeingNonNullable().And
                .HaveField("Anchor.FirstName").OfTypeText().BeingNonNullable().And
                .HaveField("Anchor.LastName").OfTypeText().BeingNonNullable().And
                .HaveField("FirstSegment.Name").OfTypeText().BeingNullable().And
                .HaveField("FirstSegment.Portrayal.FirstName").OfTypeText().BeingNullable().And
                .HaveField("FirstSegment.Portrayal.LastName").OfTypeText().BeingNullable().And
                .HaveField("SecondSegment.Name").OfTypeText().BeingNullable().And
                .HaveField("SecondSegment.Portrayal.FirstName").OfTypeText().BeingNullable().And
                .HaveField("SecondSegment.Portrayal.LastName").OfTypeText().BeingNullable().And
                .HaveField("NumJokes").OfTypeInt8().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Airing.Day", "Airing.Month", "Airing.Year")
                    .Against(translator[typeof(WeekendUpdate.Date)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Anchor.FirstName", "Anchor.LastName")
                    .Against(translator[typeof(WeekendUpdate.Actor)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("FirstSegment.Portrayal.FirstName", "FirstSegment.Portrayal.LastName")
                    .Against(translator[typeof(WeekendUpdate.Actor)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("SecondSegment.Portrayal.FirstName", "SecondSegment.Portrayal.LastName")
                    .Against(translator[typeof(WeekendUpdate.Actor)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void ReferencesNestedWithinReferences() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DannyPhantomGhost);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().BeEmpty();
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Powers").OfTypeEnumeration(
                    DannyPhantomGhost.Ability.Intangibiblity,
                    DannyPhantomGhost.Ability.Blast,
                    DannyPhantomGhost.Ability.Overshadowing,
                    DannyPhantomGhost.Ability.Duplication,
                    DannyPhantomGhost.Ability.Telekinesis
                ).BeingNonNullable().And
                .HaveField("Appearances").OfTypeInt32().BeingNonNullable().And
                .HaveField("Debut.Overall").OfTypeInt16().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Debut.Overall")
                    .Against(translator[typeof(DannyPhantomGhost.Episode)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NonNullableRelationsOfNonNullableElements() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CMakeTarget);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(4);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+CMakeTarget.FilesTable").And
                .HaveField("CMakeTarget.Project").OfTypeText().BeingNonNullable().And
                .HaveField("CMakeTarget.TargetName").OfTypeText().BeingNonNullable().And
                .HaveField("Item").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("CMakeTarget.Project", "CMakeTarget.TargetName")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[1].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+CMakeTarget.LinkAgainstTable").And
                .HaveField("CMakeTarget.Project").OfTypeText().BeingNonNullable().And
                .HaveField("CMakeTarget.TargetName").OfTypeText().BeingNonNullable().And
                .HaveField("Index").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Item").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("CMakeTarget.Project", "CMakeTarget.TargetName")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[2].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+CMakeTarget.MacrosTable").And
                .HaveField("CMakeTarget.Project").OfTypeText().BeingNonNullable().And
                .HaveField("CMakeTarget.TargetName").OfTypeText().BeingNonNullable().And
                .HaveField("Item.Symbol").OfTypeText().BeingNonNullable().And
                .HaveField("Item.Value").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("CMakeTarget.Project", "CMakeTarget.TargetName")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[3].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+CMakeTarget.OptimizationLevelTable").And
                .HaveField("CMakeTarget.Project").OfTypeText().BeingNonNullable().And
                .HaveField("CMakeTarget.TargetName").OfTypeText().BeingNonNullable().And
                .HaveField("Key").OfTypeEnumeration(
                    CMakeTarget.Mode.Public, CMakeTarget.Mode.Private, CMakeTarget.Mode.Interface
                ).BeingNonNullable().And
                .HaveField("Value").OfTypeInt32().And
                .HaveNoOtherFields().And
                .HaveForeignKey("CMakeTarget.Project", "CMakeTarget.TargetName")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NullableRelationsOfNonNullableElements() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Forecast);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(4);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+Forecast.DailiesTable").And
                .HaveField("Forecast.City").OfTypeText().BeingNonNullable().And
                .HaveField("Item.Date").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Item.HighTemp").OfTypeSingle().BeingNonNullable().And
                .HaveField("Item.LowTemp").OfTypeSingle().BeingNonNullable().And
                .HaveField("Item.ChanceRain").OfTypeDouble().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Forecast.City")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[1].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+Forecast.DataSourcesTable").And
                .HaveField("Forecast.City").OfTypeText().BeingNonNullable().And
                .HaveField("Key").OfTypeText().BeingNonNullable().And
                .HaveField("Value").OfTypeBoolean().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Forecast.City")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[2].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+Forecast.ExtremeWeatherTable").And
                .HaveField("Forecast.City").OfTypeText().BeingNonNullable().And
                .HaveField("Index").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Item").OfTypeEnumeration(
                    Forecast.Extremity.Hurricane,
                    Forecast.Extremity.Tornado,
                    Forecast.Extremity.Blizzard,
                    Forecast.Extremity.Thunderstorm,
                    Forecast.Extremity.Hailstorm,
                    Forecast.Extremity.Sandstorm
                ).BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Forecast.City")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[3].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+Forecast.MeteorologistsTable").And
                .HaveField("Forecast.City").OfTypeText().BeingNonNullable().And
                .HaveField("Item").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Forecast.City")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void ReadOnlyRelations() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CivVIDistrict);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(4);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+CivVIDistrict.AllowedTerrainTable").And
                .HaveField("CivVIDistrict.DistrictName").OfTypeText().BeingNonNullable().And
                .HaveField("Item").OfTypeEnumeration(
                    CivVIDistrict.Terrain.Flat,
                    CivVIDistrict.Terrain.Grasslands,
                    CivVIDistrict.Terrain.Marsh,
                    CivVIDistrict.Terrain.Floodplains,
                    CivVIDistrict.Terrain.Hills,
                    CivVIDistrict.Terrain.Desert,
                    CivVIDistrict.Terrain.Coast,
                    CivVIDistrict.Terrain.Ocean,
                    CivVIDistrict.Terrain.Reef,
                    CivVIDistrict.Terrain.Lake,
                    CivVIDistrict.Terrain.Mountain
                ).BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("CivVIDistrict.DistrictName")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[1].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+CivVIDistrict.BuildingsTable").And
                .HaveField("CivVIDistrict.DistrictName").OfTypeText().BeingNonNullable().And
                .HaveField("Item.BuildingName").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("CivVIDistrict.DistrictName")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Item.BuildingName")
                    .Against(translator[typeof(CivVIDistrict.CivVIBuilding)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[2].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+CivVIDistrict.IconsTable").And
                .HaveField("CivVIDistrict.DistrictName").OfTypeText().BeingNonNullable().And
                .HaveField("Index").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Item").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("CivVIDistrict.DistrictName")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[3].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+CivVIDistrict.YieldsTable").And
                .HaveField("CivVIDistrict.DistrictName").OfTypeText().BeingNonNullable().And
                .HaveField("Key").OfTypeInt32().BeingNonNullable().And
                .HaveField("Value.Amount").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Value.OneTimeOnly").OfTypeBoolean().BeingNonNullable().And
                .HaveField("Value.Multiplier").OfTypeDouble().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("CivVIDistrict.DistrictName")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void RelationsNestedWithinAggregates() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Gelateria);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.PropertyTypes+Gelateria.Owners.PeopleTable").And
                .HaveField("Gelateria.GelateriaID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Item").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Gelateria.GelateriaID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void RelationNestedWithinRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BlackHole);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NestedRelationException>()
                .WithLocation("`BlackHole` → <synthetic> `Measurements` → Value")
                .WithProblem("nested Relations are not supported")
                .EndMessage();
        }

        [TestMethod] public void RelationNestedWithAggregateNestedWithinRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Poll);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<NestedRelationException>()
                .WithLocation("`Poll` → <synthetic> `Questions` → `Question` (from \"Item\") → Answers")
                .WithProblem("nested Relations are not supported")
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsListSetOfKeyValuePair_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Caricature);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Caricature` → <synthetic> `SaleHistory` → Item")
                .WithProblem("type `KeyValuePair<DateTime, decimal>` is a closed generic type and cannot be the backing type of a property")
                .EndMessage();
        }

        [TestMethod] public void PropertyTypeIsIRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Perfume);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPropertyInDataModelException>()
                .WithLocation("`Perfume` → PatentNumbers")
                .WithProblem("type `IRelation` is the `IRelation` interface and cannot be the backing type of a property")
                .EndMessage();
        }
    }
}
