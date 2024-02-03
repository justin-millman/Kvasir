using Ardalis.GuardClauses;
using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="IObjectCreator"/> that produces a single, complex CLR object by invoking a constructor via
    ///   reflection.
    /// </summary>
    public sealed class ByConstructorCreator : IObjectCreator {
        /// <inheritdoc/>
        public Type Target { get; }

        /// <summary>
        ///   Constructs a new <see cref="ByConstructorCreator"/>.
        /// </summary>
        /// <param name="ctor">
        ///   The <see cref="ConstructorInfo"/> describing the constructor to be invoked.
        /// </param>
        /// <param name="argReconstitutors">
        ///   A list of <see cref="IReconstitutor">Reconstitutors</see> to be used to create the arguments to the
        ///   reflection invocation of <paramref name="ctor"/>, in the order in which they are listed.
        /// </param>
        /// <param name="opt">
        ///   Whether or not the object being constructed is "optional." An optional object is one that can take on the
        ///   value of <see langword="null"/>. Specifically, this argument controls the behavior of
        ///   <see cref="Execute(IReadOnlyList{DBValue})"/> in the event that a set of all <see cref="DBValue.NULL"/>
        ///   is provided.
        /// </param>
        /// <pre>
        ///   <paramref name="argReconstitutors"/> is not empty
        ///     --and--
        ///   The <see cref="IReconstitutor.Target">target type</see> of each element of
        ///   <paramref name="argReconstitutors"/> is compatible with the corresponding argument to
        ///   <paramref name="ctor"/>.
        /// </pre>
        internal ByConstructorCreator(ConstructorInfo ctor, IEnumerable<IReconstitutor> argReconstitutors, bool opt) {
            Guard.Against.Null(ctor, nameof(ctor));
            Guard.Against.NullOrEmpty(argReconstitutors, nameof(argReconstitutors));
            Debug.Assert(ctor.IsPublic);
            Debug.Assert(argReconstitutors.All((i, r) => r.Target.IsInstanceOf(ctor.GetParameters()[i].ParameterType)));

            Target = ctor.ReflectedType!;
            ctor_ = ctor;
            arguments_ = argReconstitutors;
            constructFromNulls_ = !opt;
        }

        /// <inheritdoc/>
        public object? Execute(DBData rawValues) {
            Guard.Against.NullOrEmpty(rawValues, nameof(rawValues));

            if (constructFromNulls_ || rawValues.Any(v => v != DBValue.NULL)) {
                var args = arguments_.Select(r => r.ReconstituteFrom(rawValues));
                return ctor_.Invoke(args.ToArray());
            }
            return null;
        }


        private readonly bool constructFromNulls_;
        private readonly ConstructorInfo ctor_;
        private readonly IEnumerable<IReconstitutor> arguments_;
    }
}
