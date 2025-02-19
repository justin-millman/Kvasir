using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Relations;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   A utility for tracking annotations applied to Aggregate-nested Relation-type properties.
    /// </summary>
    internal sealed class RelationTracker {
        /// <summary>
        ///   The value of the <c>[Name]</c> annotation applied to the backing Relation-type property, if any.
        /// </summary>
        public Option<string> AnnotatedName { get; private set; }

        /// <summary>
        ///   The access path, relative to some arbitrary "current" location in the translation process, at which
        ///   <see cref="Property"/> resides.
        /// </summary>
        public string Path => string.Join('.', decontextualizedAccessPath_);

        /// <summary>
        ///   The <see cref="PropertyInfo">property</see> defining the Relation.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        ///   Constructs a new empty <see cref="RelationTracker"/>.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="Property"/>.
        /// </param>
        public RelationTracker(PropertyInfo source) {
            Debug.Assert(source is not null);
            Debug.Assert(source.PropertyType.IsInstanceOf(typeof(IRelation)) && source.PropertyType != typeof(IRelation));

            AnnotatedName = Option.None<string>();
            Property = source;
            contextualizedAccessPath_ = new List<string>() { source.Name.Split('.')[^1] };
            decontextualizedAccessPath_ = new List<string>() { source.Name.Split('.')[^1] };
            annotations_ = new Dictionary<string, List<Attribute>>();
        }

        /// <summary>
        ///   Constructs a new <see cref="RelationTracker"/> that is identical to another but with an additional
        ///   property prepended to its <see cref="Path"/>.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="RelationTracker"/>.
        /// </param>
        /// <param name="upstreamProperty">
        ///   The additional property.
        /// </param>
        private RelationTracker(RelationTracker source, PropertyInfo upstreamProperty) {
            Debug.Assert(source is not null);
            Debug.Assert(upstreamProperty is not null);

            AnnotatedName = source.AnnotatedName;
            Property = source.Property;
            annotations_ = new Dictionary<string, List<Attribute>>(source.annotations_);

            var contextualized = new List<string>(source.contextualizedAccessPath_);
            var type = Nullable.GetUnderlyingType(upstreamProperty.PropertyType) ?? upstreamProperty.PropertyType;
            var newContextualizedSegment = $"{type.DisplayName()} (from \"{upstreamProperty.Name.Split('.')[^1]}\")";
            contextualized.Insert(0, newContextualizedSegment);
            contextualizedAccessPath_ = contextualized;

            var decontextualized = new List<string>(source.decontextualizedAccessPath_);
            decontextualized.Insert(0, upstreamProperty.Name.Split('.')[^1]);
            decontextualizedAccessPath_ = decontextualized;
        }

        /// <summary>
        ///   Process an annotation that was applied to the backing Aggregate-nested property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="annotation"/> was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The annotation.
        /// </param>
        /// <exception cref="InapplicableAnnotationException">
        ///   if <paramref name="annotation"/> applies to the Relation itself and is not a
        ///   <see cref="NameAttribute">[Name]</see> annotation.
        /// </exception>
        public void AttachAnnotation<T>(Context context, Nested<T> annotation) where T : INestableAnnotation {
            Debug.Assert(context is not null);
            Debug.Assert(annotation.Annotation.Path.StartsWith(Property.Name.Split('.')[0]));

            if (annotation.AppliesHere) {
                if (typeof(T) == typeof(NameAttribute)) {
                    Debug.Assert(annotation.AppliesHere);
                    AnnotatedName = Option.Some((annotation.Annotation as NameAttribute)!.Name);
                }
                else {
                    throw new InapplicableAnnotationException(context, annotation.Annotation, Property.PropertyType, MultiKind.Relation);
                }
            }
            else {
                var field = annotation.NextPath;
                var stepped = annotation.Step();
                var effective = stepped.Annotation.WithPath(stepped.NextPath);
                annotations_.TryAdd(field, new List<Attribute>());
                annotations_[field].Add((effective as Attribute)!);
            }
        }

        /// <summary>
        ///   Prepends a property to the <see cref="Path"/>.
        /// </summary>
        /// <param name="property">
        ///   The <see cref="PropertyInfo">property</see> to prepend to the <see cref="Path"/>. This also affects the
        ///   <see cref="GetSyntheticTypenameOn(Type)">implied synthetic typename</see>.
        /// </param>
        /// <returns>
        ///   A new <see cref="RelationTracker"/> identical to the current one, but with <paramref name="property"/>
        ///   prepended to the <see cref="Path"/>.
        /// </returns>
        public RelationTracker Extend(PropertyInfo property) {
            Debug.Assert(property is not null);
            return new RelationTracker(this, property);
        }

        /// <summary>
        ///   Produces all <see cref="AttachAnnotation{T}(Context, Nested{T})">attached annotations</see> that apply to
        ///   Fields encountered starting at a particular path.
        /// </summary>
        /// <param name="fieldPath">
        ///   The starting path.
        /// </param>
        /// <returns>
        ///   A collection of attached annotations, no particular order, where the path of the annotation at the time of
        ///   application is exactly <paramref name="fieldPath"/> or is nested thereunder.
        /// </returns>
        public IEnumerable<Attribute> AnnotationsFor(string fieldPath) {
            Debug.Assert(fieldPath is not null && fieldPath != "");

            foreach ((var key, var value) in annotations_) {
                if (key == fieldPath || key.StartsWith(fieldPath + '.')) {
                    foreach (var annotation in value) {
                        yield return annotation;
                    }
                }
            }
        }

        /// <summary>
        ///   Produces a <see cref="PropertyChain"/> that can be used to access the Relation-type property being tracked
        ///   from a particular CLR type.
        /// </summary>
        /// <param name="source">
        ///   The starting-point CLR type.
        /// </param>
        /// <returns>
        ///   A <see cref="PropertyChain"/> that, when given an instance of <paramref name="source"/>, will produce the
        ///   value of the Relation-type property being tracked.
        /// </returns>
        public PropertyChain AsPropertyChainOn(Type source) {
            Debug.Assert(source is not null);

            var accessChain = new PropertyChain(source, decontextualizedAccessPath_[0]);
            for (int idx = 1; idx < decontextualizedAccessPath_.Count; ++idx) {
                if (Nullable.GetUnderlyingType(accessChain.PropertyType) is not null) {
                    accessChain = accessChain.Append("Value");
                }
                accessChain = accessChain.Append(decontextualizedAccessPath_[idx]);
            }

            return accessChain;
        }

        /// <summary>
        ///   Produces the full, decorated typename of the <see cref="SyntheticType"/> implied by the current
        ///   <see cref="RelationTracker"/>.
        /// </summary>
        /// <param name="entity">
        ///   The "owning Entity" of the tracked Relation. The name of this Entity type will appear first in the implied
        ///   synthetic typename.
        /// </param>
        /// <returns>
        ///   The display-caliber name of the <see cref="SyntheticType"/> implied by the current
        ///   <see cref="RelationTracker"/> when considered "owned" by <paramref name="entity"/>.
        /// </returns>
        public string GetSyntheticTypenameOn(Type entity) {
            Debug.Assert(entity is not null && entity.IsClass);

            var first = entity.DisplayName();
            var middle = contextualizedAccessPath_.SkipLast(1);
            var last = $"<synthetic> `{decontextualizedAccessPath_[^1]}`";
            return string.Join(" → ", Enumerable.Repeat(first, 1).Concat(middle).Append(last));
        }


        private readonly IReadOnlyList<string> contextualizedAccessPath_;
        private readonly IReadOnlyList<string> decontextualizedAccessPath_;
        private readonly Dictionary<string, List<Attribute>> annotations_;
    }
}
