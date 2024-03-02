using Cybele.Core;
using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Core;
using Kvasir.Schema;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Type FieldType {
            get {
                var type = converter_.Match(some: c => c.ResultType, none: () => source_.PropertyType);
                return Nullable.GetUnderlyingType(type) ?? type;
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
            nullable_ = (Nullability)((int)source.nullable_ >> 2);
            converter_ = source.converter_;
            inPrimaryKey_ = source.inPrimaryKey_;
            keyMemberships_ = new HashSet<string>(source.keyMemberships_);
            allowedValues_ = new HashSet<object>(source.allowedValues_);
            disallowedValues_ = new HashSet<object>(source.disallowedValues_);
            checks_ = new List<IConstraintGenerator>(source.checks_);
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
            nullable_ = (Nullability)(new NullabilityInfoContext().Create(source).ReadState == NullabilityState.Nullable ? 1 : 2);
            column_ = 0;
            converter_ = Option.None<DataConverter>();
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
            converter_ = Option.Some(annotation.DataConverter);
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


        // The different "kinds" of nullability. Yes, there are a lot of different representations of this (we have the
        // enumeration in the Schema, and there's the C# nullability markings), but we need something specifically to
        // distinguish between the native nullability of a property and the nullability imposed by an annotation,
        // particularly for the purposes of identifying duplicate and conflicting annotations.
        [Flags] private enum Nullability : byte {
            NativeNull = 0b0001,
            NativeNonNull = 0b0010,
            AnnotatedNull = 0b0100,
            AnnotatedNonNull = 0b1000
        }


        private PropertyInfo source_;
        private string name_;
        private Nullability nullable_;
        private int column_;
        private Option<DataConverter> converter_;
        private bool inPrimaryKey_;
        private IReadOnlySet<string> keyMemberships_;
        private IReadOnlySet<object> allowedValues_;
        private IReadOnlySet<object> disallowedValues_;
        private IReadOnlyList<IConstraintGenerator> checks_;
    }
}
