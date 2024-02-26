using Cybele.Core;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   A collection of helper extension methods for attempting an operation and throwing a
    ///   <see cref="FailedOperationException"/> in the event of an error.
    /// </summary>
    internal static class Try {
        /// <summary>
        ///   Attempt to perform a data conversion.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="DataConverter"/> on which the extension method was invoked.
        /// </param>
        /// <param name="source">
        ///   The possibly <see langword="null"/> object to convert.
        /// </param>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the attempted conversion is occurring.
        /// </param>
        /// <returns>
        ///   The conversion of <paramref name="source"/> using <paramref name="self"/>.
        /// </returns>
        /// <exception cref="FailedOperationException">
        ///   if attempting to convert <paramref name="source"/> using <paramref name="self"/> throws an exception.
        /// </exception>
        public static object? TryConvert(this DataConverter self, object? source, Context context) {
            Debug.Assert(self is not null);
            Debug.Assert(context is not null);

            try {
                return self.Convert(source);
            }
            catch (Exception ex) {
                throw new FailedOperationException(context, source, ex);
            }
        }

        /// <summary>
        ///   Attempt to generate a custom simple <c>CHECK</c> constraint.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="CheckAttribute"/> on which the extension method was invoked.
        /// </param>
        /// <param name="field">
        ///   The <see cref="IField">Field</see> that is constituent to the constraint.
        /// </param>
        /// <param name="converter">
        ///   The <see cref="DataConverter"/> that is constituent to the constraint.
        /// </param>
        /// <param name="settings">
        ///   The <see cref="Settings"/> to use.
        /// </param>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="self"/> was encountered.
        /// </param>
        /// <returns>
        ///   The <c>CHECK</c> constraint clause generated from <paramref name="self"/> for <paramref name="field"/>.
        /// </returns>
        /// <exception cref="FailedOperationException">
        ///   if attempting to generate the custom <c>CHECK</c> constraint throws an exception.
        /// </exception>
        public static Clause TryMakeClause(this CheckAttribute self, IField field, DataConverter converter,
            Settings settings, Context context) {
            Debug.Assert(self is not null);
            Debug.Assert(self.UserError is null);
            Debug.Assert(field is not null);
            Debug.Assert(context is not null);

            try {
                var generator = self.ConstraintGenerator;
                return generator.MakeConstraint(Enumerable.Repeat(field, 1), Enumerable.Repeat(converter, 1), settings);
            }
            catch (Exception ex) {
                throw new FailedOperationException(context, self, ex);
            }
        }

        /// <summary>
        ///   Attempt to generate a custom complex <c>CHECK</c> constraint.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="Check.ComplexAttribute"/> on which the extension method was invoked.
        /// </param>
        /// <param name="fields">
        ///   The <see cref="IField">Fields</see> that are constituent to the constraint.
        /// </param>
        /// <param name="converters">
        ///   The <see cref="DataConverter">DataConverters</see> that are constituent to the constraint.
        /// </param>
        /// <param name="settings">
        ///   The <see cref="Settings"/> to use.
        /// </param>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="self"/> was encountered.
        /// </param>
        /// <returns>
        ///   The <c>CHECK</c> constraint generated with <paramref name="self"/> from <paramref name="fields"/> and
        ///   <paramref name="converters"/> via <paramref name="settings"/>.
        /// </returns>
        /// <exception cref="FailedOperationException">
        ///   if attempting to generate the custom <c>CHECK</c> constraint throws an exception.
        /// </exception>
        public static Clause TryMakeClause(this Check.ComplexAttribute self, IEnumerable<IField> fields,
            IEnumerable<DataConverter> converters, Settings settings, Context context) {

            Debug.Assert(self is not null);
            Debug.Assert(fields is not null);
            Debug.Assert(converters is not null);
            Debug.Assert(settings is not null);
            Debug.Assert(context is not null);
            Debug.Assert(fields.Count() == converters.Count());

            try {
                var generator = self.ConstraintGenerator;
                return generator.MakeConstraint(fields, converters, settings);
            }
            catch (Exception ex) {
                throw new FailedOperationException(context, self, ex);
            }
        }
    }
}
