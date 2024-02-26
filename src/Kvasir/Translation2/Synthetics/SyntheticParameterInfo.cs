using System.Collections.Generic;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The reflection representation of the return value of a <see cref="SyntheticMethodInfo">Synthetic Method</see>.
    /// </summary>
    internal sealed class SyntheticParameterInfo : ParameterInfo {
        /// <inheritdoc/>
        public sealed override IList<CustomAttributeData> GetCustomAttributesData() {
            return new List<CustomAttributeData>();
        }
    }
}
