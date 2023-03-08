using Ardalis.GuardClauses;
using Kvasir.Schema;
using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An attribute that specifies that the Field backing a particular property is part of the owning Entity's
    ///   Primary Key.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     By default, the Kvasir framework will determine the Primary Key for a Table by analyzing the names of the
    ///     Table's Fields and applying a sequence of well-defined rule. For example, a Field named <c>FooID</c> or
    ///     <c>FooId</c> for a Table named <c>Foo</c> is assumed to be the Table's single-Field Primary Key. The
    ///     <see cref="PrimaryKeyAttribute"/> can be used to override the default deduction when that deduction would
    ///     be incorrect or undesirable.
    ///   </para>
    ///   <para>
    ///     When a <see cref="PrimaryKeyAttribute"/> is applied to multiple POCO properties, the result is a composite
    ///     Primary Key where the order of the constituent Fields is undefined (and mathematically irrelevant).
    ///     Regardless of the number of Fields involved, a Primary Key by default has no name; to provide a name to a
    ///     Primary Key (either the automatic deduction or the annotation-specified), apply a
    ///     <see cref="NamedPrimaryKeyAttribute"/> to the POCO class.
    ///   </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class PrimaryKeyAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { get; init; } = "";
    }

    /// <summary>
    ///   An attribute that specifies the name of the Primary Key of the Table backing a particular class.
    /// </summary>
    /// <seealso cref="PrimaryKeyAttribute"/>
    /// <seealso cref="KeyName"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class NamedPrimaryKeyAttribute : Attribute {
        /// <summary>
        ///   The Primary Key name specified by the annotation.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="NamedPrimaryKeyAttribute"/>.
        /// </summary>
        /// <param name="name">
        ///   The name of the Primary Key.
        /// </param>
        public NamedPrimaryKeyAttribute(string name) {
            Name = Guard.Against.Null(name);
        }
    }
}
