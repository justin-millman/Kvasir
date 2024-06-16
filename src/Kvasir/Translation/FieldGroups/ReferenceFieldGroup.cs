using Cybele.Extensions;
using Optional;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   A <see cref="MultiFieldGroup"/> backed by an Reference property (i.e. one that is an Entity).
    /// </summary>
    internal sealed class ReferenceFieldGroup : MultiFieldGroup {
        /// <inheritdoc/>
        protected sealed override MultiKind Kind => MultiKind.Reference;

        /// <summary>
        ///   Constructs a new <see cref="ReferenceFieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property backing the new <see cref="ReferenceFieldGroup"/>.
        /// </param>
        /// <param name="fields">
        ///   The collection of constituent <see cref="FieldGroup">FieldGroups</see> that comprise the new
        ///   <see cref="ReferenceFieldGroup"/>. The order is irrelevant. The Fields should be in a position to be
        ///   directly modified (i.e. already <see cref="FieldGroup.Reset">reset</see>).
        /// </param>
        public ReferenceFieldGroup(Context context, PropertyInfo source, IEnumerable<FieldGroup> fields)
            : base(context, source, fields) {

            Debug.Assert(!fields.IsEmpty());
        }

        /// <summary>
        ///   Constructs a new <see cref="ReferenceFieldGroup"/> that is largely identical to another.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="ReferenceFieldGroup"/>.
        /// </param>
        /// <seealso cref="FieldGroup.Clone"/>
        private ReferenceFieldGroup(ReferenceFieldGroup source)
            : base(source) {}

        /// <summary>
        ///   Constructs a new <see cref="ReferenceFieldGroup"/> that is largely identical to another, but with each
        ///   constituent Field reset.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="ReferenceFieldGroup"/>.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        /// <seealso cref="Reset"/>
        private ReferenceFieldGroup(ReferenceFieldGroup source, ResetTag _)
            : base(source, RESET) {}

        /// <summary>
        ///   Constructs a new <see cref="ReferenceFieldGroup"/> that is largely identical to another, but with a subset
        ///   of the Fields.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="ReferenceFieldGroup"/>.
        /// </param>
        /// <param name="fields">
        ///   The constituent Fields, which should be a subset of those of <paramref name="source"/>.
        /// </param>
        /// <seealso cref="FieldGroup.Filter(IEnumerable{FieldDescriptor})"/>
        private ReferenceFieldGroup(ReferenceFieldGroup source, IEnumerable<FieldGroup> fields)
            : base(source, fields) {}

        /// <inheritdoc/>
        public sealed override ReferenceFieldGroup Clone() {
            return new ReferenceFieldGroup(this);
        }

        /// <inheritdoc/>
        public sealed override Option<FieldGroup> Filter(IEnumerable<FieldDescriptor> constituents) {
            var matches = FilterFields(constituents);
            if (!matches.IsEmpty()) {
                return Option.Some<FieldGroup>(new ReferenceFieldGroup(this, matches));
            }
            else {
                return Option.None<FieldGroup>();
            }
        }

        /// <inheritdoc/>
        public sealed override ReferenceFieldGroup Reset() {
            return new ReferenceFieldGroup(this, RESET);
        }
    }
}
