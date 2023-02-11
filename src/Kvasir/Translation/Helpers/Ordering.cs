using Kvasir.Schema;

namespace Kvasir.Translation.Extensions {
    internal static partial class TranslationExtensions {
        /// <summary>
        ///   Checks if a <see cref="DBType"/> represents a semantic that implements a total order, and can therefore
        ///   be constrained by a ranged inequality.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="DBType"/> on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="self"/> is a <see cref="DBType"/> for a numeric (integer or
        ///   floating point), text (string or character), or date/time; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsOrderable(this DBType self) {
            return self == DBType.Character ||
                self == DBType.DateTime ||
                self == DBType.Decimal ||
                self == DBType.Double ||
                self == DBType.Int8 ||
                self == DBType.Int16 ||
                self == DBType.Int32 ||
                self == DBType.Int64 ||
                self == DBType.Single ||
                self == DBType.Text ||
                self == DBType.UInt8 ||
                self == DBType.UInt16 ||
                self == DBType.UInt32 ||
                self == DBType.UInt64;
        }
    }
}
