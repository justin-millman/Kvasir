# Aggregate Properties

An **Aggregate Property** is one whose CLR Type is a `struct` or `record struct` that is not `System.DateTime` or
`System.Guid`. Aggregate Properties are useful for semantically grouping related data in the application's object model
without creating an entirely new Entity and Table; for example, one might wish to break down the pieces of an address
for efficient filtering by state, but having a dedicated Table for all addresses may be overkill. The Fields of an
Aggregate Property are "lifted" into the Table of the owning Entity; because Aggregates can themselves have Aggregate
Properties, this "lifting" may extend across multiple "levels."

## General Field Annotations

Aggregate Properties may be annotated much like Scalar Properties, except that the annotations may take an optional
`Path` argument denoting the Nested Property to which the annotation applies. If there is no `Path` argument, the
annotation affects the base property (and often therefore, _all_ Nested Properties), if possible; in some cases, this
may be an error. It is an error for the `Path` argument of an annotation on an Aggregate Property to not resolve to an
extant property or to resolve to a property that is not included in the Data Model.

## Nullability

Aggregate Properties, like Scalar Properties, may be nullable or non-nullable. This determination is made using the same
rules as for Scalar Properties, with the same error conditions surrounding the use of the `[Nullable]` and
`[NonNullable]` annotations. If an Aggregate Property is nullable, then all of the Fields that it contributes to the
Data Model are also nullable; this overrides any nullability determination made for the Nested Properties in isolation.

It is an error for a nullable Aggregate Property to only contribute Fields that, if the Aggregate Property were
non-nullable, would themselves be nullable. This is because it such a situation creates an ambiguity when all `null`
values are read from the database: does this indicate the absence of a value for the Aggregate Property, or the presence
with all `null` Nested Properties? It is legal for individual Nested Properties to be nullable, but at least one must be
non-nullable to avoid the confusion.

## Naming

The names of Fields contributed by Nested Properties of an Aggregate Property are determined by applying the following
rules sequentially:

1. Begin with `P` as the Aggregate Property on the Entity Type and `N` as the empty string
1. If `P` is annotated with the theoretical annotation `[OverrideName(S)]` and `P` is a Scalar Property, then the name
of the Field contributed by `P` is `N`
1. Replace the theoretical annotation `[OverrideName(S)]`, if present, with `[Name(S)]`
1. If `P` is annotated with `[Name(S)]`, append `S` following by a dot (`.`) to `N`
1. For each annotation `[Name(S) { Path = "Q" }]` present on `P`, apply the theoretical annotation `[OverrideName(S)]`
to the Nested Property at path `Q`
1. For each Nested Property `P'` of `P`, repeat from (2) with `P = P'` 

Note that this allows for `[Name(S)]` annotations on Nested Properties to serve as "default names" that can then be
overridden by the ultimate owning Entity. It is therefore an error for a `[Name(S)]` attribute with a non-empty `Path`
value to be present on an Aggregate Property that is itself in an Aggregate structure. Note also that name manipulation
rules for other types of properties (e.g. Relation Properties, Reference Properties, etc.) still apply.

## Column Ordering

The Fields to which an Aggregate Property corresponds will always be ordered sequentially within the Table. By default,
the starting column index for the Fields is undefined; however, one can specify a `0`-based column index for the set of
Fields by using the `[Column(N)]` annotation. An Aggregate Property that is annotated with `[Column(N)]` will correspond
to Fields whose smallest column index is exactly `N`. It is an error for a Field to have a negative column index, and it
is also an error for two (or more) Fields in the same Table to have the same column index.

## Default Values

A nullable Aggregate Property may be annotated with `[Default(null)]` to indicate that the default value for all its
corresponding Fields should be `null`. This is the only way to define default values for multiple Nested Properties with
one annotaiton. Separately, Aggregate Properties may be annotated with `[Default(obj) { Path = "Q" }]`, in which case
the annotation is treated as applying to the Field at path `Q` (as with any other `Path`-bearing annotation on an
Aggregate Property).