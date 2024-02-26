using Cybele.Extensions;
using Kvasir.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The intermediate base class for a <see cref="FieldGroup"/> backed by an Aggregate, Reference, or Relation
    ///   property, and therefore corresponding to one or more Fields.
    /// </summary>
    internal abstract class MultiFieldGroup : FieldGroup {
        /// <inheritdoc/>
        public sealed override int Size => fields_.Values.Sum(g => g.Size);

        /// <inheritdoc/>
        public sealed override bool AllNullable => fields_.Values.All(g => g.AllNullable);

        /// <summary>
        ///   The nullable-stripped CLR type of the <see cref="FieldGroup.Source">source</see> property.
        /// </summary>
        protected Type CLRType => Nullable.GetUnderlyingType(Source.PropertyType) ?? Source.PropertyType;

        /// <inheritdoc/>
        public sealed override string this[int column] {
            get {
                Debug.Assert(column >= 0 && column < Size);

                // We have to allow for the possibility that the column indices aren't actually continuous. This is the
                // case when we are dealing with a ReferenceFieldGroup, which may be comprised of partial sub-groups.

                foreach (var group in fields_.Values.OrderBy(g => g.Column.Unwrap())) {
                    if (column < group.Size) {
                        return Source.Name + '.' + group[column];
                    }
                    column -= group.Size;
                    Debug.Assert(column >= 0);
                }
                throw new ApplicationException("This should be algorithmically unreachable code!");
            }
        }

        /// <summary>
        ///   The "kind" of the backing property (i.e. Aggregate vs. Reference vs. Relation).
        /// </summary>
        protected abstract MultiKind Kind { get; }

        /// <summary>
        ///   Constructs a new <see cref="MultiFieldGroup"/> that has no nested Relations.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property backing the new <see cref="MultiFieldGroup"/>.
        /// </param>
        /// <param name="fields">
        ///   The collection of constituent <see cref="FieldGroup">FieldGroups</see> that comprise the new
        ///   <see cref="MultiFieldGroup"/>. The order is irrelevant. The Fields should be in a position to be directly
        ///   modified (i.e. already <see cref="FieldGroup.Clone">cloned</see>).
        /// </param>
        protected MultiFieldGroup(Context context, PropertyInfo source, IEnumerable<FieldGroup> fields)
            : this(context, source, fields, Enumerable.Empty<RelationTracker>()) {}

        /// <summary>
        ///   Constructs a new <see cref="MultiFieldGroup"/> that may, but does not necessarily, have at least one
        ///   nested Relations.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> was accessed via reflection. This context
        ///   should include that property.
        /// </param>
        /// <param name="source">
        ///   The CLR property backing the new <see cref="MultiFieldGroup"/>.
        /// </param>
        /// <param name="fields">
        ///   The collection of constituent <see cref="FieldGroup">FieldGroups</see> that comprise the new
        ///   <see cref="MultiFieldGroup"/>. The order is irrelevant. The Fields should be in a position to be directly
        ///   modified (i.e. already <see cref="FieldGroup.Clone">cloned</see>).
        /// </param>
        /// <param name="trackers">
        ///   The collection of <see cref="RelationTracker">RelationTrackers</see> that represent the Relations nested
        ///   within the new <see cref="MultiFieldGroup"/>. These Trackers allow for "caching" of any annotations
        ///   applied to the source property that, via their <c>Path</c>, actually resolve against a Relation; without
        ///   the Trackers, such annotations would trigger an <see cref="InvalidPathException"/>.
        /// </param>
        protected MultiFieldGroup(Context context, PropertyInfo source, IEnumerable<FieldGroup> fields,
            IEnumerable<RelationTracker> trackers)
                : base(source) {

            Debug.Assert(context is not null);
            Debug.Assert(source is not null);
            Debug.Assert(fields is not null);
            Debug.Assert(fields.All(g => g.Column.HasValue));
            Debug.Assert(trackers is not null);

            name_ = source.Name.Split('.')[^1];
            nullabilityAnnotated_ = false;
            nameAnnotated_ = false;
            fields_ = fields.ToDictionary(f => f.Source.Name.Split('.')[^1], f => f);
            trackers_ = trackers.ToDictionary(t => t.Property.Name.Split('.')[^1], t => t);
            SetNamePrefix(context, new List<string>() {});
            ProcessAnnotations(context);
        }

        /// <summary>
        ///   Constructs a new <see cref="MultiFieldGroup"/> that is largely identical to another.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="MultiFieldGroup"/>.
        /// </param>
        /// <seealso cref="FieldGroup.Clone"/>
        protected MultiFieldGroup(MultiFieldGroup source)
            : base(source) {

            name_ = source.name_;
            nameAnnotated_ = false;
            nullabilityAnnotated_ = false;
            fields_ = source.fields_.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone());
            trackers_ = source.trackers_.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        ///   Constructs a new <see cref="MultiFieldGroup"/> that is largely identical to another, but with each
        ///   constituent Field reset.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="MultiFieldGroup"/>.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        /// <seealso cref="FieldGroup.Reset"/>
        protected MultiFieldGroup(MultiFieldGroup source, ResetTag _)
            : base(source) {

            name_ = source.name_;
            nameAnnotated_ = false;
            nullabilityAnnotated_ = false;
            fields_ = source.fields_.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Reset());
            trackers_ = source.trackers_.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        ///   Constructs a new <see cref="MultiFieldGroup"/> that is largely identical to another, but with a subset of
        ///   the Fields.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="MultiFieldGroup"/>.
        /// </param>
        /// <param name="fields">
        ///   The constituent Fields, which should be a subset of those of <paramref name="source"/>.
        /// </param>
        /// <seealso cref="FieldGroup.Filter(IEnumerable{FieldDescriptor})"/>
        protected MultiFieldGroup(MultiFieldGroup source, IEnumerable<FieldGroup> fields)
            : base(source) {

            Debug.Assert(!fields.IsEmpty());
            Debug.Assert(source.Size >= fields.Count());

            name_ = source.name_;
            nameAnnotated_ = false;
            nullabilityAnnotated_ = false;
            fields_ = fields.ToDictionary(f => f.Source.Name, f => f);
            trackers_ = source.trackers_.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<CheckAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableAnnotationException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (trackers_.TryGetValue(annotation.NextPath, out var tracker)) {
                tracker.AttachAnnotation(context, annotation.Step());
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.ApplyConstraint(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.ComparisonAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableAnnotationException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (trackers_.TryGetValue(annotation.NextPath, out var tracker)) {
                tracker.AttachAnnotation(context, annotation.Step());
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.ApplyConstraint(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.InclusionAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableAnnotationException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (trackers_.TryGetValue(annotation.NextPath, out var tracker)) {
                tracker.AttachAnnotation(context, annotation.Step());
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.ApplyConstraint(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.SignednessAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableAnnotationException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (trackers_.TryGetValue(annotation.NextPath, out var tracker)) {
                tracker.AttachAnnotation(context, annotation.Step());
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.ApplyConstraint(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public sealed override void ApplyConstraint(Context context, Nested<Check.StringLengthAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableAnnotationException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (trackers_.TryGetValue(annotation.NextPath, out var tracker)) {
                tracker.AttachAnnotation(context, annotation.Step());
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.ApplyConstraint(context, annotation.Step());
            }
        }

        /// <summary>
        ///   Produces a filtering of the Fields that comprise this <see cref="MultiFieldGroup"/>.
        /// </summary>
        /// <param name="constituents">
        ///   The Fields that should be kept by the filter operation.
        /// </param>
        /// <returns>
        ///   A collection of <see cref="FieldGroup">FieldGroups</see>, in no particular order, where each is a
        ///   constituent FieldGroup where only the members that are also in <paramref name="constituents"/> have been
        ///   kept.
        /// </returns>
        /// <seealso cref="FieldGroup.Filter(IEnumerable{FieldDescriptor})"/>
        protected IEnumerable<FieldGroup> FilterFields(IEnumerable<FieldDescriptor> constituents) {
            return fields_.Values.Select(g => g.Filter(constituents)).Where(o => o.HasValue).Select(o => o.Unwrap());
        }

        /// <inheritdoc/>
        public sealed override IEnumerator<FieldDescriptor> GetEnumerator() {
            Debug.Assert(fields_.Values.All(g => g.Column.HasValue));

            foreach (var group in fields_.Values.OrderBy(g => g.Column.Unwrap())) {
                foreach (var field in group) {
                    yield return field;
                }
            }
        }

        /// <inheritdoc/>
        public sealed override IEnumerable<ReferenceFieldGroup> References() {
            if (this is ReferenceFieldGroup rfg) {
                yield return rfg;
            }
            else {
                foreach (var refgroup in fields_.Values.SelectMany(f => f.References())) {
                    yield return refgroup;
                }
            }
        }

        /// <inheritdoc/>
        public sealed override void SetDefault(Context context, Nested<DefaultAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                throw new InapplicableAnnotationException(context, annotation.Annotation, CLRType, Kind);
            }
            else if (trackers_.TryGetValue(annotation.NextPath, out var tracker)) {
                tracker.AttachAnnotation(context, annotation.Step());
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.SetDefault(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public override void SetInCandidateKey(Context context, Nested<UniqueAttribute> annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                foreach (var field in fields_.Values) {
                    field.SetInCandidateKey(context, annotation);
                }
            }
            else if (trackers_.TryGetValue(annotation.NextPath, out var tracker)) {
                tracker.AttachAnnotation(context, annotation.Step());
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.SetInCandidateKey(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public override void SetInPrimaryKey(Context context, Nested<PrimaryKeyAttribute> annotation, string cascadePath) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation is not null);

            if (annotation.AppliesHere) {
                foreach (var field in fields_.Values) {
                    field.SetInPrimaryKey(context, annotation, $"{cascadePath}{field.Source.Name}.");
                }
            }
            else if (trackers_.TryGetValue(annotation.NextPath, out var tracker)) {
                tracker.AttachAnnotation(context, annotation.Step());
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.SetInPrimaryKey(context, annotation.Step(), $"{cascadePath}{group.Source.Name}.");
            }
        }

        /// <inheritdoc/>
        public sealed override void SetName(Context context, Nested<NameAttribute> annotation) {
            if (annotation.AppliesHere) {
                if (nameAnnotated_ && annotation.Annotation.Name != name_) {
                    throw new DuplicateAnnotationException(context, annotation.Annotation.Path, typeof(NameAttribute));
                }
                name_ = annotation.Annotation.Name;
                nameAnnotated_ = true;

                // If a [Name] annotation is applied to a Relation, it changes the name of the RelationTable but has no
                // effect on the names of the constituent Fields. The _right_ way to do this is to make the `SetName`
                // method virtual and override it in the `RelationFieldGroup`, but that would result in duplicating the
                // `name_` and `nameAnnotated_` variables, and I just don't feel like doing that right now.
                if (Kind != MultiKind.Relation) {
                    foreach (var group in fields_.Values) {
                        group.SetNamePrefix(context, Enumerable.Repeat(name_, 1));
                    }
                }
            }
            else if (trackers_.TryGetValue(annotation.NextPath, out var tracker)) {
                tracker.AttachAnnotation(context, annotation.Step());
            }
            else if (!fields_.TryGetValue(annotation.NextPath, out var group)) {
                throw new InvalidPathException(context, annotation.Annotation);
            }
            else {
                group.SetName(context, annotation.Step());
            }
        }

        /// <inheritdoc/>
        public override void SetNamePrefix(Context context, IEnumerable<string> prefix) {
            var pfx = prefix.Append(name_);
            foreach (var field in fields_.Values) {
                field.SetNamePrefix(context, pfx);
            }
        }

        /// <inheritdoc/>
        public override void SetNullability(Context context, bool nullable) {
            Debug.Assert(context is not null);
            Debug.Assert(!nullabilityAnnotated_);

            // Nullability on an grouping property is a bit nuanced. As with a scalar/enumeration, the property cannot
            // be annotated as both [Nullable] and [NonNullable]. If the property is annotated as [Nullable], then all
            // the nested Fields are also made nullable. However, if the property is annotated as [NonNullable], this
            // only suppresses the property's native nullability from applying to nested Fields. In particular, nullable
            // nested Fields are not made non-nullable by a [NonNullable] annotation.

            if (nullable && AllNullable) {
                throw new AmbiguousNullabilityException(context, new NullableAttribute());
            }
            else if (nullable) {
                foreach (var group in fields_.Values) {
                    group.SetNullability(context, true);
                }
                nullabilityAnnotated_ = true;
            }
            else {
                nullabilityAnnotated_ = true;
            }
        }

        /// <summary>
        ///   Sets the nullability of the FieldGroup based on the native nullability of the backing property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the backing property was translated via reflection.
        /// </param>
        /// <exception cref="AmbiguousNullabilityException">
        ///   if the backing property is natively nullable, the FieldGroup was not annotated as non-nullable, and
        ///   <see cref="AllNullable"/> are <see langword="true"/>.
        /// </exception>
        protected override void ProcessNativeNullability(Context context) {
            Debug.Assert(context is not null);

            // For more details on how we deal with nullability for these kinds of properties, see the more in-depth
            // comment in SetNullability

            if (nullabilityAnnotated_) {
                return;
            }
            var nullable = new NullabilityInfoContext().Create(Source).ReadState == NullabilityState.Nullable;

            if (nullable && !fields_.IsEmpty() && AllNullable) {
                throw new AmbiguousNullabilityException(context);
            }
            else if (nullable) {
                foreach (var group in fields_.Values) {
                    group.SetNullability(context, true);
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="InapplicableAnnotationException">
        ///   if a <see cref="DataConverterAttribute">[DataConverter]</see> annotation is encountered
        /// </exception>
        /// <exception cref="InvalidDataConverterException">
        ///   if a <see cref="AsStringAttribute">[AsString]</see> annotation or a
        ///   <see cref="NumericAttribute">[Numeric]</see> annotation is encountered.
        /// </exception>
        protected sealed override void ProcessAnnotations(Context context) {
            base.ProcessAnnotations(context);

            // [DataConverter]
            if (Source.HasAttribute<DataConverterAttribute>()) {
                throw new InapplicableAnnotationException(context, typeof(DataConverterAttribute), CLRType, Kind);
            }

            // [AsString]
            if (Source.HasAttribute<AsStringAttribute>()) {
                var annotation = Source.GetCustomAttribute<AsStringAttribute>()!;
                throw new InvalidDataConverterException(context, CLRType, annotation);
            }

            // [Numeric]
            if (Source.HasAttribute<NumericAttribute>()) {
                var annotation = Source.GetCustomAttribute<NumericAttribute>()!;
                throw new InvalidDataConverterException(context, CLRType, annotation);
            }
        }


        private string name_;
        private bool nullabilityAnnotated_;
        private bool nameAnnotated_;
        private readonly IReadOnlyDictionary<string, FieldGroup> fields_;
        private readonly IReadOnlyDictionary<string, RelationTracker> trackers_;
    }
}
