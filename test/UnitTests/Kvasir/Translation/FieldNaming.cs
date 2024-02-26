using FluentAssertions;
using Kvasir.Schema;
using Kvasir.Translation2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static UT.Kvasir.Translation.FieldNaming;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Field Naming")]
    public class FieldNamingTests {
        [TestMethod] public void NonPascalCasedNames() {
            // Arrange
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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

        [TestMethod] public void ChangeToNameOfExistingField_IsError() {
            // Arrange
            var translator = new Translator();
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

        [TestMethod] public void TwoFieldsNamesChangedToSameName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ticket2RideRoute);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<DuplicateNameException>()
                .WithLocation("`Ticket2RideRoute`")
                .WithProblem("there are two or more Fields with the name \"Destination\"")
                .EndMessage();
        }

        [TestMethod] public void MultipleIdenticalNameChangesOnScalarProperty() {
            // Arrange
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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

        [TestMethod] public void NameChangeOnAggregateOverridesOriginalNameChange() {
            // Arrange
            var translator = new Translator();
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

        [TestMethod] public void NameChangeOnRelationOverridesOriginalNameChange() {
            // Arrange
            var translator = new Translator();
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

        [TestMethod] public void MultipleNameChangesOnNestedProperty_IsError() {
            // Arrange
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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

        [TestMethod] public void PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
            var translator = new Translator();
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
    }
}
