using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.Globals;
using static UT.Kvasir.Translation.FieldNaming;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Field Naming")]
    public class FieldNamingTests {
        [TestMethod] public void NonPascalCasedNames() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Surah);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("_EnglishName").OfTypeText().BeingNonNullable().And
                .HaveField("__ArabicName").OfTypeText().BeingNonNullable().And
                .HaveField("juz_start").OfTypeDecimal().BeingNonNullable().And
                .HaveField("juzEnd").OfTypeDecimal().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ScalarFieldNameChangedToBrandNewIdentifier() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(River);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("SourceElevation").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Length").OfTypeUInt16().BeingNonNullable().And
                .HaveField("MouthLatitude").OfTypeDecimal().BeingNonNullable().And
                .HaveField("MouthLongitude").OfTypeDecimal().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void AggregateFieldNameChanged() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BorderCrossing);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Degrees.Latitude").OfTypeSingle().BeingNonNullable().And
                .HaveField("Degrees.Longitude").OfTypeSingle().BeingNonNullable().And
                .HaveField("CountryA").OfTypeText().BeingNonNullable().And
                .HaveField("CountryB").OfTypeText().BeingNonNullable().And
                .HaveField("Length").OfTypeUInt64().BeingNonNullable().And
                .HaveField("YearlyCrossings").OfTypeUInt64().BeingNonNullable().And
                .HaveField("IsDriveable").OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void AggregateNestedFieldNameChangedToBrandNewIdentifier() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Ziggurat);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ZigguratID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Height").OfTypeUInt64().BeingNonNullable().And
                .HaveField("NumTerraces").OfTypeUInt16().BeingNonNullable().And
                .HaveField("NumSteps").OfTypeUInt32().BeingNonNullable().And
                .HaveField("CivWho").OfTypeText().BeingNonNullable().And
                .HaveField("CivWhere").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ChangeNameOfNestedAggregate() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(DogShow);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Year").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Sponsor").OfTypeText().BeingNonNullable().And
                .HaveField("Participants").OfTypeUInt64().BeingNonNullable().And
                .HaveField("BestInShow.Name").OfTypeText().BeingNonNullable().And
                .HaveField("BestInShow.Breed.Genus").OfTypeText().BeingNonNullable().And
                .HaveField("BestInShow.Breed.Species").OfTypeText().BeingNonNullable().And
                .HaveField("BestInShow.Breed.Common").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ComplexNameChangesWithAggregates() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cliff);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("CliffID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Height").OfTypeUInt64().BeingNonNullable().And
                .HaveField("SheerAngle").OfTypeDouble().BeingNonNullable().And
                .HaveField("IsUNESCO").OfTypeBoolean().BeingNonNullable().And
                .HaveField("PrimaryStone").OfTypeText().BeingNonNullable().And
                .HaveField("Place.Location.CityName").OfTypeText().BeingNonNullable().And
                .HaveField("Place.Location.GeoCity.SubLocale").OfTypeText().BeingNullable().And
                .HaveField("Place.Location.GeoCity.LATITUDE").OfTypeSingle().BeingNonNullable().And
                .HaveField("Place.Location.GeoCity.GridIntersection.LONG").OfTypeSingle().BeingNonNullable().And
                .HaveField("PolityName").OfTypeText().BeingNullable().And
                .HaveField("Place.PolitySubLocale").OfTypeText().BeingNullable().And
                .HaveField("Place.Location.GeoPolity.LATITUDE").OfTypeSingle().BeingNullable().And
                .HaveField("Place.Location.GeoPolity.GridIntersection.LONG").OfTypeSingle().BeingNullable().And
                .HaveField("Place.Location.Country").OfTypeText().BeingNonNullable().And
                .HaveField("Place.NumEntrances").OfTypeUInt16().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ReferenceFieldNameChanged() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Ballerina);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("SSN").OfTypeUInt32().BeingNonNullable().And
                .HaveField("FirstName").OfTypeText().BeingNonNullable().And
                .HaveField("LastName").OfTypeText().BeingNonNullable().And
                .HaveField("Height").OfTypeDouble().BeingNonNullable().And
                .HaveField("ShoeSize").OfTypeUInt8().BeingNonNullable().And
                .HaveField("DebutBallet.BalletID").OfTypeGuid().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ReferenceNestedFieldNameChangedToBrandNewIdentifier() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(DMZ);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("DMZName").OfTypeText().BeingNonNullable().And
                .HaveField("Length").OfTypeDouble().BeingNonNullable().And
                .HaveField("Value").OfTypeDouble().BeingNonNullable().And
                .HaveField("Definition.Lat_or_Long").OfTypeEnumeration(
                    DMZ.LineType.Latitude,
                    DMZ.LineType.Longitude
                ).BeingNonNullable().And
                .HaveField("Definition.Dir").OfTypeEnumeration(
                    DMZ.Direction.North,
                    DMZ.Direction.South,
                    DMZ.Direction.East,
                    DMZ.Direction.West
                ).BeingNonNullable().And
                .HaveField("OverseenBy").OfTypeText().BeingNonNullable().And
                .HaveField("Established").OfTypeDateTime().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ChangeNameOfNestedReference() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Carnival);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("CarnivalID").OfTypeGuid().BeingNonNullable().And
                .HaveField("CarnivalName").OfTypeText().BeingNonNullable().And
                .HaveField("City").OfTypeText().BeingNonNullable().And
                .HaveField("IsTravelling").OfTypeBoolean().BeingNonNullable().And
                .HaveField("CarnivalStaff.HeadCarny.ID").OfTypeInt32().BeingNonNullable().And
                .HaveField("CarnivalStaff.HeadCarny.Title").OfTypeText().BeingNonNullable().And
                .HaveField("CarnivalStaff.Zookeeper.ID").OfTypeInt32().BeingNonNullable().And
                .HaveField("CarnivalStaff.Zookeeper.Title").OfTypeText().BeingNonNullable().And
                .HaveField("CarnivalStaff.SanitationLord.ID").OfTypeInt32().BeingNonNullable().And
                .HaveField("CarnivalStaff.SanitationLord.Title").OfTypeText().BeingNonNullable().And
                .HaveField("CarnivalStaff.Spokesperson.ID").OfTypeInt32().BeingNonNullable().And
                .HaveField("CarnivalStaff.Spokesperson.Title").OfTypeText().BeingNonNullable().And
                .HaveField("PopcornCost").OfTypeDecimal().BeingNonNullable().And
                .HaveField("NumTents").OfTypeUInt16().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ChangeRelatonFieldName_AffectsRelationTable() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(KidneyStone);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.FieldNaming+KidneyStone.MaterialsTableTable").And
                .HaveField("KidneyStone.KidneyStoneID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Item").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("KidneyStone.KidneyStoneID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void RelationNestedFieldNameChangedToBrandNewIdentifier() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(SwissCanton);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(3);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.FieldNaming+SwissCanton.CouncilorsTable").And
                .HaveField("CantonID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Councilor").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("CantonID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[1].Table.Should()
                .HaveName("UT.Kvasir.Translation.FieldNaming+SwissCanton.NamesTable").And
                .HaveField("Canton.ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Key").OfTypeText().BeingNonNullable().And
                .HaveField("Value").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Canton.ID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[2].Table.Should()
                .HaveName("UT.Kvasir.Translation.FieldNaming+SwissCanton.ReligionsTable").And
                .HaveField("SwissCanton.ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Religion").OfTypeText().BeingNonNullable().And
                .HaveField("%PCNT").OfTypeDouble().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("SwissCanton.ID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void ChangeNameOfNestedRelation_AffectsRelationTable() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Gulag);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.FieldNaming+Gulag.GulagOverseersTable").And
                .HaveField("Gulag.GulagID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Item").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Gulag.GulagID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void FieldsSwapNames() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Episode);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Number").OfTypeUInt8().BeingNonNullable().And
                .HaveField("Season").OfTypeInt16().BeingNonNullable().And
                .HaveField("Length").OfTypeSingle().BeingNonNullable().And
                .HaveField("Part").OfTypeInt32().BeingNullable().And
                .HaveField("Title").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NameConflictWithExplicitInterfaceImplementation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(GroundMeat);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().FailWith<DuplicateNameException>()
                .WithLocation("`GroundMeat`")
                .WithProblem("there are two or more Fields with the name \"CaloriesPerGram\"")
                .EndMessage();
        }

        [TestMethod] public void ChangeToNameOfExistingField_PrincipalTable_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ComputerLock);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().FailWith<DuplicateNameException>()
                .WithLocation("`ComputerLock`")
                .WithProblem("there are two or more Fields with the name \"IsReentrant\"")
                .EndMessage();
        }

        [TestMethod] public void TwoFieldsNamesChangedToSameName_PrincipalTable_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Ticket2RideRoute);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateNameException>()
                .WithLocation("`Ticket2RideRoute`")
                .WithProblem("there are two or more Fields with the name \"Destination\"")
                .EndMessage();
        }

        [TestMethod] public void ChangeToNameOfExistingField_RelationTable_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Cookbook);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().FailWith<DuplicateNameException>()
                .WithLocation("`Cookbook` → <synthetic> `Recipes`")
                .WithProblem("there are two or more Fields with the name \"Cookbook.ISBN\"")
                .EndMessage();
        }

        [TestMethod] public void TwoFieldsNamesChangedToSameName_RelationTable_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HostageSituation);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateNameException>()
                .WithLocation("`HostageSituation` → <synthetic> `Hostages`")
                .WithProblem("there are two or more Fields with the name \"Value\"")
                .EndMessage();
        }

        [TestMethod] public void MultipleIdenticalNameChangesOnScalarProperty() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Antiparticle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("Spin").OfTypeDouble().BeingNonNullable().And
                .HaveField("Charge").OfTypeInt32().BeingNonNullable().And
                .HaveField("Counterpart").OfTypeText().BeingNonNullable().And
                .HaveField("Mass").OfTypeDecimal().BeingNonNullable().And
                .HaveField("DiscoveredBy").OfTypeText().BeingNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void MultipleDifferentNameChangesOnScalarProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BankAccount);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateAnnotationException>()
                .WithLocation("`BankAccount` → RoutingNumber")
                .WithProblem("only one copy of the annotation can be applied to a given Field at a time")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void RedundantAndImpactfulNameChangesOnScalarProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Billionaire);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateAnnotationException>()
                .WithLocation("`Billionaire` → FirstReached")
                .WithProblem("only one copy of the annotation can be applied to a given Field at a time")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void MultipleIdenticalNameChangesOnAggregateProperty() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Militia);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("MilitiaID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Name").OfTypeText().BeingNullable().And
                .HaveField("Created").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Disbanded").OfTypeDateTime().BeingNullable().And
                .HaveField("Members.Generals").OfTypeInt32().BeingNonNullable().And
                .HaveField("Members.Colonels").OfTypeInt32().BeingNonNullable().And
                .HaveField("Members.Lieutenants").OfTypeInt32().BeingNonNullable().And
                .HaveField("Members.Privates").OfTypeInt32().BeingNonNullable().And
                .HaveField("Members.Corporals").OfTypeInt32().BeingNonNullable().And
                .HaveField("WellRegulated").OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void MultipleDifferentNameChangesOnAggregateProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Walkabout);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateAnnotationException>()
                .WithLocation("`Walkabout` → InitialLocation")
                .WithProblem("only one copy of the annotation can be applied to a given Field at a time")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void RedundantAndImpactfulNameChangesOnAggregateProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Treadmill);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateAnnotationException>()
                .WithLocation("`Treadmill` → Manufacturer")
                .WithProblem("only one copy of the annotation can be applied to a given Field at a time")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void MultipleIdenticalNameChangesOnReferenceProperty() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MongolKhan);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name").OfTypeText().BeingNonNullable().And
                .HaveField("ReignStart").OfTypeDateTime().BeingNonNullable().And
                .HaveField("ReignEnd").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Children").OfTypeUInt16().BeingNonNullable().And
                .HaveField("LivingDescendants").OfTypeUInt64().BeingNonNullable().And
                .HaveField("CapitalCity.Name").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void MultipleDifferentNameChangesOnReferenceProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(QuizBowlProtest);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateAnnotationException>()
                .WithLocation("`QuizBowlProtest` → ProtestedQuestion")
                .WithPath("Question")
                .WithProblem("only one copy of the annotation can be applied to a given Field at a time")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void RedundantAndImpactfulNameChangesOnReferenceProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Grassland);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateAnnotationException>()
                .WithLocation("`Grassland` → DominantGrass")
                .WithProblem("only one copy of the annotation can be applied to a given Field at a time")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void MultipleIdenticalNameChangesOnRelationProperty() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Necromancer);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.FieldNaming+Necromancer.SpellbookTable").And
                .HaveField("Necromancer.MagicUserID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Item.Name").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("Necromancer.MagicUserID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveForeignKey("Item.Name")
                    .Against(translator[typeof(Necromancer.Spell)].Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void MultipleDifferentNameChangesOnRelationProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Genocide);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateAnnotationException>()
                .WithLocation("`Genocide` → <synthetic> `Timeline`")
                .WithProblem("only one copy of the annotation can be applied to a given Field at a time")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void RedundantAndImpactfulNameChangesOnRelationProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PrideParade);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateAnnotationException>()
                .WithLocation("`PrideParade` → <synthetic> `Participants`")
                .WithProblem("only one copy of the annotation can be applied to a given Field at a time")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void ChangeNameOfLocalizationField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Shiva);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Decedent").OfTypeText().BeingNonNullable().And
                .HaveField("ShivaAddress").OfTypeText().BeingNonNullable().And
                .HaveField("Date").OfTypeDate().BeingNonNullable().And
                .HaveField("IsBuffet").OfTypeBoolean().BeingNonNullable().And
                .HaveField("Attendees").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Judaism").OfTypeEnumeration(
                    Shiva.Denomination.Reform,
                    Shiva.Denomination.Conservative,
                    Shiva.Denomination.Orthodox,
                    Shiva.Denomination.Haredi,
                    Shiva.Denomination.Secular
                ).BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ChangeNameOfNestedLocalizationField() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(BollywoodMovie);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("IMDbID").OfTypeUInt64().BeingNonNullable().And
                .HaveField("DanceNumbers").OfTypeUInt16().BeingNonNullable().And
                .HaveField("DirectorsGuild").OfTypeText().BeingNonNullable().And
                .HaveField("FilmDirector.Name").OfTypeText().BeingNonNullable().And
                .HaveField("FilmDirector.DirectorNumber").OfTypeUInt64().BeingNonNullable().And
                .HaveField("RuntimeMinutes").OfTypeUInt32().BeingNonNullable().And
                .HaveField("Year").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Budget").OfTypeDecimal().BeingNonNullable().And
                .HaveField("BoxOffice").OfTypeDecimal().BeingNonNullable().And
                .HaveField("StarsShahRukhKhan").OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NameChangeOnAggregateNestedFieldOverridesOriginalNameChange() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(HashMap);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("MemoryAddress").OfTypeUInt64().BeingNonNullable().And
                .HaveField("ResolveViaChaining").OfTypeBoolean().BeingNonNullable().And
                .HaveField("KeyType.Typename").OfTypeText().BeingNonNullable().And
                .HaveField("KeyType.IsPointer").OfTypeBoolean().BeingNonNullable().And
                .HaveField("ConstQualified").OfTypeBoolean().BeingNonNullable().And
                .HaveField("KeyType.IsReference").OfTypeBoolean().BeingNonNullable().And
                .HaveField("ValueType.Typename").OfTypeText().BeingNonNullable().And
                .HaveField("ValueType.IsPointer").OfTypeBoolean().BeingNonNullable().And
                .HaveField("ValueType.IsConst").OfTypeBoolean().BeingNonNullable().And
                .HaveField("Ref").OfTypeBoolean().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NameChangeOnRelationNestedFieldOverridesOriginalNameChange() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ArchaeologicalSite);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Should().HaveCount(1);
            translation.Relations[0].Table.Should()
                .HaveField("ArchaeologicalSite.SiteID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Item.Name").OfTypeText().BeingNonNullable().And
                .HaveField("Item.Description").OfTypeText().BeingNonNullable().And
                .HaveField("Item.Latitude").OfTypeSingle().BeingNonNullable().And
                .HaveField("Item.Longitude").OfTypeSingle().BeingNonNullable().And
                .HaveField("TotalArea").OfTypeDouble().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("ArchaeologicalSite.SiteID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NameChangeOnNestedAggregateOverridesOriginalNameChange() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Sarcophagus);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Entombed").OfTypeText().BeingNonNullable().And
                .HaveField("Details.Measure.Height").OfTypeSingle().BeingNonNullable().And
                .HaveField("Details.Measure.Width").OfTypeSingle().BeingNonNullable().And
                .HaveField("Details.Measure.Length").OfTypeSingle().BeingNonNullable().And
                .HaveField("Details.Discovered").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Details.Weight").OfTypeSingle().BeingNonNullable().And
                .HaveField("StoneType").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NameChangeOnNestedReferenceOverridesOriginalNameChange() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MariachiBand);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Repertoire.#1.Name").OfTypeText().BeingNonNullable().And
                .HaveField("Repertoire.#2.Name").OfTypeText().BeingNonNullable().And
                .HaveField("Repertoire.#3.Name").OfTypeText().BeingNonNullable().And
                .HaveField("Members").OfTypeUInt16().BeingNonNullable().And
                .HaveField("UsesVihuelas").OfTypeBoolean().BeingNonNullable().And
                .HaveField("UsesGuitars").OfTypeBoolean().BeingNonNullable().And
                .HaveField("HomeCity").OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NameChangeOnNestedRelationOverridesOriginalNameChange() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(PolarVortex);

            // Act
            var translation = translator[source];

            // Assert
            translation.Relations.Count.Should().Be(2);
            translation.Relations[0].Table.Should()
                .HaveName("UT.Kvasir.Translation.FieldNaming+PolarVortex.HIGHSTable").And
                .HaveField("PolarVortex.VortexID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Key").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Value").OfTypeDouble().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("PolarVortex.VortexID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
            translation.Relations[1].Table.Should()
                .HaveName("UT.Kvasir.Translation.FieldNaming+PolarVortex.LOWSTable").And
                .HaveField("PolarVortex.VortexID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Key").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Value").OfTypeDouble().BeingNonNullable().And
                .HaveNoOtherFields().And
                .HaveForeignKey("PolarVortex.VortexID")
                    .Against(translation.Principal.Table)
                    .WithOnDeleteBehavior(OnDelete.Cascade)
                    .WithOnUpdateBehavior(OnUpdate.Cascade).And
                .HaveNoOtherForeignKeys();
        }

        [TestMethod] public void NameChangeOnNestedLocalizationOverridesOriginalNameChange() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MovingCompany);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Company.License").OfTypeGuid().BeingNonNullable().And
                .HaveField("Trademark").OfTypeText().BeingNonNullable().And
                .HaveField("Founding").OfTypeGuid().BeingNonNullable().And
                .HaveField("Employees").OfTypeUInt64().BeingNonNullable().And
                .HaveField("FleetSize").OfTypeUInt16().BeingNonNullable().And
                .HaveField("YearlyRevenue").OfTypeDecimal().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void MultipleNameChangesOnNestedProperty_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Helicopter);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateAnnotationException>()
                .WithLocation("`Helicopter` → Debut")
                .WithPath("Year")
                .WithProblem("only one copy of the annotation can be applied to a given Field at a time")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void FieldNameIsUnchangedByAnnotation_Redundant() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Opera);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Composer").OfTypeText().BeingNonNullable().And
                .HaveField("PremiereDate").OfTypeDateTime().BeingNonNullable().And
                .HaveField("Length").OfTypeUInt32().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NewNameIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Longbow);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`Longbow` → Weight")
                .WithProblem("the name of a Field cannot be 'null'")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void NewNameIsEmptyString_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Volcano);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidNameException>()
                .WithLocation("`Volcano` → IsActive")
                .WithProblem("the name of a Field cannot be empty")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void AppliedToPreDefinedInstance_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Murder);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Murder` → Regicide")
                .WithProblem("the annotation cannot be applied to a pre-defined instance property")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void PathIsNull_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(MedalOfHonor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`MedalOfHonor` → Recipient")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Legume);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Legume` → Energy")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Madonna);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Madonna` → Painter")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CapitolBuilding);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`CapitolBuilding` → Architect")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Rabbi);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Rabbi` → CurrentTemple")
                .WithProblem("the path \"Denomination\" does not exist")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void PathOnReferenceRefersToPartiallyExposedAggregate() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(CarAccident);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("AccidentReportID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Casualties").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Instigator.Reg.ID").OfTypeGuid().BeingNonNullable().And
                .HaveField("Other.Registration.ID").OfTypeGuid().BeingNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(ProcessRegister);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`ProcessRegister` → <synthetic> `Architectures`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Yeshiva);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Yeshiva` → <synthetic> `Students`")
                .WithProblem("the path \"Yeshiva.City\" does not exist")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void NonExistentPathOnLocalization_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Slushy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Slushy` → Flavor")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Name]")
                .EndMessage();
        }

        [TestMethod] public void NestedPathOnLocalization_IsError() {
            // Arrange
            var translator = new Translator(NO_ENTITIES);
            var source = typeof(Spoonerism);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Spoonerism` → SpoonerizedText")
                .WithProblem("the path \"Locale\" does not exist")
                .WithAnnotations("[Name]")
                .EndMessage();
        }
    }
}
