using Cybele.Core;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;

namespace Kvasir.Translation {
    internal delegate Clause CheckGen(IField field, DataConverter converter);
    internal delegate Clause ComplexCheckGen(FieldSeq fields, ConverterSeq converters);

    internal sealed partial class Translator {
        /// <summary>
        ///   A descriptor of a single back-end database Field.
        /// </summary>
        private record struct FieldDescriptor(
            Type SourceType,
            string AccessPath,
            FieldName Name,
            Type CLRType,
            Option<int> Column,
            IsNullable Nullability,
            Option<object?> RawDefault,
            bool IsInPrimaryKey,
            DataConverter Converter,
            IReadOnlyList<KeyName> KeyMemberships,
            IReadOnlyList<CheckGen> CHECKs
        );

        /// <summary>
        ///   A descriptor of a single in-source CLR Type, translated into two or more Fields.
        /// </summary>
        private record struct TypeDescriptor(
            Type CLRType,
            IReadOnlyList<FieldDescriptor> Fields,
            IReadOnlyList<ComplexCheckGen> CHECKs
        );

        /// <summary>
        ///   A descriptor of a single in-source property of a CLR Type, translated into at least one Field.
        /// </summary>
        private record struct PropertyDescriptor(
            IReadOnlyList<FieldDescriptor> Fields
        );
    }
}
