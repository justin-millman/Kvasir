using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Kvasir.Translation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using static Kvasir.Translation.Extensions.TranslationExtensions;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        private IEnumerable<CheckGen> ConstraintsOn(PropertyInfo property, DataConverter converter) {
            foreach (var generator in ComparisonConstraintsOn(property, converter)) {
                yield return generator;
            }

            var signedness = SignednessConstraintOn(property, converter);
            if (signedness is not null) {
                yield return signedness;
            }

            var inclusion = InclusionConstraintOn(property, converter);
            if (inclusion is not null) {
                yield return inclusion;
            }

            foreach (var generator in LengthConstraintsOn(property, converter)) {
                yield return generator;
            }

            foreach (var generator in CustomConstraintsOn(property)) {
                yield return generator;
            }
        }

        private IEnumerable<ComplexCheckGen> ConstraintsOn(Type entity) {
            foreach (var annotation in entity.GetCustomAttributes<Check.ComplexAttribute>()) {
                // It is an error for a [Check.Complex] annotation to have a populated <UserError>
                if (annotation.UserError is not null) {
                    throw new KvasirException(
                        $"Error translating Entity Type {entity.Name}: " +
                        $"data provided to [Check.Complex] annotation is invalid ({annotation.UserError})"
                    );
                }

                // It is an error for a [Check.Complex] annotation to have zero constituent Fields listed
                if (annotation.FieldNames.IsEmpty()) {
                    throw new KvasirException(
                        $"Error translating Entity Type {entity.Name}: " +
                        $"[Check.Complex] annotation must have at least one constituent Field"
                    );
                }

                yield return (fs, cs) => {
                    var zip = fs.Zip(cs);
                    var results = new List<(IField, DataConverter)>();
                    foreach (var name in annotation.FieldNames) {
                        var found = zip.FirstOrDefault(p => p.First.Name == name);
                        
                        // It is an error for any Field specified for a [Check.Complex] annotation to not exist
                        if (found == default) {
                            throw new KvasirException(
                                $"Error translating Entity Type {entity.Name}: " +
                                $"Field \"{name}\" required by [Check.Complex] annotation does not exist " +
                                $"(available = {string.Join(", ", fs.Select(f => f.Name))})"
                            );
                        }
                        results.Add(found);
                    }

                    var generator = annotation.ConstraintGenerator;
                    return generator.MakeConstraint(results.Select(p => p.Item1), results.Select(p => p.Item2), settings_);
                };
            }
        }

        private static CheckGen? SignednessConstraintOn(PropertyInfo property, DataConverter converter) {
            var dataType = DBType.Lookup(converter.ResultType);
            var numeric = dataType.NumericStyle();

            CheckGen? MarkedWith<TAnnotation>() where TAnnotation : Check.ComparisonAttribute {
                var annotation = property.Only<TAnnotation>();
                var variety = typeof(TAnnotation).Name[0..^9];

                // If there is no annotation, then there is no Signedness Constraint
                if (annotation is null) {
                    return null;
                }

                // It is an error for any Signedness annotation on a scalar property to have a non-empty <Path> property
                if (annotation.Path != "") {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"path \"{annotation.Path}\" of [Check.{variety}] annotation does not exist"
                    );
                }

                // It is an error for any Signedness annotation to be placed on a property whose post-conversion data
                // type is not numeric
                if (numeric == NumericKind.None) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"[Check.{variety}] annotation can only be applied to a Field whose data type is numeric, " +
                        $"and {converter.ResultType.Name} is not numeric"
                    );
                }

                // No errors detected
                var zero = DBValue.Create(dataType.Zero());
                return (f, _) => new ConstantClause(new FieldExpression(f), annotation.Operator, zero);
            }

            var positive = MarkedWith<Check.IsPositiveAttribute>();
            var negative = MarkedWith<Check.IsNegativeAttribute>();
            var nonzero = MarkedWith<Check.IsNonZeroAttribute>();

            // It is an error for a Field whose post-conversion type is an unsigned integer to be annotated with
            // [Check.IsNegative]
            if (negative is not null && numeric == NumericKind.Unsigned) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Check.IsNegative] can never be satisfied by a Field whose data type is an unsigned integer " +
                    $"({converter.ResultType.Name})"
                );
            }

            // It is an error for a Field to be annotated with two or more different Signedness constraints
            if (positive is not null && negative is not null) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Check.IsPositive] and [Check.IsNegative] are mutually exclusive"
                );
            }
            if (positive is not null && nonzero is not null) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Check.IsPositive] and [Check.IsNonZero] are mutually exclusive " +
                    "(the former implies the latter)"
                );
            }
            if (negative is not null && nonzero is not null) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Check.IsNegative] and [Check.IsNonZero] are mutually exclusive " +
                    "(the former implies the latter"
                );
            }

            // No errors detected
            return positive ?? negative ?? nonzero;
        }

        private static IEnumerable<CheckGen> ComparisonConstraintsOn(PropertyInfo property, DataConverter converter) {
            var dataType = DBType.Lookup(converter.ResultType);
            var isOrderable = dataType.IsOrderable();

            CheckGen? MarkedWith<TAnnotation>() where TAnnotation : Check.ComparisonAttribute {
                var annotation = property.Only<TAnnotation>();
                var variety = typeof(TAnnotation).Name[0..^9];

                // If there is no annotation, then there is no Ranged Comparison Constraint
                if (annotation is null) {
                    return null;
                }

                // It is an error for any Ranged Comparison annotation on a scalar property to have a non-empty <Path>
                // property
                if (annotation.Path != "") {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"path \"{annotation.Path}\" of [Check.{variety}] annotation does not exist"
                    );
                }

                // It is an error for any Ranged Comparison annotation to be placed on a property whose post-conversion
                // data type is not orderable, but [IsNot] can be applied to anything
                if (!isOrderable && annotation.Operator != ComparisonOperator.NE) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"[Check.{variety}] annotation can only be applied to a Field whose data type supports " +
                        $"a total ordering, and {converter.ResultType.Name} does not"
                    );
                }

                // It is an error for the anchor of any Ranged Comparison annotation to be null
                if (annotation.Anchor is null || annotation.Anchor.Equals(DBNull.Value)) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"[Check.{variety}] bound cannot be 'null'"
                    );
                }

                // It is an error for a LT or GTE Ranged Comparison to be anchored at the minimum possible value (for
                // numeric types)
                if (annotation.Operator == ComparisonOperator.LT || annotation.Operator == ComparisonOperator.GTE) {
                    var minima = new Dictionary<Type, object>() {
                        { typeof(sbyte), sbyte.MinValue },
                        { typeof(short), short.MinValue },
                        { typeof(int), int.MinValue },
                        { typeof(long), long.MinValue },
                        { typeof(byte), byte.MinValue },
                        { typeof(ushort), ushort.MinValue },
                        { typeof(uint), uint.MinValue },
                        { typeof(ulong), ulong.MinValue },
                        { typeof(decimal), decimal.MinValue },
                        { typeof(double), double.MinValue },
                        { typeof(float), float.MinValue }
                    };

                    if (minima.ContainsKey(converter.ResultType) && annotation.Anchor.Equals(minima[converter.ResultType])) {
                        throw new KvasirException(
                            $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                            $"[Check.{variety}] bound {annotation.Anchor} is the minimum value possible for " +
                            $"{converter.ResultType.Name}, which is redundant"
                        );
                    }
                }

                // It is an error for a GT or LTE Ranged Comparison to be anchored at the maximum possible value (for
                // numeric types)
                if (annotation.Operator == ComparisonOperator.GT || annotation.Operator == ComparisonOperator.LTE) {
                    var maxima = new Dictionary<Type, object>() {
                        { typeof(sbyte), sbyte.MaxValue },
                        { typeof(short), short.MaxValue },
                        { typeof(int), int.MaxValue },
                        { typeof(long), long.MaxValue },
                        { typeof(byte), byte.MaxValue },
                        { typeof(ushort), ushort.MaxValue },
                        { typeof(uint), uint.MaxValue },
                        { typeof(ulong), ulong.MaxValue },
                        { typeof(decimal), decimal.MaxValue },
                        { typeof(double), double.MaxValue },
                        { typeof(float), float.MaxValue  }
                    };

                    if (maxima.ContainsKey(converter.ResultType) && annotation.Anchor.Equals(maxima[converter.ResultType])) {
                        throw new KvasirException(
                            $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                            $"[Check.{variety}] bound {annotation.Anchor} is the maximum value possible for " +
                            $"{converter.ResultType.Name}, which is redundant"
                        );
                    }
                }

                // No errors detected - the nullability is irrelevant
                var bound = annotation.Anchor.ParseFor(property, converter.ResultType, IsNullable.Yes, $"[Check.{variety}] bound");
                var op = annotation.Operator;
                return (f, _) => new ConstantClause(new FieldExpression(f), op, DBValue.Create(bound));
            }

            var ne = MarkedWith<Check.IsNotAttribute>();
            if (ne is not null) {
                yield return ne;
            }

            var gt = MarkedWith<Check.IsGreaterThanAttribute>();
            if (gt is not null) {
                yield return gt;
            }

            var gte = MarkedWith<Check.IsGreaterOrEqualToAttribute>();
            if (gte is not null) {
                yield return gte;
            }

            var lt = MarkedWith<Check.IsLessThanAttribute>();
            if (lt is not null) {
                yield return lt;
            }

            var lte = MarkedWith<Check.IsLessOrEqualToAttribute>();
            if (lte is not null) {
                yield return lte;
            }
        }

        private static CheckGen? InclusionConstraintOn(PropertyInfo property, DataConverter converter) {
            CheckGen? MarkedWith<TAnnotation>() where TAnnotation : Check.InclusionAttribute {
                var annotation = property.Only<TAnnotation>();
                var variety = typeof(TAnnotation).Name[0..^9];

                // If there is no annotation, then there is no Inclusion Constraint
                if (annotation is null) {
                    return null;
                }

                // It is an error for any Inclusion annotation on a scalar property to have a non-empty <Path> property
                if (annotation.Path != "") {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"path \"{annotation.Path}\" of [Check.{variety}] annotation does not exist"
                    );
                }

                // It is an error for any value specified in an Inclusion annotation to be null
                var anchor = new HashSet<DBValue>();
                foreach (var item in annotation.Anchor) {
                    if (item is null || item.Equals(DBNull.Value)) {
                        throw new KvasirException(
                            $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                            $"'null' cannot be listed in values for [Check.{variety}]"
                        );
                    }

                    // The nullability here is irrelevant
                    var raw = item.ParseFor(property, converter.ResultType, IsNullable.Yes, $"[Check.{variety}] value");
                    anchor.Add(DBValue.Create(raw));
                }

                // No errors detected
                if (anchor.Count == 1) {
                    var op = annotation.Operator == InclusionOperator.In ? ComparisonOperator.EQ : ComparisonOperator.NE;
                    return (f, _) => new ConstantClause(new FieldExpression(f), op, anchor.Single());
                }
                return (f, _) => new InclusionClause(new FieldExpression(f), annotation.Operator, anchor);
            }

            var include = MarkedWith<Check.IsOneOfAttribute>();
            var exclude = MarkedWith<Check.IsNotOneOfAttribute>();

            // It is an error for a Field to be annotated with both [Check.IsOneOf] and [Check.IsNotOneOf]
            if (include is not null && exclude is not null) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Check.IsOneOf] and [Check.IsNotOneOf] are mutually exclusive"
                );
            }

            // No errors detected
            return include ?? exclude;
        }

        private static IEnumerable<CheckGen> LengthConstraintsOn(PropertyInfo property, DataConverter converter) {
            var isText = DBType.Lookup(converter.ResultType) == DBType.Text;

            TAnnotation? MarkedWith<TAnnotation>() where TAnnotation : Check.StringLengthAttribute {
                var annotation = property.Only<TAnnotation>();
                var variety = typeof(TAnnotation).Name[0..^9];

                // If there is no annotation, then there is no String Length Constraint
                if (annotation is null) {
                    return null;
                }

                // It is an error for any String Length annotation on a scalar property to have a non-empty <Path>
                // property
                if (annotation.Path != "") {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"path \"{annotation.Path}\" of [Check.{variety}] annotation does not exist"
                    );
                }

                // It is an error for any String Length annotation to be placed on a property whose post-conversion data
                // type is not textual
                if (!isText) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"[Check.{variety}] annotation can only be applied to a Field of type {nameof(String)} " +
                        $"(got Field of type {converter.ResultType.Name})"
                    );
                }

                // It is an error for the lower bound imposed by a String Length Constraint to not be positive
                if (annotation.Minimum != long.MinValue && annotation.Minimum <= 0) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"minimum string length of {annotation.Minimum} imposted by [Check.{variety}] is invalid " +
                        "(value must be at least 1)"
                    );
                }

                // It is an error for the upper bound imposed by a String Length Constraint to be negative (it can be 0)
                if (annotation.Maximum < 0) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"maximum string length of {annotation.Maximum} imposed by [Check.{variety}] is invalid " +
                        "(value must be at least 0)"
                    );
                }

                // It is an error for the upper bound imposed by a String Length Constraint to be strictly less than the
                // lower bound
                if (annotation.Maximum != long.MinValue && annotation.Maximum != long.MaxValue) {
                    if (annotation.Maximum < annotation.Minimum) {
                        throw new KvasirException(
                            $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                            $"maximum string length {annotation.Maximum} imposed by [Check.{variety}] " +
                            $"cannot be less than minimum string length {annotation.Minimum}"
                        );
                    }
                }

                // No errors detected
                return annotation;
            }

            var nonempty = MarkedWith<Check.IsNonEmptyAttribute>();
            var minimum = MarkedWith<Check.LengthIsAtLeastAttribute>();
            var maximum = MarkedWith<Check.LengthIsAtMostAttribute>();
            var between = MarkedWith<Check.LengthIsBetweenAttribute>();

            // It is an error for a Field to be annotated with two or more different String Length Constraints that
            // impose a limit on the same extremum
            if (nonempty is not null && minimum is not null) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Check.LengthIsAtLeast] and [Check.IsNonEmpty] are mutually exclusive"
                );
            }
            if (nonempty is not null && between is not null) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Check.LengthIsBetween] and [Check.IsNonEmpty] are mutually exclusive"
                );
            }
            if (minimum is not null && between is not null) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Check.LengthIsAtLeast] and [Check.LengthIsBetween] are mutually exclusive"
                );
            }
            if (maximum is not null && between is not null) {
                throw new KvasirException(
                    $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                    "[Check.LengthIsAtMost] and [Check.LengthIsBetween] are mutually exclusive"
                );
            }

            // No errors detected
            if (nonempty is not null) {
                yield return
                    (f, _) => new ConstantClause(
                        new FieldExpression(FieldFunction.LengthOf, f),
                        ComparisonOperator.GT,
                        DBValue.Create(0));
            }
            if (minimum is not null) {
                yield return
                    (f, _) => new ConstantClause(
                        new FieldExpression(FieldFunction.LengthOf, f),
                        ComparisonOperator.GTE,
                        DBValue.Create((int)minimum.Minimum));
            }
            if (maximum is not null) {
                if (maximum.Maximum == 0L) {
                    yield return
                        (f, _) => new ConstantClause(
                            new FieldExpression(FieldFunction.LengthOf, f),
                            ComparisonOperator.EQ,
                            DBValue.Create(0));
                }
                else {
                    yield return
                        (f, _) => new ConstantClause(
                            new FieldExpression(FieldFunction.LengthOf, f),
                            ComparisonOperator.LTE,
                            DBValue.Create((int)maximum.Maximum));
                }
            }
            if (between is not null) {
                if (between.Minimum == between.Maximum) {
                    yield return
                        (f, _) => new ConstantClause(
                            new FieldExpression(FieldFunction.LengthOf, f),
                            ComparisonOperator.EQ,
                            DBValue.Create((int)between.Minimum));
                }
                else {
                    yield return
                        (f, _) => new ConstantClause(
                            new FieldExpression(FieldFunction.LengthOf, f),
                            ComparisonOperator.GTE,
                            DBValue.Create((int)between.Minimum));
                    yield return
                        (f, _) => new ConstantClause(
                            new FieldExpression(FieldFunction.LengthOf, f),
                            ComparisonOperator.LTE,
                            DBValue.Create((int)between.Maximum));
                }
            }
        }

        private IEnumerable<CheckGen> CustomConstraintsOn(PropertyInfo property) {
            foreach (var annotation in property.GetCustomAttributes<CheckAttribute>()){
                // It is an error for a [Check] annotation on a scalar property to have a non-empty <Path> value
                if (annotation.Path != "") {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"path \"{annotation.Path}\" of [Check] annotation does not exist"
                    );
                }

                // It is an error for a [Check] annotation to have a populated <UserError>
                if (annotation.UserError is not null) {
                    throw new KvasirException(
                        $"Error translating property {property.Name} of type {property.ReflectedType!.Name}: " +
                        $"data provided to [Check] annotation is invalid ({annotation.UserError})"
                    );
                }

                var generator = annotation.ConstraintGenerator;
                yield return (f, c) => generator.MakeConstraint(new[] { f }, new[] { c }, settings_);
            }
        }
    }
}

