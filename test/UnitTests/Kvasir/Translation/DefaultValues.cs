using FluentAssertions;
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
                .HaveField("ABO").WithDefault("O").And
                .HaveField("RHPositive").WithDefault(true).And
                .HaveField("ApproxPrevalence").WithDefault(0.5f).And
                .HaveField("NumSubgroups").WithDefault(1).And
                .HaveField("AnnualDonationsL").WithNoDefault().And
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
                .HaveField("ISBN").WithNoDefault().And
                .HaveField("Title").WithNoDefault().And
                .HaveField("Author").WithNoDefault().And
                .HaveField("MarketValue").WithDefault((decimal)35.78).And
                .HaveField("NumPages").WithNoDefault().And
                .HaveField("Published").WithNoDefault().And
                .HaveField("NumBeasts").WithNoDefault().And
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
                .HaveField("UniqueUmpireNumber").WithNoDefault().And
                .HaveField("UniformNumber").WithNoDefault().And
                .HaveField("Name").WithNoDefault().And
                .HaveField("Debut").WithDefault(new DateTime(1970, 1, 1)).And
                .HaveField("Ejections").WithNoDefault().And
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
                .HaveField("SainthoodIdentifier").WithDefault(new Guid("81a130d2-502f-4cf1-a376-63edeb000e9f")).And
                .HaveField("Name").WithNoDefault().And
                .HaveField("CanonizationDate").WithNoDefault().And
                .HaveField("FeastMonth").WithNoDefault().And
                .HaveField("FeastDay").WithNoDefault().And
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
                .HaveField("Name").WithNoDefault().And
                .HaveField("Greek").WithNoDefault().And
                .HaveField("MentionedIn").WithDefault(Oceanid.Source.Hesiod | Oceanid.Source.Hyginus).And
                .HaveField("NumChildren").WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NullDefaultsOnNullableScalars() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pepper);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Genus").WithNoDefault().And
                .HaveField("Species").WithNoDefault().And
                .HaveField("CommonName").WithDefault(null).And
                .HaveField("FirstCultivated").WithDefault(null).And
                .HaveField("ScovilleRating").WithNoDefault().And
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
                .HaveField("Name").WithNoDefault().And
                .HaveField("AllegedSightings").WithNoDefault().And
                .HaveField("HomeContinent").WithDefault(null).And
                .HaveField("FeatureSet").WithDefault(null).And
                .HaveField("ProvenHoax").WithNoDefault().And
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
                .HaveField("Clergy").WithNoDefault().And
                .HaveField("DeliveredAt").WithNoDefault().And
                .HaveField("Title").WithNoDefault().And
                .HaveField("Text").WithNoDefault().And
                .HaveField("HouseOfWorship.Name").WithNoDefault().And
                .HaveField("HouseOfWorship.Address").WithNoDefault().And
                .HaveField("HouseOfWorship.CongregationSize").WithDefault(1756102UL).And
                .HaveField("ForHoliday").WithNoDefault().And
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
                .HaveField("SalsaName").WithNoDefault().And
                .HaveField("PrimaryPepper.Name").WithNoDefault().And
                .HaveField("PrimaryPepper.ScovilleRating").WithDefault(10000U).And
                .HaveField("Verde").WithNoDefault().And
                .HaveField("ClovesGarlic").WithNoDefault().And
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
                .HaveField("BikeID").WithNoDefault().And
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
                .HaveField("Gears").WithNoDefault().And
                .HaveField("TopSpeed").WithNoDefault().And
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
                .HaveField("ArchID").WithNoDefault().And
                .HaveField("Material").WithNoDefault().And
                .HaveField("Height").WithNoDefault().And
                .HaveField("Diameter").WithNoDefault().And
                .HaveField("Location.Latitude").WithNoDefault().And
                .HaveField("Location.Longitude").WithNoDefault().And
                .HaveField("KeystoneID").WithNoDefault().And
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
                .HaveField("KiteID").WithNoDefault().And
                .HaveField("KiteString.BallSource").WithNoDefault().And
                .HaveField("KiteString.CutNumber").WithDefault((ushort)31).And
                .HaveField("MajorAxis").WithNoDefault().And
                .HaveField("MinorAxis").WithNoDefault().And
                .HaveField("Material").WithNoDefault().And
                .HaveField("TopSpeed").WithNoDefault().And
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
                .HaveField("RoomID").WithNoDefault().And
                .HaveField("TimeLimit").WithNoDefault().And
                .HaveField("BestTime").WithNoDefault().And
                .HaveField("FirstPuzzle.Description").WithNoDefault().And
                .HaveField("FirstPuzzle.PuzzleType").WithDefault(EscapeRoom.Style.Linguistic).And
                .HaveField("FinalPuzzle.Description").WithNoDefault().And
                .HaveField("FinalPuzzle.PuzzleType").WithDefault(EscapeRoom.Style.Logical).And
                .HaveNoOtherFields();
        }

        [TestMethod] public void DefaultOnRelationNestedFieldPropagated() {
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
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`RadioStation` → CallSign")
                .WithProblem("the default value is 'null', but the Field is non-nullable")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void InconvertibleNonNullDefaultValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Battleship);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`Battleship` → Length")
                .WithProblem("value \"100 feet\" is of type `string`, not `ushort` as expected")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void ConvertibleNonNullDefaultValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(County);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`County` → Population")
                .WithProblem("value 5000000 is of type `int`, not `ulong` as expected")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void EnumerationDefaultOnNumericChangedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MasterClass);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`MasterClass` → Category")
                .WithProblem("value Domain.Politics is of type `Domain`, not `ushort` as expected")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void EnumerationDefaultOnStringChangedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Orphanage);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`Orphanage` → Type")
                .WithProblem("value Kind.Private is of type `Kind`, not `string` as expected")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void DefaultOnNestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StuffedAnimal);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`StuffedAnimal` → Description")
                .WithPath("Face")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Face`")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void DefaultOnNestedReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PoetLaureate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`PoetLaureate` → Of")
                .WithPath("Entity")
                .WithProblem("the annotation cannot be applied to a property of Reference type `State`")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void DefaultOnNestedRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TimeTraveler);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`TimeTraveler` → TimeMachine")
                .WithPath("Owners")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationList<string>`")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void ArrayDefaultValue_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BilliardBall);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`BilliardBall` → Number")
                .WithProblem("value cannot be an array")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void DecimalDefaultIsNotDouble_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Geocache);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`Geocache` → NetTrinketValue")
                .WithProblem("value 45109.336 is of type `float`, not `double` as expected")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void DecimalDefaultIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Screwdriver);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`Screwdriver` → HeadWidth")
                .WithProblem($"`double` {double.MaxValue} is outside the supported range for `decimal`")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void DateTimeDefaultIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RomanEmperor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`RomanEmperor` → ReignEnd")
                .WithProblem("value true is of type `bool`, not `string` as expected")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void DateTimeDefaultIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tournament);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`Tournament` → Kickoff")
                .WithProblem("unable to parse `string` value \"20030714\" as a `DateTime`")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void DateTimeDefaultIsOutOfRange_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Sculpture);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`Sculpture` → CreationDate")
                .WithProblem("unable to parse `string` value \"1344-18-18\" as a `DateTime`")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void GuidDefaultIsNotString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HogwartsHouse);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`HogwartsHouse` → TermIndex")
                .WithProblem("value '^' is of type `char`, not `string` as expected")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void GuidDefaultIsMalformatted_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Gene);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`Gene` → UUID")
                .WithProblem("unable to parse `string` value \"ee98f44827b248a2bb9fc5ef342e7ab2!!!\" as a `Guid`")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void EnumerationDefaultIsInvalidEnumerator_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MoonOfJupiter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`MoonOfJupiter` → MoonGroup")
                .WithProblem("enumerator Group.87123 is not valid")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void EnumerationDefaultIsInvalidCombination_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Newspaper);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`Newspaper` → Contents")
                .WithProblem("enumerator Section.15 is not valid")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void DefaultMatchesDataConversionSourceType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CrosswordClue);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidDefaultException>()
                .WithLocation("`CrosswordClue` → AcrossOrDown")
                .WithProblem("value 'A' is of type `char`, not `int` as expected")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void DefaultMatchesDataConversionTargetType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Coupon);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Barcode").WithNoDefault().And
                .HaveField("Code").WithNoDefault().And
                .HaveField("IsBOGO").WithDefault(0).And
                .HaveField("DiscountPercentage").WithNoDefault().And
                .HaveField("MinimumPurchase").WithNoDefault().And
                .HaveField("ExpirationDate").WithNoDefault().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void MultipleDefaultsOnScalarProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SkeeBall);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateAnnotationException>()
                .WithLocation("`SkeeBall` → L1Value")
                .WithProblem("only one copy of the annotation can be applied to a given Field at a time")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Waterfall);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Waterfall` → WorldRanking")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(NativeAmericanTribe);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`NativeAmericanTribe` → Exonym")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TourDeFrance);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`TourDeFrance` → Victor")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(InfinityStone);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`InfinityStone` → `Descriptor` (from \"Description\") → Color")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Color`")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hepatitis);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Hepatitis` → Treatment")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Calculator);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Calculator` → MakeModel")
                .WithProblem("the path \"IsInCirculation\" does not exist")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PopTart);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`PopTart` → FrostingColor")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Color`")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ArcadeGame);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`ArcadeGame` → <synthetic> `HighScores`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Monad);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Monad` → <synthetic> `Traits`")
                .WithProblem("the path \"Monad.ModelsOption\" does not exist")
                .WithAnnotations("[Default]")
                .EndMessage();
        }

        [TestMethod] public void NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LaundryDetergent);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`LaundryDetergent` → <synthetic> `Ingredients`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<string>`")
                .WithAnnotations("[Default]")
                .EndMessage();
        }
    }
}
