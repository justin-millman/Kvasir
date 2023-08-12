using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Optional;
using System;

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
            translation.Principal.Table.Should()
                .HaveField(nameof(Smorgasbord.Byte)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.Char)).OfTypeCharacter().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.DateTime)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.Decimal)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.Double)).OfTypeDouble().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.Float)).OfTypeSingle().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.Guid)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.Int)).OfTypeInt32().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.Long)).OfTypeInt64().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.SByte)).OfTypeInt8().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.Short)).OfTypeInt16().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.String)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.UInt)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.ULong)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Smorgasbord.UShort)).OfTypeUInt16().BeingNonNullable().And
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
            translation.Principal.Table.Should()
                .HaveField(nameof(Plethora.Byte)).OfTypeUInt8().BeingNullable().And
                .HaveField(nameof(Plethora.Char)).OfTypeCharacter().BeingNullable().And
                .HaveField(nameof(Plethora.DateTime)).OfTypeDateTime().BeingNullable().And
                .HaveField(nameof(Plethora.Decimal)).OfTypeDecimal().BeingNullable().And
                .HaveField(nameof(Plethora.Double)).OfTypeDouble().BeingNullable().And
                .HaveField(nameof(Plethora.Float)).OfTypeSingle().BeingNullable().And
                .HaveField(nameof(Plethora.Guid)).OfTypeGuid().BeingNullable().And
                .HaveField(nameof(Plethora.Int)).OfTypeInt32().BeingNullable().And
                .HaveField(nameof(Plethora.Long)).OfTypeInt64().BeingNullable().And
                .HaveField(nameof(Plethora.SByte)).OfTypeInt8().BeingNullable().And
                .HaveField(nameof(Plethora.Short)).OfTypeInt16().BeingNullable().And
                .HaveField(nameof(Plethora.String)).OfTypeText().BeingNullable().And
                .HaveField(nameof(Plethora.UInt)).OfTypeUInt32().BeingNullable().And
                .HaveField(nameof(Plethora.ULong)).OfTypeUInt64().BeingNullable().And
                .HaveField(nameof(Plethora.UShort)).OfTypeUInt16().BeingNullable().And
                .HaveField(nameof(Plethora.PrimaryKey)).OfTypeInt32().BeingNonNullable().And
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
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Hurricane.Form))                      // error location
                .WithMessageContaining("unsupported type")                          // category
                .WithMessageContaining("delegate")                                  // details / explanation
                .WithMessageContaining(nameof(Action));                             // details / explanation
        }

        [TestMethod] public void PropertyTypeIsDynamic_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MonopolyProperty);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MonopolyProperty.HotelCost))          // error location
                .WithMessageContaining("unsupported type")                          // category
                .WithMessageContaining("dynamic");                                  // details / explanation
        }

        [TestMethod] public void PropertyTypeIsObject_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(URL);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(URL.NetLoc))                          // error location
                .WithMessageContaining("unsupported type")                          // category
                .WithMessageContaining(nameof(Object));                             // details / explanation
        }

        [TestMethod] public void PropertyTypeIsSystemEnum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Enumeration);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Enumeration.ZeroValue))               // error location
                .WithMessageContaining("unsupported type")                          // category
                .WithMessageContaining(nameof(Enum));                               // details / explanation
        }

        [TestMethod] public void PropertyTypeIsSystemValueType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(YouTubeVideo);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(YouTubeVideo.CommentCount))           // error location
                .WithMessageContaining("unsupported type")                          // category
                .WithMessageContaining(nameof(ValueType));                          // details / explanation
        }

        [TestMethod] public void PropertyTypeIsFromStandardLibrary_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Coin);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Coin.CounterfeitResult))              // error location
                .WithMessageContaining("unsupported type")                          // category
                .WithMessageContaining(nameof(Exception))                           // details / explanation
                .WithMessageContaining(typeof(Exception).Assembly.FullName!);       // details / explanation
        }

        [TestMethod] public void PropertyTypeIsFromNugetPackage_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(UUID);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(UUID.Signature))                      // error location
                .WithMessageContaining("unsupported type")                          // category
                .WithMessageContaining(nameof(Option<string>))                      // details / explanation
                .WithMessageContaining(typeof(Option<string>).Assembly.FullName!);  // details / explanation
        }

        [TestMethod] public void PropertyTypeIsInterface_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Painting);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Painting.Artist))                     // error location
                .WithMessageContaining("unsupported type")                          // category
                .WithMessageContaining("interface")                                 // details / explanation
                .WithMessageContaining(nameof(IArtist));                            // details / explanation
        }

        [TestMethod] public void PropertyTypeIsClosedGenericClass_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlackChannel);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SlackChannel.NumMessages))            // error location
                .WithMessageContaining("unsupported type")                          // category
                .WithMessageContaining("generic")                                   // details / explanation
                .WithMessageContaining(nameof(MessageCount<short>));                // details / explanation
        }

        [TestMethod] public void PropertyTypeIsAbstractClass_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BotanicalGarden);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BotanicalGarden.OfficialFlower))      // error location
                .WithMessageContaining("unsupported type")                          // category
                .WithMessageContaining("abstract")                                  // details / explanation
                .WithMessageContaining(nameof(Flower));                             // details / explanation
        }

        [TestMethod] public void CodeOnly_PropertyOfUnsupportedType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DNDCharacter);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(DNDCharacter.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(DNDCharacter.Charisma)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(DNDCharacter.Constitution)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(DNDCharacter.Dexterity)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(DNDCharacter.Intelligence)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(DNDCharacter.Strength)).OfTypeUInt8().BeingNonNullable().And
                .HaveField(nameof(DNDCharacter.Wisdom)).OfTypeUInt8().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void Enumerations() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DNDWeapon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(DNDWeapon.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(DNDWeapon.AttackBonus)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(DNDWeapon.AverageDamage)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(DNDWeapon.Type)).OfTypeEnumeration(
                    WeaponType.Simple,
                    WeaponType.Martial,
                    WeaponType.Improvised
                ).BeingNonNullable().And
                .HaveField(nameof(DNDWeapon.Properties)).OfTypeEnumeration(
                    WeaponProperty.Finesse,
                    WeaponProperty.Silvered,
                    WeaponProperty.Ranged,
                    WeaponProperty.TwoHanded,
                    WeaponProperty.Finesse | WeaponProperty.Silvered,
                    WeaponProperty.Finesse | WeaponProperty.Ranged,
                    WeaponProperty.Finesse | WeaponProperty.TwoHanded,
                    WeaponProperty.Silvered | WeaponProperty.Ranged,
                    WeaponProperty.Silvered | WeaponProperty.TwoHanded,
                    WeaponProperty.Ranged | WeaponProperty.TwoHanded,
                    WeaponProperty.Finesse | WeaponProperty.Silvered | WeaponProperty.Ranged,
                    WeaponProperty.Finesse | WeaponProperty.Silvered | WeaponProperty.TwoHanded,
                    WeaponProperty.Silvered | WeaponProperty.Ranged | WeaponProperty.TwoHanded,
                    WeaponProperty.Ranged | WeaponProperty.TwoHanded | WeaponProperty.Finesse,
                    WeaponProperty.Finesse | WeaponProperty.Silvered | WeaponProperty.Ranged | WeaponProperty.TwoHanded
                ).BeingNonNullable().And
                .HaveField(nameof(DNDWeapon.MostEffectiveOn)).OfTypeEnumeration(
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
            translation.Principal.Table.Should()
                .HaveField(nameof(ChineseDynasty.Name)).OfTypeText().BeingNonNullable().And
                .HaveField("Founder.Name").OfTypeText().BeingNonNullable().And
                .HaveField("Founder.ReignBegin").OfTypeInt16().BeingNonNullable().And
                .HaveField("Founder.ReignEnd").OfTypeInt16().BeingNonNullable().And
                .HaveField("Founder.Death").OfTypeText().BeingNullable().And
                .HaveField(nameof(ChineseDynasty.MaxExtent)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(ChineseDynasty.Established)).OfTypeInt16().BeingNonNullable().And
                .HaveField(nameof(ChineseDynasty.Fell)).OfTypeInt16().BeingNonNullable().And
                .HaveField(nameof(ChineseDynasty.Population)).OfTypeUInt64().BeingNonNullable().And
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
            translation.Principal.Table.Should()
                .HaveField(nameof(BarbecueSauce.ID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(BarbecueSauce.Brand)).OfTypeText().BeingNonNullable().And
                .HaveField("PerServing.Calories").OfTypeUInt32().BeingNullable().And
                .HaveField("PerServing.Fat").OfTypeDouble().BeingNullable().And
                .HaveField("PerServing.Sugar").OfTypeDouble().BeingNullable().And
                .HaveField("PerServing.Carbohydrates").OfTypeDouble().BeingNullable().And
                .HaveField(nameof(BarbecueSauce.KetchupBased)).OfTypeBoolean().BeingNonNullable().And
                .HaveField(nameof(BarbecueSauce.Style)).OfTypeEnumeration(
                    BarbecueSauce.Kind.Sweet, BarbecueSauce.Kind.Spicy,
                    BarbecueSauce.Kind.Tangy, BarbecueSauce.Kind.Chocolatey
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
            translation.Principal.Table.Should()
                .HaveField(nameof(DNDMonster.Species)).OfTypeText().BeingNonNullable().And
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
                .HaveField(nameof(DNDMonster.Size)).OfTypeEnumeration(
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
                .HaveField(nameof(DNDMonster.CR)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(DNDMonster.AC)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(DNDMonster.HP)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(DNDMonster.LegendaryActions)).OfTypeUInt8().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NonNullableReferences() {
            // Arrange
            var source = typeof(Scorpion);
            var translator = new Translator();

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Scorpion.CommonName)).OfTypeText().BeingNonNullable().And
                .HaveField("Genus.Genus").OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Scorpion.Species)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Scorpion.StingIndex)).OfTypeDouble().BeingNonNullable().And
                .HaveField(nameof(Scorpion.AverageLength)).OfTypeSingle().BeingNonNullable().And
                .HaveField(nameof(Scorpion.AverageWeight)).OfTypeSingle().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Genus.Genus")
                    .Against(translator[typeof(Scorpion.TaxonomicGenus)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NullableReferences() {
            // Arrange
            var source = typeof(Ferry);
            var translator = new Translator();

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Ferry.RegistrationNumber)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Ferry.PassengerCapacity)).OfTypeUInt64().BeingNullable().And
                .HaveField(nameof(Ferry.Type)).OfTypeEnumeration(
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
            var source = typeof(WeekendUpdate);
            var translator = new Translator();

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(WeekendUpdate.ID)).OfTypeGuid().BeingNonNullable().And
                .HaveField("Airing.Month").OfTypeEnumeration(
                    WeekendUpdate.Date.MonthOfYear.JAN, WeekendUpdate.Date.MonthOfYear.FEB,
                    WeekendUpdate.Date.MonthOfYear.MAR, WeekendUpdate.Date.MonthOfYear.APR,
                    WeekendUpdate.Date.MonthOfYear.MAY, WeekendUpdate.Date.MonthOfYear.JUN,
                    WeekendUpdate.Date.MonthOfYear.JUL, WeekendUpdate.Date.MonthOfYear.AUG,
                    WeekendUpdate.Date.MonthOfYear.SEP, WeekendUpdate.Date.MonthOfYear.OCT,
                    WeekendUpdate.Date.MonthOfYear.NOV, WeekendUpdate.Date.MonthOfYear.DEC
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
                .HaveField(nameof(WeekendUpdate.NumJokes)).OfTypeInt8().And
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
            var source = typeof(DannyPhantomGhost);
            var translator = new Translator();

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(DannyPhantomGhost.Name)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(DannyPhantomGhost.Powers)).OfTypeEnumeration(
                    DannyPhantomGhost.Ability.Intangibiblity,
                    DannyPhantomGhost.Ability.Blast,
                    DannyPhantomGhost.Ability.Overshadowing,
                    DannyPhantomGhost.Ability.Duplication,
                    DannyPhantomGhost.Ability.Telekinesis
                ).BeingNonNullable().And
                .HaveField(nameof(DannyPhantomGhost.Appearances)).OfTypeInt32().BeingNonNullable().And
                .HaveField("Debut.Overall").OfTypeInt16().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Debut.Overall")
                    .Against(translator[typeof(DannyPhantomGhost.Episode)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }
    }
}
