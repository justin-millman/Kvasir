using Cybele.Extensions;
using Kvasir.Reconstitution;
using Optional;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   A <see cref="MultiFieldGroup"/> backed by an Aggregate property (i.e. one that is a struct).
    /// </summary>
    internal sealed class AggregateFieldGroup : MultiFieldGroup {
        /// <inheritdoc/>
        protected sealed override MultiKind Kind => MultiKind.Aggregate;

        /// <summary>
        ///   Constructs a new <see cref="AggregateFieldGroup"/>.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property backing the new <see cref="AggregateFieldGroup"/>.
        /// </param>
        /// <param name="fields">
        ///   The collection of constituent <see cref="FieldGroup">FieldGroups</see> that comprise the new
        ///   <see cref="AggregateFieldGroup"/>. The order is irrelevant. The Fields should be in a position to be
        ///   directly modified (i.e. already <see cref="FieldGroup.Clone">cloned</see>).
        /// </param>
        /// <param name="trackers">
        ///   The collection of <see cref="RelationTracker">RelationTrackers</see> that represent the Relations nested
        ///   within the new <see cref="AggregateFieldGroup"/>. These Trackers allow for "caching" of any annotations
        ///   applied to the Aggregate property that, via their <c>Path</c>, actually resolve against a Relation;
        ///   without the Trackers, such annotations would trigger an <see cref="InvalidPathException"/>.
        /// </param>
        public AggregateFieldGroup(Context context, PropertyInfo source, IEnumerable<FieldGroup> fields,
            IEnumerable<RelationTracker> trackers)
                : base(context, source, fields, trackers) {

            // No debug check for at least 1 Field here because there may, in fact, be 0 Fields: this happens if there
            // is an Aggregate that contains only Relation-type Fields. Such an AggregateGroup will be excluded from the
            // ongoing Translation.

            if (!IsCalculated && Size > 0) {
                using var guard = context.Push(Nullable.GetUnderlyingType(source.PropertyType) ?? source.PropertyType);
                var creator = ReconstitutionHelper.MakeCreator(context, source.PropertyType, fields, IsNativelyNullable);
                Creator = Option.Some<ICreator>(creator);
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="AggregateFieldGroup"/> that is largely identical to another.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="AggregateFieldGroup"/>.
        /// </param>
        /// <seealso cref="Clone"/>
        private AggregateFieldGroup(AggregateFieldGroup source)
            : base(source) {}

        /// <summary>
        ///   Constructs a new <see cref="AggregateFieldGroup"/> that is largely identical to another, but with each
        ///   constituent Field reset.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="AggregateFieldGroup"/>.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        /// <seealso cref="Reset"/>
        private AggregateFieldGroup(AggregateFieldGroup source, ResetTag _)
            : base(source, RESET) {}

        /// <summary>
        ///   Constructs a new <see cref="AggregateFieldGroup"/> that is largely identical to another, but with a subset
        ///   of the Fields.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="AggregateFieldGroup"/>.
        /// </param>
        /// <param name="fields">
        ///   The constituent Fields, which should be a subset of those of <paramref name="source"/>.
        /// </param>
        /// <seealso cref="FieldGroup.Filter(IEnumerable{FieldDescriptor})"/>
        private AggregateFieldGroup(AggregateFieldGroup source, IEnumerable<FieldGroup> fields)
            : base(source, fields) {}

        /// <inheritdoc/>
        public sealed override AggregateFieldGroup Clone() {
            return new AggregateFieldGroup(this);
        }

        /// <inheritdoc/>
        public sealed override Option<FieldGroup> Filter(IEnumerable<FieldDescriptor> constituents) {
            var matches = FilterFields(constituents);
            if (!matches.IsEmpty()) {
                return Option.Some<FieldGroup>(new AggregateFieldGroup(this, matches));
            }
            else {
                return Option.None<FieldGroup>();
            }
        }

        /// <inheritdoc/>
        public sealed override AggregateFieldGroup Reset() {
            return new AggregateFieldGroup(this, RESET);
        }
    }
}
