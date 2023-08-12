using Atropos.NSubstitute;
using Cybele.Core;
using FluentAssertions;
using Kvasir.Core;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

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
                        table[new FieldName(nameof(VampireSlayer.Deaths))]
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
                        table[new FieldName(nameof(Lyric.IsSpoken))]
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
                        table[new FieldName("Orbit.Aphelion")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
            CustomCheck.Generator.Received(1).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName("Orbit.Eccentricity")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void Check_AppliedToNestedAggregate() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Vineyard);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Vineyard.SignatureWine))              // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check]")                                   // details / explanation
                .WithMessageContaining("\"Vintage\"");                              // details / explanation
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
                        table[new FieldName("Sydney.Exchange.ExchangeID")]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
        }

        [TestMethod] public void Check_AppliedToNestedReference() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Werewolf);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Werewolf.Lycan))                      // error location
                .WithMessageContaining("refers to a non-scalar")                    // category
                .WithMessageContaining("[Check]")                                   // details / explanation
                .WithMessageContaining("\"Source\"");                               // details / explanation
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
                        table[new FieldName(nameof(TarotCard.Pips))]
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
                        table[new FieldName(nameof(DataStructure.RemoveBigO))]
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
                        table[new FieldName("HeightOf")]
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
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Patreon.Tier3))                       // error location
                .WithMessageContaining("[Check]")                                   // details / explanation
                .WithMessageContaining(nameof(NonSerializedAttribute))              // details / explanation
                .WithMessageContaining("does not implement")                        // details / explanation
                .WithMessageContaining(nameof(IConstraintGenerator));               // details / explanation
        }

        [TestMethod] public void Check_ConstraintGeneratorNoViableConstructor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Transistor);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Transistor.Dopant))                   // error location
                .WithMessageContaining("cannot be constructed")                     // category
                .WithMessageContaining("[Check]")                                   // details / explanation
                .WithMessageContaining(nameof(PrivateCheck))                        // details / explanation
                .WithMessageContaining("\"Dopant\", 4");                            // details / explanation
        }

        [TestMethod] public void Check_ConstraintGeneratorConstructorThrows_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(BasketballPlayer);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(BasketballPlayer.Rebounds))           // error location
                .WithMessageContaining("error constructing")                        // category
                .WithMessageContaining("[Check]")                                   // details / explanation
                .WithMessageContaining(nameof(UnconstructibleCheck))                // details / explanation
                .WithMessageContaining(CANNOT_CONSTRUCT_MSG)                        // details / explanation
                .WithMessageContaining("false, 17");                                // details / explanation
        }

        [TestMethod] public void Check_ConstraintGeneratorGenerationThrows_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Aquarium);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Aquarium.HasDolphins))                // error location
                .WithMessageContaining("unable to create custom constraint")        // category
                .WithMessageContaining("[Check]")                                   // details / explanation
                .WithMessageContaining(CANNOT_CREATE_MSG);                          // details / explanation
        }

        [TestMethod] public void Check_PathIsNull_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Trilogy);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Trilogy.Entry2))                      // error location
                .WithMessageContaining("path is null")                              // category
                .WithMessageContaining("[Check]");                                  // details / explanation
        }

        [TestMethod] public void Check_PathOnScalar_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(TarPits);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(TarPits.FirstFossil))                 // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check]")                                   // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void Check_NonExistentPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(StarCrossedLovers);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(StarCrossedLovers.Lover2))            // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check]")                                   // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void Check_NoPathOnAggregate_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Zombie);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Zombie.Legs))                         // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check]");                                  // details / explanation
        }

        [TestMethod] public void Check_NonExistentPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Piano);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Piano.Manufacturer))                  // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check]")                                   // details / explanation
                .WithMessageContaining("\"---\"");                                  // details / explanation
        }

        [TestMethod] public void Check_NonPrimaryKeyFieldPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Exorcism);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Exorcism.Target))                     // error location
                .WithMessageContaining("path*does not exist")                       // category
                .WithMessageContaining("[Check]")                                   // details / explanation
                .WithMessageContaining("\"Incipience\"");                           // details / explanation
        }

        [TestMethod] public void Check_NoPathOnReference_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pond);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining(nameof(Pond.Location))                       // error location
                .WithMessageContaining("path is required")                          // category
                .WithMessageContaining("[Check]");                                  // details / explanation
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
                        table[new FieldName(nameof(CanterburyTale.FirstLine))]
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
                        table[new FieldName(nameof(Pope.ConclaveRounds))]
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
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("[Check.Complex]")                           // details / explanation
                .WithMessageContaining("at least 1 Field name required");           // details / explanation
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
                        table[new FieldName(nameof(LinuxDistribution.Major))],
                        table[new FieldName(nameof(LinuxDistribution.Minor))],
                        table[new FieldName(nameof(LinuxDistribution.Patch))]
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
                        table[new FieldName(nameof(Muppet.Name))],
                        table[new FieldName(nameof(Muppet.Name))],
                        table[new FieldName(nameof(Muppet.Name))],
                        table[new FieldName(nameof(Muppet.Name))]
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
                        table[new FieldName("Cuisine")],
                        table[new FieldName("ContainsTomatoes")]
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
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("[Check.Complex]")                           // details / explanation
                .WithMessageContaining("Field*does not exist")                      // details / explanation
                .WithMessageContaining("\"Width\"");                                // details / explanation
        }

        [TestMethod] public void ComplexCheck_UnrecognizedField_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(PeaceTreaty);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("[Check.Complex]")                           // details / explanation
                .WithMessageContaining("Field*does not exist")                      // details / explanation
                .WithMessageContaining("\"Belligerents\"");                         // details / explanation
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
                        table[new FieldName(nameof(Massacre.When))],
                        table[new FieldName(nameof(Massacre.Casualties))],
                        table[new FieldName(nameof(Massacre.When))]
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
                        table[new FieldName(nameof(Musical.LengthMinutes))]
                    }
                ),
                Arg.Is<IEnumerable<DataConverter>>(s => s.Count() == 1),
                Settings.Default
            );
            CustomCheck.Generator.Received(2).MakeConstraint(
                NArg.IsSameSequence<IEnumerable<IField>>(
                    new IField[] {
                        table[new FieldName(nameof(Musical.SungThrough))]
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
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("[Check.Complex]")                           // details / explanation
                .WithMessageContaining(nameof(AssemblyLoadEventArgs))               // details / explanation
                .WithMessageContaining("does not implement")                        // details / explanation
                .WithMessageContaining(nameof(IConstraintGenerator));               // details / explanation
        }

        [TestMethod] public void ComplexCheck_ConstraintGeneratorNoViableConstructor_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(CookingOil);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("cannot be constructed")                     // category
                .WithMessageContaining("[Check.Complex]")                           // details / explanation
                .WithMessageContaining(nameof(PrivateCheck))                        // details / explanation
                .WithMessageContaining("'O', 'I', 'L', '!'");                       // details / explanation
        }

        [TestMethod] public void ComplexCheck_ConstraintGeneratorConstructorThrows_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(Pirate);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("error constructing")                        // category
                .WithMessageContaining("[Check.Complex]")                           // details / explanation
                .WithMessageContaining(nameof(UnconstructibleCheck))                // details / explanation
                .WithMessageContaining(CANNOT_CONSTRUCT_MSG)                        // details / explanation
                .WithMessageContaining("\"Lifespan\", 2918.01, true");              // details / explanation
        }

        [TestMethod] public void ComplexCheck_ConstraintGeneratorGenerationThrows_IsError() {
            // Arrange
            var translator = new Translator();
            var source = typeof(ComplexCheckConstraints.Attribute);

            // Act
            var translate = () => translator[source];

            // Assert
            translate.Should().ThrowExactly<KvasirException>()
                .WithMessageContaining(source.Name)                                 // source type
                .WithMessageContaining("unable to create custom constraint")        // category
                .WithMessageContaining("[Check.Complex]")                           // details / explanation
                .WithMessageContaining(CANNOT_CREATE_MSG);                          // details / explanation
        }
    }
}
