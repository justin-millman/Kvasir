using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.DefaultValues;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Default Values")]
    public class DefaultValueTests {
        [TestMethod] public void NonNullBasicScalarDefaults() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BloodType);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(BloodType.ABO)).WithDefault("O").And
                .HaveField(nameof(BloodType.RHPositive)).WithDefault(true).And
                .HaveField(nameof(BloodType.ApproxPrevalence)).WithDefault(0.5f).And
                .HaveField(nameof(BloodType.NumSubgroups)).WithDefault(1).And
                .HaveField(nameof(BloodType.AnnualDonationsL)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonNullDecimalDefault() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bestiary);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Bestiary.ISBN)).WithNoDefault().And
                .HaveField(nameof(Bestiary.Title)).WithNoDefault().And
                .HaveField(nameof(Bestiary.Author)).WithNoDefault().And
                .HaveField(nameof(Bestiary.MarketValue)).WithDefault((decimal)35.78).And
                .HaveField(nameof(Bestiary.NumPages)).WithNoDefault().And
                .HaveField(nameof(Bestiary.Published)).WithNoDefault().And
                .HaveField(nameof(Bestiary.NumBeasts)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonNullDateTimeDefault() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Umpire);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Umpire.UniqueUmpireNumber)).WithNoDefault().And
                .HaveField(nameof(Umpire.UniformNumber)).WithNoDefault().And
                .HaveField(nameof(Umpire.Name)).WithNoDefault().And
                .HaveField(nameof(Umpire.Debut)).WithDefault(new DateTime(1970, 1, 1)).And
                .HaveField(nameof(Umpire.Ejections)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonNullGuidDefault() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Saint);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Saint.SainthoodIdentifier)).WithDefault(new Guid("81a130d2-502f-4cf1-a376-63edeb000e9f")).And
                .HaveField(nameof(Saint.Name)).WithNoDefault().And
                .HaveField(nameof(Saint.CanonizationDate)).WithNoDefault().And
                .HaveField(nameof(Saint.FeastMonth)).WithNoDefault().And
                .HaveField(nameof(Saint.FeastDay)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonNullValidEnumerationDefault() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Oceanid);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Oceanid.Name)).WithNoDefault().And
                .HaveField(nameof(Oceanid.Greek)).WithNoDefault().And
                .HaveField(nameof(Oceanid.MentionedIn)).WithDefault(Oceanid.Source.Hesiod | Oceanid.Source.Hyginus).And
                .HaveField(nameof(Oceanid.NumChildren)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonNullInvalidEnumerationDefault_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HallOfFame);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(HallOfFame.Categorization))           // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("Category.185")                              // details / explanation
                .WithMessageContaining("enumerator is invalid");                    // details / explanation
        }

        [TestMethod] public void NullDefaultsOnNullableScalars() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pepper);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Pepper.Genus)).WithNoDefault().And
                .HaveField(nameof(Pepper.Species)).WithNoDefault().And
                .HaveField(nameof(Pepper.CommonName)).WithDefault(null).And
                .HaveField(nameof(Pepper.FirstCultivated)).WithDefault(null).And
                .HaveField(nameof(Pepper.ScovilleRating)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NullDefaultOnNullableEnumeration() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Cryptid);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Cryptid.Name)).WithNoDefault().And
                .HaveField(nameof(Cryptid.AllegedSightings)).WithNoDefault().And
                .HaveField(nameof(Cryptid.HomeContinent)).WithDefault(null).And
                .HaveField(nameof(Cryptid.FeatureSet)).WithDefault(null).And
                .HaveField(nameof(Cryptid.ProvenHoax)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void DefaultOnAggregateNestedFieldPropagated() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sermon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Sermon.Clergy)).WithNoDefault().And
                .HaveField(nameof(Sermon.DeliveredAt)).WithNoDefault().And
                .HaveField(nameof(Sermon.Title)).WithNoDefault().And
                .HaveField(nameof(Sermon.Text)).WithNoDefault().And
                .HaveField("HouseOfWorship.Name").WithNoDefault().And
                .HaveField("HouseOfWorship.Address").WithNoDefault().And
                .HaveField("HouseOfWorship.CongregationSize").WithDefault(1756102UL).And
                .HaveField(nameof(Sermon.ForHoliday)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void FirstDefaultOnAggregateNestedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Salsa);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Salsa.SalsaName)).WithNoDefault().And
                .HaveField("PrimaryPepper.Name").WithNoDefault().And
                .HaveField("PrimaryPepper.ScovilleRating").WithDefault(10000U).And
                .HaveField(nameof(Salsa.Verde)).WithNoDefault().And
                .HaveField(nameof(Salsa.ClovesGarlic)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void SubsequentDefaultOnAggregateNestedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Bicycle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Bicycle.BikeID)).WithNoDefault().And
                .HaveField("FrontWheel.Diameter").WithNoDefault().And
                .HaveField("FrontWheel.NumSpokes").WithNoDefault().And
                .HaveField("FrontWheel.Material.Metal1").WithNoDefault().And
                .HaveField("FrontWheel.Material.Metal2").WithDefault("Titanium").And
                .HaveField("BackWheel.Diameter").WithNoDefault().And
                .HaveField("BackWheel.NumSpokes").WithNoDefault().And
                .HaveField("BackWheel.Material.Metal1").WithNoDefault().And
                .HaveField("BackWheel.Material.Metal2").WithDefault("Copper").And
                .HaveField("SpareWheel.Diameter").WithNoDefault().And
                .HaveField("SpareWheel.NumSpokes").WithNoDefault().And
                .HaveField("SpareWheel.Material.Metal1").WithNoDefault().And
                .HaveField("SpareWheel.Material.Metal2").WithDefault(null).And
                .HaveField(nameof(Bicycle.Gears)).WithNoDefault().And
                .HaveField(nameof(Bicycle.TopSpeed)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void DefaultOnReferenceNestedFieldNotPropagated() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Arch);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Arch.ArchID)).WithNoDefault().And
                .HaveField(nameof(Arch.Material)).WithNoDefault().And
                .HaveField(nameof(Arch.Height)).WithNoDefault().And
                .HaveField(nameof(Arch.Diameter)).WithNoDefault().And
                .HaveField("Location.Latitude").WithNoDefault().And
                .HaveField("Location.Longitude").WithNoDefault().And
                .HaveField(nameof(Arch.KeystoneID)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void FirstDefaultOnReferenceNestedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Kite);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Kite.KiteID)).WithNoDefault().And
                .HaveField("KiteString.BallSource").WithNoDefault().And
                .HaveField("KiteString.CutNumber").WithDefault((ushort)31).And
                .HaveField(nameof(Kite.MajorAxis)).WithNoDefault().And
                .HaveField(nameof(Kite.MinorAxis)).WithNoDefault().And
                .HaveField(nameof(Kite.Material)).WithNoDefault().And
                .HaveField(nameof(Kite.TopSpeed)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void SubsequentDefaultOnReferenceNestedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(EscapeRoom);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(EscapeRoom.RoomID)).WithNoDefault().And
                .HaveField(nameof(EscapeRoom.TimeLimit)).WithNoDefault().And
                .HaveField(nameof(EscapeRoom.BestTime)).WithNoDefault().And
                .HaveField("FirstPuzzle.Description").WithNoDefault().And
                .HaveField("FirstPuzzle.PuzzleType").WithDefault(EscapeRoom.Style.Linguistic).And
                .HaveField("FinalPuzzle.Description").WithNoDefault().And
                .HaveField("FinalPuzzle.PuzzleType").WithDefault(EscapeRoom.Style.Logical).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void DefaultOnRelationNestedFieldMaybePropagated() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DockerContainer);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveField("DockerContainer.Image").WithNoDefault().And
                .HaveField("DockerContainer.PID").WithNoDefault().And
                .HaveField("Key.Path").WithNoDefault().And
                .HaveField("Key.Permissions").WithDefault((ushort)777).And
                .HaveField("Key.IsSymlink").WithNoDefault().And
                .HaveField("Key.Created").WithNoDefault().And
                .HaveField("Key.Modified").WithNoDefault().And
                .HaveField("Value").WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void FirstDefaultOnRelationNestedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Kami);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveField("Kami.Name").WithNoDefault().And
                .HaveField("Item").WithDefault("n/a").And
                .HaveNoOtherFields();
            translation.Relations[1].Table.Should()
                .HaveField("Kami.Name").WithDefault("Susano'o").And
                .HaveField("Key").WithNoDefault().And
                .HaveField("Value").WithDefault((short)19).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void SubsequentDefaultOnRelationNestedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tamagotchi);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations[0].Table.Should()
                .HaveField("Tamagotchi.ID").WithNoDefault().And
                .HaveField("Item.ID").WithNoDefault().And
                .HaveField("Item.Name").WithNoDefault().And
                .HaveField("Item.Cost").WithDefault((decimal)3.75).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NullDefaultOnNonNullableScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RadioStation);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(RadioStation.CallSign))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void InconvertibleNonNullDefaultValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Battleship);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Battleship.Length))                   // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining($"\"100 feet\" of type {nameof(String)}")    // details / explanation
                .WithMessageContaining(nameof(UInt16));                             // details / explanation
        }

        [TestMethod] public void ConvertibleNonNullDefaultValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(County);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(County.Population))                   // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining($"5000000 of type {nameof(Int32)}")          // details / explanation
                .WithMessageContaining(nameof(UInt64));                             // details / explanation
        }

        [TestMethod] public void EnumerationDefaultOnNumericChangedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MasterClass);
            var enumTypename = nameof(MasterClass.Domain);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MasterClass.Category))                // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining($"Domain.Politics of type {enumTypename}")   // details / explanation
                .WithMessageContaining(nameof(UInt16));                             // details / explanation
        }

        [TestMethod] public void EnumerationDefaultOnStringChangedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Orphanage);
            var enumTypename = nameof(Orphanage.Kind);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Orphanage.Type))                      // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining($"Kind.Private of type {enumTypename}")      // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void DefaultOnNetedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StuffedAnimal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(StuffedAnimal.Description))           // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"Stuffing\"");                             // details / explanation
        }

        [TestMethod] public void DefaultOnNestedReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PoetLaureate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(PoetLaureate.Of))                     // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"Entity\"");                               // details / explanation
        }

        [TestMethod] public void DefaultOnNestedRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TimeTraveler);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(TimeTraveler.TimeMachine))            // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"Owners\"");                               // details / explanation
        }

        [TestMethod] public void ArrayDefaultValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BilliardBall);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BilliardBall.Number))                 // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("array")                                     // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void DecimalDefaultIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Geocache);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Geocache.NetTrinketValue))            // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining($"45109.336 of type {nameof(Single)}")       // details / explanation
                .WithMessageContaining(nameof(Decimal))                             // details / explanation
                .WithMessageContaining(nameof(Double));                             // details / explanation
        }

        [TestMethod] public void DecimalDefaultIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Screwdriver);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Screwdriver.HeadWidth))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining(double.MaxValue.ToString())                  // details / explanation
                .WithMessageContaining("could not convert")                         // details / explanation
                .WithMessageContaining(nameof(Decimal));                            // details / explanation
        }

        [TestMethod] public void DateTimeDefaultIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RomanEmperor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(RomanEmperor.ReignEnd))               // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining($"true of type {nameof(Boolean)}")           // details / explanation
                .WithMessageContaining(nameof(DateTime))                            // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void DateTimeDefaultIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tournament);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Tournament.Kickoff))                  // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"20030714\"")                              // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void DateTimeDefaultIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sculpture);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Sculpture.CreationDate))              // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"1344-18-18\"")                            // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(DateTime));                           // details / explanation
        }

        [TestMethod] public void GuidDefaultIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HogwartsHouse);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(HogwartsHouse.TermIndex))             // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining($"'^' of type {nameof(Char)}")               // details / explanation
                .WithMessageContaining(nameof(Guid))                                // details / explanation
                .WithMessageContaining(nameof(String));                             // details / explanation
        }

        [TestMethod] public void GuidDefaultIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Gene);

            // Act
            var translate = () => translator[source];
            var badGuid = "ee98f44827b248a2bb9fc5ef342e7ab2!!!";

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Gene.UUID))                           // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining(badGuid)                                     // details / explanation
                .WithMessageContaining("could not parse")                           // details / explanation
                .WithMessageContaining(nameof(Guid));                               // details / explanation
        }

        [TestMethod] public void EnumerationDefaultIsInvalidEnumerator_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MoonOfJupiter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MoonOfJupiter.MoonGroup))             // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("Group.87123")                               // details / explanation
                .WithMessageContaining("enumerator is invalid");                    // details / explanation
        }

        [TestMethod] public void EnumerationDefaultIsInvalidCombination_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Newspaper);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Newspaper.Contents))                  // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("Section.15")                                // details / explanation
                .WithMessageContaining("enumerator is invalid");                    // details / explanation
        }

        [TestMethod] public void DefaultMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CrosswordClue);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CrosswordClue.AcrossOrDown))          // error location
                .WithMessageContaining("user-provided value*is invalid")            // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining($"'A' of type {nameof(Char)}")               // details / explanation
                .WithMessageContaining(nameof(Int32));                              // details / explanation
        }

        [TestMethod] public void DefaultMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Coupon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Coupon.Barcode)).WithNoDefault().And
                .HaveField(nameof(Coupon.Code)).WithNoDefault().And
                .HaveField(nameof(Coupon.IsBOGO)).WithDefault(0).And
                .HaveField(nameof(Coupon.DiscountPercentage)).WithNoDefault().And
                .HaveField(nameof(Coupon.MinimumPurchase)).WithNoDefault().And
                .HaveField(nameof(Coupon.ExpirationDate)).WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void MultipleDefaultsOnScalarProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SkeeBall);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(SkeeBall.L1Value))                    // error location
                .WithMessageContaining("annotation is duplicated")                  // category
                .WithMessageContaining("[Default]");                                // details / explanation
        }

        [TestMethod] public void PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Waterfall);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Waterfall.WorldRanking))              // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Default]");                                // details / explanation
        }

        [TestMethod] public void PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NativeAmericanTribe);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(NativeAmericanTribe.Exonym))          // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TourDeFrance);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(TourDeFrance.Victor))                 // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(InfinityStone);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(nameof(InfinityStone.Descriptor))            // source type
                .WithMessageContaining(nameof(InfinityStone.Descriptor.Color))      // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Default]");                                // details / explanation
        }

        [TestMethod] public void NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hepatitis);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Hepatitis.Treatment))                 // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Calculator);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Calculator.MakeModel))                // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"IsInCirculation\"");                      // details / explanation
        }

        [TestMethod] public void NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PopTart);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(PopTart.FrostingColor))               // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Default]");                                // details / explanation
        }

        [TestMethod] public void NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ArcadeGame);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ArcadeGame.HighScores))               // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Monad);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Monad.Traits))                        // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Default]")                                 // details / explanation
                .WithMessageContaining("\"ModelsOption\"");                         // details / explanation
        }

        [TestMethod] public void NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LaundryDetergent);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(LaundryDetergent.Ingredients))        // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Default]");                                // details / explanation
        }
    }
}
