# Constraints

## `UNIQUE` & Candidate Keys

A **Candidate Key** is a set of one or more Fields that must form a unique tuple for each instance of an Entity. The
Primary Key for a Table is necessarily a Candidate Key; additional Candidate Keys can be specified by applying the
`[Unique]` annotation to a property. The `[Unique]` annotation takes an optional argument specifying the name of the
Candidate Key: the Fields of properties annotated as `[Unique]` with the same name argument will be part of a common
Candidate Key, while the name of a Candidate Key for a `[Unique]` annotation without a name argument will be
implementation-defined. It is an error for two or more `[Unique]` annotations with the same name to be applied to the
same property.

It is an error for a `[Unique]` constraint to be placed on a Relation Property, even if a `Path` value is provided.

## Signedness

The `[Check.IsNonZero]`, `[Check.IsPositive]`, and `[Check.IsNegative]` annotations can be applied to properties that
contribute Fields whose Data Type is numeric according to the following table:

| Data Type | `[Check.IsNonZero]` | `[Check.IsPositive]` | `[Check.IsNegative]` |
|:---------:|:-------------------:|:--------------------:|:--------------------:|
| `Int8`    | Valid				  | Valid				 | Valid				|
| `Int16`   | Valid				  | Valid				 | Valid				|
| `Int32`   | Valid				  | Valid				 | Valid				|
| `Int64`   | Valid				  | Valid				 | Valid				|
| `UInt8`   | Valid				  | INVALID				 | INVALID				|
| `UInt16`  | Valid				  | INVALID				 | INVALID				|
| `UInt32`  | Valid				  | INVALID				 | INVALID				|
| `UInt64`  | Valid				  | INVALID				 | INVALID				|
| `Single`  | Valid				  | Valid				 | Valid				|
| `Double`  | Valid				  | Valid				 | Valid				|
| `Decimal` | Valid				  | Valid				 | Valid				|

## Value Comparisons

There are five annotations that can be applied to properties to constrain the values to those in some range. The
`[Check.IsNot]` annotation can be applied to any property, while the other four (`[Check.IsGreaterThan]`,
`[Check.IsGreaterThanOrEqualTo]`, `[Check.IsLessThan]`, and `[Check.IsLessThanOrEqualTo]`) can only be applied to
properties that contribute Fields whose Data Type is numeric or is `Text` or `DateTime`. If multiple such annotations
are applied to a single property, they form a _conjunction_ (i.e. an "and"); it is possible to mix the annotations.
Kvasir will not apply any reductions, nor will it identify tautologically unsatisfiable predicates.

The arguments to the five comparison annotations must be exactly the CLR Type of the property before its Extrinsic
Data Conversion is performed; implicit type conversions (e.g. widening of integers) are not supported. The only
exceptions to this rule are for Scalar Properties whoe pre-conversion CLR Type is either `System.DateTime` or
`System.Guid`: because values of these types cannot be provided directly to a C# attribute, a `string` is expected
(converted into the appropriate type based on the Translation Settings).

## Inclusion & Exclusion

The value of a Field can be restricted to one of a specific subset of values by applying the `[Check.IsOneOf]`
annotation to the source property. Similarly, a specific subset of values can be _prohibited_ by applying the
`[Check.IsNotOneOf]` annotation. These annotations may be applied to any property except one whose CLR Type is an
enumeration. It is an error for more than one of either annotation, or for both, to be applied to a single property.

The arguments to the two inclusion/exclusion annotations must be exactly the CLR Type of the property before its
Extrinsic Data Conversion is performed; implicit type conversions (e.g. widening of integers) are not supported. The
only exceptions to this rule are for Scalar Properties whoe pre-conversion CLR Type is either `System.DateTime` or
`System.Guid`: because values of these types cannot be provided directly to a C# attribute, a `string` is expected
(converted into the appropriate type based on the Translation Settings).

## String Length

A property that corresponds to a Field whose Data Type is `string` can be annotated to control the length of the text
values. The `[Check.IsNonEmpty]`, `[Check.LengthIsAtLeast]`, `[Check.LenghtIsAtMost]`, and `[Check.LengthIsBetween]`
attributes may not be applied to any other property. It is an error for a negative value to be provided as the argument
to `[Check.LengthIsAtMost]` or for a non-positive number to be provided as the argument to `[Check.LengthIsAtLeast`]. It
is similarly an error for the upper bound argument to `[Check.LengthIsBetween]` to be less than the lower bound
argument.

## Custom `CHECK`s

Custom constraints can be applied to a Table by applying a `[Check.Complex(T, Fs...)]` annotation to either the Entity
Type or the Relation Property. The first argument is interpreted as a Type that must be default constructible and
implement the `IConstraintGenerator` interface, which permits the creation of arbitrarily complex predicates (including
disjunctions). The remaining arguments, of which at least one is required, are the name of the Fields in the order in
which they are used to populate the specified custom constraint. A single Field may appear more than once in this list;
however, it is an error for any name to not be the actual name of a Field on the Table. (Note that the name must match
after any name-changing annotations are considered.)