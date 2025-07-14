using Kvasir.Relations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kvasir.Localization {
    /// <summary>
    ///   The internal framework interface denoting a custom collection type that serves as a mapping of localized
    ///   values.
    /// </summary>
    public interface ILocalization {
        /// <summary>
        ///   The type of the Localization triplet. This should be three-element tuple where the first constituent type
        ///   is the type of the localization key, the second is the type of the locale, and the third is the type of
        ///   the localized value.
        /// </summary>
        /// <remarks>
        ///   A few notes about this. We need something that is <see langword="static"/> so that we can access its value
        ///   via reflection when holding just the type, which is the situation we find ourself in when doing
        ///   Translation. However, for mocking purposes, we also need to be able to use <see cref="ILocalization"/> as
        ///   a generic type parameter, which we cannot do if this property were <see langword="abstract"/>. We
        ///   therefore make it regular <see langword="virtual"/> with an error-throwing base implementation. While this
        ///   isn't ideal (technically, an implementation could choose not to override the base functionality), it's not
        ///   a huge deal because the property is <c>internal</c>, and we can write exhaustive tests for the
        ///   implementations authored within Kvasir itself.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        internal static virtual Type Triplet => throw new NotImplementedException();
    }
}
