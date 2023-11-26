using System.Collections.Generic;
using System.Reflection;

namespace Kvasir.Translation.Synthetic {
    /// <summary>
    ///   A reflection representation of the value returned by a <see cref="SyntheticMethodInfo">Synthetic Method</see>.
    /// </summary>
    internal sealed partial class SyntheticParameterInfo : ParameterInfo {
        /// <inheritdoc/>
        public sealed override IList<CustomAttributeData> GetCustomAttributesData() {
            return new List<CustomAttributeData>();
        }
    }
}
