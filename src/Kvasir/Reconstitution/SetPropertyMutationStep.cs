using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Extraction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="IMutationStep"/> that changes an existing CLR object by calling the setter on a writeable
    ///   instance property.
    /// </summary>
    public class SetPropertyMutationStep : IMutationStep {
        /// <inheritdoc/>
        public Type ExpectedSubject { get; }

        /// <summary>
        ///   Constructs a new <see cref="SetPropertyMutationStep"/>.
        /// </summary>
        /// <param name="extractor">
        ///   The <see cref="IFieldExtractor"/> describing how to access the object whose property to mutate.
        /// </param>
        /// <param name="property">
        ///   The <see cref="PropertyInfo"/> describing the property to which to write.
        /// </param>
        /// <param name="valueReconstitutor">
        ///   The <see cref="IReconstitutor"/> to use to create the value to write to <paramref name="property"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="property"/> is a writeable instance property
        ///     --and--
        ///   <paramref name="property"/> is accessible from the <see cref="IFieldExtractor.FieldType">type produced by
        ///   <paramref name="extractor"/></see>.
        ///     --and--
        ///   the <see cref="IReconstitutor.Target">target type</see> of <paramref name="valueReconstitutor"/> is
        ///   compatible with the property type of <paramref name="property"/>.
        /// </pre>
        internal SetPropertyMutationStep(IFieldExtractor extractor, PropertyInfo property,
            IReconstitutor valueReconstitutor) {

            Guard.Against.Null(extractor, nameof(extractor));
            Guard.Against.Null(property, nameof(property));
            Guard.Against.Null(valueReconstitutor, nameof(valueReconstitutor));
            Debug.Assert(property.CanWrite);
            Debug.Assert(!property.GetSetMethod()!.IsStatic);
            Debug.Assert(extractor.FieldType.IsInstanceOf(property.ReflectedType!));
            Debug.Assert(valueReconstitutor.Target.IsInstanceOf(property.PropertyType));

            ExpectedSubject = extractor.ExpectedSource;
            property_ = property;
            extractor_ = extractor;
            argument_ = valueReconstitutor;
        }

        /// <inheritdoc/>
        public void Execute(object subject, IReadOnlyList<object?> rawValues) {
            Guard.Against.Null(subject, nameof(subject));
            Guard.Against.NullOrEmpty(rawValues, nameof(rawValues));
            Debug.Assert(subject.GetType().IsInstanceOf(ExpectedSubject));

            var arg = argument_.ReconstituteFrom(rawValues);
            var target = extractor_.Execute(subject);
            property_.SetValue(target, arg);
        }


        private readonly PropertyInfo property_;
        private readonly IFieldExtractor extractor_;
        private readonly IReconstitutor argument_;
    }
}
