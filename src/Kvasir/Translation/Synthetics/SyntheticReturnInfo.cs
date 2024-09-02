using System.Collections.Generic;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The reflection representation of the return value of a <see cref="SyntheticMethodInfo">Synthetic Method</see>.
    /// </summary>
    internal sealed class SyntheticReturnInfo : ParameterInfo {
        /// <inheritdoc/>
        public sealed override IList<CustomAttributeData> GetCustomAttributesData() {
            return new List<CustomAttributeData>();
        }
    }
}
