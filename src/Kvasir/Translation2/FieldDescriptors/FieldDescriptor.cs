using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   The base class for the data state that manages the translation of a single Field in the back-end database,
    ///   derived from a single scalar (or enumeration) CLR property.
    /// </summary>
    internal abstract class FieldDescriptor {
        /// <summary>
        ///   The CLR type of the Field, accounting for any data conversions.
        /// </summary>
        protected Type FieldType {
            get {
                var type = converter_.Match(some: c => c.ResultType, none: () => source_.PropertyType);
                return Nullable.GetUnderlyingType(type) ?? type;
            }
        }

        /// <summary>
        ///   Mark a Field as either nullable or non-nullable.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the [Nullable] or [NonNullable] annotation was translated via
        ///   reflection.
        /// </param>
        /// <param name="nullable">
        ///   <see langword="true"/> to mark the Field as nullable, <see langword="false"/> to mark the Field as
        ///   non-nullable.
        /// </param>
        /// <exception cref="ConflictingAnnotationsException">
        ///   if this Field has already been marked as nullable or non-nullable.
        /// </exception>
        public void MarkNullability(Context context, bool nullable) {
            Debug.Assert(context is not null);

            if (annotations_.HasFlag(Annotation.Nullability)) {
                throw new ConflictingAnnotationsException(context, typeof(NullableAttribute), typeof(NonNullableAttribute));
            }
            else {
                isNullable_ = nullable;
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="FieldDescriptor"/> that is largely identical to another.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="FieldDescriptor"/>.
        /// </param>
        /// <seealso cref="Clone"/>
        protected FieldDescriptor(FieldDescriptor source) {
            Debug.Assert(source is not null);

            source_ = source.source_;
            name_ = source.name_;
            isNullable_ = source.isNullable_;
            converter_ = source.converter_;
            default_ = source.default_;
            inPrimaryKey_ = source.inPrimaryKey_;
            keyMemberships_ = new HashSet<string>(source.keyMemberships_);
            allowedValues_ = new HashSet<object>(source.allowedValues_);
            disallowedValues_ = new HashSet<object>(source.disallowedValues_);
            checks_ = new List<IConstraintGenerator>(source.checks_);
            annotations_ = Annotation.None;
        }

        /// <summary>
        ///   Constructs a new <see cref="FieldDescriptor"/> that has no Data Converter.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the new <see cref="FieldDescriptor"/> was created via translation.
        /// </param>
        /// <param name="source">
        ///   The <see cref="PropertyInfo">property</see> that underlies the new <see cref="FieldDescriptor"/>.
        /// </param>
        protected FieldDescriptor(Context context, PropertyInfo source) {
            Debug.Assert(source is not null);
            Debug.Assert(context is not null);

            source_ = source;
            name_ = source.Name;
            isNullable_ = new NullabilityInfoContext().Create(source).ReadState == NullabilityState.Nullable;
            column_ = 0;
            converter_ = Option.None<DataConverter>();
            default_ = Option.None<object?>();
            inPrimaryKey_ = false;
            keyMemberships_ = new HashSet<string>();
            allowedValues_ = new HashSet<object>();
            disallowedValues_ = new HashSet<object>();
            checks_ = new List<IConstraintGenerator>();
        }

        /// <summary>
        ///   Constructs a new <see cref="FieldDescriptor"/> that has a Data Converter.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the new <see cref="FieldDescriptor"/> was created via translation.
        /// </param>
        /// <param name="source">
        ///   The <see cref="PropertyInfo">property</see> that underlies the new <see cref="FieldDescriptor"/>.
        /// </param>
        /// <param name="annotation">
        ///   The <see cref="DataConverterAttribute">[DataConverter]</see> annotation applied to
        ///   <paramref name="source"/>.
        /// </param>
        /// <exception cref="InvalidDataConverterException">
        ///   if <paramref name="annotation"/> has a populated
        ///   <see cref="DataConverterAttribute.UserError">user error</see>
        ///     --or--
        ///   if the CLR type of <paramref name="source"/> is incompatible with the
        ///   <see cref="DataConverter.SourceType">expected source type</see> of the Data Converter carried by
        ///   <paramref name="annotation"/>
        ///     --or--
        ///   if the <see cref="DataConverter.ResultType">result type</see> of the Data Converter carried by
        ///   <paramref name="annotation"/> is not supported by Kvasir
        /// </exception>
        protected FieldDescriptor(Context context, PropertyInfo source, DataConverterAttribute annotation)
            : this(context, source) {

            Debug.Assert(annotation is not null);
            Debug.Assert(!converter_.HasValue);

            if (annotation.UserError is not null) {
                throw new InvalidDataConverterException(context, annotation.UserError);
            }
            else if (FieldType.IsInstanceOf(annotation.DataConverter.SourceType)) {
                throw new InvalidDataConverterException(context, FieldType, annotation.DataConverter.SourceType);
            }
            else if (!DBType.IsSupported(annotation.DataConverter.ResultType)) {
                throw new InvalidDataConverterException(context, annotation.DataConverter.ResultType);
            }

            Debug.Assert(annotation.DataConverter.IsBidirectional);
            var conv = annotation.DataConverter;
            converter_ = Option.Some(conv);

            // If the CLR type of the property is an enumeration but the result of the Data Converter is not, then there
            // won't be a restricted image; instead, the enumerators are fed through the Data Converter and the results
            // become the set of allowed values, as if via a [Check.IsOneOf] constraint
            if (conv.SourceType.IsEnum && !conv.ResultType.IsEnum) {
                allowedValues_ = conv.SourceType.ValidValues().Select(e => conv.Convert(e)!).ToHashSet();
            }
        }

        /// <summary>
        ///   Clones this <see cref="FieldDescriptor"/>.
        /// </summary>
        /// <remarks>
        ///   When a <see cref="FieldDescriptor"/> is cloned, the majority of the translation state is copied over to
        ///   the new instance verbatim. However, some state is slightly altered to reflect the fact that cloning occurs
        ///   when the scope of translation changes. In particular, the nullability of a <see cref="FieldDescriptor"/>
        ///   will always be converted into the "native" equivalent of its current value if that value was imparted via
        ///   an annotation. This is to allow [Nullable] and [NonNullable] annotations on aggregates that won't induce
        ///   errors if there was already an annotation on the individual nested property
        /// </remarks>
        /// <returns>
        ///   A deep copy of this <see cref="FieldDescriptor"/> as a brand new instance. Modifications to the returned
        ///   value will not affect the source instance, and vice-versa.
        /// </returns>
        protected abstract FieldDescriptor Clone();

        /// <summary>
        ///   Takes a user-provided value from an annotation (e.g. [Default]) and coerces it into a value that is
        ///   consistent with the type of the Field.
        /// </summary>
        /// <remarks>
        ///   Derived classes are encouraged to override this method to impose additional restrictions on user-provided
        ///   values or to allow the source value to be a different type than the <see cref="FieldType"/>, which the
        ///   default implementation considers to be invalid.
        /// </remarks>
        /// <param name="raw">
        ///   The raw value, as provided by the user.
        /// </param>
        /// <returns>
        ///   If <paramref name="raw"/> is <see langword="null"/>, a <c>SOME</c> instance containing
        ///   <see langword="null"/>
        ///     --else--
        ///   If <paramref name="raw"/> can be coerced into a valid value for a Field of type <see cref="FieldType"/>,
        ///   a <c>SOME</c> instance containing that coercion, which may or may not just be <paramref name="raw"/>
        ///   directly
        ///     --else--
        ///   A <c>NONE</c> instance containing a string explaining why <paramref name="raw"/> could not be coerced
        /// </returns>
        protected virtual Option<object?, string> CoerceUserValue(object? raw) {
            if (raw is null) {
                return Option.Some<object?, string>(null);
            }
            else if (raw.GetType() == FieldType) {
                // Since we know that `raw` comes from an annotation, we know that it must be `null`, a string, a
                // primitive value (int, bool, char, etc.), an enumerator, or an array thereof. As such, we don't need
                // to do any nullable-unwrapping or instance-of checking.
                return Option.Some<object?, string>(raw);
            }
            else if (raw.GetType().IsArray) {
                var msg = "value cannot be an array";
                return Option.None<object?, string>(msg);
            }
            else {
                var msg = $"value {raw.ForDisplay()} is of type {raw.GetType().Name}, not {FieldType.Name} as expected";
                return Option.None<object?, string>(msg);
            }
        }


        // The different annotations (or, in some cases, groups of annotations) for which only one is allowed per Field
        // at a given time. In most cases, it is permissible for multiple annotations of the same type to be applied to
        // a single Field from different locations in the aggregation hierarchy, so the tracking of which annotations
        // have been applied gets cleared when the FieldDescriptor is "cloned."
        [Flags] private enum Annotation {
            None = 0,
            Nullability = 1
        }


        private readonly PropertyInfo source_;
        private string name_;
        private bool isNullable_;
        private int column_;
        private readonly Option<DataConverter> converter_;
        private Option<object?> default_;
        private bool inPrimaryKey_;
        private readonly HashSet<string> keyMemberships_;
        private readonly HashSet<object> allowedValues_;
        private readonly HashSet<object> disallowedValues_;
        private readonly List<IConstraintGenerator> checks_;
        private Annotation annotations_;
    }
}
