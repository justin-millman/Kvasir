using FluentAssertions;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static UT.Kvasir.Translation.TestComponents;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Identification")]
    public class IdentificationTests {
        [TestMethod] public void NonNullableScalars() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Smorgasbord);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Should()
                .HaveName("UT.Kvasir.Translation.TestComponents+SmorgasbordTable").And
                .HaveField("Byte", DBType.UInt8, IsNullable.No).And
                .HaveField("Char", DBType.Character, IsNullable.No).And
                .HaveField("DateTime", DBType.DateTime, IsNullable.No).And
                .HaveField("Decimal", DBType.Decimal, IsNullable.No).And
                .HaveField("Double", DBType.Double, IsNullable.No).And
                .HaveField("Float", DBType.Single, IsNullable.No).And
                .HaveField("Guid", DBType.Guid, IsNullable.No).And
                .HaveField("Int", DBType.Int32, IsNullable.No).And
                .HaveField("Long", DBType.Int64, IsNullable.No).And
                .HaveField("SByte", DBType.Int8, IsNullable.No).And
                .HaveField("Short", DBType.Int16, IsNullable.No).And
                .HaveField("String", DBType.Text, IsNullable.No).And
                .HaveField("UInt", DBType.UInt32, IsNullable.No).And
                .HaveField("ULong", DBType.UInt64, IsNullable.No).And
                .HaveField("UShort", DBType.UInt16, IsNullable.No);
        }

        [TestMethod] public void NullableScalars() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Plethora);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Should()
                .HaveName("UT.Kvasir.Translation.TestComponents+PlethoraTable").And
                .HaveField("Byte", DBType.UInt8, IsNullable.Yes).And
                .HaveField("Char", DBType.Character, IsNullable.Yes).And
                .HaveField("DateTime", DBType.DateTime, IsNullable.Yes).And
                .HaveField("Decimal", DBType.Decimal, IsNullable.Yes).And
                .HaveField("Double", DBType.Double, IsNullable.Yes).And
                .HaveField("Float", DBType.Single, IsNullable.Yes).And
                .HaveField("Guid", DBType.Guid, IsNullable.Yes).And
                .HaveField("Int", DBType.Int32, IsNullable.Yes).And
                .HaveField("Long", DBType.Int64, IsNullable.Yes).And
                .HaveField("SByte", DBType.Int8, IsNullable.Yes).And
                .HaveField("Short", DBType.Int16, IsNullable.Yes).And
                .HaveField("String", DBType.Text, IsNullable.Yes).And
                .HaveField("UInt", DBType.UInt32, IsNullable.Yes).And
                .HaveField("ULong", DBType.UInt64, IsNullable.Yes).And
                .HaveField("UShort", DBType.UInt16, IsNullable.Yes);
        }

        [TestMethod] public void RecordClassEntityType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Color);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Should()
                .HaveName("UT.Kvasir.Translation.TestComponents+ColorTable").And
                .HaveField("Red", DBType.UInt8, IsNullable.No).And
                .HaveField("Green", DBType.UInt8, IsNullable.No).And
                .HaveField("Blue", DBType.UInt8, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void PartialEntityType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PresidentialElection);

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Should()
                .HaveName("UT.Kvasir.Translation.TestComponents+PresidentialElectionTable").And
                .HaveField("Year", DBType.UInt16, IsNullable.No).And
                .HaveField("DemocraticCandidate", DBType.Text, IsNullable.No).And
                .HaveField("DemocraticPVs", DBType.UInt64, IsNullable.No).And
                .HaveField("DemocraticEVs", DBType.UInt16, IsNullable.No).And
                .HaveField("RepublicanCandidate", DBType.Text, IsNullable.No).And
                .HaveField("RepublicanPVs", DBType.UInt64, IsNullable.No).And
                .HaveField("RepublicanEVs", DBType.UInt16, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void NonPublicEntityType() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TestComponents).GetNestedType("GitCommit", System.Reflection.BindingFlags.NonPublic)!;

            // Act
            var translation = translator[source];

            // Assert
            translation.CLRSource.Should().Be(source);
            translation.Principal.Table.Should()
                .HaveName("UT.Kvasir.Translation.TestComponents+GitCommitTable").And
                .HaveField("Hash", DBType.Text, IsNullable.No).And
                .HaveField("Author", DBType.Text, IsNullable.No).And
                .HaveField("Message", DBType.Text, IsNullable.No).And
                .HaveField("Timestamp", DBType.DateTime, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void StructEntityType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Carbohydrate);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*cannot be an Entity Type*")              // rationale
                .WithMessage("*struct or*record struct*");              // explanation
        }

        [TestMethod] public void RecordStructEntityType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AminoAcid);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*cannot be an Entity Type*")              // rationale
                .WithMessage("*struct or*record struct*");              // explanation
        }

        [TestMethod] public void AbstractEntityType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SuperBowl);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*cannot be an Entity Type*")              // rationale
                .WithMessage("*abstract*");                             // explanation
        }

        [TestMethod] public void OpenGenericEntityType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Speedometer<>);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*cannot be an Entity Type*")              // rationale
                .WithMessage("*open generic*");                         // explanation
        }
        
        [TestMethod] public void ClosedGenericEntityType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Speedometer<int>);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*cannot be an Entity Type*")              // rationale
                .WithMessage("*closed generic*");                       // explanation
        }

        [TestMethod] public void InterfaceEntityType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ILiquor);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*cannot be an Entity Type*")              // rationale
                .WithMessage("*interface*");                            // explanation
        }

        [TestMethod] public void EnumEntityType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Season);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*cannot be an Entity Type*")              // rationale
                .WithMessage("*enumeration*");                          // explanation
        }

        [TestMethod] public void PrimitiveEntityType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ushort);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*cannot be an Entity Type*")              // rationale
                .WithMessage("*primitive*");                            // explanation
        }

        [TestMethod] public void EntityTypeWithZeroProperties_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Nothing);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*cannot be an Entity Type*")              // rationale
                .WithMessage("*0 Fields*")                              // details
                .WithMessage("*at least 2 Fields*");                    // explanation
        }

        [TestMethod] public void EntityTypeWithOneProperty_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Integer);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*cannot be an Entity Type*")              // rationale
                .WithMessage("*1 Field*")                               // details
                .WithMessage("*at least 2 Fields*");                    // explanation
        }

        [TestMethod] public void NonPublicPropertiesExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Animal);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .NotHaveField("Kingdom").And
                .NotHaveField("Phylum").And
                .NotHaveField("Class").And
                .NotHaveField("Order").And
                .NotHaveField("Family").And
                .HaveField("Genus", DBType.Text, IsNullable.No).And
                .HaveField("Species", DBType.Text, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void NonReadablePropertiesExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ScrabbleTile);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .NotHaveField("NumAvailable").And
                .HaveField("Letter", DBType.Character, IsNullable.No).And
                .HaveField("Value", DBType.UInt8, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void PublicPropertiesWithNonPublicAccessorsExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChemicalElement);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .NotHaveField("AtomicNumber").And
                .NotHaveField("AtomicWeight").And
                .NotHaveField("Name").And
                .NotHaveField("MeltingPoint").And
                .NotHaveField("BoilingPoint").And
                .HaveField("Symbol", DBType.Text, IsNullable.No).And
                .HaveField("NumAllotropes", DBType.Int8, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void StaticPropertiesExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Circle);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .NotHaveField("PI").And
                .HaveField("CenterX", DBType.Int32, IsNullable.No).And
                .HaveField("CenterY", DBType.Int32, IsNullable.No).And
                .HaveField("Radius", DBType.UInt64, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void IndexerPropertiesExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BattingOrder);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("GameID", DBType.Guid, IsNullable.No).And
                .HaveField("Team", DBType.Text, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void CodeOnlyCausesPropertyToBeExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(QuadraticEquation);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .NotHaveField("Expression").And
                .HaveField("QuadraticCoefficient", DBType.Int64, IsNullable.No).And
                .HaveField("LinearCoefficient", DBType.Int64, IsNullable.No).And
                .HaveField("Constant", DBType.Int64, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void VirtualPropertyDeclarationIncluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(GreekGod);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name", DBType.Text, IsNullable.No).And
                .HaveField("RomanEquivalent", DBType.Text, IsNullable.Yes).And
                .HaveField("NumChildren", DBType.UInt32, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void InterfacePropertiesExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Movie);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .NotHaveField("English").And
                .NotHaveField("French").And
                .NotHaveField("Spanish").And
                .HaveField("ID", DBType.Guid, IsNullable.No).And
                .HaveField("Release", DBType.DateTime, IsNullable.No).And
                .HaveField("Runtime", DBType.UInt8, IsNullable.No).And
                .HaveField("Director", DBType.Text, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void InheritedPropertiesExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Holiday);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .NotHaveField("Day").And
                .NotHaveField("Month").And
                .NotHaveField("Year").And
                .HaveField("Date", DBType.DateTime, IsNullable.No).And
                .HaveField("Name", DBType.Text, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void OverridingPropertiesExcluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(POBox);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .NotHaveField("Number").And
                .NotHaveField("Street").And
                .NotHaveField("City").And
                .NotHaveField("Country").And
                .NotHaveField("Apartment").And
                .HaveField("POBoxNumber", DBType.UInt32, IsNullable.No).And
                .HaveField("KnownAs", DBType.Text, IsNullable.Yes).And
                .NoOtherFields();
        }

        [TestMethod] public void HidingPropertiesIncluded() {
            // Arrange
            var translator = new Translator();
            var source = typeof(FighterJet);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .NotHaveField("Company").And
                .NotHaveField("Model").And
                .NotHaveField("InUse").And
                .HaveField("Type", DBType.Text, IsNullable.No).And
                .HaveField("Nickname", DBType.Text, IsNullable.No).And
                .HaveField("FirstFlight", DBType.DateTime, IsNullable.No).And
                .HaveField("Capacity", DBType.UInt8, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void IndexerIncludedInModel_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Language);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*[IncludeInModel]*")                      // annotation
                .WithMessage("*indexer*");                              // rationale
        }

        [TestMethod] public void NonReadablePropertyIncludedInModel_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(HebrewPrayer);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(HebrewPrayer.OnShabbat)}*")     // source property
                .WithMessage("*[IncludeInModel]*")                      // annotation
                .WithMessage("*write-only*");                           // rationale
        }

        [TestMethod] public void StaticPropertiesIncludedInModel() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ChessPiece);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Name", DBType.Text, IsNullable.No).And
                .HaveField("Icon", DBType.Character, IsNullable.No).And
                .HaveField("Value", DBType.UInt8, IsNullable.No).And
                .HaveField("FIDE", DBType.Text, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void NonPublicPropertiesIncludedInModel() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Song);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Title", DBType.Text, IsNullable.No).And
                .HaveField("Artist", DBType.Text, IsNullable.No).And
                .HaveField("Album", DBType.Text, IsNullable.Yes).And
                .HaveField("Length", DBType.UInt16, IsNullable.No).And
                .HaveField("ReleaseYear", DBType.UInt16, IsNullable.No).And
                .HaveField("Rating", DBType.Double, IsNullable.No).And
                .HaveField("Grammys", DBType.UInt8, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void PublicPropertiesWithNonPublicAccessorsIncludedInModel() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Country);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("Exonym", DBType.Text, IsNullable.No).And
                .HaveField("Endonym", DBType.Text, IsNullable.No).And
                .HaveField("IndependenceDay", DBType.DateTime, IsNullable.No).And
                .HaveField("Population", DBType.UInt64, IsNullable.No).And
                .HaveField("LandArea", DBType.UInt64, IsNullable.No).And
                .HaveField("Coastline", DBType.UInt64, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void InterfacePropertyIncludedInModel_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Book);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Book.Title)}*")                 // source property
                .WithMessage("*[IncludeInModel]*")                      // annotation
                .WithMessage("*inherited*")                             // rationale
                .WithMessage("*interface*");                            // details
        }

        [TestMethod] public void OverridingPropertyIncludedInModel_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Drum);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Drum.LowestKey)}*")             // source property
                .WithMessage("*[IncludeInModel]*")                      // annotation
                .WithMessage("*inherited*")                             // rationale
                .WithMessage("*base class*");                          // details
        }

        [TestMethod] public void InterfacePropertyMarkedCodeOnly_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(IPAddress);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(IPAddress.RFC)}*")              // source property
                .WithMessage("*[CodeOnly]*")                            // annotation
                .WithMessage("*redundant*inherited*")                   // rationale
                .WithMessage("*interface*");                            // details
        }

        [TestMethod] public void OverridingPropertyMarkedCodeOnly_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Submarine);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Submarine.NumWheels)}*")        // source property
                .WithMessage("*[CodeOnly]*")                            // annotation
                .WithMessage("*redundant*inherited*")                   // rationale
                .WithMessage("*base class*");                           // details
        }

        [TestMethod] public void NonReadablePropertyMarkedCodeOnly_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CourtCase);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(CourtCase.Year)}*")             // source property
                .WithMessage("*[CodeOnly]*")                            // annotation
                .WithMessage("*redundant*write-only*");                 // rationale
        }

        [TestMethod] public void NonPublicPropertyMarkedCodeOnly_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lake);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*Depth*")                                 // source property
                .WithMessage("*[CodeOnly]*")                            // annotation
                .WithMessage("*redundant*non-public*");                 // rationale
        }

        [TestMethod] public void PublicPropertyWithNonPublicAccessorMarkedCodeOnly_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mountain);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Mountain.Height)}*")            // source property
                .WithMessage("*[CodeOnly]*")                            // annotation
                .WithMessage("*redundant*non-public*");                 // rationale
        }

        [TestMethod] public void StaticPropertyMarkedCodeOnly_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Tossup);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Tossup.MinLength)}*")           // source property
                .WithMessage("*[CodeOnly]*")                            // annotation
                .WithMessage("*redundant*static*");                     // rationale
        }

        [TestMethod] public void IndexerMarkedCodeOnly_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(University);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage("*[CodeOnly]*")                            // annotation
                .WithMessage("*redundant*indexer*");                    // rationale
        }

        [TestMethod] public void PropertyBothIncludedInModelAndCodeOnly_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CreditCard);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(CreditCard.CVV)}*")             // source property
                .WithMessage("*[IncludeInModel]*[CodeOnly]*")           // annotation
                .WithMessage("*both*");                                 // rationale
        }

        [TestMethod] public void PropertyWithClrTypeDelegate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Hurricane);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Hurricane.Form)}*")             // source property
                .WithMessage("*has invalid type*")                      // rationale
                .WithMessage($"*{nameof(Action)}*")                     // details
                .WithMessage("*delegate*");                             // explanation
        }

        [TestMethod] public void PropertyWithClrTypeDynamic_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(MonopolyProperty);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(MonopolyProperty.HotelCost)}*") // source property
                .WithMessage("*has invalid type*")                      // rationale
                .WithMessage("*dynamic*");                              // explanation
        }

        [TestMethod] public void PropertyWithClrTypeObject_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(URL);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(URL.NetLoc)}*")                 // source property
                .WithMessage("*has invalid type*")                      // rationale
                .WithMessage($"*{nameof(Object)}*");                    // details
        }

        [TestMethod] public void PropertyWithClrTypeSystemEnum_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Enumeration);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Enumeration.ZeroValue)}*")      // source property
                .WithMessage("*has invalid type*")                      // rationale
                .WithMessage($"*{nameof(Enum)}*");                      // details
        }

        [TestMethod] public void PropertyWithClrTypeSystemValueType_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(YouTubeVideo);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(YouTubeVideo.CommentCount)}*")  // source property
                .WithMessage("*has invalid type*")                      // rationale
                .WithMessage($"*{nameof(ValueType)}*");                 // details
        }

        [TestMethod] public void PropertyWithClrTypeExternalAssemblyClass_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Coin);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Coin.CounterfeitResult)}*")     // source property
                .WithMessage("*has invalid type*")                      // rationale
                .WithMessage($"*{nameof(Exception)}*")                  // details
                .WithMessage("*not from assembly*");                    // explanation
        }

        [TestMethod] public void PropertyWithClrTypeInterface_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Painting);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Painting.Artist)}*")            // source property
                .WithMessage("*has invalid type*")                      // rationale
                .WithMessage($"*{nameof(IArtist)}*")                    // details
                .WithMessage("*interface*");                            // explanation
        }

        [TestMethod] public void PropertyWithClosedGenericUserClass_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(SlackChannel);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(SlackChannel.NumMessages)}*")   // source property
                .WithMessage("*has invalid type*")                      // rationale
                .WithMessage($"*{nameof(MessageCount<ushort>)}*")       // details
                .WithMessage("*closed generic class*");                 // explanation
        }

        [TestMethod] public void PropertyWithAbstractClass_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BotanicalGarden);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                                // source type
                .WithMessage($"*{nameof(BotanicalGarden.OfficialFlower)}*")     // source property
                .WithMessage("*has invalid type*")                              // rationale
                .WithMessage($"*{nameof(Flower)}*")                             // details
                .WithMessage("*abstract*");                                     // explanation
        }

        [TestMethod] public void PropertiesWithDisallowedTypesMarkedCodeOnly() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DNDCharacter);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .NotHaveField("RollDice").And
                .NotHaveField("Class").And
                .NotHaveField("Race").And
                .NotHaveField("HP").And
                .NotHaveField("AttackEconomy").And
                .NotHaveField("Armor").And
                .HaveField("Name", DBType.Text, IsNullable.No).And
                .HaveField("Charisma", DBType.UInt8, IsNullable.No).And
                .HaveField("Constitution", DBType.UInt8, IsNullable.No).And
                .HaveField("Dexterity", DBType.UInt8, IsNullable.No).And
                .HaveField("Intelligence", DBType.UInt8, IsNullable.No).And
                .HaveField("Strength", DBType.UInt8, IsNullable.No).And
                .HaveField("Wisdom", DBType.UInt8, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void NonNullableScalarMarkedNullable() {
            var translator = new Translator();
            var source = typeof(Timestamp);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("UnixSinceEpoch", DBType.UInt64, IsNullable.No).And
                .HaveField("Hour", DBType.UInt16, IsNullable.No).And
                .HaveField("Minute", DBType.UInt16, IsNullable.No).And
                .HaveField("Second", DBType.UInt16, IsNullable.No).And
                .HaveField("Millisecond", DBType.UInt16, IsNullable.Yes).And
                .HaveField("Microsecond", DBType.UInt16, IsNullable.Yes).And
                .HaveField("Nanosecond", DBType.UInt16, IsNullable.Yes).And
                .NoOtherFields();
        }

        [TestMethod] public void NullableScalarMarkedNonNullable() {
            var translator = new Translator();
            var source = typeof(Bone);

            // Act
            var translation = translator[source];

            // Assert
            translation.Principal.Table.Should()
                .HaveField("TA2", DBType.UInt32, IsNullable.No).And
                .HaveField("Name", DBType.Text, IsNullable.No).And
                .HaveField("LatinName", DBType.Text, IsNullable.Yes).And
                .HaveField("MeSH", DBType.Text, IsNullable.No).And
                .NoOtherFields();
        }

        [TestMethod] public void NullableScalarMarkedNullable_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CivMilitaryUnit);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(CivMilitaryUnit.Promotion)}*")  // source property
                .WithMessage("*[Nullable]*")                            // annotation
                .WithMessage("*redundant*");                            // rationale
        }

        [TestMethod] public void NonNullableScalarMarkedNonNullable_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Patent);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Patent.PublicationDate)}*")     // source property
                .WithMessage("*[NonNullable]*")                         // annotation
                .WithMessage("*redundant*");                            // rationale
        }

        [TestMethod] public void PropertyMarkedBothNullableAndNonNullable_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RetailProduct);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(RetailProduct.SalePrice)}*")    // source property
                .WithMessage("*[Nullable]*[NonNullable]*")              // annotation
                .WithMessage("*both*");                                 // rationale
        }

        [TestMethod] public void PublicInstancePropertyAnnotatedIncludeInModel_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Haiku);

            // Act
            var act = () => translator[source];

            // Assert
            act.Should().Throw<KvasirException>()
                .WithMessage($"*{source.Name}*")                        // source type
                .WithMessage($"*{nameof(Haiku.Line2)}*")                // source property
                .WithMessage("*[IncludeInModel]*")                      // annotation
                .WithMessage("*public*non-static*");                    // rationale
        }
    }
}
