using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kvasir.Core {
    /// <summary>
    ///   A set of internal utilities for converting a value provided by the user through the front-end UI.
    /// </summary>
    internal static class TransformUserValues {
        /// <summary>
        ///   Transforms a user-provided value--either a single object or a set thereof--so that it of the correct form
        ///   for a particular Field.
        /// </summary>
        /// <remarks>
        ///   This function is largely responsible for performing the following actions:
        ///     <list type="bullet">
        ///       <item>Converting enumerator values to their string or numeric representations</item>
        ///       <item>Converting any other type of value according to a user-specified Data Converter</item>
        ///       <item>Parsing a string into a Guid for Guid-typed Fields</item>
        ///       <item>Parsing a string into a DateTime for DateTime-typed Fields</item>
        ///     </list>
        /// </remarks>
        /// <param name="value">
        ///   The user-provided value.
        /// </param>
        /// <param name="field">
        ///   The Field to which <paramref name="value"/> belongs.
        /// </param>
        /// <param name="converter">
        ///   The <see cref="DataConverter"/> for <paramref name="field"/>.
        /// </param>
        /// <param name="settings">
        ///   The Kvasir framework settings.
        /// </param>
        /// <returns>
        ///   The result of converting <paramref name="value"/> according to <paramref name="field"/>,
        ///   <paramref name="converter"/>, and <paramref name="settings"/>. It will either be a single
        ///   <see cref="DBValue"/> or a set thereof.
        /// </returns>
        internal static object Transform(object? value, IField field, DataConverter converter, Settings settings) {
            // string is an IEnumerable<char>, but we don't want to treat them as Enumerables
            if (value is not string && value is IEnumerable e) {
                return DoTransform(e.Cast<object?>(), field, converter, settings);
            }
            return DoTransform(value, field, converter, settings);
        }

        /// <see cref="Transform(object?, IField, DataConverter, Settings)"/>
        private static DBValue DoTransform(object? val, IField field, DataConverter converter,
            Settings settings) {
            
            Debug.Assert(field is not null);
            Debug.Assert(converter is not null);
            Debug.Assert(settings is not null);

            if (val is null) {
                return DBValue.Create(null);
            }
            else if (field.DataType == DBType.Enumeration) {
                return DBValue.Create(converter.Convert(val));
            }
            else if (field.DataType == DBType.Guid) {
                Debug.Assert(val is string);
                return new DBValue(Guid.Parse((string)val));
            }
            else if (field.DataType == DBType.DateTime) {
                Debug.Assert(val is string);
                return new DBValue(DateTime.Parse((string)val, null));
            }
            else {
                return DBValue.Create(val);
            }
        }

        /// <see cref="Transform(object?, IField, DataConverter, Settings)"/>
        private static IEnumerable<DBValue> DoTransform(IEnumerable<object?> val, IField field,
            DataConverter converter, Settings settings) {

            Debug.Assert(val is not null);
            Debug.Assert(field is not null);
            Debug.Assert(converter is not null);
            Debug.Assert(settings is not null);
            Debug.Assert(!val.IsEmpty());

            return val.Select(o => DoTransform(o, field, converter, settings));
        }
    }
}
