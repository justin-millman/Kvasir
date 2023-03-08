# Tables

## Primary Tables

The **Primary Table** for an Entity is the Table in the back-end database into which the data extracted from each
non-Relation property is stored. Every Entity Type has exactly one Primary Table whose name is `<E>Table`, where `<E>`
is the fully namespace-qualified name of the Entity Type (pursuant to commonality elision, see below). If the Entity
Type is annotated with `[Table(T)]`, then the name of the Primary Table is instead exactly `T`, with no namespace
qualification and all casing and punctuation (e.g. for `snake_case` names) preserved. Alternatively, if the Entity Type
is annotated with `[ExcludeNamespaceFromName]`, the name of the Primary Table will be `<X>Table` where `<X>` is simply
the name of the Entity Type. It is an error for an Entity Type to be annotated with both `[Table(T)]` and
`[ExcludeNamespaceFromName]`.

If all the Entity Types in an application's object model are in the same namespace, and none of the Entity Types are
annotated with `[Table(T)]`, then the common namespace will be _removed_ from the name of the Primary Table.

## Relation Tables

A **Relation Table** is a Table into which the data from a Relation Property is stored. Each Relation Property has
exactly one Relation Table whose name is `<E>.<P>`, where `<E>` is the name of the owning Entity's Primary Table (wit
the `Table` suffix stripped, if present) and `<P>` is the name of the Relation Property. Note that both of these pieces
may be modified by annotations (e.g. `[Table(T)]` or `[Name(S)]`).

## Name Uniqueness

All Tables in the database must have unique names. The default naming conventions for both Primary Tables and Relation
Table guarantee this, but the use of annotations to alter Tables' names may induce duplications. It is an error for two
(or more) Tables (Primary Tables and/or Relation Tables) to have the same name.