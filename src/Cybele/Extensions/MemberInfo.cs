using System;
using System.Reflection;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the <see cref="MemberInfo"/> class.
    /// </summary>
    public static partial class TypeExtensions {
        /// <summary>
        ///   Determines if a member (e.g. a <see cref="Type"/> or a <see cref="PropertyInfo"/>) is annotated with a
        ///   particular <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">
        ///   [explicit] The type of <see cref="Attribute"/> whose presence is being checked.
        /// </typeparam>
        /// <param name="self">
        ///   The <see cref="MemberInfo"/> instance on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="self"/> is member that is annotated with at least one instance
        ///   of <typeparamref name="TAttribute"/>, possibly inherited; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasAttribute<TAttribute>(this MemberInfo self) where TAttribute : Attribute {
            return self.GetCustomAttribute<TAttribute>(true) is not null;
        }
    }
}
