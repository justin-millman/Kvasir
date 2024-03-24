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
    ///   The base class for the data state that manages the translation of a single Field in a back-end database,
    ///   derived from a single scalar (or enumeration) CLR property.
    /// </summary>
    internal abstract class FieldDescriptor {
        /// <summary>
        ///   The CLR type of the Field, accounting for any data conversions.
        /// </summary>
        protected Type FieldType {
            get {
                var type = converter_.Match(some: c => c.ResultType, none: () => clrType_);
                return Nullable.GetUnderlyingType(type) ?? type;
            }
        }

        /// <summary>
        ///   Applies a <see cref="CheckAttribute">[Check]</see> annotation to the Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the <see cref="CheckAttribute">[Check]</see> annotation was translated
        ///   via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The <see cref="CheckAttribute">[Check]</see> annotation.
        /// </param>
        /// <exception cref="InvalidCustomConstraintException">
        ///   if <paramref name="annotation"/> has a populated <see cref="CheckAttribute.UserError">user error</see>.
        /// </exception>
        public void ApplyConstraint(Context context, CheckAttribute annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation is not null);

            if (annotation.UserError is not null) {
                throw new InvalidCustomConstraintException(context, annotation);
            }
            checks_.Add(annotation.ConstraintGenerator);
        }

        /// <summary>
        ///   Applies a <see cref="Check.IsNotAttribute">[Check.IsNot]</see>,
        ///   <see cref="Check.IsLessThanAttribute">[Check.IsLessThan]</see>,
        ///   <see cref="Check.IsGreaterThanAttribute">[Check.IsGreaterThan]</see>,
        ///   <see cref="Check.IsLessOrEqualToAttribute">[Check.IsLessOrEqualTo]</see>, or
        ///   <see cref="Check.IsGreaterOrEqualToAttribute">[Check.IsGreaterOrEqualTo</see> constraint to the Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the comparison constraint annotation was translated via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The comparison constraint annotation.
        /// </param>
        /// <exception cref="InapplicableConstraintException">
        ///   if the Field's <see cref="FieldType">type</see> is not orderable.
        /// </exception>
        /// <exception cref="InvalidConstraintValueException">
        ///   if the anchor value of <paramref name="annotation"/> is invalid.
        /// </exception>
        /// <exception cref="UnsatisfiableConstraintException">
        ///   if the Field has at least one discretely allowed value, and all such values are disallowed by
        ///   <paramref name="annotation"/>.
        /// </exception>
        /// <exception cref="InvalidatedDefaultException">
        ///   if the Field has a default value that is disallowed by <paramref name="annotation"/>.
        /// </exception>
        public void ApplyConstraint(Context context, Check.ComparisonAttribute annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation is not null);

            DoApplyConstraint(context, annotation);
            PostProcessConstraint(context, annotation);
        }

        /// <summary>
        ///   Applies a <see cref="Check.IsOneOfAttribute">[Check.IsOneOf]</see> or
        ///   <see cref="Check.IsNotOneOfAttribute">[Check.IsNotOneOf]</see> constraint to the Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the comparison constraint annotation was translated via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The comparison constraint annotation.
        /// </param>
        /// <exception cref="InvalidConstraintValueException">
        ///   if any of the values of <paramref name="annotation"/> is invalid.
        /// </exception>
        /// <exception cref="UnsatisfiableConstraintException">
        ///   if the Field has at least one discretely allowed value, and all such values are disallowed by
        ///   <paramref name="annotation"/>.
        /// </exception>
        /// <exception cref="InvalidatedDefaultException">
        ///   if the Field has a default value that is disallowed by <paramref name="annotation"/>.
        /// </exception>
        public void ApplyConstraint(Context context, Check.InclusionAttribute annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation is not null);

            DoApplyConstraint(context, annotation);
            PostProcessConstraint(context, annotation);
        }

        /// <summary>
        ///   Applies a <see cref="Check.IsNonZeroAttribute">[Check.IsNonZero]</see>,
        ///   <see cref="Check.IsNegativeAttribute">[Check.IsNegative]</see>, or
        ///   <see cref="Check.IsPositiveAttribute">[Check.IsPositive]</see> constraint to the Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the signedness constraint annotation was translated via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The signedness constraint annotation.
        /// </param>
        /// <exception cref="InapplicableConstraintException">
        ///   if the Field's <see cref="FieldType">type</see> is not numeric.
        /// </exception>
        /// <exception cref="UnsatisfiableConstraintException">
        ///   if the Field has at least one discretely allowed value, and all such values are disallowed by
        ///   <paramref name="annotation"/>.
        /// </exception>
        /// <exception cref="InvalidatedDefaultException">
        ///   if the Field has a default value that is disallowed by <paramref name="annotation"/>.
        /// </exception>
        public void ApplyConstraint(Context context, Check.SignednessAttribute annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation is not null);

            DoApplyConstraint(context, annotation);
            PostProcessConstraint(context, annotation);
        }

        /// <summary>
        ///   Applies a <see cref="Check.IsNonEmptyAttribute">[Check.IsNonEmpty]</see>,
        ///   <see cref="Check.LengthIsAtLeastAttribute">[Check.LengthIsAtLeast</see>,
        ///   <see cref="Check.LengthIsAtMostAttribute">[Check.LengthIsAtMost</see>, or
        ///   <see cref="Check.LengthIsBetweenAttribute"/> constraint to the Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the string length constraint annotation was translated via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The string length constraint annotation.
        /// </param>
        /// <exception cref="InapplicableConstraintException">
        ///   if the Field's <see cref="FieldType">type</see> is not <see cref="string"/>.
        /// </exception>
        /// <exception cref="UnsatisfiableConstraintException">
        ///   if the Field has at least one discretely allowed value, and all such values are disallowed by
        ///   <paramref name="annotation"/>.
        /// </exception>
        /// <exception cref="InvalidatedDefaultException">
        ///   if the Field has a default value that is disallowed by <paramref name="annotation"/>.
        /// </exception>
        public void ApplyConstraint(Context context, Check.StringLengthAttribute annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation is not null);

            DoApplyConstraint(context, annotation);
            PostProcessConstraint(context, annotation);
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
        public abstract FieldDescriptor Clone();

        /// <summary>
        ///   Sets the default value for the Field.
        /// </summary>
        /// <remarks>
        ///   If the Field already has a default value, it will be overwritten. This is only permissible if the original
        ///   default value and the new default value come from different annotations at different translation scopes.
        /// </remarks>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the <see cref="DefaultAttribute">[Default]</see> annotation was
        ///   translated via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The <see cref="DefaultAttribute">[Default]</see> annotation.
        /// </param>
        /// <exception cref="DuplicateAnnotationException">
        ///   if this Field has already been annotated with a <see cref="DefaultAttribute">[Default]</see> attribute  in
        ///   the translation scope described by <paramref name="context"/>.
        /// </exception>
        /// <exception cref="InvalidDefaultException">
        ///   if the value carried by <paramref name="annotation"/> is not valid for the current Field (e.g. it is
        ///   <see langword="null"/> but the Field is not nullable, the value is not of the correct type, etc.).
        /// </exception>
        public void SetDefault(Context context, DefaultAttribute annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation is not null);

            if (annotations_.HasFlag(Annotation.Default)) {
                throw new DuplicateAnnotationException(context, annotation.Path, typeof(DefaultAttribute));
            }

            var value = CoerceUserValue(annotation.Value).Match(
                some: v => v,
                none: msg => throw new InvalidDefaultException(context, annotation.Path, msg)
            );

            if (!IsValidValue(value)) {
                if (value is null) {
                    var msg = $"the default value is {value.ForDisplay()}, but the Field is non-nullable";
                    throw new InvalidDefaultException(context, annotation.Path, msg);
                }
                else {
                    var msg = $"the default value {value.ForDisplay()} does not pass all the Field's constraints";
                    throw new InvalidDefaultException(context, annotation.Path, msg);
                }
            }
            else {
                default_ = Option.Some(value);
                annotations_ |= Annotation.Default;
            }
        }

        /// <summary>
        ///   Marks the Field as being part of a candidate key.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the <see cref="UniqueAttribute">[Unique]</see> annotation was
        ///   translated via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The <see cref="UniqueAttribute">[Unique]</see> annotation.
        /// </param>
        /// <exception cref="InvalidNameException">
        ///   if the name of <paramref name="annotation"/> is <see langword="null"/> or the empty string
        ///     --or--
        ///   if the name of <paramref name="annotation"/> begins with the prefix reserved by Kvasir for anonymous
        ///   candidate keys, but <paramref name="annotation"/> is not itself anonymous.
        /// </exception>
        public void SetInCandidateKey(Context context, UniqueAttribute annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation is not null);
            var ANONYMOUS_PREFIX = UniqueAttribute.ANONYMOUS_PREFIX;

            if (annotation.Name is null || annotation.Name == "") {
                throw new InvalidNameException(context, annotation);
            }
            if (!annotation.IsAnonymous && annotation.Name.StartsWith(ANONYMOUS_PREFIX)) {
                throw new InvalidNameException(context, annotation);
            }
            else if (!annotation.IsAnonymous) {
                keyMemberships_.Add(annotation.Name);
            }
            else {
                // A Field can only be in one anonymous Candidate Key; all the rest are redundant. When a Field is
                // placed into multiple, either directly or indirectly, we take the key whose name is lexicographically
                // later (for determinism).
                var existingAnonymous = keyMemberships_.FirstOrDefault(k => k.StartsWith(ANONYMOUS_PREFIX)) ?? annotation.Name;
                var comp = StringComparer.Ordinal.Compare(existingAnonymous, annotation.Name);
                var key = comp >= 0 ? existingAnonymous : annotation.Name;
                keyMemberships_.Add(key);
            }
        }

        /// <summary>
        ///   Marks the Field as being part of the primary key of the Entity that owns it.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the <see cref="PrimaryKeyAttribute">[PrimaryKey]</see> annotation was
        ///   translated via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The <see cref="PrimaryKeyAttribute">[PrimaryKey]</see> annotation.
        /// </param>
        /// <param name="cascadePath">
        ///   The access path in between the property to which <paramref name="annotation"/> applies, accounting for a
        ///   possible <see cref="PrimaryKeyAttribute.Path">nested path</see>, and the Field. This parameter defaults to
        ///   the empty string and should be non-empty when <paramref name="annotation"/> applies to an Aggregate. This
        ///   argument is only used for contextualizing error messages.
        /// </param>
        /// <exception cref="InvalidPrimaryKeyFieldException">
        ///   if the depth of <paramref name="context"/> is not <c>0</c>, indicating that the annotation was placed
        ///   on a property nested within an Aggregate
        ///     --or--
        ///   if the Field is nullable
        /// </exception>
        public void SetInPrimaryKey(Context context, PrimaryKeyAttribute annotation, string cascadePath = "") {
            Debug.Assert(context is not null);
            Debug.Assert(annotation is not null);
            Debug.Assert(cascadePath is not null);

            if (context.Depth != 0) {
                var msg = "nested properties cannot be directly annotated as part of an Entity's primary key";
                throw new InvalidPrimaryKeyFieldException(context, annotation.Path, cascadePath, msg);
            }
            else if (isNullable_) {
                var msg = "a nullable Field cannot be part of an Entity's primary key";
                throw new InvalidPrimaryKeyFieldException(context, annotation.Path, cascadePath, msg);
            }
            else {
                inPrimaryKey_ = true;
            }
        }

        /// <summary>
        ///   Sets the name of the Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the <see cref="NameAttribute">[Name]</see> annotation was translated
        ///   via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The <see cref="NameAttribute">[Name]</see> annotation.
        /// </param>
        /// <exception cref="DuplicateAnnotationException">
        ///   if the name of the Field has already been set.
        /// </exception>
        /// <exception cref="InvalidNameException">
        ///   if the name imparted by <paramref name="annotation"/> is either <see langword="null"/> or the empty
        ///   string.
        /// </exception>
        public void SetName(Context context, NameAttribute annotation) {
            Debug.Assert(context is not null);
            Debug.Assert(annotation is not null);

            if (annotations_.HasFlag(Annotation.Name)) {
                throw new DuplicateAnnotationException(context, annotation.Path, typeof(NameAttribute));
            }
            else if (annotation.Name is null || annotation.Name == "") {
                throw new InvalidNameException(context, annotation);
            }

            name_.SetNamePart(annotation.Name);
            annotations_ |= Annotation.Name;
        }

        /// <summary>
        ///   Sets the name prefix of the Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="prefix"/> was determined. (This parameter is not used,
        ///   but is kept for symmetry with other methods and as a forward compatibility mechanism.)
        /// </param>
        /// <param name="prefix">
        ///   The suffix of the name's prefix.
        /// </param>
        public void SetNamePrefix(Context context, IReadOnlyList<string> prefix) {
            Debug.Assert(context is not null);
            Debug.Assert(prefix is not null && !prefix.IsEmpty());
            Debug.Assert(prefix.None(s => s is null || s == ""));

            name_.SetPrefixPart(prefix);
        }

        /// <summary>
        ///   Sets the nullability or non-nullability of the Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the <see cref="NullableAttribute">[Nullable]</see> or
        ///   <see cref="NonNullableAttribute">[NonNullable]</see> annotation was translated via reflection. (This
        ///   parameter is not used, but is kept for symmetry with other methods and as a forward compatibility
        ///   mechanism.)
        /// </param>
        /// <param name="nullable">
        ///   <see langword="true"/> to mark the Field as nullable, <see langword="false"/> to mark the Field as
        ///   non-nullable.
        /// </param>
        public void SetNullability(Context context, bool nullable) {
            Debug.Assert(context is not null);
            Debug.Assert(nullable || !default_.HasValue);       // Nullability must be processed before [Default]
            Debug.Assert(!inPrimaryKey_);                       // Nullability must be processed before [PrimaryKey]

            isNullable_ = nullable;
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

            clrType_ = source.clrType_;
            name_ = new FieldName(source.name_);
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

            clrType_ = source.PropertyType;
            name_ = new FieldName(source.Name);
            isNullable_ = new NullabilityInfoContext().Create(source).ReadState == NullabilityState.Nullable;
            converter_ = Option.None<DataConverter>();
            default_ = Option.None<object?>();
            inPrimaryKey_ = false;
            keyMemberships_ = new HashSet<string>();
            allowedValues_ = new HashSet<object>();
            disallowedValues_ = new HashSet<object>();
            checks_ = new List<IConstraintGenerator>();
        }

        /// <summary>
        ///   Constructs a new <see cref="FieldDescriptor"/> that has a Data Converter defined by a
        ///   <c>[DataConverter]</c> annotation.
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

            Debug.Assert(annotation.DataConverter.IsBidirectional);
            var conv = annotation.DataConverter;
            converter_ = Option.Some(conv);

            if (FieldType.IsInstanceOf(annotation.DataConverter.SourceType)) {
                throw new InvalidDataConverterException(context, FieldType, annotation.DataConverter.SourceType);
            }
            else if (!DBType.IsSupported(annotation.DataConverter.ResultType)) {
                throw new InvalidDataConverterException(context, annotation.DataConverter.ResultType);
            }

            // If the CLR type of the property is an enumeration but the result of the Data Converter is not, then there
            // won't be a restricted image; instead, the enumerators are fed through the Data Converter and the results
            // become the set of allowed values, as if via a [Check.IsOneOf] constraint
            if (conv.SourceType.IsEnum && !conv.ResultType.IsEnum) {
                allowedValues_ = conv.SourceType.ValidValues().Select(e => conv.Convert(e)!).ToHashSet();
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="FieldDescriptor"/> that has a Data Converter defined by something other than
        ///   a <c>[DataConveter]</c> annotation.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the new <see cref="FieldDescriptor"/> was created via translation.
        /// </param>
        /// <param name="source">
        ///   The <see cref="PropertyInfo">property</see> that underlies the new <see cref="FieldDescriptor"/>.
        /// </param>
        /// <param name="converter">
        ///   The <see cref="DataConverter"/> for the new <see cref="FieldDescriptor"/>. This must be a bidirectional
        ///   Data Converter that converts from a type compatible with <paramref name="source"/> into a type supported
        ///   by Kvasir.
        /// </param>
        protected FieldDescriptor(Context context, PropertyInfo source, DataConverter converter)
            : this(context, source) {

            Debug.Assert(converter is not null);
            Debug.Assert(!converter_.HasValue);
            Debug.Assert(converter.IsBidirectional);

            converter_ = Option.Some(converter);
            Debug.Assert(FieldType.IsInstanceOf(converter.SourceType));
            Debug.Assert(DBType.IsSupported(converter.ResultType));

            // If the CLR type of the property is an enumeration but the result of the Data Converter is not, then there
            // won't be a restricted image; instead, the enumerators are fed through the Data Converter and the results
            // become the set of allowed values, as if via a [Check.IsOneOf] constraint
            if (converter.SourceType.IsEnum && !converter.ResultType.IsEnum) {
                allowedValues_ = converter.SourceType.ValidValues().Select(e => converter.Convert(e)!).ToHashSet();
            }
        }

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
            if (raw is null || raw == DBNull.Value) {
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

        /// <seealso cref="ApplyConstraint(Context, Check.ComparisonAttribute)"/>
        /// <remarks>Intended to be overridden by derived classes for which the constraint is applicable.</remarks>
        protected virtual void DoApplyConstraint(Context context, Check.ComparisonAttribute annotation) {
            throw new InapplicableConstraintException(context, annotation, FieldType);
        }

        /// <seealso cref="ApplyConstraint(Context, Check.InclusionAttribute)"/>
        /// <remarks>Separated from the public API to enable translation of [Check.IsNot] constraints.</remarks>
        /// <remarks>Non-virtual because there's no custom handling, protected for managing enumerators.</remarks>
        protected void DoApplyConstraint(Context context, Check.InclusionAttribute annotation) {
            // The [Check.IsOneOf] and [Check.IsNotOneOf] constructors enforce that at least one value be provided, so
            // it's impossible for the anchor list to be empty

            if (annotation.Anchor.Any(v => v is null)) {
                object? n = null;
                throw new InvalidConstraintValueException(context, annotation, $"constraint cannot contain {n.ForDisplay()}");
            }

            var values = annotation.Anchor.Select(
                v => CoerceUserValue(v).Match(
                    some: v => v!,
                    none: msg => throw new InvalidConstraintValueException(context, annotation, msg)
                )
            );

            if (annotation.Operator == InclusionOperator.In) {
                allowedValues_.UnionWith(values);
            }
            else {
                disallowedValues_.UnionWith(values);
            }

            // If a value is disallowed, we have to remove it from the set of allowed values. Note that we don't clear
            // the set of disallowed values just because we have at least one allowed value, because that could lead to
            // future [Check.IsOneOf] constraints re-introducing the allowed value.
            allowedValues_.ExceptWith(disallowedValues_);
        }

        /// <seealso cref="ApplyConstraint(Context, Check.SignednessAttribute)"/>
        /// <remarks>Intended to be overridden by derived classes for which the constraint is applicable.</remarks>
        protected virtual void DoApplyConstraint(Context context, Check.SignednessAttribute annotation) {
            throw new InapplicableConstraintException(context, annotation, FieldType);
        }

        /// <seealso cref="ApplyConstraint(Context, Check.StringLengthAttribute)"/>
        /// <remarks>Intended to be overridden by derived classes for which the constraint is applicable.</remarks>
        protected virtual void DoApplyConstraint(Context context, Check.StringLengthAttribute annotation) {
            throw new InapplicableConstraintException(context, annotation, FieldType);
        }

        /// <summary>
        ///   Determines if a user-provided value, guaranteed to be of the correct type, is a valid value for the Field,
        ///   accounting for nullability and any active constraints.
        /// </summary>
        /// <param name="value">
        ///   The proposed value.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="value"/> is a valid value for this Field; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        protected virtual bool IsValidValue(object? value) {
            Debug.Assert(value is null || value.GetType() == FieldType);

            if (value is null) {
                return isNullable_;
            }
            else if (!allowedValues_.IsEmpty()) {
                return allowedValues_.Contains(value);
            }
            else if (!disallowedValues_.IsEmpty()) {
                return !disallowedValues_.Contains(value);
            }
            else {
                return true;
            }
        }

        /// <summary>
        ///   Updates the allowed/disallowed value and performs additional consistency checking after a constraint has
        ///   been applied.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the constraint annotation was translated via reflection.
        /// </param>
        /// <param name="annotation">
        ///   The constraint annotation that was just applied.
        /// </param>
        /// <exception cref="UnsatisfiableConstraintException">
        ///   if the Field currently has at least one discretely allowed value, and all such values are no longer valid.
        /// </exception>
        /// <exception cref="InvalidatedDefaultException">
        ///   if the Field has a default value that is no longer valid.
        /// </exception>
        private void PostProcessConstraint(Context context, INestableAnnotation annotation) {
            disallowedValues_.RemoveWhere(v => !IsValidValue(v));

            if (allowedValues_.RemoveWhere(v => !IsValidValue(v)) > 0 && allowedValues_.IsEmpty()) {
                throw new UnsatisfiableConstraintException(context, annotation);
            }

            if (default_.Exists(v => !IsValidValue(v))) {
                throw new InvalidatedDefaultException(context, default_.Unwrap(), annotation);
            }
        }


        // The different annotations (or, in some cases, groups of annotations) for which only one is allowed per Field
        // at a given time. In most cases, it is permissible for multiple annotations of the same type to be applied to
        // a single Field from different locations in the aggregation hierarchy, so the tracking of which annotations
        // have been applied gets cleared when the FieldDescriptor is "cloned."
        [Flags] private enum Annotation {
            None = 0,
            Default = 1,
            Name = 2
        }


        private readonly Type clrType_;
        private readonly FieldName name_;
        private bool isNullable_;
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
