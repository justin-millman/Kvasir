﻿using Kvasir.Schema;
using System;

namespace Kvasir.Annotations {
    /// <summary>
    ///   An annotation that specifies the name of the Field backing a particular property.
    /// </summary>
    /// <remarks>
    ///   The Kvasir framework will automatically determine the name of backing Fields based on the name of the POCO
    ///   property. The <see cref="NameAttribute"/> can be used to override the default deduction when that deduction
    ///   would be incorrect or undesirable.
    /// </remarks>
    /// <seealso cref="FieldName"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class NameAttribute : Attribute {
        /// <summary>
        ///   The dot-separated path, relative to the property on which the annotation is placed, to the property to
        ///   which the annotation actually applies.
        /// </summary>
        public string Path { internal get; init; } = "";

        /// <summary>
        ///   The <see cref="FieldName"/> specified by the annotation.
        /// </summary>
        internal FieldName Name { get; }

        /// <summary>
        ///   Constructs a new instance of the <see cref="NameAttribute"/>.
        /// </summary>
        /// <param name="name">
        ///   The name of the Field.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="name"/> is not a valid name for a database Field.
        /// </exception>
        public NameAttribute(string name) {
            Name = new FieldName(name);
        }
    }
}
