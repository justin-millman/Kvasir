using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Extraction;
using Kvasir.Relations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="IRepopulator"/> that obtains its target <see cref="IRelation"/> by calling the getter on a
    ///   readable instance property of a CLR object.
    /// </summary>
    public sealed class FromPropertyRepopulator : IRepopulator {
        /// <inheritdoc/>
        public Type ExpectedSubject { get; }

        /// <summary>
        ///   Constructs a new <see cref="FromPropertyRepopulator"/>.
        /// </summary>
        /// <param name="extractor">
        ///   The <see cref="IFieldExtractor"/> describing how to access the object on which the
        ///   <see cref="IRelation"/> resides.
        /// </param>
        /// <param name="property">
        ///   The <see cref="PropertyInfo"/> describing the property from which to read the <see cref="IRelation"/>
        ///   instance to be populated.
        /// </param>
        /// <pre>
        ///   <paramref name="property"/> is a readable instance property
        ///     --and--
        ///   <paramref name="property"/> is accessible from the <see cref="IFieldExtractor.FieldType">type produced by
        ///   <paramref name="extractor"/></see>.
        ///     --and--
        ///   the <see cref="PropertyInfo.PropertyType"/> of <paramref name="property"/> is <see cref="IRelation"/> or
        ///   a type that implements the <see cref="IRelation"/> interface.
        /// </pre>
        internal FromPropertyRepopulator(IFieldExtractor extractor, PropertyInfo property) {
            Guard.Against.Null(extractor, nameof(extractor));
            Guard.Against.Null(property, nameof(property));
            Debug.Assert(property.PropertyType.IsInstanceOf(typeof(IRelation)));
            Debug.Assert(property.CanRead);
            Debug.Assert(!property.GetGetMethod()!.IsStatic);
            Debug.Assert(property.ReflectedType is not null);
            Debug.Assert(extractor.FieldType.IsInstanceOf(property.ReflectedType!));

            extractor_ = extractor;
            property_ = property;
            ExpectedSubject = extractor.ExpectedSource;
        }

        /// <inheritdoc/>
        public void Execute(object subject, IEnumerable<object> entries) {
            Debug.Assert(subject.GetType().IsInstanceOf(ExpectedSubject));
            Debug.Assert(!entries.IsEmpty());

            var target = extractor_.Execute(subject);
            var relation = (IRelation)property_.GetValue(target)!;
            foreach (var entry in entries) {
                relation.Repopulate(entry);
            }
        }


        private readonly IFieldExtractor extractor_;
        private readonly PropertyInfo property_;
    }
}
