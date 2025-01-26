using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cybele.Extensions {
    /// <summary>
    ///   A collection of <see href="https://tinyurl.com/y8q6ojue">extension methods</see> that extend the public API
    ///   of the <see cref="PropertyInfo"/> class.
    /// </summary>
    public static class PropertyInfoExtensions {
        /// <summary>
        ///   Determines if a property is init-only.
        /// </summary>
        /// <param name="propertyInfo">
        ///   The <see cref="PropertyInfo"/> instance on which the extension method is invoked.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="propertyInfo"/> is init-only; otherwise (i.e. if it is
        ///   read-only or freely writeable), <see langword="false"/>.
        /// </returns>
        public static bool IsInitOnly(this PropertyInfo propertyInfo) {
            if (!propertyInfo.CanWrite) {
                return false;
            }

            var setter = propertyInfo.SetMethod!;
            var returnModifiers = setter.ReturnParameter.GetRequiredCustomModifiers();
            return returnModifiers.Contains(typeof(IsExternalInit));
        }
    }
}
