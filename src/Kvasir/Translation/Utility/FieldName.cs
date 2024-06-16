using Cybele.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Translation {
    /// <summary>
    ///   The name of a Field or a Field Group, as tracked throughout the process of translating a property and the
    ///   types that enclose it.
    /// </summary>
    /// <remarks>
    ///   A Field's name consists of two parts: the "Name Part" and the "Prefix Part." By default the "Name Part" is the
    ///   name of the CLR source property, but that can be changed by a <c>[Name]</c> annotation placed directly on the
    ///   property or applying to the property via a path. The "Prefix Part" is the dot-separated names of the access
    ///   path from a given scope to the property. When a <c>[Name]</c> annotation is applied to a non-scalar, it alters
    ///   the value that particular property contributes to the "Prefix Part" of any nested Fields. When the "Name Part"
    ///   is set via annotation, the "Prefix Part" is cleared out.
    /// </remarks>
    internal sealed class FieldName {
        /// <summary>
        ///   The <see cref="FieldName"/>, converted into a schema-applicable <see cref="Schema.FieldName"/>.
        /// </summary>
        public Schema.FieldName SchemaName {
            get {
                var str = string.Join(PART_SEPARATOR, prefixPart_.Append(namePart_).Where(p => p != IGNORE_SENTNEL));
                return new Schema.FieldName(str);
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="FieldName"/> with a given Name Part.
        /// </summary>
        /// <param name="name">
        ///   The Name Part.
        /// </param>
        public FieldName(string name) {
            Debug.Assert(name is not null && name != "");

            namePart_ = name;
            prefixPart_ = new List<string>();
        }

        /// <summary>
        ///   Constructs a <see cref="FieldName"/> that is a deep copy of another.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="FieldName"/>.
        /// </param>
        public FieldName(FieldName source) {
            Debug.Assert(source is not null);

            namePart_ = source.namePart_;
            prefixPart_ = new List<string>(source.prefixPart_);
        }

        /// <summary>
        ///   Sets the Name Part of the <see cref="FieldName"/>, overwriting whatever Name Part is currently set and
        ///   clearing out the entirety of the Prefix Part.
        /// </summary>
        /// <param name="name">
        ///   The new Name Part.
        /// </param>
        public void SetNamePart(string name) {
            Debug.Assert(name is not null && name != "");

            namePart_ = name;
            for (int i = 0; i < prefixPart_.Count; ++i) {
                prefixPart_[i] = IGNORE_SENTNEL;
            }
        }

        /// <summary>
        ///   Sets the Prefix Part of the <see cref="FieldName"/>.
        /// </summary>
        /// <param name="prefix">
        ///   The suffix of the new Prefix Part.
        /// </param>
        public void SetPrefixPart(IReadOnlyList<string> prefix) {
            Debug.Assert(prefix is not null && !prefix.IsEmpty());
            Debug.Assert(prefix.None(s => s is null || s == ""));

            for (int i = 1; i <= prefix.Count; ++i) {
                if (prefixPart_.Count < i) {
                    prefixPart_.Insert(0, prefix[^i]);
                }
                else if (prefixPart_[^i] != IGNORE_SENTNEL) {
                    prefixPart_[^i] = prefix[^i];
                }
            }
        }


        private string namePart_;
        private readonly List<string> prefixPart_;

        private const char PART_SEPARATOR = '.';        // separator in resulting name that separates "segments"
        private const string IGNORE_SENTNEL = "";       // segment is ignored because a full name change erased it
    }
}
