# Scalar Properties

A **Scalar Property** is one whose CLR Type is a (possibly nullable) built-in primitive, `string`, `System.DateTime`, or
`System.Guid`. A Scalar Property corresponds to exactly one Field in the database, with the storage mechanism depending
on both the property's CLR Type and the specific support of the database provider. Ultimately, all properties of any
categorization (except for Enumeration) decay into Scalar Properties, as Scalar Properties represent the most atomic
data representation possible.

Scalar Properties come in two flavors: Top-Level and Nested. A **Top-Level Scalar Property** is one defined on an Entity
Type that corresponds to a Field on the Primary Table of that type. A **Nested Scalar Property** is one that is defined
either on a non-Entity Type (such as an Aggregate Type) or that corresponds to a Field on the Primary Table of a type
other than the Entity Type on which it is defined (as is the case when dealing with Relation Properties). By and large,
this document will consider only the former; discussions of the latter can be found in the documents dedicated to other
property categorizations.

## Data Type Mapping

The Data Type of a Scalar Property is based on the CLR Type of the property after its Extrinsic Data Conversions is
performed. If the CLR Type is an instantiation of the `System.Nullable<T>` generic (e.g. `int?`), then the generic
argument `T` is considered instead. Likewise, the underlying reference type of a nullable reference type (e.g. `string`
for `string?`) is the determining factor. The specific conversion between CLR Type and Data Type is:

| CLR Type          | Data Type   |
|:-----------------:|:-----------:|
| `bool`            | `Boolean`   |
| `byte`            | `UInt8`     |
| `char`            | `Character` |
| `decimal`         | `Decimal`   |
| `double`          | `Double`    |
| `float`           | `Single`    |
| `int`             | `Int32`     |
| `long`            | `Int64`     |
| `sbyte`           | `Int8`      |
| `short`           | `Int16`     |
| `string`          | `Text`      |
| `System.DateTime` | `DateTime`  |
| `System.Guid`     | `Guid`      |
| `uint`            | `UInt32`    |
| `ulong`           | `UInt64`    |
| `ushort`          | `UInt16`    |

## Nullability

The nullability of a Field sourced from a Primitive Property is based on two factors: the nullability of the property's
own CLR Type (after its Extrinsic Data Conversions are applied) and the nullability annotation applied to the property.
Specifically, the following rules are applied in order, stopping as soon as the Field's nullability is resolved:

1. If the property is annotated as `[Nullable]`, then the corresponding Field will be nullable
1. If the property is annotated as `[NonNullable]`, then the corresponding Field will be non-nullable
1. If the property's CLR Type is an instantiation of the `System.Nullable<T>` generic, then the corresponding Field will
be nullable
1. If the property's CLR Type is a value type, then the corresponding Field will be non-nullable
1. If the Entity Type is defined in a nullable-enabled context and the CLR Type is a non-nullable reference type (e.g.
`string`), then the corresponding Field will be non-nullable
1. Else, the corresponding Field will be nullable

Note that a Field's participation in an Entity's Primary Key may supersede the above deduction rules, and in some cases
may induce additional errors.

## Naming

By default, a Scalar Property named `P` will correspond to a Field whose name is exactly `P`, with all casing and
punctuation (e.g. for `snake_case` names) preserved. That being said, a Scalar Property annotated with `[Name(S)]` will
be named `S` instead (again, with casing and punctuation preserved). `S` can contain any characters, but it is an error
for `S` to be the empty string.

It is an error for two or more Fields in the same Table to have the same name. Absent any annotations, this pattern is
guaranteed by Kvasir. If annotations are used to change the default name translations, it is the user's responsibility
to ensure uniqueness of names. The `[Name(S)]` annotation may be used, for example, to switch the names of two Fields
without running afoul of this requirement.

## Column Ordering

By default, the order of Fields within a Table is undefined. However, one can specify a `0`-based column index for a
Field by using the `[Column(N)]` annotation. A Scalar Property that is annotated with `[Column(N)]` will correspond to a
Field with column index exactly `N`. It is an error for a Field to have a negative column index, and it is also an error
for two (or more) Fields in the same Table to have the same column index.

## Default Values

Fields may have a **Default Value**, which is the datum assigned to the Field when a new instance of the owning Entity
is created (such as with an `INSERT` SQL statement) and no value for the Field is explicitly specified. By default,
Fields do not have a Default Value. However, a Scalar Property annotated with `[Default(obj)]` will have a Default Value
obtained by passing `obj` through the property's Extrinsic Data Conversion.

The type of `obj` must be exactly the CLR Type of the property before its Extrinsic Data Conversion is performed;
implicit type conversions (e.g. widening of integers) are not supported. The only exception to this rule is for Scalar
Properties whose pre-conversion CLR Type is either `System.DateTime` or `System.Guid`: because values of these types
cannot be provided directly to a C# attribute, a `string` is expected (converted into the appropriate type based on the
Translation Settings).

Note that not having a Default Value is different than having a Default Value of `null`. `null` is permitted as the
value for `obj` only if the Field is nullable; a Default Value of `null` specified for a non-nullable Field is an error.