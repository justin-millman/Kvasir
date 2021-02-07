using Atropos;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.Atropos {
    [TestClass, TestCategory("FullCheck: Comparison")]
    public class Comparison : FullCheckTests {
        [TestMethod] public void StructsInterfacesOnly() {
            // Arrange
            var lhs = new StructInterfaces() { ID = 1 };
            var rhs = new StructInterfaces() { ID = 2 };

            // Act
            StructInterfaces.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var areEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);

            // Assert
            isLess.Should().NotHaveValue();
            areEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            StructInterfaces.Tracker[CallKey.OpEQ].Should().Be(0);
            StructInterfaces.Tracker[CallKey.OpNEQ].Should().Be(0);
            StructInterfaces.Tracker[CallKey.OpLT].Should().Be(0);
            StructInterfaces.Tracker[CallKey.OpGT].Should().Be(0);
            StructInterfaces.Tracker[CallKey.OpLTE].Should().Be(0);
            StructInterfaces.Tracker[CallKey.OpGTE].Should().Be(0);
            StructInterfaces.Tracker[CallKey.StrongEQ].Should().Be(0);
            StructInterfaces.Tracker[CallKey.WeakEQ].Should().Be(0);
            StructInterfaces.Tracker[CallKey.Comparison].Should().Be(6);
            StructInterfaces.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void StructsStandardOperators() {
            // Arrange
            var lhs = new StructStandardOperators() { ID = 1 };
            var rhs = new StructStandardOperators() { ID = 2 };

            // Act
            StructStandardOperators.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var areEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);

            // Assert
            isLess.Should().NotHaveValue();
            areEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            StructStandardOperators.Tracker[CallKey.OpEQ].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.OpNEQ].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.OpLT].Should().Be(6);
            StructStandardOperators.Tracker[CallKey.OpGT].Should().Be(6);
            StructStandardOperators.Tracker[CallKey.OpLTE].Should().Be(6);
            StructStandardOperators.Tracker[CallKey.OpGTE].Should().Be(6);
            StructStandardOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.Comparison].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void StructsAlternativeOperators() {
            // Arrange
            var lhs = new StructAlternateOperators() { ID = 1 };
            var rhs = new StructAlternateOperators() { ID = 2 };

            // Act
            StructAlternateOperators.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var areEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);

            // Assert
            isLess.Should().NotHaveValue();
            areEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            StructAlternateOperators.Tracker[CallKey.OpEQ].Should().Be(0);
            StructAlternateOperators.Tracker[CallKey.OpNEQ].Should().Be(0);
            StructAlternateOperators.Tracker[CallKey.OpLT].Should().Be(0);
            StructAlternateOperators.Tracker[CallKey.OpGT].Should().Be(0);
            StructAlternateOperators.Tracker[CallKey.OpLTE].Should().Be(0);
            StructAlternateOperators.Tracker[CallKey.OpGTE].Should().Be(0);
            StructAlternateOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            StructAlternateOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            StructAlternateOperators.Tracker[CallKey.Comparison].Should().Be(0);
            StructAlternateOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void StructsNonBoolOperators() {
            // Arrange
            var lhs = new StructNonBoolOperators() { ID = 1 };
            var rhs = new StructNonBoolOperators() { ID = 2 };

            // Act
            StructNonBoolOperators.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var areEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);

            // Assert
            isLess.Should().NotHaveValue();
            areEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            StructNonBoolOperators.Tracker[CallKey.OpEQ].Should().Be(0);
            StructNonBoolOperators.Tracker[CallKey.OpNEQ].Should().Be(0);
            StructNonBoolOperators.Tracker[CallKey.OpLT].Should().Be(0);
            StructNonBoolOperators.Tracker[CallKey.OpGT].Should().Be(0);
            StructNonBoolOperators.Tracker[CallKey.OpLTE].Should().Be(0);
            StructNonBoolOperators.Tracker[CallKey.OpGTE].Should().Be(0);
            StructNonBoolOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            StructNonBoolOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            StructNonBoolOperators.Tracker[CallKey.Comparison].Should().Be(0);
            StructNonBoolOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void StructsComparisonOperators() {
            // Arrange
            var lhs = new StructConversionOperators() { ID = 1 };
            var rhs = new StructConversionOperators() { ID = 2 };

            // Act
            StructConversionOperators.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var areEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);

            // Assert
            isLess.Should().NotHaveValue();
            areEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            StructConversionOperators.Tracker[CallKey.OpEQ].Should().Be(0);
            StructConversionOperators.Tracker[CallKey.OpNEQ].Should().Be(0);
            StructConversionOperators.Tracker[CallKey.OpLT].Should().Be(0);
            StructConversionOperators.Tracker[CallKey.OpGT].Should().Be(0);
            StructConversionOperators.Tracker[CallKey.OpLTE].Should().Be(0);
            StructConversionOperators.Tracker[CallKey.OpGTE].Should().Be(0);
            StructConversionOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            StructConversionOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            StructConversionOperators.Tracker[CallKey.Comparison].Should().Be(0);
            StructConversionOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void ClassesInterfacesOnly() {
            // Arrange
            var lhs = new ClassInterfaces() { ID = 1 };
            var rhs = new ClassInterfaces() { ID = 2 };
            ClassInterfaces? nll = null;

            // Act
            ClassInterfaces.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var isEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);
            var nullIsLess = FullCheck.ExpectLessThan(nll, lhs);
            var nullIsEquiv = FullCheck.ExpectEquivalent(nll, nll);
            var nullIsGreater = FullCheck.ExpectGreaterThan(lhs, nll);

            // Assert
            isLess.Should().NotHaveValue();
            isEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            nullIsLess.Should().NotHaveValue();
            nullIsEquiv.Should().NotHaveValue();
            nullIsGreater.Should().NotHaveValue();
            ClassInterfaces.Tracker[CallKey.OpEQ].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.OpNEQ].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.OpLT].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.OpGT].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.OpLTE].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.OpGTE].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.StrongEQ].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.WeakEQ].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.Comparison].Should().Be(8);
            ClassInterfaces.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void ClassesStandardOperators() {
            // Arrange
            var lhs = new ClassStandardOperators() { ID = 1 };
            var rhs = new ClassStandardOperators() { ID = 2 };
            ClassStandardOperators? nll = null;

            // Act
            ClassStandardOperators.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var isEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);
            var nullIsLess = FullCheck.ExpectLessThan(nll, lhs);
            var nullIsEquiv = FullCheck.ExpectEquivalent(nll, nll);
            var nullIsGreater = FullCheck.ExpectGreaterThan(lhs, nll);

            // Assert
            isLess.Should().NotHaveValue();
            isEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            nullIsLess.Should().NotHaveValue();
            nullIsEquiv.Should().NotHaveValue();
            nullIsGreater.Should().NotHaveValue();
            ClassStandardOperators.Tracker[CallKey.OpEQ].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.OpNEQ].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.OpLT].Should().Be(12);
            ClassStandardOperators.Tracker[CallKey.OpGT].Should().Be(12);
            ClassStandardOperators.Tracker[CallKey.OpLTE].Should().Be(12);
            ClassStandardOperators.Tracker[CallKey.OpGTE].Should().Be(12);
            ClassStandardOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.Comparison].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void ClassesAlternativeOperators() {
            // Arrange
            var lhs = new ClassAlternateOperators() { ID = 1 };
            var rhs = new ClassAlternateOperators() { ID = 2 };
            ClassAlternateOperators? nll = null;

            // Act
            ClassAlternateOperators.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var isEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);
            var nullIsLess = FullCheck.ExpectLessThan(nll, lhs);
            var nullIsEquiv = FullCheck.ExpectEquivalent(nll, nll);
            var nullIsGreater = FullCheck.ExpectGreaterThan(lhs, nll);

            // Assert
            isLess.Should().NotHaveValue();
            isEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            nullIsLess.Should().NotHaveValue();
            nullIsEquiv.Should().NotHaveValue();
            nullIsGreater.Should().NotHaveValue();
            ClassAlternateOperators.Tracker[CallKey.OpEQ].Should().Be(0);
            ClassAlternateOperators.Tracker[CallKey.OpNEQ].Should().Be(0);
            ClassAlternateOperators.Tracker[CallKey.OpLT].Should().Be(0);
            ClassAlternateOperators.Tracker[CallKey.OpGT].Should().Be(0);
            ClassAlternateOperators.Tracker[CallKey.OpLTE].Should().Be(0);
            ClassAlternateOperators.Tracker[CallKey.OpGTE].Should().Be(0);
            ClassAlternateOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            ClassAlternateOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            ClassAlternateOperators.Tracker[CallKey.Comparison].Should().Be(0);
            ClassAlternateOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void ClassesNonBoolOperators() {
            // Arrange
            var lhs = new ClassNonBoolOperators() { ID = 1 };
            var rhs = new ClassNonBoolOperators() { ID = 2 };
            ClassNonBoolOperators? nll = null;

            // Act
            ClassNonBoolOperators.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var isEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);
            var nullIsLess = FullCheck.ExpectLessThan(nll, lhs);
            var nullIsEquiv = FullCheck.ExpectEquivalent(nll, nll);
            var nullIsGreater = FullCheck.ExpectGreaterThan(lhs, nll);

            // Assert
            isLess.Should().NotHaveValue();
            isEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            nullIsLess.Should().NotHaveValue();
            nullIsEquiv.Should().NotHaveValue();
            nullIsGreater.Should().NotHaveValue();
            ClassNonBoolOperators.Tracker[CallKey.OpEQ].Should().Be(0);
            ClassNonBoolOperators.Tracker[CallKey.OpNEQ].Should().Be(0);
            ClassNonBoolOperators.Tracker[CallKey.OpLT].Should().Be(0);
            ClassNonBoolOperators.Tracker[CallKey.OpGT].Should().Be(0);
            ClassNonBoolOperators.Tracker[CallKey.OpLTE].Should().Be(0);
            ClassNonBoolOperators.Tracker[CallKey.OpGTE].Should().Be(0);
            ClassNonBoolOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            ClassNonBoolOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            ClassNonBoolOperators.Tracker[CallKey.Comparison].Should().Be(0);
            ClassNonBoolOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void ClassesConversionOperators() {
            // Arrange
            var lhs = new ClassConversionOperators() { ID = 1 };
            var rhs = new ClassConversionOperators() { ID = 2 };
            ClassConversionOperators? nll = null;

            // Act
            ClassConversionOperators.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var isEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);
            var nullIsLess = FullCheck.ExpectLessThan(nll, lhs);
            var nullIsEquiv = FullCheck.ExpectEquivalent(nll, nll);
            var nullIsGreater = FullCheck.ExpectGreaterThan(lhs, nll);

            // Assert
            isLess.Should().NotHaveValue();
            isEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            nullIsLess.Should().NotHaveValue();
            nullIsEquiv.Should().NotHaveValue();
            nullIsGreater.Should().NotHaveValue();
            ClassConversionOperators.Tracker[CallKey.OpEQ].Should().Be(0);
            ClassConversionOperators.Tracker[CallKey.OpNEQ].Should().Be(0);
            ClassConversionOperators.Tracker[CallKey.OpLT].Should().Be(0);
            ClassConversionOperators.Tracker[CallKey.OpGT].Should().Be(0);
            ClassConversionOperators.Tracker[CallKey.OpLTE].Should().Be(0);
            ClassConversionOperators.Tracker[CallKey.OpGTE].Should().Be(0);
            ClassConversionOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            ClassConversionOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            ClassConversionOperators.Tracker[CallKey.Comparison].Should().Be(0);
            ClassConversionOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void ClassesInheritingInterfaces() {
            // Arrange
            var lhs = new ClassInheritedInterfaces() { ID = 1 };
            var rhs = new ClassInheritedInterfaces() { ID = 2 };
            ClassInheritedInterfaces? nll = null;

            // Act
            ClassInheritedInterfaces.Tracker.Reset();
            ClassInterfaces.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var isEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);
            var nullIsLess = FullCheck.ExpectLessThan(nll, lhs);
            var nullIsEquiv = FullCheck.ExpectEquivalent(nll, nll);
            var nullIsGreater = FullCheck.ExpectGreaterThan(lhs, nll);

            // Assert
            isLess.Should().NotHaveValue();
            isEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            nullIsLess.Should().NotHaveValue();
            nullIsEquiv.Should().NotHaveValue();
            nullIsGreater.Should().NotHaveValue();
            ClassInheritedInterfaces.Tracker[CallKey.OpEQ].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.OpNEQ].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.OpLT].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.OpGT].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.OpLTE].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.OpGTE].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.StrongEQ].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.WeakEQ].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.Comparison].Should().Be(8);
            ClassInterfaces.Tracker[CallKey.Comparison].Should().Be(8);
            ClassInheritedInterfaces.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void ClassesInheritingOperators() {
            // Arrange
            var lhs = new ClassInheritedOperators() { ID = 1 };
            var rhs = new ClassInheritedOperators() { ID = 2 };
            ClassInheritedOperators? nll = null;

            // Act
            ClassInheritedOperators.Tracker.Reset();
            var isLess = FullCheck.ExpectLessThan(lhs, rhs);
            var isEquiv = FullCheck.ExpectEquivalent(lhs, lhs);
            var isGreater = FullCheck.ExpectGreaterThan(rhs, lhs);
            var nullIsLess = FullCheck.ExpectLessThan(nll, lhs);
            var nullIsEquiv = FullCheck.ExpectEquivalent(nll, nll);
            var nullIsGreater = FullCheck.ExpectGreaterThan(lhs, nll);

            // Assert
            isLess.Should().NotHaveValue();
            isEquiv.Should().NotHaveValue();
            isGreater.Should().NotHaveValue();
            nullIsLess.Should().NotHaveValue();
            nullIsEquiv.Should().NotHaveValue();
            nullIsGreater.Should().NotHaveValue();
            ClassInheritedOperators.Tracker[CallKey.OpEQ].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.OpNEQ].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.OpLT].Should().Be(12);
            ClassInheritedOperators.Tracker[CallKey.OpGT].Should().Be(12);
            ClassInheritedOperators.Tracker[CallKey.OpLTE].Should().Be(12);
            ClassInheritedOperators.Tracker[CallKey.OpGTE].Should().Be(12);
            ClassInheritedOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.Comparison].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void Incomparable() {
            // Arrange
            var lhs = new ClassIncomparable() { ID = 1 };
            ClassIncomparable? nll = null;

            // Act
            ClassIncomparable.Tracker.Reset();
            var isIncomp = FullCheck.ExpectIncomparable(lhs, lhs);
            var nullIncomp = FullCheck.ExpectIncomparable(nll, nll);

            // Assert
            isIncomp.Should().NotHaveValue();
            nullIncomp.Should().NotHaveValue();
            ClassIncomparable.Tracker[CallKey.OpEQ].Should().Be(0);
            ClassIncomparable.Tracker[CallKey.OpNEQ].Should().Be(0);
            ClassIncomparable.Tracker[CallKey.OpLT].Should().Be(4);
            ClassIncomparable.Tracker[CallKey.OpGT].Should().Be(4);
            ClassIncomparable.Tracker[CallKey.OpLTE].Should().Be(4);
            ClassIncomparable.Tracker[CallKey.OpGTE].Should().Be(4);
            ClassIncomparable.Tracker[CallKey.StrongEQ].Should().Be(0);
            ClassIncomparable.Tracker[CallKey.WeakEQ].Should().Be(0);
            ClassIncomparable.Tracker[CallKey.Comparison].Should().Be(0);
            ClassIncomparable.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void FailedComparison() {
            static void EnsureFailure(Controller.CompFailureKey reason) {
                // Arrange
                var ltController = new Controller() { LHS = 1, RHS = 2, ComparisonFailure = reason };
                var gtController = new Controller() { LHS = 2, RHS = 1, ComparisonFailure = reason };
                var ltOne = new Controllable() { ID = 1, Controller = ltController };
                var ltTwo = new Controllable() { ID = 2, Controller = ltController };
                var gtOne = new Controllable() { ID = 1, Controller = gtController };
                var gtTwo = new Controllable() { ID = 2, Controller = gtController };

                // Act
                var failLess = FullCheck.ExpectLessThan(ltOne, ltTwo);
                var failGreater = FullCheck.ExpectGreaterThan(gtTwo, gtOne);

                // Assert
                failLess.Should().HaveValue();
                failGreater.Should().HaveValue();
            }

            EnsureFailure(Controller.CompFailureKey.LltR);
            EnsureFailure(Controller.CompFailureKey.RltL);
            EnsureFailure(Controller.CompFailureKey.LgtR);
            EnsureFailure(Controller.CompFailureKey.RgtL);
            EnsureFailure(Controller.CompFailureKey.LlteR);
            EnsureFailure(Controller.CompFailureKey.RlteL);
            EnsureFailure(Controller.CompFailureKey.LgteR);
            EnsureFailure(Controller.CompFailureKey.RgteL);
            EnsureFailure(Controller.CompFailureKey.LcompR);
            EnsureFailure(Controller.CompFailureKey.RcompL);
        }
    }
}