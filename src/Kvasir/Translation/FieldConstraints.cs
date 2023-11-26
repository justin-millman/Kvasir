using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

// This code takes a "base translation" that has undergone basic annotation analysis and applies various constraints.
// Generally speaking, it is not necessary to know the initial category of a translation in order to apply constraints.
// However, certain constraints are only applicable to Fields of certain shapes (e.g. signedness constraints must be
// applied to totally-orderable Fields). If there are no constraint annotations, the incoming translation is 100%
// accurate. Note that to support Aggregate properties, where constraints may be compounded, the overall set of
// constraints is not simplified until the final stage where the Entity's data model is being created.
//
// The code for producing the original "base translations" can be found in the BaseTranslations.cs file. The code for
// applying initial structural annotations can be found in the FieldAnnotations.cs file.

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        private void ProcessConstraints(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            ApplySignednessConstraints(property, state);
            ApplyComparisonConstraints(property, state);
            ApplyStringLengthConstraints(property, state);
            ApplyDiscretenessConstraints(property, state);
            ApplyCustomConstraints(property, state);
            ResolveConflictingConstraints(property, state);         // must after everything except Custom constraints
            EnsureViableDefaults(property, state);                  // must after everything except Custom constraints
        }

        private static void ApplySignednessConstraints(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            foreach (var annotation in property.GetCustomAttributes<Check.SignednessAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a Signedness attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a Signedness attribute to have a non-existent Path
                if (!state.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a Signedness attribute to be applied to a non-numeric Field
                var dbtype = DBType.Lookup(target.Converter.ResultType);
                if (!dbtype.IsNumeric()) {
                    var msg = $"type {target.Converter.ResultType} is not numeric";
                    throw Error.InapplicableConstraint(context, annotation, msg);
                }

                // It is an error for a [Check.IsNegative] attribute to be applied to an unsigned numeric type
                if (annotation.Operator == ComparisonOperator.LT && dbtype.IsUnsignedNumeric()) {
                    var msg = $"type {target.Converter.ResultType} is unsigned";
                    throw Error.InapplicableConstraint(context, annotation, msg);
                }

                // It is an error for a property to be annotated as both [Check.IsPositive] and [Check.IsNegative]
                bool already = target.Constraints.RelativeToZero.Exists(v => v != ComparisonOperator.NE);
                bool now = annotation.Operator != ComparisonOperator.NE;
                if (already && now && target.Constraints.RelativeToZero.Exists(v => v != annotation.Operator)) {
                    throw Error.MutuallyExclusive(context, new Check.IsPositiveAttribute(), new Check.IsNegativeAttribute());
                }

                // No errors encountered
                if (!already) {
                    var original = state[annotation.Path].Constraints;
                    var type = Nullable.GetUnderlyingType(target.Converter.ResultType) ?? target.Converter.ResultType;
                    var zero = Convert.ChangeType(0, type);

                    state[annotation.Path] = state[annotation.Path] with {
                        Constraints = original with {
                            RelativeToZero = Option.Some(annotation.Operator)
                        }
                    };
                    original = state[annotation.Path].Constraints;

                    // Signedness Constraints are shorthand for other constraints relative to the numeric value 0. We
                    // keep track of the specific operator solely for the purpose of identifying mutually exclusive
                    // [Check.IsPositive] and [Check.IsNegative] Constraints, but we also convert to the equivalent
                    // "standard" constraint.
                    if (annotation.Operator == ComparisonOperator.NE) {
                        var disallowed = new HashSet<object>(original.DisallowedValues) { zero };
                        state[annotation.Path] = state[annotation.Path] with {
                            Constraints = original with {
                                DisallowedValues = disallowed
                            }
                        };
                    }
                    else if (annotation.Operator == ComparisonOperator.GT) {
                        var bound = new Bound(zero, false);
                        state[annotation.Path] = state[annotation.Path] with {
                            Constraints = original with {
                                LowerBound = Option.Some(MaxLowerBound(original.LowerBound, bound))
                            }
                        };
                    }
                    else {
                        Debug.Assert(annotation.Operator == ComparisonOperator.LT);
                        var bound = new Bound(zero, false);
                        state[annotation.Path] = state[annotation.Path] with {
                            Constraints = original with {
                                UpperBound = Option.Some(MinUpperBound(original.LowerBound, bound))
                            }
                        };
                    }
                }
            }
        }

        private static void ApplyComparisonConstraints(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            foreach (var annotation in property.GetCustomAttributes<Check.ComparisonAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a Comparison attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a Comparison attribute to have a non-existent Path
                if (!state.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a Comparison attribute to be applied to a non-orderable Field if the operator is
                // ranged (and not NE)
                if (annotation.Operator != ComparisonOperator.NE) {
                    if (!DBType.Lookup(target.Converter.ResultType).IsTotallyOrdered()) {
                        var msg = $"type {target.Converter.ResultType} is not totally ordered";
                        throw Error.InapplicableConstraint(context, annotation, msg);
                    }
                }

                // It is an error for the value of a Comparison attribute to be null or to be incompatible with the
                // expected data type
                if (annotation.Anchor == DBNull.Value) {
                    throw Error.BadValue(context, annotation, annotation.Anchor, "cannot be null");
                }
                var value = annotation.Anchor.ParseFor(target.Converter.ResultType, target.Nullability == IsNullable.Yes);
                value.MatchNone(reason => throw Error.BadValue(context, annotation, annotation.Anchor, reason));

                // It is an error for a [Check.IsLessThan] attribute to have an anchor that is the minimum value
                if (annotation.Operator == ComparisonOperator.LT) {
                    if (MINIMA.TryGetValue(target.Converter.ResultType, out object? min) && value.Contains(min)) {
                        var type = target.Converter.ResultType.Name;
                        var msg = $"exclusive upper bound cannot be the minimum value {min.ForDisplay()} for {type}";
                        throw Error.UnsatisfiableConstraint(context, annotation, msg);
                    }
                }

                // It is an error for a [Check.IsGreaterThan] attribute to have an anchor that is the maximum value
                if (annotation.Operator == ComparisonOperator.GT) {
                    if (MAXIMA.TryGetValue(target.Converter.ResultType, out object? max) && value.Contains(max)) {
                        var type = target.Converter.ResultType.Name;
                        var msg = $"exclusive lower bound cannot be the maximum value {max.ForDisplay()} for {type}";
                        throw Error.UnsatisfiableConstraint(context, annotation, msg);
                    }
                }

                // No errors encountered - process exclusion comparison
                if (annotation.Operator == ComparisonOperator.NE) {
                    state[annotation.Path] = state[annotation.Path] with {
                        Constraints = state[annotation.Path].Constraints with {
                            DisallowedValues = new HashSet<object>(
                                state[annotation.Path].Constraints.DisallowedValues
                            ) { value.WithoutException().Unwrap()! }
                        }
                    };
                }

                // No errors encountered - process lower bounds
                if (annotation.Operator == ComparisonOperator.GT || annotation.Operator == ComparisonOperator.GTE) {
                    bool inclusive = (annotation.Operator == ComparisonOperator.GTE);
                    var bound = new Bound(value.WithoutException().Unwrap()!, inclusive);

                    state[annotation.Path] = state[annotation.Path] with {
                        Constraints = state[annotation.Path].Constraints with {
                            LowerBound = Option.Some(MaxLowerBound(state[annotation.Path].Constraints.LowerBound, bound))
                        }
                    };
                }

                // No errors encountered - process upper bounds
                if (annotation.Operator == ComparisonOperator.LT || annotation.Operator == ComparisonOperator.LTE) {
                    bool inclusive = (annotation.Operator == ComparisonOperator.LTE);
                    var bound = new Bound(value.WithoutException().Unwrap()!, inclusive);

                    state[annotation.Path] = state[annotation.Path] with {
                        Constraints = state[annotation.Path].Constraints with {
                            UpperBound = Option.Some(MinUpperBound(state[annotation.Path].Constraints.UpperBound, bound))
                        }
                    };
                }
            }
        }

        private static void ApplyStringLengthConstraints(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            foreach (var annotation in property.GetCustomAttributes<Check.StringLengthAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a String Length attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a String Length attribute to have a non-existent Path
                if (!state.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a String Length attribute to be applied to a non-string type
                if (DBType.Lookup(target.Converter.ResultType) != DBType.Text) {
                    var msg = $"type {target.Converter.ResultType} is not {nameof(String)}";
                    throw Error.InapplicableConstraint(context, annotation, msg);
                }

                // It is an error for the bound on a String Length attribute to be negative
                if (annotation.Minimum != long.MinValue && annotation.Minimum < 0) {
                    var msg = $"minimum length of {annotation.Minimum} cannot be negative";
                    throw Error.UserError(context, annotation, msg);
                }
                if (annotation.Maximum < 0) {
                    var msg = $"maximum length of {annotation.Maximum} cannot be negative";
                    throw Error.UserError(context, annotation, msg);
                }

                // No errors encountered - process the minimum
                if (annotation.Minimum != long.MinValue) {
                    var bound = new Bound((int)annotation.Minimum, true);
                    state[annotation.Path] = state[annotation.Path] with {
                        Constraints = state[annotation.Path].Constraints with {
                            MinimumLength = Option.Some(MaxLowerBound(state[annotation.Path].Constraints.MinimumLength, bound))
                        }
                    };
                }

                // No errors encountered - process the maximum
                if (annotation.Maximum != long.MaxValue) {
                    var bound = new Bound((int)annotation.Maximum, true);
                    state[annotation.Path] = state[annotation.Path] with {
                        Constraints = state[annotation.Path].Constraints with {
                            MaximumLength = Option.Some(MinUpperBound(state[annotation.Path].Constraints.MaximumLength, bound))
                        }
                    };
                }
            }
        }

        private static void ApplyDiscretenessConstraints(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            foreach (var annotation in property.GetCustomAttributes<Check.InclusionAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a Discreteness attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a Discreteness attribute to have a non-existent Path
                if (!state.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for any value of a Discreteness attribute to be null or to be incompatible with the
                // expected data type
                var values = new HashSet<object>();
                foreach (var element in annotation.Anchor) {
                    if (element == DBNull.Value) {
                        throw Error.BadValue(context, annotation, element, "elements cannot be null");
                    }

                    element.ParseFor(target.Converter.ResultType, target.Nullability == IsNullable.Yes).Match(
                        some: v => values.Add(v!),
                        none: r => throw Error.BadValue(context, annotation, element, r)
                    );
                }

                // No errors encountered
                if (annotation.Operator == InclusionOperator.In) {
                    values.UnionWith(state[annotation.Path].Constraints.AllowedValues);
                    state[annotation.Path] = state[annotation.Path] with {
                        Constraints = state[annotation.Path].Constraints with {
                            AllowedValues = values
                        }
                    };
                }
                else {
                    Debug.Assert(annotation.Operator == InclusionOperator.NotIn);
                    values.UnionWith(state[annotation.Path].Constraints.DisallowedValues);
                    state[annotation.Path] = state[annotation.Path] with {
                        Constraints = state[annotation.Path].Constraints with {
                            DisallowedValues = values
                        }
                    };
                }
            }
        }

        private void ApplyCustomConstraints(PropertyInfo property, Dictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            foreach (var annotation in property.GetCustomAttributes<CheckAttribute>()) {
                var context = new PropertyTranslationContext(property, annotation.Path);

                // It is an error for a [Check] attribute to have a null Path
                if (annotation.Path is null) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a [Check] attribute to have a non-existent Path
                if (!state.TryGetValue(annotation.Path, out FieldDescriptor target)) {
                    throw Error.InvalidPath(context, annotation);
                }

                // It is an error for a [Check] annotation to have a populated <UserError>
                if (annotation.UserError is not null) {
                    throw Error.UserError(context, annotation, annotation.UserError);
                }

                // No errors encountered
                CheckGen gen = (f, dc) => {
                    try {
                        return annotation.ConstraintGenerator.MakeConstraint(
                            Enumerable.Repeat(f, 1),
                            Enumerable.Repeat(dc, 1),
                            settings_
                        );
                    }
                    catch (Exception ex) {
                        var msg = $"unable to create custom constraint ({ex.Message})";
                        throw Error.UserError(context, annotation, msg);
                    }
                };
                state[annotation.Path] = state[annotation.Path] with {
                    Constraints = state[annotation.Path].Constraints with {
                        CHECKs = new List<CheckGen>(state[annotation.Path].Constraints.CHECKs) { gen }
                    }
                };
            }
        }

        private static void ResolveConflictingConstraints(PropertyInfo property, IReadOnlyDictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            // We only want to check that the complete set of constraints are internally consistent. We aren't going to
            // make any changes to the constraints (e.g. eliminating allowed values that don't satisfy the other
            // constraints) because that might remove information that leads to incorrect translations later. For
            // example: if an allowed value is also disallowed and it is removed from the former while the latter is
            // cleared for being redundant, a future annotation on an aggregate may specify that the value is allowed,
            // which would not be the case.

            foreach ((var path, var descriptor) in state) {
                var context = new PropertyTranslationContext(property, path);
                var constraints = descriptor.Constraints;

                // It is an error for the comparison constraints to form an invalid interval, i.e. one that is empty
                if (!IsValidInterval(constraints.LowerBound, constraints.UpperBound)) {
                    var interval = ToString(constraints.LowerBound, constraints.UpperBound);
                    var msg = $"range {interval} of valid values is empty";
                    throw Error.ConstraintsInConflict(context, msg);
                }

                // It is an error for the string length constraints to form an invalid interval, i.e. one that is empty
                if (!IsValidInterval(constraints.MinimumLength, constraints.MaximumLength)) {
                    var interval = ToString(constraints.MinimumLength, constraints.MaximumLength);
                    var msg = $"range {interval} of valid string lengths is empty";
                    throw Error.ConstraintsInConflict(context, msg);
                }

                // It is an error for all of the allowed values to be disallowed by other constraints
                var inclusions = new HashSet<object>();
                var effective = constraints.AllowedValues.IsEmpty() ? constraints.RestrictedImage : constraints.AllowedValues;
                var hadInclusions = !effective.IsEmpty();
                foreach (var inclusion in effective) {
                    if (constraints.DisallowedValues.Contains(inclusion)) {
                        continue;
                    }
                    if (!constraints.RestrictedImage.IsEmpty() && !constraints.RestrictedImage.Contains(inclusion)) {
                        continue;
                    }

                    if (IsWithinInterval(inclusion, constraints.LowerBound, constraints.UpperBound)) {
                        if (inclusion is string str) {
                            if (IsWithinInterval(str.Length, constraints.MinimumLength, constraints.MaximumLength)) {
                                inclusions.Add(inclusion);
                            }
                        }
                        else {
                            inclusions.Add(inclusion);
                        }
                    }
                }
                if (hadInclusions && inclusions.IsEmpty()) {
                    var allowed = effective.ToArray().ForDisplay();
                    var msg = $"each of the allowed values {allowed} is disallowed by another constraint";
                    throw Error.ConstraintsInConflict(context, msg);
                }

                // It is an error for both Boolean values to be disallowed
                if (constraints.DisallowedValues.Contains(true) && constraints.DisallowedValues.Contains(false)) {
                    var msg = "no available values (both true and false explicitly disallowed)";
                    throw Error.ConstraintsInConflict(context, msg);
                }

                // It is an error for the single value allowed by a ranged Comparison Constraint to be disallowed
                if (constraints.LowerBound.Exists(lb => constraints.UpperBound.Exists(ub => lb == ub))) {
                    if (constraints.DisallowedValues.Contains(constraints.LowerBound.Unwrap().Value)) {
                        var allowed = (new object[] { constraints.LowerBound.Unwrap().Value }).ForDisplay();
                        var msg = $"each of the allowed values {allowed} is disallowed by another constraint";
                        throw Error.ConstraintsInConflict(context, msg);
                    }
                }
            }
        }

        private static void EnsureViableDefaults(PropertyInfo property, IReadOnlyDictionary<string, FieldDescriptor> state) {
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            foreach ((var path, var descriptor) in state) {
                if (!descriptor.Default.HasValue || descriptor.Default.Contains(null)) {
                    continue;
                }

                var context = new PropertyTranslationContext(property, path);
                var defaultValue = descriptor.Default.Unwrap()!;
                var constraints = descriptor.Constraints;

                // Is an error for the default value to not be one of the allowed values, if there are such values
                if (!constraints.AllowedValues.IsEmpty() && !constraints.AllowedValues.Contains(defaultValue)) {
                    var values = constraints.AllowedValues.ToArray();
                    var msg = $"allowed values are {values.ForDisplay()}";
                    throw Error.DefaultFailsConstraints(context, defaultValue, msg);
                }

                // It is an error for the default value to be a disallowed value, if there are such values
                if (constraints.DisallowedValues.Contains(defaultValue)) {
                    throw Error.DefaultFailsConstraints(context, defaultValue, "value is explicitly disallowed");
                }

                // It is an error for the default value to be outside the allowed range
                if (!IsWithinInterval(defaultValue, constraints.LowerBound, constraints.UpperBound)) {
                    var interval = ToString(constraints.LowerBound, constraints.UpperBound);
                    var msg = $"value is not in interval {interval}";
                    throw Error.DefaultFailsConstraints(context, defaultValue, msg);
                }

                // It is an error for the length of the default value to be outside the allowed range
                if (defaultValue is string str) {
                    if (!IsWithinInterval(str.Length, constraints.MinimumLength, constraints.MaximumLength)) {
                        var lowerBound = constraints.MinimumLength.Or(new Bound(0, true));
                        var interval = ToString(lowerBound, constraints.MaximumLength);
                        var msg = $"length is {str.Length}, which is not in interval {interval}";
                        throw Error.DefaultFailsConstraints(context, defaultValue, msg);
                    }
                }
            }
        }


        private static readonly Dictionary<Type, object> MINIMA = new Dictionary<Type, object>{
            { typeof(byte), byte.MinValue },
            { typeof(sbyte), sbyte.MinValue },
            { typeof(ushort), ushort.MinValue },
            { typeof(short), short.MinValue },
            { typeof(uint), uint.MinValue },
            { typeof(int), int.MinValue },
            { typeof(ulong), ulong.MinValue },
            { typeof(long), long.MinValue }
        };
        private static readonly Dictionary<Type, object> MAXIMA = new Dictionary<Type, object>{
            { typeof(byte), byte.MaxValue },
            { typeof(sbyte), sbyte.MaxValue },
            { typeof(ushort), ushort.MaxValue },
            { typeof(short), short.MaxValue },
            { typeof(uint), uint.MaxValue },
            { typeof(int), int.MaxValue },
            { typeof(ulong), ulong.MaxValue },
            { typeof(long), long.MaxValue }
        };
    }
}
