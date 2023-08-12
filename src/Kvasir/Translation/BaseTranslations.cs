using Cybele.Core;
using Cybele.Extensions;
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
        private static FieldsListing ScalarBaseTranslation(PropertyInfo property) {
            Debug.Assert(property is not null);
            Debug.Assert(DBType.IsSupported(property.PropertyType));
            Debug.Assert(DBType.Lookup(property.PropertyType) != DBType.Enumeration);

            var descriptor = new FieldDescriptor(
                AccessPath: "",
                Name: Enumerable.Repeat(property.Name, 1).ToList(),
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

            return new Dictionary<string, FieldDescriptor>() { { "", descriptor } };
        }

        private static FieldsListing EnumBaseTranslation(PropertyInfo property) {
            Debug.Assert(property is not null);
            Debug.Assert(DBType.Lookup(property.PropertyType) == DBType.Enumeration);
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            var descriptor = new FieldDescriptor(
                AccessPath: "",
                Name: Enumerable.Repeat(property.Name, 1).ToList(),
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

            return new Dictionary<string, FieldDescriptor>() { { "", descriptor } };
        }

        private FieldsListing AggregateBaseTranslation(PropertyInfo property) {
            Debug.Assert(property is not null);
            Debug.Assert(!DBType.IsSupported(property.PropertyType) && property.PropertyType.IsValueType);

            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var typeTranslation = TranslateType(type);

            var result = new Dictionary<string, FieldDescriptor>();
            foreach (var (path, descriptor) in typeTranslation.Fields) {
                result[path] = descriptor with {
                    Name = new List<string>(descriptor.Name).Prepend(property.Name).ToList()
                };
            }
            return result;
        }

        private FieldsListing ReferenceBaseTranslation(PropertyInfo property) {
            Debug.Assert(property is not null);
            Debug.Assert(!DBType.IsSupported(property.PropertyType) && property.PropertyType.IsClass);

            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var _ = TranslateEntity(type);
            var refKey = primaryKeyCache_[type].OrderBy(kvp => kvp.Value.RelativeColumn);

            var result = new Dictionary<string, FieldDescriptor>();
            foreach ((int idx, var (path, descriptor)) in refKey.Select((kvp, idx) => (idx, kvp))) {
                result[path] = descriptor with {
                    Name = new List<string>(descriptor.Name).Prepend(property.Name).ToList(),
                    AbsoluteColumn = Option.None<int>(),
                    RelativeColumn = idx,
                    Default = Option.None<object?>(),
                    InPrimaryKey = false,
                    CandidateKeyMemberships = new HashSet<string>(),
                    ForeignReference = Option.Some(type)
                };
            }
            return result;
        }
    }
}
