using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Schema;
using Optional;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

// A "base translation" is one that does not account for any annotations. The role of a "base translation" is to
// decompose a property, of _any_ category, into a uniform structure so that annotations can be applied without worrying
// about what kind of property the Fields came from. In the absence of any annotations whatsoever, the "base
// translation" will be 100% accurate.
//
// The code for dealing with most annotations can be found in the FieldAnnotations.cs file. The code for dealing with
// constraint annotations in particular can be found in the FieldConstraints.cs file.

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        private static FieldsListing ScalarBaseTranslation(PropertyInfo property) {
            Debug.Assert(property is not null);
            Debug.Assert(DBType.IsSupported(property.PropertyType));

            var descriptor = new FieldDescriptor(
                Name: property.Name,
                Nullability: property.GetNullability() == Nullability.NonNullable ? IsNullable.No : IsNullable.Yes,
                Column: Option.None<int>(),
                Converter: DataConverter.Identity(property.PropertyType),
                Default: Option.None<object?>(),
                InPrimaryKey: false,
                CandidateKeyMemberships: new HashSet<string>(),
                Constraints: new ConstraintBucket(
                    RelativeToZero: Option.None<ComparisonOperator>(),
                    LowerBound: Option.None<Bound>(),
                    UpperBound: Option.None<Bound>(),
                    MinimumLength: Option.None<Bound>(),
                    MaximumLength: Option.None<Bound>(),
                    AllowedValues: new HashSet<object>(),
                    DisallowedValues: new HashSet<object>(),
                    CHECKs: new List<CheckGen>()
                )
            );

            return new Dictionary<string, FieldDescriptor>() { { "", descriptor } };
        }
    }
}
