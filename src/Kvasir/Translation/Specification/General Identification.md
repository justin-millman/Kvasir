# General Identification

## Entities

An **Entity** is an `N`-tuple of data that describes some real-world (or in-application) object, concept, or idea; in
the database, each Entity is realized as one or more Tables. A CLR Type that corresponds to an Entity is known as an
**Entity Type**, the properties of which ultimately determine the structure of the database Table(s). Each CLR Type in
the application's object model that meets the following criteria will be automatically identified by Kvasir as being an
Entity Type:

* It is defined in the application's assembly
* It has `public` visibility
* It is a `class` or a `record class`
* It is not `abstract`
* It is not generic

A CLR Type that has non-`public` visibility but meets the other criteria may be added to the Data Model by applying the
`[IncludeInModel]` annotation to the type's definition. However, applying this attribute to any other type (e.g. an
`abstract` type, a generic type, a `struct`, etc.) will produce an error. Entity Types may be `partial` and may also be
`sealed`. An Entity Type may have an inheritance hierarchy, though it is generally discouraged, as it may not result in
Fields being identified as expected.

A CLR Type that meets the above criteria but is _not_ annotated with `[IncludeInModel]` may still be included in the
Data Model if it the CLR Type of a property that is included in the data model for an Entity or a Synthetic Entity.

## Fields

A **Field** is a single datum that describes an Entity, realized as a single Column in a database Table. A Table's
Fields are automatically identified by Kvasir by looking at the properties of the source type (either an Entity Type or
an Aggregate Type). Each property that meets the following criteria will correspond to at least one Field, depending on
the type of the property and any annotations applied:

* It has `public` visibility and a `public` `get` method
* It is an instance method
* It is not an indexer
* It is not `abstract`
* It is first declared by the source type (i.e. it is neither inherited nor declared by an interface)
* It is not annotated as `[CodeOnly]`

A property that has non-`public` visibility, has a non-`public` `get` method, and/or is `static` may be added to the
Data Model by applying the `[IncludeInModel]` annotation to the property's definition. However, applying this attribute to
any other type (e.g. an indexer, a property without a `get` method, etc.) will produce an error. Properties included in
the Data Model may be `virtual`, though note that overridden `virtual` functions will be unconditionally excluded.

Each property that is included in the Data Model is placed into one of five Property Categories that describes how the
property maps to Fields, how those Fields are named, what annotations are valid on the property, etc. The rules for each
category are describes in separate documents of the spec, but the categories themselves are:

1. Scalar — the CLR Type is a built-in primitive, `string`, or one of a handful of standard library `struct`s
1. Enumeration — the CLR Type is an `enum`
1. Reference — the CLR Type is an Entity Type
1. Relation — the CLR Type is a type that implements the `IRelation` interface
1. Aggregate — the CLR Type is a `struct` or `record struct`

It is an error for a property that is included in the Data Model to have a CLR Type that does not fall into one of these
categories (e.g. a `delegate`, `System.Enum`, `dynamic`, etc.). Due to other rules regarding generic Entity Types and
inheritance, it is also the case that the CLR Type of a property included in the Data Model cannot be generic reference
type; closed generic value types (i.e. structs) are legal (and they may be from the C# standard library). Nullable types
are fully supported (both `System.Nullable<T>` instantiations and nullable reference types); the category of a property
with such a CLR Type is that of its non-nullable equivalent.

Note that a Data Converter cannot be used to include a property in the Data Model if that property's type does not fit
one of the above five categories. If you wish to store data of an otherwise unsupported type in the back-end database,
you must provide an façade property that fronts the ineligible property and performs the data conversion automatically
on `get` operations.

Properties need not be writeable to be included in the Data Model: a property's `set` method can have `public`
visibility, non-`public` visibility, or be absent altogether. That being said, the presence and/or absence of a `set`
method may have an impact on the Reconstitution of Entity Types.

Note that a property's CLR Type for the purpose of Field categorization is not always the actual `PropertyType` of the
corresponding `System.Reflection.PropertyInfo`: the presence of extrinsic data conversions may result in a different
identification.

## Data Types

Each Field in the Data Model has a Data Type that broadly describes the domain of values that are valid for that Field.
Data Types do not directly correspond to database storage, as some providers support storage formats that others do not
(e.g. signed vs. unsigned integers). Additionally, the domain of permitted values may be shrunk by constraints applied
to source properties, which is not embedded in a Field's Data Type. The Data Types supported by Kvasir are:

| Data Type     | Allowed Values                                       |
|:-------------:|:----------------------------------------------------:|
| `Boolean`     | Either `true` or `false`                             |
| `Character`   | A single UTF-16 code unit                            |
| `DateTime`    | A calendar date and time                             |
| `Decimal`     | A `16`-byte floating point number                    |
| `Double`      | An `8`-byte floating point number                    |
| `Enumeration` | A (varying) limited subset of strings                |
| `Guid`        | A globally unique identifier                         |
| `Int8`        | An `8`-bit signed integer                            |
| `Int16`       | A `16`-bit signed integer                            |
| `Int32`       | A `32`-bit signed integer                            |
| `Int64`       | A `64`-bit signed integer                            |
| `Single`      | A `4`-byte floating point number                     |
| `Text`        | A sequence of UTF-16 code points of arbitrary length |
| `UInt8`       | An `8`-bit unsigned integer                          |
| `UInt16`      | A `16`-bit unsigned integer                          |
| `UInt32`      | A `32`-bit unsigned integer                          |
| `UInt64`      | A `64`-bit unsigned integer                          |

Note that nullability is not embedded in the Data Type of a Field, either. Whether or not `null` is valid value for a
Field is determined separately (though still based on the CLR Type of the source property).