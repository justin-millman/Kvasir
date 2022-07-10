using Ardalis.GuardClauses;
using Cybele.Extensions;
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
        /// <param name="property">
        ///   The <see cref="PropertyInfo"/> describing the property from which to read the <see cref="IRelation"/>
        ///   instance to be populated.
        /// </param>
        /// <pre>
        ///   <paramref name="property"/> is a readable instance property
        ///     --and--
        ///   the <see cref="PropertyInfo.PropertyType"/> of <paramref name="property"/> is <see cref="IRelation"/> or
        ///   a type that implements the <see cref="IRelation"/> interface.
        /// </pre>
        internal FromPropertyRepopulator(PropertyInfo property) {
            Guard.Against.Null(property, nameof(property));
            Debug.Assert(property.PropertyType.IsInstanceOf(typeof(IRelation)));
            Debug.Assert(property.CanRead);
            Debug.Assert(!property.GetGetMethod()!.IsStatic);
            Debug.Assert(property.ReflectedType is not null);

            property_ = property;
            ExpectedSubject = property.ReflectedType!;
        }

        /// <inheritdoc/>
        public void Execute(object subject, IEnumerable<object> entries) {
            Debug.Assert(subject.GetType().IsInstanceOf(ExpectedSubject));
            Debug.Assert(!entries.IsEmpty());

            var relation = (IRelation)property_.GetValue(subject)!;
            foreach (var entry in entries) {
                relation.Repopulate(entry);
            }
        }


        private readonly PropertyInfo property_;
    }
}
