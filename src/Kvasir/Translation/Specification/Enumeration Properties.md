# Enumeration Properties

An **Enumeration Property** is one whose CLR Type is a (possibly nullable) `enum`. Enumeration Properties are further
classified into two disjoint subcategories: Standard Enumeration Properties and Flag Enumeration Properties

## Standard Enumeration Properties

A **Standard Enumeration Property** is one whose CLR Type is either `E` or `System.Nullable<E>` where `E` is an `enum`
that is not annotated as `[System.Flags]`. A Standard Enumeration Property corresponds to exactly one Field whose Data
Type is `Enumeration`. Aside from the Data Type mapping and some nuances surrounding Extrinsic Data Conversions, a
Standard Enumeration Property is treated identically to a Scalar Property.

The Field to which a Standard Enumeration Property corresponds is only allowed to take on certain values. Those values
are known as the **Permitted Values** for the Field, and are determined by executing the Field's Extrinsic Data
Conversion on the named enumerators of the source property's CLR Type.

## Flags Enumeration Properties

A **Flags Enumeration Property** is one whose CLR Type is either `E` or `System.Nullable<E>` where `E` is an `enum` that
is annotated as `[System.Flags]`. A Flags Enumeration Property is treated identically to a Relation Property whose CLR
Type is `RelationSet<E'>`, with `E'` being a non-flags version of `E` with any named combination enumerators (i.e. those
whose numeric value is not a perfect power of two) removed.