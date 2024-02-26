using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Relations;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   A utility for tracking annotations applied to Aggregate-nested Relation-type properties.
    /// </summary>
    internal sealed class RelationTracker {
        /// <summary>
        ///   The name of the backing Relation-type property, accounting for any <see cref="NameAttribute">[Name]</see>
        ///   annotation that has been applied.
        /// </summary>
        public string Name {
            get {
                return annotatedName_.ValueOr(Path);
            }
        }

        /// <summary>
        ///   The access path, relative to some arbitrary "current" location in the translation process, at which
        ///   <see cref="Property"/> resides.
        /// </summary>
        public string Path { get; }

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

            Property = source;
            Path = source.Name;
            annotations_ = new Dictionary<string, List<Attribute>>();
            annotatedName_ = Option.None<string>();
        }

        /// <summary>
        ///   Constructs a new <see cref="RelationTracker"/> that is identical to another but with a different
        ///   <see cref="Path"/>.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="RelationTracker"/>.
        /// </param>
        /// <param name="path">
        ///   The new <see cref="Path">access path</see>.
        /// </param>
        private RelationTracker(RelationTracker source, string path) {
            Debug.Assert(source is not null);
            Debug.Assert(path is not null && path != "");

            Property = source.Property;
            Path = path;
            annotations_ = new Dictionary<string, List<Attribute>>(source.annotations_);
            annotatedName_ = source.annotatedName_;
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
                    annotatedName_ = Option.Some((annotation.Annotation as NameAttribute)!.Name);
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
        ///   Prepend a segment to the <see cref="Path"/>.
        /// </summary>
        /// <param name="path">
        ///   The new path segment.
        /// </param>
        /// <returns>
        ///   A new <see cref="RelationTracker"/> that is equivalent to this one but with <paramref name="path"/> (and a
        ///   following <c>.</c>) prepended to the <see cref="Path"/>.
        /// </returns>
        public RelationTracker ExtendPath(string path) {
            Debug.Assert(path is not null && path != "");
            return new RelationTracker(this, path + '.' + Path);
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


        private readonly Dictionary<string, List<Attribute>> annotations_;
        private Option<string> annotatedName_;
    }
}
