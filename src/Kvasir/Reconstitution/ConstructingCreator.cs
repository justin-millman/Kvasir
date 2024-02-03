using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   An <see cref="ICreator"/> that creates a new CLR object by invoking a constructor via reflection.
    /// </summary>
    internal sealed class ConstructingCreator : ICreator {
        /// <inheritdoc/>
        public Type ResultType { get; }

        /// <summary>
        ///   Construct a new <see cref="ConstructingCreator"/>.
        /// </summary>
        /// <param name="ctor">
        ///   The <see cref="ConstructorInfo">constructor</see> to invoke.
        /// </param>
        /// <param name="argumentCreators">
        ///   A collection of <see cref="ICreator">ICreators</see> that define how each argument in the
        ///   <paramref name="ctor">constructor</paramref> is created from the
        ///   <see cref="DBValue">database values</see>.
        /// </param>
        /// <param name="allowAllNulls">
        ///   If <see langword="true"/>, then a new non-<see langword="null"/> CLR object will be constructed even if
        ///   each of the <see cref="DBValue">database values</see> provided is <see cref="DBValue.NULL"/>. If
        ///   <see langword="false"/>, then such a set of values will result in a <see langword="null"/> object.
        /// </param>
        public ConstructingCreator(ConstructorInfo ctor, IEnumerable<ICreator> argumentCreators, bool allowAllNulls) {
            Debug.Assert(ctor is not null);
            Debug.Assert(ctor.ReflectedType is not null);
            Debug.Assert(argumentCreators is not null);
            Debug.Assert(argumentCreators.Count() == ctor.GetParameters().Length);
            Debug.Assert(argumentCreators.All((i, c) => c.ResultType.IsInstanceOf(ctor.GetParameters()[i].ParameterType)));

            constructFromAllNulls_ = allowAllNulls;
            constructor_ = ctor;
            argumentCreators_ = new List<ICreator>(argumentCreators);
            ResultType = constructor_.ReflectedType!;
        }

        /// <inheritdoc/>
        public object? CreateFrom(IReadOnlyList<DBValue> dbValues) {
            Debug.Assert(dbValues is not null && !dbValues.IsEmpty());

            // If all the values in the row are null, it means one of two things: either we're dealing with a nullable
            // Aggregate (constructFromNulls_ will be true) or we're dealing with a non-nullable Aggregate (in which
            // case constructFromNulls_ will be false). In the former case, the Translation Layer ensures that at least
            // one of the nested Fields is itself non-nullable, and it may be possible that running the argument
            // creators induces an error.

            if (!constructFromAllNulls_ && dbValues.All(v => v == DBValue.NULL)) {
                return null;
            }
            else {
                var arguments = argumentCreators_.Select(c => c.CreateFrom(dbValues));
                return constructor_.Invoke(arguments.ToArray());
            }
        }


        private readonly bool constructFromAllNulls_;
        private readonly ConstructorInfo constructor_;
        private readonly IReadOnlyList<ICreator> argumentCreators_;
    }
}
