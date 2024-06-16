using Atropos.NSubstitute;
using Cybele.Core;
using FluentAssertions;
using Kvasir.Core;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

using NameOfField = Kvasir.Schema.FieldName;

using static UT.Kvasir.Translation.TestConstraints;
using static UT.Kvasir.Translation.CustomCheckConstraints;
using static UT.Kvasir.Translation.ComplexCheckConstraints;

namespace UT.Kvasir.Translation {
    [TestClass, TestCategory("Constraints - Custom")]
    public class CustomConstraintTests {
        [TestMethod] public void Check_DefaultConstructed() {
            // Arrange
            var translator = new Translator();
            var source = typeof(VampireSlayer);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.LastCtorArgs.Should().BeEmpty();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("Deaths")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void Check_ConstructedFromArguments() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Lyric);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.LastCtorArgs.Should().BeEquivalentTo(new object?[] { 13, false, "ABC", null });
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("IsSpoken")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void Check_AppliedToAggregateNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Asteroid);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(2);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[1].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            table.CheckConstraints[1].Name.Should().NotHaveValue();
            CustomCheck.LastCtorArgs.Should().BeEmpty();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("Orbit.Aphelion")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("Orbit.Eccentricity")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void Check_AppliedToNestedAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Vineyard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Vineyard` → SignatureWine")
                .WithPath("Vintage")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Vintage`")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_AppliedToReferenceNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Stock);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.LastCtorArgs.Should().BeEmpty();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("Sydney.Exchange.ExchangeID")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void Check_AppliedToNestedReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Werewolf);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Werewolf` → Lycan")
                .WithPath("Source")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Lycanthropy`")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_AppliedToRelationNestedScalar() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CareBear);

            // Act
            var translation = translator[source];
            var table = translation.Relations[0].Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.LastCtorArgs.Should().BeEmpty();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("Item")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void Check_AppliedToNestedRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(RiverWalk);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`RiverWalk` → Hours")
                .WithPath("HolidaysClosed")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<string>`")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_ScalarConstrainedMultipleTimes() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TarotCard);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(2);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[1].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            table.CheckConstraints[1].Name.Should().NotHaveValue();
            CustomCheck.Generator.Received(2).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("Pips")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void Check_DataConvertedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(DataStructure);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("RemoveBigO")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void Check_NameChangedField() {
            // Arrange
            var translator = new Translator();
            var source = typeof(AronKodesh);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("HeightOf")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void Check_ConstraintGeneratorDoesNotImplementInterface_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Patreon);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidCustomConstraintException>()
                .WithLocation("`Patreon` → Tier3")
                .WithProblem("`NonSerializedAttribute` does not implement the `IConstraintGenerator` interface")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_ConstraintGeneratorNoViableDefaultConstructor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Seizure);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidCustomConstraintException>()
                .WithLocation("`Seizure` → SufferedBy")
                .WithProblem("`PrivateCheck` cannot be constructed from arguments {<none>}")
                .WithAnnotations("[Check]")
                .EndMessage();
        }
        
        [TestMethod] public void Check_ConstraintGeneratorNoViableArgumentsConstructor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Transistor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidCustomConstraintException>()
                .WithLocation("`Transistor` → Dopant")
                .WithProblem("`PrivateCheck` cannot be constructed from arguments {\"Dopant\", 4}")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_ConstraintGeneratorDefaultConstructorThrows_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Buffet);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidCustomConstraintException>()
                .WithLocation("`Buffet` → Cuisine")
                .WithProblem($"error constructing `UnconstructibleCheck` from arguments {{<none>}} ({CANNOT_CONSTRUCT_MSG})")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_ConstraintGeneratorArgumentsConstructorThrows_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BasketballPlayer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidCustomConstraintException>()
                .WithLocation("`BasketballPlayer` → Rebounds")
                .WithProblem($"error constructing `UnconstructibleCheck` from arguments {{false, 17}} ({CANNOT_CONSTRUCT_MSG})")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_ConstraintGeneratorGenerationThrows_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Aquarium);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<FailedOperationException>()
                .WithLocation("`Aquarium` → HasDolphins")
                .WithProblem($"unable to generate custom CHECK constraint ({CANNOT_CREATE_MSG})")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Trilogy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Trilogy` → Entry2")
                .WithProblem("the path cannot be 'null'")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TarPits);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`TarPits` → FirstFossil")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StarCrossedLovers);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`StarCrossedLovers` → Lover2")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Zombie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Zombie` → Legs")
                .WithProblem("the annotation cannot be applied to a property of Aggregate type `Necrotization`")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Piano);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Piano` → Manufacturer")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_NonPrimaryKeyFieldPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Exorcism);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Exorcism` → Target")
                .WithProblem("the path \"Incipience\" does not exist")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pond);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Pond` → Location")
                .WithProblem("the annotation cannot be applied to a property of Reference type `Coordinate`")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_NonExistentPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CanadianProvince);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`CanadianProvince` → <synthetic> `Cities`")
                .WithProblem("the path \"---\" does not exist")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_NonAnchorPrimaryKeyPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Skydiver);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidPathException>()
                .WithLocation("`Skydiver` → <synthetic> `Dives`")
                .WithProblem("the path \"Skydiver.Height\" does not exist")
                .WithAnnotations("[Check]")
                .EndMessage();
        }

        [TestMethod] public void Check_NoPathOnRelation_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Spring);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InapplicableAnnotationException>()
                .WithLocation("`Spring` → <synthetic> `ConstituentMetals`")
                .WithProblem("the annotation cannot be applied to a property of Relation type `RelationSet<string>`")
                .WithAnnotations("[Check]")
                .EndMessage();
        }
    }

    [TestClass, TestCategory("Constraints - Custom")]
    public class ComplexConstraintTests {
        [TestMethod] public void ComplexCheck_DefaultConstructed() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CanterburyTale);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.LastCtorArgs.Should().BeEmpty();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("FirstLine")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void ComplexCheck_ConstructedFromArguments() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pope);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.LastCtorArgs.Should().BeEquivalentTo(new object?[] { -93, true, 'X' });
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("ConclaveRounds")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void ComplexCheck_NoFields_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Terminator);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidCustomConstraintException>()
                .WithLocation("`Terminator`")
                .WithProblem("expected at least 1 Field, but found 0")
                .WithAnnotations("[Check.Complex]")
                .EndMessage();
        }

        [TestMethod] public void ComplexCheck_MultipleFieldsInOneConstraint() {
            // Arrange
            var translator = new Translator();
            var source = typeof(LinuxDistribution);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("Major")],
                        table[new NameOfField("Minor")],
                        table[new NameOfField("Patch")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 3),
                Settings.Default
            );
        }

        [TestMethod] public void ComplexCheck_SingleFieldMultipleTimesInOneConstraint() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Muppet);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("Name")],
                        table[new NameOfField("Name")],
                        table[new NameOfField("Name")],
                        table[new NameOfField("Name")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 4),
                Settings.Default
            );
        }

        [TestMethod] public void ComplexCheck_NameSwappedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PastaSauce);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("Cuisine")],
                        table[new NameOfField("ContainsTomatoes")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 2),
                Settings.Default
            );
        }

        [TestMethod] public void ComplexCheck_OriginalNameOfSwappedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Dam);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnrecognizedFieldException>()
                .WithLocation("`Dam`")
                .WithProblem("no Field named \"Width\" exists on the Table")
                .WithAnnotations("[Check.Complex]")
                .EndMessage();
        }

        [TestMethod] public void ComplexCheck_UnrecognizedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PeaceTreaty);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<UnrecognizedFieldException>()
                .WithLocation("`PeaceTreaty`")
                .WithProblem("no Field named \"Belligerents\" exists on the Table")
                .WithAnnotations("[Check.Complex]")
                .EndMessage();
        }

        [TestMethod] public void ComplexCheck_DataConvertedFields() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Massacre);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(1);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("When")],
                        table[new NameOfField("Casualties")],
                        table[new NameOfField("When")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 3),
                Settings.Default
            );
        }

        [TestMethod] public void ComplexCheck_RepeatedConstraint() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Musical);

            // Act
            var translation = translator[source];
            var table = translation.Principal.Table;

            // Assert
            table.CheckConstraints.Should().HaveCount(3);
            table.CheckConstraints[0].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[0].Name.Should().NotHaveValue();
            table.CheckConstraints[1].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[1].Name.Should().NotHaveValue();
            table.CheckConstraints[2].Condition.Should().BeSameAs(CustomCheck.Clause);
            table.CheckConstraints[2].Name.Should().NotHaveValue();
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("LengthMinutes")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
            CustomCheck.Generator.Received(2).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new NameOfField("SungThrough")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void ComplexCheck_ConstraintGeneratorDoesNotImplementInterface_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Mutant);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidCustomConstraintException>()
                .WithLocation("`Mutant`")
                .WithProblem("`AssemblyLoadEventArgs` does not implement the `IConstraintGenerator` interface")
                .WithAnnotations("[Check.Complex]")
                .EndMessage();
        }

        [TestMethod] public void ComplexCheck_ConstraintGeneratorNoViableArgumentsConstructor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CookingOil);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidCustomConstraintException>()
                .WithLocation("`CookingOil`")
                .WithProblem("`PrivateCheck` cannot be constructed from arguments {'O', 'I', 'L', '!'}")
                .WithAnnotations("[Check.Complex]")
                .EndMessage();
        }

        [TestMethod] public void ComplexCheck_ConstraintGeneratorArgumentsConstructorThrows_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pirate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<InvalidCustomConstraintException>()
                .WithLocation("`Pirate`")
                .WithProblem($"error constructing `UnconstructibleCheck` from arguments {{\"Lifespan\", 2918.01, true}} ({CANNOT_CONSTRUCT_MSG})")
                .WithAnnotations("[Check.Complex]")
                .EndMessage();
        }

        [TestMethod] public void ComplexCheck_ConstraintGeneratorGenerationThrows_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ComplexCheckConstraints.Attribute);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().FailWith<FailedOperationException>()
                .WithLocation("`Attribute`")
                .WithProblem($"unable to generate custom CHECK constraint ({CANNOT_CREATE_MSG})")
                .WithAnnotations("[Check.Complex]")
                .EndMessage();
        }
    }
}
