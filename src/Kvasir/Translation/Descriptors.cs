global using FieldsListing = System.Collections.Generic.IReadOnlyDictionary<string, Kvasir.Translation.FieldDescriptor>;
global using RelationsListing = System.Collections.Generic.IReadOnlyDictionary<string, Kvasir.Translation.IRelationDescriptor>;

using Cybele.Core;
using Kvasir.Annotations;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

// Descriptors are the intermediate stages of Translation. The purpose of Descriptors is to reflect the information
// known up to a point, with the expectation being that future logic may update or extend the initial translation. The
// attributes of each Descriptor is in CLR terms rather than Schema terms to simplify processing during Translation; the
// various attributes will be converted into Schema terms at the stage's conclusion.

namespace Kvasir.Translation {
    internal delegate Clause CheckGen(IField field, DataConverter converter);
    internal delegate Clause ComplexCheckGen(FieldSeq fields, ConverterSeq converters);

    //////////////////////////////////////// CONSTRAINTS ////////////////////////////////////////

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

    ////////////////////////////////////////// FIELDS ///////////////////////////////////////////

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
        Option<Type> ForeignReference,
        ConstraintBucket Constraints
    );

    ///////////////////////////////////////// RELATIONS /////////////////////////////////////////

    internal interface IRelationDescriptor {
        string AccessPath { get; }
        IReadOnlyList<string> Name { get; }
        Option<string> TableName { get; }
        IReadOnlyDictionary<string, Type> FieldTypes { get; }
        IReadOnlyDictionary<string, IReadOnlyList<Attribute>> Attributes { get; }

        IRelationDescriptor WithEntity(Type entityType);
        public IRelationDescriptor WithAccessPath(string accessPath);
        IRelationDescriptor WithName(IEnumerable<string> name);
        IRelationDescriptor WithTableName(string tableName);
        IRelationDescriptor WithAnnotation(string field, INestableAnnotation annotation);
    }

    internal readonly record struct ListSetRelationDescriptor : IRelationDescriptor {
        public string AccessPath { get; init; }
        public IReadOnlyList<string> Name { get; init; }
        public Option<string> TableName { get; init; }
        public IReadOnlyDictionary<string, Type> FieldTypes { get; init; }
        public IReadOnlyDictionary<string, IReadOnlyList<Attribute>> Attributes { get; init; }

        public ListSetRelationDescriptor(string name, Type itemType, NullabilityInfo nullability) {
            Debug.Assert(name is not null && name != "");
            Debug.Assert(itemType is not null);

            AccessPath = "";
            Name = new List<string>() { name };
            TableName = Option.None<string>();
            FieldTypes = new Dictionary<string, Type>() { { "Item", itemType } };

            var attributes = new Dictionary<string, IReadOnlyList<Attribute>>() { { "Item", new List<Attribute>() } };
            if (nullability.GenericTypeArguments[0].ReadState == NullabilityState.Nullable) {
                attributes["Item"] = new List<Attribute>(attributes["Item"]) { new NullableAttribute() };
            }
            Attributes = attributes;
        }

        public IRelationDescriptor WithEntity(Type entityType) {
            Debug.Assert(FieldTypes.Count == 1);
            var newAttributes = new Dictionary<string, IReadOnlyList<Attribute>>(Attributes);
            newAttributes[entityType.Name] = new List<Attribute>() { new ColumnAttribute(0) };

            var updated = this with {
                AccessPath = AccessPath,
                Name = Name,
                TableName = TableName,
                FieldTypes = new Dictionary<string, Type>(FieldTypes) { { entityType.Name, entityType } },
                Attributes = newAttributes
            };

            return updated;
        }
        public IRelationDescriptor WithAccessPath(string accessPath) {
            return this with { AccessPath = accessPath };
        }
        public IRelationDescriptor WithName(IEnumerable<string> name) {
            return this with { Name = name.ToList() };
        }
        public IRelationDescriptor WithTableName(string tableName) {
            Debug.Assert(!TableName.HasValue);
            return this with { TableName = Option.Some(tableName) };
        }
        public IRelationDescriptor WithAnnotation(string field, INestableAnnotation annotation) {
            Debug.Assert(Attributes.ContainsKey(field));
            Debug.Assert(annotation is not null);

            var prefix = AccessPath == "" ? field : AccessPath + "." + field;
            var newPath = annotation.Path[prefix.Length..];
            newPath = newPath.StartsWith(".") ? newPath[1..] : newPath;

            var newAttributes = new Dictionary<string, IReadOnlyList<Attribute>>(Attributes);
            var newList = new List<Attribute>(newAttributes[field]) { (annotation.WithPath(newPath) as Attribute)! };
            newAttributes[field] = newList;

            return this with { Attributes = newAttributes };
        }
    }

    internal readonly struct MapRelationDescriptor : IRelationDescriptor {
        public string AccessPath { get; init; }
        public IReadOnlyList<string> Name { get; init; }
        public Option<string> TableName { get; init; }
        public IReadOnlyDictionary<string, Type> FieldTypes { get; init; }
        public IReadOnlyDictionary<string, IReadOnlyList<Attribute>> Attributes { get; init; }

        public MapRelationDescriptor(string name, Type itemType, NullabilityInfo nullability) {
            Debug.Assert(name is not null && name != "");
            Debug.Assert(itemType is not null);
            Debug.Assert(itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>));

            var keyType = itemType.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public)!.PropertyType;
            var valueType = itemType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public)!.PropertyType;

            AccessPath = "";
            Name = new List<string>() { name };
            TableName = Option.None<string>();
            FieldTypes = new Dictionary<string, Type>() { { "Key", keyType }, { "Value", valueType } };

            var attributes = new Dictionary<string, IReadOnlyList<Attribute>>() {
                { "Key", new List<Attribute>() { new UniqueAttribute('\0') } },
                { "Value", new List<Attribute>() }
            };
            if (nullability.GenericTypeArguments[0].ReadState == NullabilityState.Nullable) {
                attributes["Key"] = new List<Attribute>(attributes["Key"]) { new NullableAttribute() };
            }
            if (nullability.GenericTypeArguments[1].ReadState == NullabilityState.Nullable) {
                attributes["Value"] = new List<Attribute>(attributes["Value"]) { new NullableAttribute() };
            }
            Attributes = attributes;
        }

        public IRelationDescriptor WithEntity(Type entityType) {
            Debug.Assert(FieldTypes.Count == 2);
            var newAttributes = new Dictionary<string, IReadOnlyList<Attribute>>(Attributes);
            newAttributes[entityType.Name] = new List<Attribute>() { new ColumnAttribute(0), new UniqueAttribute('\0') };

            var updated = this with {
                AccessPath = AccessPath,
                Name = Name,
                TableName = TableName,
                FieldTypes = new Dictionary<string, Type>(FieldTypes) { { entityType.Name, entityType } },
                Attributes = newAttributes
            };

            return updated;
        }
        public IRelationDescriptor WithAccessPath(string accessPath) {
            return this with { AccessPath = accessPath };
        }
        public IRelationDescriptor WithName(IEnumerable<string> name) {
            return this with { Name = name.ToList() };
        }
        public IRelationDescriptor WithTableName(string tableName) {
            Debug.Assert(!TableName.HasValue);
            return this with { TableName = Option.Some(tableName) };
        }
        public IRelationDescriptor WithAnnotation(string field, INestableAnnotation annotation) {
            Debug.Assert(Attributes.ContainsKey(field));
            Debug.Assert(annotation is not null);

            var prefix = AccessPath == "" ? field : AccessPath + "." + field;
            var newPath = annotation.Path[prefix.Length..];
            newPath = newPath.StartsWith(".") ? newPath[1..] : newPath;

            var newAttributes = new Dictionary<string, IReadOnlyList<Attribute>>(Attributes);
            var newList = new List<Attribute>(newAttributes[field]) { (annotation.WithPath(newPath) as Attribute)! };
            newAttributes[field] = newList;

            return this with { Attributes = newAttributes };
        }
    }

    internal readonly struct OrderedListRelationDescriptor : IRelationDescriptor {
        public string AccessPath { get; init; }
        public IReadOnlyList<string> Name { get; init; }
        public Option<string> TableName { get; init; }
        public IReadOnlyDictionary<string, Type> FieldTypes { get; init; }
        public IReadOnlyDictionary<string, IReadOnlyList<Attribute>> Attributes { get; init; }

        public OrderedListRelationDescriptor(string name, Type itemType, NullabilityInfo nullability) {
            Debug.Assert(name is not null && name != "");
            Debug.Assert(itemType is not null);
            Debug.Assert(itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>));

            var indexType = itemType.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public)!.PropertyType;
            var elementType = itemType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public)!.PropertyType;
            Debug.Assert(indexType == typeof(uint));

            AccessPath = "";
            Name = new List<string>() { name };
            TableName = Option.None<string>();
            FieldTypes = new Dictionary<string, Type>() { { "Index", indexType }, { "Item", elementType } };

            var attributes = new Dictionary<string, IReadOnlyList<Attribute>>() {
                { "Index", new List<Attribute>() { new UniqueAttribute('\0') } },
                { "Item", new List<Attribute>() }
            };
            if (nullability.GenericTypeArguments[0].ReadState == NullabilityState.Nullable) {
                attributes["Item"] = new List<Attribute>(attributes["Item"]) { new NullableAttribute() };
            }
            Attributes = attributes;
        }

        public IRelationDescriptor WithEntity(Type entityType) {
            Debug.Assert(FieldTypes.Count == 2);
            var newAttributes = new Dictionary<string, IReadOnlyList<Attribute>>(Attributes);
            newAttributes[entityType.Name] = new List<Attribute>() { new ColumnAttribute(0), new UniqueAttribute('\0') };

            var updated = this with {
                AccessPath = AccessPath,
                Name = Name,
                TableName = TableName,
                FieldTypes = new Dictionary<string, Type>(FieldTypes) { { entityType.Name, entityType } },
                Attributes = newAttributes
            };

            return updated;
        }
        public IRelationDescriptor WithAccessPath(string accessPath) {
            return this with { AccessPath = accessPath };
        }
        public IRelationDescriptor WithName(IEnumerable<string> name) {
            return this with { Name = name.ToList() };
        }
        public IRelationDescriptor WithTableName(string tableName) {
            Debug.Assert(!TableName.HasValue);
            return this with { TableName = Option.Some(tableName) };
        }
        public IRelationDescriptor WithAnnotation(string field, INestableAnnotation annotation) {
            Debug.Assert(Attributes.ContainsKey(field));
            Debug.Assert(annotation is not null);

            var prefix = AccessPath == "" ? field : AccessPath + "." + field;
            var newPath = annotation.Path[prefix.Length..];
            newPath = newPath.StartsWith(".") ? newPath[1..] : newPath;

            var newAttributes = new Dictionary<string, IReadOnlyList<Attribute>>(Attributes);
            var newList = new List<Attribute>(newAttributes[field]) { (annotation.WithPath(newPath) as Attribute)! };
            newAttributes[field] = newList;

            return this with { Attributes = newAttributes };
        }
    }

    /////////////////////////////////////////// TYPES ///////////////////////////////////////////

    internal readonly record struct TypeDescriptor(
        FieldsListing Fields,
        RelationsListing Relations,
        IReadOnlyList<ComplexCheckGen> CHECKs
    );
}
