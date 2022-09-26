# Reference Properties

A **Reference Property** is one whose CLR Type is a (possibly nullable) Entity Type. Conceptually, a Reference Property
represents a connection between two Entity Types in which the at least one of the cardinalities is `1` (e.g. one-to-one,
one-to-many, or many-to-one). Each Reference Property contributes Fields that are based on the Primary Key of the
referenced Entity Type. Exactly one Field is contributed for each Field in the target's Primary Key, with the Field's
Data Type being carried over from its source. Other attributes may be modified by annotating the Reference Property
itself.

## Nullability

Because the Fields contributed by a Reference Property are based on another Entity's Primary Key, they are inherently
all non-nullable. However, if the Reference Property would be deduced as nullable using the nullability deduction rules
for Scalar Properties, then the Fields become nullable instead. (Note that the same error conditions for the
`[Nullable]` and `[NonNullable]` attributes apply as for Scalar Properties.) In such scenarios, the Reference Property
represents an "optional reference": either all the Fields are `null` (indicating the absence of a reference) or all the
Fields are non-`null` (indicating the presence of a reference).

## Naming

The Fields to which a Reference Property corresponds are named identically to those of an Aggregate Property, with the
`[Name(S) { Path = "Q" }]` attribute modifying the naming scheme in the same way.

## Column Ordering

The Fields to which a Reference Property corresponds will always be ordered sequentially within the Table, with the same
relative order as in the referenced Entity Type's Primary Key. By default, the starting column index for the Fields is
undefined; however, one can specify a `0`-based column index for the set of Fields by using the `[Column(N)]`
annotation. A Reference Property that is annotated with `[Column(N)]` will correspond to Fields whose smallest column
index is exactly `N`. It is an error for a Field to have a negative column index, and it is also an error for two (or
more) Fields in the same Table to have the same column index.

## Default Values

It is an error for a Reference Property to be annotated with `[Default(obj)]`.