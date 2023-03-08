using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Cybele.Extensions {
    /// An indication of the nullability of an aspect of a program.
    public enum Nullability : ushort {
        /// <see langword="null"/> is a valid value for the aspect in question.
        Nullable,

        /// <see langword="null"/> is an invalid value for the aspect in question.
        NonNullable,

        /// <see langword="null"/> may or may not be a valid value for the aspect in question; this generally applies
        /// only to generic types, with the ambiguity being resolved by the selection of a concrete type.
        Ambiguous
    }

    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that provide insight to the
    ///   nullability of various aspects of a program.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     All types in C# can be separated into one of two categories: value types (primitives, enums, and structs)
    ///     and reference types (classes, interfaces, delegates, and dynamic types). Prior to C# 8.0, one of the
    ///     distinctions between these two types was that <see langword="null"/> was a valid value only for the former.
    ///     However, C# 8.0 introduced the notion of nullable and non-nullable reference types, meaning that the
    ///     nullability of a particular aspect of a program depends not only on the aspect's static type but also on
    ///     any user-supplied annotations <i>and</i> on the nullability context in which the aspect is defined.
    ///   </para>
    ///   <para>
    ///     The <c>GetNullability</c> family of methods can operate on properties, events, function parameters,
    ///     function return types, and member variables. There is limited support for open generics (see below) and
    ///     full support for closed generics. Specifically, the following types are identified as nullable:
    ///         <list type="bullet">
    ///             <item><description>
    ///               Instantiations of the <see cref="Nullable{T}"/> generic wrapper.
    ///             </description></item>
    ///             <item><description>
    ///               Reference types defined in a nullable-disabled context; this includes code from before C# 8.0.
    ///             </description></item>
    ///             <item><description>
    ///               Nullable-annotated reference types (e.g. <c>string?</c>); these can only be defined in a
    ///               nullable-enabled context in code from C# 8.0 or later.
    ///             </description></item>
    ///             <item><description>
    ///               Open generic types that, through a combination of annotations and constraints, can be analyzed as
    ///               one of the above.
    ///             </description></item>
    ///         </list>
    ///   </para>
    ///   <para>
    ///     Some uses of closed generics can be ambiguous as to the imparted nullability; these scenarios generally
    ///     arise when the generic is unconstrained or constrained in such a way so as to allow both nullable and
    ///     non-nullable types as <c>T</c>. Unfortunately, the reflection APIs offered by the C# standard library do
    ///     not provide complete access to all forms of constraint, leading to an inability to properly identify these
    ///     ambiguities. Specifically, the following scenarios should produce <see cref="Nullability.Ambiguous"/> but
    ///     currently do not:
    ///         <list type="bullet">
    ///             <item><description>
    ///               <c>T [unconstrained]</c> resolves as either nullable or non-nullable, depending on its context
    ///             </description></item>
    ///             <item><description>
    ///               <c>T? [unconstrained]</c> resolves as nullable.
    ///             </description></item>
    ///             <item><description>
    ///               <c>T [where T : class?]</c> resolves as non-nullable.
    ///             </description></item>
    ///             <item><description>
    ///               <c>T? [where T : notnull]</c> resolves as nullable.
    ///             </description></item>
    ///         </list>
    ///   </para>
    ///   <para>
    ///     A complete description of how the nullable context of a type is embedded in compiler metadata for C# 8.0+
    ///     can be found on a <a href="https://tinyurl.com/jsm-roslyn-nullable-metadata">writeup</a> on the Roslyn
    ///     GitHub page.
    ///   </para>
    /// </remarks>
    public static class NullabilityExtensions {
        /// <summary>
        ///   Determines if a particular property of a class or struct is nullable.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="PropertyInfo"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   A <see cref="Nullability"/> indicator reflective of the traits of <paramref name="self"/>.
        /// </returns>
        public static Nullability GetNullability(this PropertyInfo self) {
            return GetNullability(self.PropertyType, self.DeclaringType!, self.GetCustomAttributes());
        }

        /// <summary>
        ///   Determines if a particular field (e.g. member variable) of a class or struct is nullable.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="FieldInfo"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   A <see cref="Nullability"/> indicator reflective of the traits of <paramref name="self"/>.
        /// </returns>
        public static Nullability GetNullability(this FieldInfo self) {
            return GetNullability(self.FieldType, self.DeclaringType!, self.GetCustomAttributes());
        }

        /// <summary>
        ///   Determines if a particular parameter or return value of a method is nullable.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="ParamArrayAttribute"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   A <see cref="Nullability"/> indicator reflective of the traits of <paramref name="self"/>.
        /// </returns>
        public static Nullability GetNullability(this ParameterInfo self) {
            return GetNullability(self.ParameterType, self.Member, self.GetCustomAttributes());
        }

        /// <summary>
        ///   Determines if a particular event of a class or struct is nullable.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="EventInfo"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   A <see cref="Nullability"/> indicator reflective of the traits of <paramref name="self"/>.
        /// </returns>
        public static Nullability GetNullability(this EventInfo self) {
            return GetNullability(self.EventHandlerType!, self.DeclaringType!, self.GetCustomAttributes());
        }

        /// <summary>
        ///   Determines if a program aspect with a given static <see cref="Type"/> defined in a particular context
        ///   with (or without) certain attributes is nullable.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="Type"/> of the aspect being evaluated.
        /// </param>
        /// <param name="context">
        ///   The context of the aspect being evaluated. For most aspects, this should be the <see cref="Type"/> in
        ///   which the aspect is being declared. For method parameters and return types, this should be the method
        ///   itself.
        /// </param>
        /// <param name="attributes">
        ///   The collection of attributes applied to the aspect.
        /// </param>
        /// <returns>
        ///   A <see cref="Nullability"/> indicator reflective of the traits of an aspect with static
        ///   <see cref="Type"/> <paramref name="type"/> defined in <paramref name="context"/> with
        ///   <paramref name="attributes"/>.
        /// </returns>
        private static Nullability GetNullability(Type type, MemberInfo? context, IEnumerable<Attribute> attributes) {
            // These values are defined by Roslyn
            byte OBLIVIOUS = 0;
            byte NOT_ANNOTATED = 1;
            byte ANNOTATED = 2;

            // If the type in question is a value type (primitive, enum, struct, etc.) or a generic type on which a
            // struct or enum constraint is applied, then we can determine the nullability of the type without doing
            // heavy reflection
            if (type.IsValueType) {
                if (Nullable.GetUnderlyingType(type) is null) {
                    return Nullability.NonNullable;
                }
                return Nullability.Nullable;
            }

            // We now know that we are dealing with a reference type (class, interface, delegate, etc.), a generic
            // type on which a class or notnull constraint is applied, or an unconstrained generic type. To determine
            // the type's nullability, we have to perform heavy reflection according to the Roslyn docs.
            var nullableAttribute = attributes.FirstOrDefault(a => a.GetType().FullName == NULLABLE_ATTR_NAME);
            if (nullableAttribute is not null) {
                var flags = nullableAttribute.GetType().GetField("NullableFlags")!.GetValue(nullableAttribute)!;
                var asBytes = (byte[])flags;
                Debug.Assert(asBytes[0] == OBLIVIOUS || asBytes[0] == NOT_ANNOTATED || asBytes[0] == ANNOTATED);

                // The first byte in the compiler-generated flags describes the nullability of the type itself;
                // additional bytes describe the nullability of generic parameters (e.g. for Tuple<string, string?> the
                // bytes would be { 1, 1, 2 }).
                return asBytes[0] == ANNOTATED ? Nullability.Nullable : Nullability.NonNullable;
            }

            // If there's no [NullableAttribute] applied directly to the type, we have to look at the surrounding
            // contexts, traversing ancestrally until a [NullableContextAttribute] is encountered or the contexts are
            // exhausted
            for (MemberInfo? current = context; current is not null; current = current.DeclaringType) {
                var attrs = current.GetCustomAttributes();
                var contextAttribute = attrs.FirstOrDefault(a => a.GetType().FullName == NULLABLE_CONTEXT_ATTR_NAME);

                if (contextAttribute is not null) {
                    var flag = (byte)contextAttribute.GetType().GetField("Flag")!.GetValue(contextAttribute)!;
                    Debug.Assert(flag == OBLIVIOUS || flag == NOT_ANNOTATED || flag == ANNOTATED);

                    // A flag value of OBLIVIOUS indicates that the nullable context is disabled, akin to pre-C#8.0
                    // code. Because we are dealing with a reference type, this obliviousness imparts nullability.
                    if (flag != NOT_ANNOTATED) {
                        return Nullability.Nullable;
                    }
                    return Nullability.NonNullable;
                }
            }

            // There was no [NullableContextAttribute] anywhere in the context hierarchy, so we have the equivalent
            // of an OBLIVIOUS flag. As above, this imparts nullability.
            return Nullability.Nullable;
        }


        // The attributes that are relevant to determining nullability cannot be directly used in code: the enclosing
        // namespace is not even made available. As such, we have to hard code the fully qualified names of the
        // attributes rather than accessing them through reflection APIs.
        private static readonly string NULLABLE_ATTRS_NS = "System.Runtime.CompilerServices";
        private static readonly string NULLABLE_CONTEXT_ATTR_NAME = $"{NULLABLE_ATTRS_NS}.NullableContextAttribute";
        private static readonly string NULLABLE_ATTR_NAME = $"{NULLABLE_ATTRS_NS}.NullableAttribute";
    }
}
