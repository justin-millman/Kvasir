using Ardalis.GuardClauses;
using System;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the <see cref="IGuardClause"/> interface.
    /// </summary>
    public static class GuardClauseExtensions {
        /// <summary>
        ///   Throws an <see cref="ArgumentException"/> if input isn't a <see cref="Type"/> that "is-a" 'nother
        ///   <see cref="Type"/>.
        /// </summary>
        /// <seealso cref="TypeExtensions.IsInstanceOf(Type, Type)"/>
        public static Type TypeOtherThan(this IGuardClause self, Type input, string parameterName, Type requiredBase,
            string? message = null) {

            Guard.Against.Null(input, parameterName);
            return self.InvalidInput(input, parameterName, t => t.IsInstanceOf(requiredBase), message);
        }
    }
}
