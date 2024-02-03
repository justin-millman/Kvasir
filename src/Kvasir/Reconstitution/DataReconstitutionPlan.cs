using Ardalis.GuardClauses;
using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Reconstitution {
    /// <summary>
    ///   A description of the way in which data for a particular CLR object type is to be extracted from a back-end
    ///   database, transformed, and re-imagined as an instance.
    /// </summary>
    public sealed class DataReconstitutionPlan {
        /// <summary>
        ///   The <see cref="Type"/> of CLR object produced by this <see cref="DataReconstitutionPlan"/>.
        /// </summary>
        public Type Target { get; }

        /// <summary>
        ///   Construct a new <see cref="DataReconstitutionPlan"/>.
        /// </summary>
        /// <param name="reconstitutor">
        ///   The <see cref="IReconstitutor"/> with which to produce the CLR object when the new
        ///   <see cref="DataReconstitutionPlan"/> is executed.
        /// </param>
        /// <param name="reverters">
        ///   The ordered sequence of <see cref="DataConverter">data converters</see> that transform the values
        ///   extracted from the back-end database..
        /// </param>
        /// <pre>
        ///   <paramref name="reverters"/> is not empty
        ///     --and--
        ///   each element of <paramref name="reverters"/> is a bidirectional <see cref="DataConverter"/>.
        /// </pre>
        internal DataReconstitutionPlan(IReconstitutor reconstitutor, ConverterSeq reverters) {
            Guard.Against.Null(reconstitutor, nameof(reconstitutor));
            Guard.Against.Null(reverters, nameof(reverters));
            Debug.Assert(!reverters.IsEmpty());
            Debug.Assert(reverters.All(dc => dc.IsBidirectional));

            Target = reconstitutor.Target;
            reconstitutor_ = reconstitutor;
            reverters_ = reverters;
        }

        /// <summary>
        ///   Execute this <see cref="DataReconstitutionPlan"/> to create a brand new CLR object from a "row" of
        ///   database values.
        /// </summary>
        /// <param name="rawValues">
        ///   The database values from which to reconstitute a CLR object.
        /// </param>
        /// <pre>
        ///   <paramref name="rawValues"/> is not empty.
        /// </pre>
        /// <returns>
        ///   A CLR object of type <see cref="Target"/> that, when run through the dedicated extractor for
        ///   <see cref="Target"/>, produces <paramref name="rawValues"/>.
        /// </returns>
        public object ReconstituteFrom(DBData rawValues) {
            Guard.Against.Null(rawValues, nameof(rawValues));
            Debug.Assert(!rawValues.IsEmpty());

            var reversions = new List<object?>();
            var reverterIter = reverters_.GetEnumerator();

            foreach (var value in rawValues) {
                reverterIter.MoveNext();
                var datum = value == DBValue.NULL ? null : value.Datum;
                reversions.Add(reverterIter.Current.Revert(datum));
            }

            return reconstitutor_.ReconstituteFrom(reversions)!;
        }


        private readonly IReconstitutor reconstitutor_;
        private readonly ConverterSeq reverters_;
    }
}
