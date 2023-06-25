using Cybele.Extensions;
using Optional;
using System;

namespace Kvasir.Translation {
    internal static partial class Extensions {
        /// <summary>
        ///   Attempt to coerce an untyped, possibly <see langword="null"/> user-defined value into a particular type.
        /// </summary>
        /// <remarks>
        ///   The coercion process depends entirely on the target type:
        ///   <list type="bullet">
        ///     <item>
        ///       If the target type is <see cref="DateTime"/> or <see cref="Guid"/>, the value is expected to be a
        ///       <see cref="string"/> that is properly formatted;
        ///     </item>
        ///     <item>
        ///       If the target type is <see cref="decimal"/>, the value is expected to be a <see cref="double"/> whose
        ///       numeric value is within the range of a <see cref="decimal"/>;
        ///     </item>
        ///     <item>
        ///       For any other target type, the value is expected to be exactly that type; no implicit conversions or
        ///       numeric promotions are performed
        ///     </item>
        ///   </list>
        ///   Note that <see langword="null"/> (or <see cref="DBNull.Value"/>) is considered "already coerced" and valid
        ///   for any target type, though such values may be explicitly disallowed by an argument.
        /// </remarks>
        /// <param name="self">
        ///   The possibly <see langword="null"/> <see cref="object"/> on which the extension method is invoked.
        /// </param>
        /// <param name="into">
        ///   The <see cref="Type"/> into which to coerce <paramref name="self"/>.
        /// </param>
        /// <param name="allowNull">
        ///   If <see langword="true"/>, then <paramref name="self"/> is allowed to be <see langword="null"/> or
        ///   <see cref="DBNull.Value"/>; otherwise, such values fail coercion.
        /// </param>
        /// <returns>
        ///   An optional that either contains the coerced value (re-boxed as an <see cref="object"/>), or an error
        ///   message (as a <see cref="string"/>) explaining why coercion failed. If <paramref name="self"/> is
        ///   <see cref="DBNull.Value"/> and <see langword="null"/> values are allowed, the returned optional will
        ///   contain <see langword="null"/>.
        /// </returns>
        public static Option<object?, string> ParseFor(this object? self, Type into, bool allowNull) {
            // A null value cannot be further parsed
            if (self is null || self == DBNull.Value) {
                if (allowNull) {
                    return Option.Some<object?, string>(null);
                }
                else {
                    return Option.None<object?, string>("Field is non-nullable");
                }
            }

            // We need to unwrap the "into" type if it is a Nullable<T>
            into = Nullable.GetUnderlyingType(into) ?? into;

            // It is an error for an annotation value to be an array
            if (self.GetType().IsArray) {
                return Option.None<object?, string>("value cannot be an array");
            }

            // It is an error for an annotation value to be different than the post-conversion CLR type of the annotated
            // property. For a property whose post-conversion CLR type is either DateTime or Guid, all annotation values
            // must be strings (which will be parsed later in this function); for a property whose post-conversion CLR
            // type is Decimal, all annotation values must be doubles.
            if (into == typeof(DateTime) || into == typeof(Guid)) {
                if (self.GetType() != typeof(string)) {
                    var msg = $"expected {nameof(String)} value that would later be parsed into a {into.Name}";
                    return Option.None<object?, string>(msg);
                }
            }
            else if (into == typeof(decimal)) {
                if (self.GetType() != typeof(double)) {
                    var msg = $"expected {nameof(Double)} value that would later be parsed into a {nameof(Decimal)}";
                    return Option.None<object?, string>(msg);
                }
            }
            else if (!into.IsInstanceOf(self.GetType())) {
                return Option.None<object?, string>($"expected value of type '{into.Name}'");
            }

            // Perform actual parsing, if necessary
            if (into == typeof(DateTime)) {
                if (!DateTime.TryParse((string)self, out DateTime result)) {
                    return Option.None<object?, string>($"could not parse into {nameof(DateTime)}");
                }
                return Option.Some<object?, string>(result);
            }
            else if (into == typeof(Guid)) {
                if (!Guid.TryParse((string)self, out Guid result)) {
                    return Option.None<object?, string>($"could not parse into {nameof(Guid)}");
                }
                return Option.Some<object?, string>(result);
            }
            else if (into == typeof(decimal)) {
                var dbl = (double)self;
                if (dbl < (double)decimal.MinValue || dbl > (double)decimal.MaxValue) {
                    return Option.None<object?, string>($"could not convert into {nameof(Decimal)}");
                }
                return Option.Some<object?, string>((decimal)dbl);
            }
            else {
                return Option.Some<object?, string>(self);
            }
        }
    }
}