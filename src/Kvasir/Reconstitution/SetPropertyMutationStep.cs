using Ardalis.GuardClauses;
using Cybele.Extensions;
using System;
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
        /// <param name="property">
        ///   The <see cref="PropertyInfo"/> describing the property to which to write.
        /// </param>
        /// <param name="valueReconstitutor">
        ///   The <see cref="IReconstitutor"/> to use to create the value to write to <paramref name="property"/>.
        /// </param>
        /// <pre>
        ///   <paramref name="property"/> is a writeable instance property
        ///     --and--
        ///   the <see cref="IReconstitutor.Target">target type</see> of <paramref name="valueReconstitutor"/> is
        ///   compatible with the property type of <paramref name="property"/>.
        /// </pre>
        internal SetPropertyMutationStep(PropertyInfo property, IReconstitutor valueReconstitutor) {
            Guard.Against.Null(property, nameof(property));
            Guard.Against.Null(valueReconstitutor, nameof(valueReconstitutor));
            Debug.Assert(property.CanWrite);
            Debug.Assert(!property.GetSetMethod()!.IsStatic);
            Debug.Assert(valueReconstitutor.Target.IsInstanceOf(property.PropertyType));

            ExpectedSubject = property.ReflectedType!;
            property_ = property;
            argument_ = valueReconstitutor;
        }

        /// <inheritdoc/>
        public void Execute(object subject, DBData rawValues) {
            Guard.Against.Null(subject, nameof(subject));
            Guard.Against.NullOrEmpty(rawValues, nameof(rawValues));
            Debug.Assert(subject.GetType().IsInstanceOf(ExpectedSubject));

            var arg = argument_.ReconstituteFrom(rawValues);
            property_.SetValue(subject, arg);
        }


        private readonly PropertyInfo property_;
        private readonly IReconstitutor argument_;
    }
}
