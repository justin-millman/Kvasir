# Relation Properties

A **Relation Property** is one whose CLR Type is a `class` or `record class` type that implements the `IRelation`
interface. This is an interface that can _only_ be implemented by types in the Kvasir assembly, meaning that it is
**not** possible for users to create their own Relation Types. Kvasir currently provides three such types:

* `RelationList<T>` — equivalent to `List<T>`, implementing all the same interfaces and APIs
* `RelationSet<T>` — equivalent to `Set<T>`, implementing all the same interfaces and APIs
* `RelationMap<K, V>` — equivalent to `Dictionary<K, V>`, implementing all the same interfaces and APIs

## Synthetic Types

The data for a Relation Property `P` defined in an Entity Type `E` (possibly lifted from an Aggregate Type) is stored in
a dedicated Relation Table, separate from the data pulled from other properties on `E`. The structure of this Table is
based on the **Synthetic Type** of the Relation, which is defined as a hypothetical `struct` with two properties:

1. An anonymously named property whose type is `E`
1. A property named `Item` whose type is the **Connection Type** of the Relation

Because the Synthetic Type is hypothetically a `struct`, any annotations placed on a Relation Property are processed as
if they were placed on an Aggregate Property whose CLR Type was the Synthetic Type itself. In particular, any valid
`Path` value must begin with `Item`, as that is the only named property available. (If the Connection Type is a
primitive type, then `Item` is the _only_ valid `Path` value.)

## Nested Relations

It is an error for the Connection Type of a Relation Property to be a type that implements the `IRelation` interface or
to be an Aggregate Type that contains a Relation Property.