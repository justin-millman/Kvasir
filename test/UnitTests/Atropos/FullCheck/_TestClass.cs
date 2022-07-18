#pragma warning disable CS0660    // overrides operator== or operator== but does not override Object.Equals(object?)
#pragma warning disable CS0661    // overrides operator== or operator!= but does not override Object.GetHashCode()
#pragma warning disable CA2231    // Implement the equality operators

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace UT.Atropos {
    public class FullCheckTests {
        protected enum CallKey { OpEQ, OpNEQ, OpLT, OpGT, OpLTE, OpGTE, StrongEQ, WeakEQ, Comparison, HashCode }

        protected sealed class CallTracker {
            public CallTracker() {
                tracker_ = new ConcurrentDictionary<CallKey, int>();
            }
            public int this[CallKey key] {
                get {
                    tracker_.TryGetValue(key, out var value);
                    return value;
                }
            }
            public void Increment(CallKey key) {
                tracker_.TryGetValue(key, out var current);
                tracker_[key] = ++current;
            }
            public void Decrement(CallKey key) {
                tracker_.TryGetValue(key, out var current);
                tracker_[key] = --current;
            }
            public void Reset() {
                tracker_.Clear();
            }


            private readonly IDictionary<CallKey, int> tracker_;
        }
        protected sealed class Controller {
            public enum EqFailureKey { LeqR, ReqL, LneqR, RneqL, LstrongR, RstrongL, LweakR, RweakL, Hash };
            public enum CompFailureKey { LltR, RltL, LgtR, RgtL, LlteR, RlteL, LgteR, RgteL, LcompR, RcompL }

            public int LHS { private get; init; }
            public int RHS { private get; init; }
            public EqFailureKey EqualityFailure { private get; init; }
            public CompFailureKey ComparisonFailure { private get; init; }

            public Controller() {
                hashCodeInvokeCount_ = 0;
                equalityFamilies_ = new Dictionary<EqFailureKey, CallKey>() {
                    [EqFailureKey.LeqR] = CallKey.OpEQ,
                    [EqFailureKey.ReqL] = CallKey.OpEQ,
                    [EqFailureKey.LneqR] = CallKey.OpNEQ,
                    [EqFailureKey.RneqL] = CallKey.OpNEQ,
                    [EqFailureKey.LstrongR] = CallKey.StrongEQ,
                    [EqFailureKey.RstrongL] = CallKey.StrongEQ,
                    [EqFailureKey.LweakR] = CallKey.WeakEQ,
                    [EqFailureKey.RweakL] = CallKey.WeakEQ,
                    [EqFailureKey.Hash] = CallKey.HashCode
                };
                comparisonFamilies_ = new Dictionary<CompFailureKey, CallKey>() {
                    [CompFailureKey.LltR] = CallKey.OpLT,
                    [CompFailureKey.RltL] = CallKey.OpLT,
                    [CompFailureKey.LgtR] = CallKey.OpGT,
                    [CompFailureKey.RgtL] = CallKey.OpGT,
                    [CompFailureKey.LlteR] = CallKey.OpLTE,
                    [CompFailureKey.RlteL] = CallKey.OpLTE,
                    [CompFailureKey.LgteR] = CallKey.OpGTE,
                    [CompFailureKey.RgteL] = CallKey.OpGTE,
                    [CompFailureKey.LcompR] = CallKey.Comparison,
                    [CompFailureKey.RcompL] = CallKey.Comparison
                };
            }
            public bool EvaluateEquality(int lhs, int rhs, bool negate, CallKey key) {
                var failureFamily = equalityFamilies_[EqualityFailure];
                var correctResult = negate ? (lhs != rhs) : (lhs == rhs);

                if (failureFamily != key) {
                    return correctResult;
                }
                else if (lhs == LHS && (int)EqualityFailure % 2 == 0) {
                    return !correctResult;
                }
                else if (lhs == RHS && (int)EqualityFailure % 2 == 1) {
                    return !correctResult;
                }
                else {
                    return correctResult;
                }
            }
            public int EvaluateHashCode(int value) {
                var failureFamily = equalityFamilies_[EqualityFailure];
                var correctValue = HashCode.Combine(value);

                if (hashCodeInvokeCount_++ % 2 == 1 || failureFamily != CallKey.HashCode) {
                    return correctValue;
                }
                return -1 * correctValue;
            }
            public int EvaluateComparison(int lhs, int rhs, CallKey key) {
                var failureFamily = comparisonFamilies_[ComparisonFailure];
                var correctResult = lhs.CompareTo(rhs);

                if (failureFamily != key) {
                    return correctResult;
                }
                else if (lhs == LHS && (int)ComparisonFailure % 2 == 0) {
                    return -1 * correctResult;
                }
                else if (lhs == RHS && (int)ComparisonFailure % 2 == 1) {
                    return -1 * correctResult;
                }
                else {
                    return correctResult;
                }
            }


            private readonly IReadOnlyDictionary<EqFailureKey, CallKey> equalityFamilies_;
            private readonly IReadOnlyDictionary<CompFailureKey, CallKey> comparisonFamilies_;
            private int hashCodeInvokeCount_;
        }

        protected readonly struct StructInterfaces : IEquatable<StructInterfaces>, IComparable<StructInterfaces> {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public readonly bool Equals(StructInterfaces rhs) {
                Tracker.Increment(CallKey.StrongEQ);
                return ID == rhs.ID;
            }
            public readonly override bool Equals(object? rhs) {
                Tracker.Increment(CallKey.WeakEQ);
                if (rhs is StructInterfaces s) {
                    Tracker.Decrement(CallKey.StrongEQ);
                    return Equals(s);
                }
                return false;
            }
            public readonly override int GetHashCode() {
                Tracker.Increment(CallKey.HashCode);
                return HashCode.Combine(ID);
            }
            public readonly int CompareTo(StructInterfaces rhs) {
                Tracker.Increment(CallKey.Comparison);
                return ID.CompareTo(rhs.ID);
            }
        }
        protected readonly struct StructStandardOperators {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public static bool operator ==(StructStandardOperators lhs, StructStandardOperators rhs) {
                Tracker.Increment(CallKey.OpEQ);
                return lhs.ID == rhs.ID;
            }
            public static bool operator !=(StructStandardOperators lhs, StructStandardOperators rhs) {
                Tracker.Increment(CallKey.OpNEQ);
                Tracker.Decrement(CallKey.OpEQ);
                return !(lhs == rhs);
            }
            public static bool operator <(StructStandardOperators lhs, StructStandardOperators rhs) {
                Tracker.Increment(CallKey.OpLT);
                return Compare(lhs, rhs) < 0;
            }
            public static bool operator >(StructStandardOperators lhs, StructStandardOperators rhs) {
                Tracker.Increment(CallKey.OpGT);
                return Compare(lhs, rhs) > 0;
            }
            public static bool operator <=(StructStandardOperators lhs, StructStandardOperators rhs) {
                Tracker.Increment(CallKey.OpLTE);
                return Compare(lhs, rhs) <= 0;
            }
            public static bool operator >=(StructStandardOperators lhs, StructStandardOperators rhs) {
                Tracker.Increment(CallKey.OpGTE);
                return Compare(lhs, rhs) >= 0;
            }

            private static int Compare(StructStandardOperators lhs, StructStandardOperators rhs) {
                return lhs.ID.CompareTo(rhs.ID);
            }
        }
        protected readonly struct StructAlternateOperators {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public static bool operator ==(StructAlternateOperators _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator !=(StructAlternateOperators _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator <(StructAlternateOperators _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator >(StructAlternateOperators _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator <=(StructAlternateOperators _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator >=(StructAlternateOperators _, int _1) {
                throw new NotSupportedException();
            }
        }
        protected readonly struct StructNonBoolOperators {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public static int operator ==(StructNonBoolOperators _, StructNonBoolOperators _1) {
                throw new NotSupportedException();
            }
            public static int operator !=(StructNonBoolOperators _, StructNonBoolOperators _1) {
                throw new NotSupportedException();
            }
            public static int operator <(StructNonBoolOperators _, StructNonBoolOperators _1) {
                throw new NotSupportedException();
            }
            public static int operator >(StructNonBoolOperators _, StructNonBoolOperators _1) {
                throw new NotSupportedException();
            }
            public static int operator <=(StructNonBoolOperators _, StructNonBoolOperators _1) {
                throw new NotSupportedException();
            }
            public static int operator >=(StructNonBoolOperators _, StructNonBoolOperators _1) {
                throw new NotSupportedException();
            }
        }
        protected readonly struct StructConversionOperators {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public static implicit operator int(StructConversionOperators source) {
                return source.ID;
            }

            public static bool operator ==(StructConversionOperators _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator !=(StructConversionOperators _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator <(StructConversionOperators _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator >(StructConversionOperators _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator <=(StructConversionOperators _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator >=(StructConversionOperators _, int _1) {
                throw new NotSupportedException();
            }
        }
        protected readonly struct StructIncomparable {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public static bool operator ==(StructIncomparable _, StructIncomparable _1) {
                Tracker.Increment(CallKey.OpEQ);
                return false;
            }
            public static bool operator !=(StructIncomparable _, StructIncomparable _1) {
                Tracker.Increment(CallKey.OpNEQ);
                return false;
            }
            public static bool operator <(StructIncomparable _, StructIncomparable _1) {
                Tracker.Increment(CallKey.OpLT);
                return false;
            }
            public static bool operator >(StructIncomparable _, StructIncomparable _1) {
                Tracker.Increment(CallKey.OpGT);
                return false;
            }
            public static bool operator <=(StructIncomparable _, StructIncomparable _1) {
                Tracker.Increment(CallKey.OpLTE);
                return false;
            }
            public static bool operator >=(StructIncomparable _, StructIncomparable _1) {
                Tracker.Increment(CallKey.OpGTE);
                return false;
            }
        }

        protected class ClassInterfaces : IEquatable<ClassInterfaces>, IComparable<ClassInterfaces> {
            public int ID { protected get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public bool Equals(ClassInterfaces? rhs) {
                Tracker.Increment(CallKey.StrongEQ);
                return ID == rhs?.ID;
            }
            public override bool Equals(object? rhs) {
                Tracker.Increment(CallKey.WeakEQ);
                if (rhs is ClassInterfaces c) {
                    Tracker.Decrement(CallKey.StrongEQ);
                    return Equals(c);
                }
                return false;
            }
            public override int GetHashCode() {
                Tracker.Increment(CallKey.HashCode);
                return HashCode.Combine(ID);
            }
            public int CompareTo(ClassInterfaces? rhs) {
                Tracker.Increment(CallKey.Comparison);
                return ID.CompareTo(rhs?.ID);
            }
        }
        protected sealed class ClassInheritedInterfaces : ClassInterfaces, IEquatable<ClassInheritedInterfaces>,
            IComparable<ClassInheritedInterfaces> {

            public static new CallTracker Tracker { get; } = new CallTracker();

            public bool Equals(ClassInheritedInterfaces? rhs) {
                Tracker.Increment(CallKey.StrongEQ);
                return ID == rhs?.ID;
            }
            public sealed override bool Equals(object? rhs) {
                Tracker.Increment(CallKey.WeakEQ);
                if (rhs is ClassInheritedInterfaces c) {
                    Tracker.Decrement(CallKey.StrongEQ);
                    return Equals(c);
                }
                return false;
            }
            public sealed override int GetHashCode() {
                Tracker.Increment(CallKey.HashCode);
                return HashCode.Combine(ID);
            }
            public int CompareTo(ClassInheritedInterfaces? rhs) {
                Tracker.Increment(CallKey.Comparison);
                return ID.CompareTo(rhs?.ID);
            }
        }
        protected class ClassStandardOperators {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public static bool operator ==(ClassStandardOperators? lhs, ClassStandardOperators? rhs) {
                Tracker.Increment(CallKey.OpEQ);
                return lhs?.ID == rhs?.ID;
            }
            public static bool operator !=(ClassStandardOperators? lhs, ClassStandardOperators? rhs) {
                Tracker.Increment(CallKey.OpNEQ);
                Tracker.Decrement(CallKey.OpEQ);
                return !(lhs == rhs);
            }
            public static bool operator <(ClassStandardOperators? lhs, ClassStandardOperators? rhs) {
                Tracker.Increment(CallKey.OpLT);
                return Compare(lhs, rhs) < 0;
            }
            public static bool operator >(ClassStandardOperators? lhs, ClassStandardOperators? rhs) {
                Tracker.Increment(CallKey.OpGT);
                return Compare(lhs, rhs) > 0;
            }
            public static bool operator <=(ClassStandardOperators? lhs, ClassStandardOperators? rhs) {
                Tracker.Increment(CallKey.OpLTE);
                return Compare(lhs, rhs) <= 0;
            }
            public static bool operator >=(ClassStandardOperators? lhs, ClassStandardOperators? rhs) {
                Tracker.Increment(CallKey.OpGTE);
                return Compare(lhs, rhs) >= 0;
            }

            private static int Compare(ClassStandardOperators? lhs, ClassStandardOperators? rhs) {
                if (lhs is null) {
                    return rhs is null ? 0 : -1;
                }
                return lhs.ID.CompareTo(rhs?.ID);
            }
        }
        protected sealed class ClassInheritedOperators : ClassStandardOperators {
        }
        protected sealed class ClassAlternateOperators {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public static bool operator ==(ClassAlternateOperators? _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator !=(ClassAlternateOperators? _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator <(ClassAlternateOperators? _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator >(ClassAlternateOperators? _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator <=(ClassAlternateOperators? _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator >=(ClassAlternateOperators? _, int _1) {
                throw new NotSupportedException();
            }
        }
        protected sealed class ClassNonBoolOperators {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public static int operator ==(ClassNonBoolOperators? _, ClassNonBoolOperators? _1) {
                throw new NotSupportedException();
            }
            public static int operator !=(ClassNonBoolOperators? _, ClassNonBoolOperators? _1) {
                throw new NotSupportedException();
            }
            public static int operator <(ClassNonBoolOperators? _, ClassNonBoolOperators? _1) {
                throw new NotSupportedException();
            }
            public static int operator >(ClassNonBoolOperators? _, ClassNonBoolOperators? _1) {
                throw new NotSupportedException();
            }
            public static int operator <=(ClassNonBoolOperators? _, ClassNonBoolOperators? _1) {
                throw new NotSupportedException();
            }
            public static int operator >=(ClassNonBoolOperators? _, ClassNonBoolOperators? _1) {
                throw new NotSupportedException();
            }
        }
        protected sealed class ClassConversionOperators {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public static implicit operator int?(ClassConversionOperators? source) {
                return source?.ID;
            }

            public static bool operator ==(ClassConversionOperators? _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator !=(ClassConversionOperators? _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator <(ClassConversionOperators? _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator >(ClassConversionOperators? _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator <=(ClassConversionOperators? _, int _1) {
                throw new NotSupportedException();
            }
            public static bool operator >=(ClassConversionOperators? _, int _1) {
                throw new NotSupportedException();
            }
        }
        protected sealed class ClassIncomparable {
            public int ID { private get; init; }
            public static CallTracker Tracker { get; } = new CallTracker();

            public static bool operator ==(ClassIncomparable? _, ClassIncomparable? _1) {
                Tracker.Increment(CallKey.OpEQ);
                return false;
            }
            public static bool operator !=(ClassIncomparable? _, ClassIncomparable? _1) {
                Tracker.Increment(CallKey.OpNEQ);
                return false;
            }
            public static bool operator <(ClassIncomparable? _, ClassIncomparable? _1) {
                Tracker.Increment(CallKey.OpLT);
                return false;
            }
            public static bool operator >(ClassIncomparable? _, ClassIncomparable? _1) {
                Tracker.Increment(CallKey.OpGT);
                return false;
            }
            public static bool operator <=(ClassIncomparable? _, ClassIncomparable? _1) {
                Tracker.Increment(CallKey.OpLTE);
                return false;
            }
            public static bool operator >=(ClassIncomparable? _, ClassIncomparable? _1) {
                Tracker.Increment(CallKey.OpGTE);
                return false;
            }
        }

        protected readonly struct Controllable : IEquatable<Controllable>, IComparable<Controllable>, IDisposable {
            public int ID { private get; init; }
            public Controller Controller { private get; init; }

            public void Dispose() { }
            public readonly bool Equals(Controllable rhs) {
                return Controller.EvaluateEquality(ID, rhs.ID, false, CallKey.StrongEQ);
            }
            public readonly override bool Equals(object? rhs) {
                return Controller.EvaluateEquality(ID, ((Controllable)rhs!).ID, false, CallKey.WeakEQ);
            }
            public readonly override int GetHashCode() {
                return Controller.EvaluateHashCode(ID);
            }
            public readonly int CompareTo(Controllable rhs) {
                return Controller.EvaluateComparison(ID, rhs.ID, CallKey.Comparison);
            }

            public static bool operator ==(Controllable lhs, Controllable rhs) {
                return lhs.Controller.EvaluateEquality(lhs.ID, rhs.ID, false, CallKey.OpEQ);
            }
            public static bool operator !=(Controllable lhs, Controllable rhs) {
                return lhs.Controller.EvaluateEquality(lhs.ID, rhs.ID, true, CallKey.OpNEQ);
            }
            public static bool operator <(Controllable lhs, Controllable rhs) {
                return lhs.Controller.EvaluateComparison(lhs.ID, rhs.ID, CallKey.OpLT) < 0;
            }
            public static bool operator >(Controllable lhs, Controllable rhs) {
                return lhs.Controller.EvaluateComparison(lhs.ID, rhs.ID, CallKey.OpGT) > 0;
            }
            public static bool operator <=(Controllable lhs, Controllable rhs) {
                return lhs.Controller.EvaluateComparison(lhs.ID, rhs.ID, CallKey.OpLTE) <= 0;
            }
            public static bool operator >=(Controllable lhs, Controllable rhs) {
                return lhs.Controller.EvaluateComparison(lhs.ID, rhs.ID, CallKey.OpGTE) >= 0;
            }
        }
    }
}

#pragma warning restore CA2231    // Implement the equality operators
#pragma warning restore CS0660    // overrides operator== or operator== but does not override Object.Equals(object?)
#pragma warning restore CS0661    // overrides operator== or operator!= but does not override Object.GetHashCode()
