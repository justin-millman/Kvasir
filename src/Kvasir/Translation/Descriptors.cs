global using FieldsListing = System.Collections.Generic.IReadOnlyDictionary<string, Kvasir.Translation.FieldDescriptor>;

using Cybele.Core;
using Kvasir.Schema;
using Optional;
using System.Collections.Generic;

// Descriptors are the intermediate stages of Translation. The purpose of Descriptors is to reflect the information
// known up to a point, with the expectation being that future logic may update or extend the initial translation. The
// attributes of each Descriptor is in CLR terms rather than Schema terms to simplify processing during Translation; the
// various attributes will be converted into Schema terms at the stage's conclusion.

namespace Kvasir.Translation {
    internal delegate Clause CheckGen(IField field, DataConverter converter);
    internal delegate Clause ComplexCheckGen(FieldSeq fields, ConverterSeq converters);

    internal readonly record struct ConstraintBucket(
        Option<ComparisonOperator> RelativeToZero,
        Option<Bound> LowerBound,
        Option<Bound> UpperBound,
        Option<Bound> MinimumLength,
        Option<Bound> MaximumLength,
        IReadOnlySet<object> AllowedValues,
        IReadOnlySet<object> DisallowedValues,
        IReadOnlySet<object> RestrictedImage,
        IReadOnlyList<CheckGen> CHECKs
    );

    internal readonly record struct FieldDescriptor(
        string AccessPath,
        IReadOnlyList<string> Name,
        IsNullable Nullability,
        Option<int> AbsoluteColumn,
        int RelativeColumn,
        DataConverter Converter,
        Option<object?> Default,
        bool InPrimaryKey,
        IReadOnlySet<string> CandidateKeyMemberships,
        ConstraintBucket Constraints
    );

    internal readonly record struct TypeDescriptor(
        FieldsListing Fields,
        IReadOnlyList<ComplexCheckGen> CHECKs
    );
}
