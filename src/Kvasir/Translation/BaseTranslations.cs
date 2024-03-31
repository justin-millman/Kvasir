using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Relations;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

// A "base translation" is one that does not account for any annotations. The role of a "base translation" is to
// decompose a property, of _any_ category, into a uniform structure so that annotations can be applied without worrying
// about what kind of property the Fields came from. In the absence of any annotations whatsoever, the "base
// translation" will be 100% accurate.
//
// The one exception to the above statement is nullability. The default nullability of a Field does not account for any
// annotations, obviously, but _also_ does not account for the native nullability of the source property. This is
// because of the particular way that nullability of an Aggregate property affects the nullability of the nested Fields.
// In essence, Kvasir treats the nullability marker of a type (i.e. the ?) as a kind of annotation.
//
// The code for dealing with most annotations can be found in the FieldAnnotations.cs file. The code for dealing with
// constraint annotations in particular can be found in the FieldConstraints.cs file.

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        private static TranslationState ScalarBaseTranslation(PropertyInfo property) {
            Debug.Assert(property is not null);
            Debug.Assert(DBType.IsSupported(property.PropertyType));
            Debug.Assert(DBType.Lookup(property.PropertyType) != DBType.Enumeration);

            var descriptor = new FieldDescriptor(
                AccessPath: "",
                Name: Enumerable.Repeat(PropertyName(property), 1).ToList(),
                Nullability: IsNullable.No,
                AbsoluteColumn: Option.None<int>(),
                RelativeColumn: 0,
                Converter: DataConverter.Identity(property.PropertyType),
                Default: Option.None<object?>(),
                InPrimaryKey: false,
                CandidateKeyMemberships: new HashSet<string>(),
                ForeignReference: Option.None<Type>(),
                Constraints: new ConstraintBucket(
                    RelativeToZero: Option.None<ComparisonOperator>(),
                    LowerBound: Option.None<Bound>(),
                    UpperBound: Option.None<Bound>(),
                    MinimumLength: Option.None<Bound>(),
                    MaximumLength: Option.None<Bound>(),
                    AllowedValues: new HashSet<object>(),
                    DisallowedValues: new HashSet<object>(),
                    RestrictedImage: new HashSet<object>(),
                    CHECKs: new List<CheckGen>()
                )
            );

            var fields = new Dictionary<string, FieldDescriptor>() { { "", descriptor } };
            var relations = new Dictionary<string, IRelationDescriptor>();
            return new TranslationState(Fields: fields, Relations: relations);
        }

        private static TranslationState EnumBaseTranslation(PropertyInfo property) {
            Debug.Assert(property is not null);
            Debug.Assert(DBType.Lookup(property.PropertyType) == DBType.Enumeration);
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            var descriptor = new FieldDescriptor(
                AccessPath: "",
                Name: Enumerable.Repeat(PropertyName(property), 1).ToList(),
                Nullability: IsNullable.No,
                AbsoluteColumn: Option.None<int>(),
                RelativeColumn: 0,
                Converter: DataConverter.Identity(property.PropertyType),
                Default: Option.None<object?>(),
                InPrimaryKey: false,
                CandidateKeyMemberships: new HashSet<string>(),
                ForeignReference: Option.None<Type>(),
                Constraints: new ConstraintBucket(
                    RelativeToZero: Option.None<ComparisonOperator>(),
                    LowerBound: Option.None<Bound>(),
                    UpperBound: Option.None<Bound>(),
                    MinimumLength: Option.None<Bound>(),
                    MaximumLength: Option.None<Bound>(),
                    AllowedValues: new HashSet<object>(),
                    DisallowedValues: new HashSet<object>(),
                    RestrictedImage: type.ValidValues().Cast<object>().ToHashSet(),
                    CHECKs: new List<CheckGen>()
                )
            );

            var fields = new Dictionary<string, FieldDescriptor>() { { "", descriptor } };
            var relations = new Dictionary<string, IRelationDescriptor>();
            return new TranslationState(Fields: fields, Relations: relations);
        }

        private TranslationState AggregateBaseTranslation(PropertyInfo property) {
            Debug.Assert(property is not null);
            Debug.Assert(!DBType.IsSupported(property.PropertyType) && property.PropertyType.IsValueType);

            var propertyName = PropertyName(property);
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var typeTranslation = TranslateType(type);

            var fields = new Dictionary<string, FieldDescriptor>();
            foreach (var (path, descriptor) in typeTranslation.Fields) {
                fields[path] = descriptor with {
                    Name = new List<string>(descriptor.Name).Prepend(propertyName).ToList()
                };
            }

            var relations = new Dictionary<string, IRelationDescriptor>();
            foreach (var (path, descriptor) in typeTranslation.Relations) {
                relations[path] = descriptor.WithName(new List<string>(descriptor.Name).Prepend(propertyName));
                if (property.ReflectedType!.IsClass) {
                    relations[path] = relations[path].WithEntity(property.ReflectedType);
                }
            }

            return new TranslationState(Fields: fields, Relations: relations);
        }

        private TranslationState ReferenceBaseTranslation(PropertyInfo property) {
            Debug.Assert(property is not null);
            Debug.Assert(!DBType.IsSupported(property.PropertyType) && property.PropertyType.IsClass);
            Debug.Assert(!property.PropertyType.IsInstanceOf(typeof(IRelation)));

            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var _ = TranslateEntity(type);
            var refKey = primaryKeyCache_[type].OrderBy(kvp => kvp.Value.RelativeColumn);

            var fields = new Dictionary<string, FieldDescriptor>();
            foreach ((int idx, var (path, descriptor)) in refKey.Select((kvp, idx) => (idx, kvp))) {
                fields[path] = descriptor with {
                    Name = new List<string>(descriptor.Name).Prepend(PropertyName(property)).ToList(),
                    AbsoluteColumn = Option.None<int>(),
                    RelativeColumn = idx,
                    Default = Option.None<object?>(),
                    InPrimaryKey = false,
                    CandidateKeyMemberships = new HashSet<string>(),
                    ForeignReference = Option.Some(type),
                    Constraints = new ConstraintBucket(
                        RelativeToZero: Option.None<ComparisonOperator>(),
                        LowerBound: Option.None<Bound>(),
                        UpperBound: Option.None<Bound>(),
                        MinimumLength: Option.None<Bound>(),
                        MaximumLength: Option.None<Bound>(),
                        AllowedValues: new HashSet<object>(),
                        DisallowedValues: new HashSet<object>(),
                        RestrictedImage: new HashSet<object>(descriptor.Constraints.RestrictedImage),
                        CHECKs: new List<CheckGen>()
                    )
                };
            }

            var relations = new Dictionary<string, IRelationDescriptor>();
            return new TranslationState(Fields: fields, Relations: relations);
        }

        private TranslationState RelationBaseTranslation(PropertyInfo property) {
            Debug.Assert(property is not null);
            Debug.Assert(property.PropertyType.IsInstanceOf(typeof(IRelation)) && property.PropertyType != typeof(IRelation));

            var relationType = property.PropertyType;
            var flags = BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic;
            var connectionProperty = relationType.GetProperties(flags)[0];
            var connectionType = (Type)connectionProperty.GetValue(null)!;
            var nullability = new NullabilityInfoContext().Create(property);
            var propertyName = PropertyName(property);

            IRelationDescriptor GetDescriptor() {
                if (relationType.Name.Contains("Map")) {
                    return new MapRelationDescriptor(propertyName, connectionType, nullability);
                }
                else if (relationType.Name.Contains("Ordered")) {
                    return new OrderedListRelationDescriptor(propertyName, connectionType, nullability);
                }
                else {
                    return new ListSetRelationDescriptor(propertyName, connectionType, nullability);
                }
            }

            var descriptor = GetDescriptor();
            if (property.ReflectedType!.IsClass) {
                descriptor = descriptor.WithEntity(property.ReflectedType);
            }

            var fields = new Dictionary<string, FieldDescriptor>() {};
            var relations = new Dictionary<string, IRelationDescriptor>() { { "", descriptor } };
            return new TranslationState(Fields: fields, Relations: relations);
        }

        private static string PropertyName(PropertyInfo property) {
            Debug.Assert(property is not null);

            // For almost all properties, the split operation will produce an array of size 1 because there will be no
            // dot character in the property's name. However, for explicit interface implementations, the arrays's size
            // will be at least two (and likely more due to full qualification).
            return property.Name.Split(PATH_SEPARATOR)[^1];
        }
    }
}
