using Kvasir.Annotations;
using Kvasir.Schema;

namespace Kvasir.Translation {
    internal static partial class Extensions {
        /// <summary>
        ///   Check if a <see cref="DBType"/> is numeric, and is therefore accepting of signedness constraints.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="DBType"/> on which the extension method was invoked.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="self"/> is a numeric data type (signed integer, unsigned
        ///   integer, or floating point); otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsNumeric(this DBType self) {
            return
                self == DBType.Int8 ||
                self == DBType.Int16 ||
                self == DBType.Int32 ||
                self == DBType.Int64 ||
                self == DBType.UInt8 ||
                self == DBType.UInt16 ||
                self == DBType.UInt32 ||
                self == DBType.UInt64 ||
                self == DBType.Decimal ||
                self == DBType.Double ||
                self == DBType.Single;
        }

        /// <summary>
        ///   Check if a <see cref="DBType"/> is an unsigned numeric, and is therefore not accepting of a
        ///   <see cref="Check.IsNegativeAttribute"/> constraint.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="DBType"/> on which the extension method was invoked.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="self"/> is an unsigned numeric data type; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool IsUnsignedNumeric(this DBType self) {
            return
                self == DBType.UInt8 ||
                self == DBType.UInt16 ||
                self == DBType.UInt32 ||
                self == DBType.UInt64;
        }

        /// <summary>
        ///   Check if a <see cref="DBType"/> has a total ordering, and is therefore accepting of ranged comparison
        ///   constraints.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="DBType"/> on which the extension method was invoked.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="self"/> is a totally ordered data type; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool IsTotallyOrdered(this DBType self) {
            return
                self.IsNumeric() ||
                self == DBType.DateTime ||
                self == DBType.Character ||
                self == DBType.Text;
        }
    }
}
