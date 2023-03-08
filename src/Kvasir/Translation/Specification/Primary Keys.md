# Primary Keys

Each Entity has a **Primary Key**, a collection of one or more Fields that uniquely identifies an instance of the Entity.
Primary Keys are referenced by Foreign Keys (e.g. for Reference Properties) and may be used as indices in the back-end
database. The Fields that participate in a Primary Key can be of any Data Type, though they must be non-nullable.

## Entity Types

The Primary Key of an Entity is determined by looking at the nullability, naming, and annotations of the Entity Type's
properties. In the case of naming, any name-changing operations (e.g. `[Name(N)]` annotations) are applied before the
Primary Key deduction is performed. Specifically, the following rules are applied in order, stopping as soon as the
Field's nullability is resolved:

1. Each property that is annotated as `[PrimaryKey]` corresponds to Fields that are part of the Entity's Primary Key
1. The single non-nullable Field whose name is `ID` is the Entity's Primary Key
1. The single non-nullable Field whose name is `FooID`, where `Foo` is the name of the Entity Type, is the Entity's Primary Key
1. The single Candidate Key for the Entity consisting of only non-nullable Fields is instead the Entity's Primary Key
1. The single non-nullable Field is the Entity's Primary Key

It is an error if the Primary Key for an Entity cannot be deduced. It is also an error for a Field that is nullable
(either by deduction or by the presence of a `[Nullable]` annotation) to be annotated with `[PrimaryKey]`.

Some back-end database providers allow Primary Keys to be named, since Primary Keys are a type of constraint.
By default, the name assigned to a Primary Key for an Entity is implementation defined. If an Entity Type is annotated
with `[NamedPrimaryKey(N)]`, then the Primary Key for the corresponding Entity will be named exactly `N`.

## Aggregate Types

Aggregate Types do not have a Primary Key, as they do not correspond to database Entities. It is an error for a property
in an Aggregate Type to be annotated with `[PrimaryKey]`. However, the properties of an Aggregate Type may correspond to
Fields that contribute to an Entity's Primary Key if the Aggregate Property itself is annotated.

## Synthetic Types

The Primary Key for a Synthetic Type is the type's complete set of Fields. It is an error for a Relation Property to be
annotated with `[PrimaryKey]`.