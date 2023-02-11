using Kvasir.Schema;
using System;
using System.Diagnostics;

namespace Kvasir.Translation.Extensions {
    internal static partial class TranslationExtensions {
        /// <summary>A categorization for numeric <see cref="DBType">DBTypes</see>.</summary>
        public enum NumericKind {
            /// <summary>A signed integer of unspecified size.</summary>
            Signed,

            /// <summary>An unsigned integer of unspecified size.</summary>
            Unsigned,

            /// <summary>A floating-point number of unspecified precision</summary>
            FloatingPoint,

            /// <summary>A non-numeric type.</summary>
            None
        }

        /// <summary>
        ///   Checks the <see cref="NumericKind">numeric categorization</see> of a particular <see cref="DBType"/>.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="DBType"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   The <see cref="NumericKind"/> of <paramref name="self"/>.
        /// </returns>
        public static NumericKind NumericStyle(this DBType self) {
            if (self == DBType.Int8 || self == DBType.Int16 || self == DBType.Int32 || self == DBType.Int64) {
                return NumericKind.Signed;
            }
            else if (self == DBType.UInt8 || self == DBType.UInt16 || self == DBType.UInt32 || self == DBType.UInt64) {
                return NumericKind.Unsigned;
            }
            else if (self == DBType.Decimal || self == DBType.Double || self == DBType.Single) {
                return NumericKind.FloatingPoint;
            }
            else {
                return NumericKind.None;
            }
        }

        /// <summary>
        ///   Produces the value <c>0</c> with a runtime type corresponding to that of a particular
        ///   <see cref="DBType"/>.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="DBType"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   <c>0</c>.
        /// </returns>
        public static object Zero(this DBType self) {
            Debug.Assert(self.NumericStyle() != NumericKind.None);

            if (self == DBType.Int8) {
                return (sbyte)0;
            }
            else if (self == DBType.Int16) {
                return (short)0;
            }
            else if (self == DBType.Int32) {
                return 0;
            }
            else if (self == DBType.Int64) {
                return 0L;
            }
            else if (self == DBType.UInt8) {
                return (byte)0;
            }
            else if (self == DBType.UInt16) {
                return (ushort)0;
            }
            else if (self == DBType.UInt32) {
                return 0U;
            }
            else if (self == DBType.UInt64) {
                return 0UL;
            }
            else if (self == DBType.Decimal) {
                return (decimal)0.0;
            }
            else if (self == DBType.Double) {
                return 0.0;
            }
            else if (self == DBType.Single) {
                return 0.0f;
            }
            else {
                throw new ApplicationException($"Cannot compute zero value for non-numeric data type {self}");
            }
        }
    }
}
