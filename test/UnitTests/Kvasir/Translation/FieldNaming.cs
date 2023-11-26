using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
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
                .HaveField(nameof(Surah._EnglishName)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Surah.__ArabicName)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Surah.juz_start)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Surah.juzEnd)).OfTypeDecimal().BeingNonNullable().And
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
                .HaveField(nameof(River.Name)).OfTypeText().BeingNonNullable().And
                .HaveField("SourceElevation").OfTypeUInt16().BeingNonNullable().And
                .HaveField("Length").OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(River.MouthLatitude)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(River.MouthLongitude)).OfTypeDecimal().BeingNonNullable().And
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
                .HaveField(nameof(BorderCrossing.Name)).OfTypeText().BeingNonNullable().And
                .HaveField("Degrees.Latitude").OfTypeSingle().BeingNonNullable().And
                .HaveField("Degrees.Longitude").OfTypeSingle().BeingNonNullable().And
                .HaveField(nameof(BorderCrossing.CountryA)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(BorderCrossing.CountryB)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(BorderCrossing.Length)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(BorderCrossing.YearlyCrossings)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(BorderCrossing.IsDriveable)).OfTypeBoolean().BeingNonNullable().And
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
                .HaveField(nameof(Ziggurat.ZigguratID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Ziggurat.Height)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Ziggurat.NumTerraces)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(Ziggurat.NumSteps)).OfTypeUInt32().BeingNonNullable().And
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
                .HaveField(nameof(DogShow.Year)).OfTypeUInt16().BeingNonNullable().And
                .HaveField(nameof(DogShow.Sponsor)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(DogShow.Participants)).OfTypeUInt64().BeingNonNullable().And
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
                .HaveField(nameof(Cliff.CliffID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Cliff.Height)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(Cliff.SheerAngle)).OfTypeDouble().BeingNonNullable().And
                .HaveField(nameof(Cliff.IsUNESCO)).OfTypeBoolean().BeingNonNullable().And
                .HaveField(nameof(Cliff.PrimaryStone)).OfTypeText().BeingNonNullable().And
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
                .HaveField(nameof(Ballerina.SSN)).OfTypeUInt32().BeingNonNullable().And
                .HaveField(nameof(Ballerina.FirstName)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Ballerina.LastName)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Ballerina.Height)).OfTypeDouble().BeingNonNullable().And
                .HaveField(nameof(Ballerina.ShoeSize)).OfTypeUInt8().BeingNonNullable().And
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
                .HaveField(nameof(DMZ.DMZName)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(DMZ.Length)).OfTypeDouble().BeingNonNullable().And
                .HaveField("Value").OfTypeDouble().BeingNonNullable().And
                .HaveField("Definition.Lat_or_Long").OfTypeEnumeration(
                    DMZ.LineType.Latitude, DMZ.LineType.Longitude
                ).BeingNonNullable().And
                .HaveField("Definition.Dir").OfTypeEnumeration(
                    DMZ.Direction.North, DMZ.Direction.South, DMZ.Direction.East, DMZ.Direction.West
                ).BeingNonNullable().And
                .HaveField(nameof(DMZ.OverseenBy)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(DMZ.Established)).OfTypeDateTime().BeingNonNullable().And
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
                .HaveField(nameof(Carnival.CarnivalID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Carnival.CarnivalName)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Carnival.City)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Carnival.IsTravelling)).OfTypeBoolean().BeingNonNullable().And
                .HaveField("CarnivalStaff.HeadCarny.ID").OfTypeInt32().BeingNonNullable().And
                .HaveField("CarnivalStaff.HeadCarny.Title").OfTypeText().BeingNonNullable().And
                .HaveField("CarnivalStaff.Zookeeper.ID").OfTypeInt32().BeingNonNullable().And
                .HaveField("CarnivalStaff.Zookeeper.Title").OfTypeText().BeingNonNullable().And
                .HaveField("CarnivalStaff.SanitationLord.ID").OfTypeInt32().BeingNonNullable().And
                .HaveField("CarnivalStaff.SanitationLord.Title").OfTypeText().BeingNonNullable().And
                .HaveField("CarnivalStaff.Spokesperson.ID").OfTypeInt32().BeingNonNullable().And
                .HaveField("CarnivalStaff.Spokesperson.Title").OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Carnival.PopcornCost)).OfTypeDecimal().BeingNonNullable().And
                .HaveField(nameof(Carnival.NumTents)).OfTypeUInt16().BeingNonNullable().And
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
                .HaveField(nameof(Episode.Length)).OfTypeSingle().BeingNonNullable().And
                .HaveField(nameof(Episode.Part)).OfTypeInt32().BeingNullable().And
                .HaveField(nameof(Episode.Title)).OfTypeText().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void ChangeToNameOfExistingField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ComputerLock);

            // Act
            var translate = () => translator[source];

            // Assert
            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("two or more Fields with name")              // category
                .WithMessageContaining("\"IsReentrant\"");                          // details / explanation
        }

        [TestMethod] public void TwoFieldsNamesChangedToSameName_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Ticket2RideRoute);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("two or more Fields with name")              // category
                .WithMessageContaining("\"Destination\"");                          // details / explanation
        }

        [TestMethod] public void MultipleNameChangesOnScalarProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BankAccount);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BankAccount.RoutingNumber))           // error location
                .WithMessageContaining("duplicated")                                // category
                .WithMessageContaining("[Name]");                                   // details / explanation
        }

        [TestMethod] public void NameChangeOnAggregateOverridesOriginalNameChange() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HashMap);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(HashMap.ID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(HashMap.MemoryAddress)).OfTypeUInt64().BeingNonNullable().And
                .HaveField(nameof(HashMap.ResolveViaChaining)).OfTypeBoolean().BeingNonNullable().And
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
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("Debut")                                     // error location
                .WithMessageContaining("\"Year\"")                                  // error sub-location
                .WithMessageContaining("duplicated")                                // category
                .WithMessageContaining("[Name]");                                   // details / explanation
        }

        [TestMethod] public void FieldNameIsUnchangedByAnnotation_Redundant() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Opera);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(Opera.ID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(Opera.Composer)).OfTypeText().BeingNonNullable().And
                .HaveField(nameof(Opera.PremiereDate)).OfTypeDateTime().BeingNonNullable().And
                .HaveField(nameof(Opera.Length)).OfTypeUInt32().BeingNonNullable().And
                .HaveNoOtherFields();
        }

        [TestMethod] public void NewNameIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Longbow);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Longbow.Weight))                      // error location
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("null");                                     // details / explanation
        }

        [TestMethod] public void NewNameIsEmptyString_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Volcano);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Volcano.IsActive))                    // error location
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("\"\"");                                     // details / explanation
        }

        [TestMethod] public void PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MedalOfHonor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(MedalOfHonor.Recipient))              // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Name]");                                   // details / explanation
        }

        [TestMethod] public void PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Legume);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Legume.Energy))                       // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Madonna);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Madonna.Painter))                     // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CapitolBuilding);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(CapitolBuilding.Architect))           // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonPrimaryKeyPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Rabbi);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Rabbi.CurrentTemple))                 // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("\"Denomination\"");                         // details / explanation
        }

        [TestMethod] public void PathOnReferenceRefersToPartiallyExposedAggregate() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CarAccident);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField(nameof(CarAccident.AccidentReportID)).OfTypeGuid().BeingNonNullable().And
                .HaveField(nameof(CarAccident.Casualties)).OfTypeUInt16().BeingNonNullable().And
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
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(ProcessRegister.Architectures))       // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Yeshiva);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Yeshiva.Students))                    // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Name]")                                    // details / explanation
                .WithMessageContaining("\"City\"");                                 // details / explanation
        }
    }
}
