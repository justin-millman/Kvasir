using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   The reflection representation of an argument to a <see cref="SyntheticConstructorInfo"/>.
    /// </summary>
    internal sealed class SyntheticParameterInfo : ParameterInfo {
        /// <summary>
        ///   Constructs a new <see cref="SyntheticParameterInfo"/>.
        /// </summary>
        /// <param name="constructor">
        ///   The <see cref="SyntheticConstructorInfo"/> in which the parameter exists.
        /// </param>
        /// <param name="name">
        ///   The <see cref="ParameterInfo.Name">name</see> of the new parameter.
        /// </param>
        /// <param name="type">
        ///   The <see cref="ParameterInfo.ParameterType">type</see> of the new parameter.
        /// </param>
        public SyntheticParameterInfo(SyntheticConstructorInfo constructor, string name, Type type) {
            Debug.Assert(name is not null && name != "");
            Debug.Assert(type is not null);

            // These are *protected* members of the base class. I have no idea why it is implemented that way, versus
            // with a constructor. But I don't make the rules for the C# Standard Library.
            NameImpl = name;
            ClassImpl = type;
            MemberImpl = constructor;
        }

        /// <inheritdoc/>
        public sealed override IList<CustomAttributeData> GetCustomAttributesData() {
            return new List<CustomAttributeData>();
        }
    }
}
