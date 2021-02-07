using Atropos;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.Atropos {
    [TestClass, TestCategory("FullCheck: Equality")]
    public class Equality : FullCheckTests {
        [TestMethod] public void StructsInterfacesOnly() {
            // Arrange
            var lhs = new StructInterfaces() { ID = 1 };
            var rhs = new StructInterfaces() { ID = 2 };

            // Act
            StructInterfaces.Tracker.Reset();
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            StructInterfaces.Tracker[CallKey.OpEQ].Should().Be(0);
            StructInterfaces.Tracker[CallKey.OpNEQ].Should().Be(0);
            StructInterfaces.Tracker[CallKey.OpLT].Should().Be(0);
            StructInterfaces.Tracker[CallKey.OpGT].Should().Be(0);
            StructInterfaces.Tracker[CallKey.OpLTE].Should().Be(0);
            StructInterfaces.Tracker[CallKey.OpGTE].Should().Be(0);
            StructInterfaces.Tracker[CallKey.StrongEQ].Should().Be(4);
            StructInterfaces.Tracker[CallKey.WeakEQ].Should().Be(4);
            StructInterfaces.Tracker[CallKey.Comparison].Should().Be(0);
            StructInterfaces.Tracker[CallKey.HashCode].Should().Be(2);
        }

        [TestMethod] public void StructsStandardOperators() {
            // Arrange
            var lhs = new StructStandardOperators() { ID = 1 };
            var rhs = new StructStandardOperators() { ID = 2 };

            // Act
            StructStandardOperators.Tracker.Reset();
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            StructStandardOperators.Tracker[CallKey.OpEQ].Should().Be(4);
            StructStandardOperators.Tracker[CallKey.OpNEQ].Should().Be(4);
            StructStandardOperators.Tracker[CallKey.OpLT].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.OpGT].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.OpLTE].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.OpGTE].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.Comparison].Should().Be(0);
            StructStandardOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void StructsAlternateOperators() {
            // Arrange
            var lhs = new StructAlternateOperators() { ID = 1 };
            var rhs = new StructAlternateOperators() { ID = 2 };

            // Act
            StructAlternateOperators.Tracker.Reset();
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
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
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
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

        [TestMethod] public void StructsConversionOperators() {
            // Arrange
            var lhs = new StructConversionOperators() { ID = 1 };
            var rhs = new StructConversionOperators() { ID = 2 };

            // Act
            StructConversionOperators.Tracker.Reset();
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
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
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);
            var nullAreEqual = FullCheck.ExpectEqual(nll, nll);
            var nullAreNotEqualL = FullCheck.ExpectNotEqual(nll, lhs);
            var nullAreNotEqualR = FullCheck.ExpectNotEqual(lhs, nll);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqualL.Should().NotHaveValue();
            nullAreNotEqualR.Should().NotHaveValue();
            ClassInterfaces.Tracker[CallKey.OpEQ].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.OpNEQ].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.OpLT].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.OpGT].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.OpLTE].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.OpGTE].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.StrongEQ].Should().Be(6);
            ClassInterfaces.Tracker[CallKey.WeakEQ].Should().Be(6);
            ClassInterfaces.Tracker[CallKey.Comparison].Should().Be(0);
            ClassInterfaces.Tracker[CallKey.HashCode].Should().Be(2);
        }

        [TestMethod] public void ClassesStandardOperators() {
            // Arrange
            var lhs = new ClassStandardOperators() { ID = 1 };
            var rhs = new ClassStandardOperators() { ID = 2 };
            ClassStandardOperators? nll = null;

            // Act
            ClassStandardOperators.Tracker.Reset();
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);
            var nullAreEqual = FullCheck.ExpectEqual(nll, nll);
            var nullAreNotEqualL = FullCheck.ExpectNotEqual(nll, lhs);
            var nullAreNotEqualR = FullCheck.ExpectNotEqual(lhs, nll);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqualL.Should().NotHaveValue();
            nullAreNotEqualR.Should().NotHaveValue();
            ClassStandardOperators.Tracker[CallKey.OpEQ].Should().Be(10);
            ClassStandardOperators.Tracker[CallKey.OpNEQ].Should().Be(10);
            ClassStandardOperators.Tracker[CallKey.OpLT].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.OpGT].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.OpLTE].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.OpGTE].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.Comparison].Should().Be(0);
            ClassStandardOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void ClassesAlternateOperators() {
            // Arrange
            var lhs = new ClassAlternateOperators() { ID = 1 };
            var rhs = new ClassAlternateOperators() { ID = 2 };
            ClassAlternateOperators? nll = null;

            // Act
            ClassAlternateOperators.Tracker.Reset();
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);
            var nullAreEqual = FullCheck.ExpectEqual(nll, nll);
            var nullAreNotEqualL = FullCheck.ExpectNotEqual(nll, lhs);
            var nullAreNotEqualR = FullCheck.ExpectNotEqual(lhs, nll);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqualL.Should().NotHaveValue();
            nullAreNotEqualR.Should().NotHaveValue();
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
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);
            var nullAreEqual = FullCheck.ExpectEqual(nll, nll);
            var nullAreNotEqualL = FullCheck.ExpectNotEqual(nll, lhs);
            var nullAreNotEqualR = FullCheck.ExpectNotEqual(lhs, nll);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqualL.Should().NotHaveValue();
            nullAreNotEqualR.Should().NotHaveValue();
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
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);
            var nullAreEqual = FullCheck.ExpectEqual(nll, nll);
            var nullAreNotEqualL = FullCheck.ExpectNotEqual(nll, lhs);
            var nullAreNotEqualR = FullCheck.ExpectNotEqual(lhs, nll);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqualL.Should().NotHaveValue();
            nullAreNotEqualR.Should().NotHaveValue();
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
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);
            var nullAreEqual = FullCheck.ExpectEqual(nll, nll);
            var nullAreNotEqualL = FullCheck.ExpectNotEqual(nll, lhs);
            var nullAreNotEqualR = FullCheck.ExpectNotEqual(lhs, nll);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqualL.Should().NotHaveValue();
            nullAreNotEqualR.Should().NotHaveValue();
            ClassInheritedInterfaces.Tracker[CallKey.OpEQ].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.OpNEQ].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.OpLT].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.OpGT].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.OpLTE].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.OpGTE].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.StrongEQ].Should().Be(6);
            ClassInterfaces.Tracker[CallKey.StrongEQ].Should().Be(6);
            ClassInheritedInterfaces.Tracker[CallKey.WeakEQ].Should().Be(6);
            ClassInheritedInterfaces.Tracker[CallKey.Comparison].Should().Be(0);
            ClassInheritedInterfaces.Tracker[CallKey.HashCode].Should().Be(2);
        }

        [TestMethod] public void ClassesInheritingOperators() {
            // Arrange
            var lhs = new ClassInheritedOperators() { ID = 1 };
            var rhs = new ClassInheritedOperators() { ID = 2 };
            ClassInheritedOperators? nll = null;

            // Act
            ClassInheritedOperators.Tracker.Reset();
            var areEqual = FullCheck.ExpectEqual(lhs, lhs);
            var areNotEqual = FullCheck.ExpectNotEqual(lhs, rhs);
            var nullAreEqual = FullCheck.ExpectEqual(nll, nll);
            var nullAreNotEqualL = FullCheck.ExpectNotEqual(nll, lhs);
            var nullAreNotEqualR = FullCheck.ExpectNotEqual(lhs, nll);

            // Assert
            areEqual.Should().NotHaveValue();
            areNotEqual.Should().NotHaveValue();
            nullAreEqual.Should().NotHaveValue();
            nullAreNotEqualL.Should().NotHaveValue();
            nullAreNotEqualR.Should().NotHaveValue();
            ClassInheritedOperators.Tracker[CallKey.OpEQ].Should().Be(10);
            ClassInheritedOperators.Tracker[CallKey.OpNEQ].Should().Be(10);
            ClassInheritedOperators.Tracker[CallKey.OpLT].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.OpGT].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.OpLTE].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.OpGTE].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.StrongEQ].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.WeakEQ].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.Comparison].Should().Be(0);
            ClassInheritedOperators.Tracker[CallKey.HashCode].Should().Be(0);
        }

        [TestMethod] public void FailedEquality() {
            static void EnsureFailure(Controller.EqFailureKey reason) {
                // Arrange
                var eqController = new Controller() { LHS = 1, RHS = 1, EqualityFailure = reason };
                var neqController = new Controller() { LHS = 1, RHS = 2, EqualityFailure = reason };
                var eqOne = new Controllable() { ID = 1, Controller = eqController };
                var neqOne = new Controllable() { ID = 1, Controller = neqController };
                var neqTwo = new Controllable() { ID = 2, Controller = neqController };

                // Act
                var failAreEqual = FullCheck.ExpectEqual(eqOne, eqOne);
                var failAreNotEqual = FullCheck.ExpectNotEqual(neqOne, neqTwo);

                // Assert
                failAreEqual.Should().HaveValue();
                if (reason != Controller.EqFailureKey.Hash) {
                    failAreNotEqual.Should().HaveValue();
                }
            }

            EnsureFailure(Controller.EqFailureKey.LeqR);
            EnsureFailure(Controller.EqFailureKey.ReqL);
            EnsureFailure(Controller.EqFailureKey.LneqR);
            EnsureFailure(Controller.EqFailureKey.RneqL);
            EnsureFailure(Controller.EqFailureKey.LstrongR);
            EnsureFailure(Controller.EqFailureKey.RstrongL);
            EnsureFailure(Controller.EqFailureKey.LweakR);
            EnsureFailure(Controller.EqFailureKey.RweakL);
            EnsureFailure(Controller.EqFailureKey.Hash);
        }
    }
}
