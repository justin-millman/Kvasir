# Data Conversions

## Extrinsic Data Conversions

An **Extrinsic Data Conversion** is one that is applied based on the presence of a `[DataConverter(T)]` annotation.
Extrinsic Data Conversions are performed on the C# value extracted from a property before it is stored in the database
(and, therefore, before it is subjected to constraints). Conversely, the reverse conversion is applied when the value is
loaded out of the database prior to Entity Reconstitution. Within the Data Model, however, the application of a
`[DataConverter(T)]` attribute effectively modifies the CLR Type of the annotated property, which can then impact the
Data Type of the corresponding Fields. It is an error for a property to be annotated with more than one instance of the
`[DataConverter(T)]` attribute with the same `Path` value.

### For Scalar Properties

When applied to a Scalar Property, it is an error for the `Path` of a `[DataConverter(T)]` annotation to be anything
other than the empty string. Futhermore, the `SourceType` of `T` must be exactly the native type of the property;
implicit type conversions (e.g. widening of integers) are not supported. The CLR Type of the Scalar Property is then
the `ResultType` of `T`, but the nullability is unaffected. It is an error for a Scalar Property to be directly
annotated with more than one `[DataConverter(T)]` attributes; to achieve a complex, multi-step conversion, simply
construct a suitable converter type `T`.

A Scalar Property that is not annotated with `[DataConverter(T)]` has no Extrinsic Data Conversion: its CLR Type is the
property's own native type.

### For Enumeration Properties

Enumeration Properties are special when it comes to Extrinsic Data Conversions, as they are the only category of
property that _requires_ such a conversion to be present. This requirement is due to the fact that there is no real way
to store a language-bound enumeration in a database directly: individual enumerators (i.e. the values) are just scoped
names for integer constants. Some back-end database providers have native support for enumeration-type columns, which
are generally implemented as numeric memory storage with a string interface; other providers have no such support.

If an Enumeration Property is annotated with `[DataConverter(T)]` and `T` is neither an `enum` nor `string` (nullability
notwithstanding), the behavior is identical to that of Scalar Properties. If the property is specifically a Flags
Enumeration Property, the change to its CLR Type enacted by the conversion inhibits the normal "treated as-if it were a
Relation" behavior.

If an Enumeration Property is annotated as `[Numeric]`, then Kvasir will behave as if it were annotated with a
`[DataProvider(T)]` attribute where `T` is a synthetic data converter that interoperates between instances of the
property's native `enum` type and the underlying numeric representation thereof. In this case, the CLR Type of the
property is viewed as that underlying numeric representation; however, this does _not_ inhibit the "treated as-if it
were a Relation" behavior for Flags Enumeration Properties. It is an error for a property that is not an Enumeration
Property to be annotated as `[Numeric]`, and it is also an error for a single property to be annotated as both
`[Numeric]` and with `[DataConverter(T)]`.

If an Enumeration Property has no `[DataConverter(T)]` annotation, then Kvasir will behave as if it _did_. Specifically,
Kvasir will create a synthetic data converter whose `SourceType` is the property's native type, whose `ResultType` is
`string` or `string?` (with nullability matching that of `SourceType`), and whose conversion/reversion is
stringification (via `.ToString()`) and from-string parsing. This automatic Extrinsic Data Conversion is also applied if
an Enumeration Property is annotated with `[DataConverter(T)]` where `T` is an `enum`, except that the `SourceType`
becomes `T` instead. In either case, the CLR Type of the property is the `SourceType` of the automatic converter.

If an Enumeration Property is annotated with `[DataConverter(T)]` and `T` is `string` or `string?`, then the CLR Type of
the property is treated as if it were the property's native `enum` type with the nullability of the converter's
`ResultType`. The presence of such an annotation inhibits the automatic Extrinsic Data Conversion described in the
previous paragraph, and is generally useful to enact a custom string mapping.

### For Reference Properties

It is an error for a Reference Property to be annotated with `[DataConverter(T)]`, regardless of the presence or absence
of a specified `Path`. The Nested Fields lifted out of the referenced Entity's Primary Key must match exactly, and so
any Extrinsic Data Conversions are taken from the source definition.

### For Relation Properties

Any `[DataConverter(T)]` attribute applied to a Relation Property is treated as applying to the Synthetic Type of the
Relation Pair. As such, it is an error for the `Path` of such an annotation to be the empty string.

### For Aggregate Properties

When applied to an Aggregate Property, a `[DataConverter(T)]` annotation is treated as if it had been applied to the
Nested Property accessed via the annotation's `Path`. It is an error for no such property to exist or for the `Path` to
be an empty string or for the `Path` to resolve to a property that is excluded from the Data Model. It is also an error
if the target Nested Property is already annotated with its own `[DataConverter(T)]` attribute.

If the Neted Property obtained via `Path` is an Enumeration Property, the application of the `[DataConverter(T)]` taken
from the Aggregate Property is applied before any automatic Extrinsic Data Conversion is synthesized.

## Intrinsic Data Conversions

An **Intrinsic Data Conversion** is one that is applied automatically by the framework because the specific back-end
database provider does not have a native data type that allows for the storage of a Field's data. For example, some
RDBMSes support only signed integers, meaning that unsigned integers must be converted (e.g. via a bitwise
reinterpretation) before being stored. Intrinsic Data Conversions occur after Extrinsic Data Conversions, as the former
is predicated on the Data Type of the Field, which can be modified by the latter. Intrinsic Data Conversions cannot be
controlled by the user.