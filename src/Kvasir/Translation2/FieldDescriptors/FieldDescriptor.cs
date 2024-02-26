using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Exceptions;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    ///
    internal abstract class FieldDescriptor {
        ///
        public static FieldDescriptor CreateFrom(PropertyInfo property) {
            var converter = GetDataConverter(property);
            var dataType = converter.Match(some: c => c.ResultType, none: () => property.PropertyType);
            dataType = Nullable.GetUnderlyingType(dataType) ?? dataType;

            Debug.Assert(DBType.IsSupported(dataType));
            if (dataType == typeof(string)) {
                return new StringFieldDescriptor(property, converter);
            }
            else if (dataType == typeof(DateTime)) {
                return new DateTimeFieldDescriptor(property, converter);
            }
            else if (dataType == typeof(Guid)) {
                return new GuidFieldDescriptor(property, converter);
            }
            else if (dataType == typeof(decimal)) {
                return new DecimalFieldDescriptor(property, converter);
            }
            else if (dataType.IsEnum) {
                return new EnumFieldDescriptor(property, converter);
            }
            else if (dataType == typeof(bool) || dataType == typeof(char)) {
                return new BasicNonOrderableFieldDescriptor(property, converter);
            }
            else {
                return new BasicNumericFieldDescriptor(property, converter);
            }
        }

        ///
        private static Option<DataConverter> GetDataConverter(PropertyInfo property) {
            Debug.Assert(property is not null);

            var attribute = property.GetCustomAttribute<DataConverterAttribute>();
            if (attribute is not null) {
                if (attribute.UserError != "") {
                    throw new KvasirException(
                        "Error Performing Translation:\n" +
                       $"  • Location: property '{property.Name}' of type '{property.ReflectedType!.Name}'\n" +
                        "  • Annotation: [DataConverter]\n" +
                       $"  • Problem: {attribute.UserError}\n"
                    );
                }
                else {
                    Debug.Assert(attribute.DataConverter.IsBidirectional);
                    return Option.Some(attribute.DataConverter);
                }
            }
            else {
                return Option.None<DataConverter>();
            }
        }


        private readonly PropertyInfo source_;
        private readonly string name_;
        private readonly bool nullable_;
        private readonly int column_;
        private readonly Option<DataConverter> converter_;
        private readonly bool inPrimaryKey_;
        private readonly IReadOnlySet<string> keyMemberships_;
        private readonly IReadOnlySet<object> AllowedValues_;
        private readonly IReadOnlySet<object> DisallowedValues_;
        private readonly IReadOnlyList<IConstraintGenerator> checks_;
    }
}
