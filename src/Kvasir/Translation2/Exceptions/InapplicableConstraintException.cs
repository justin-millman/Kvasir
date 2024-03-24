using Kvasir.Annotations;
using System;
using System.Diagnostics;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   An exception that is raised when a constraint is placed on a property for which it is inapplicable.
    /// </summary>
    internal sealed class InapplicableConstraintException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InapplicableConstraintException"/> caused by an annotation being placed on an
        ///   Aggregate property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable constraint.
        /// </param>
        /// <param name="propertyType">
        ///   The type of the property to which <paramref name="annotation"/> was applied.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        public InapplicableConstraintException(Context context, INestableAnnotation annotation, Type propertyType, AggregateTag _)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a property of Aggregate type {propertyType}"),
                new Annotation(annotation.GetType().Name[..^9])
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InapplicableConstraintException"/> caused by an annotation being placed on an
        ///   Reference property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable constraint.
        /// </param>
        /// <param name="propertyType">
        ///   The type of the property to which <paramref name="annotation"/> was applied.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        public InapplicableConstraintException(Context context, INestableAnnotation annotation, Type propertyType, ReferenceTag _)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a property of Reference type {propertyType}"),
                new Annotation(annotation.GetType().Name[..^9])
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InapplicableConstraintException"/> caused by an annotation being placed on an
        ///   Relation property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable constraint.
        /// </param>
        /// <param name="propertyType">
        ///   The type of the property to which <paramref name="annotation"/> was applied.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        public InapplicableConstraintException(Context context, INestableAnnotation annotation, Type propertyType, RelationTag _)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a property of Relation type {propertyType}"),
                new Annotation(annotation.GetType().Name[..^9])
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InapplicableConstraintException"/> caused by a
        ///   <see cref="Check.ComparisonAttribute">comparison constraint annotation</see> being placed on a scalar
        ///   property corresponding to a non-orderable Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable constraint.
        /// </param>
        /// <param name="fieldType">
        ///   The data type of the Field to which <paramref name="annotation"/> was applied, accounting for any data
        ///   conversions.
        /// </param>
        public InapplicableConstraintException(Context context, Check.ComparisonAttribute annotation, Type fieldType)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a Field of non-orderable type {fieldType}"),
                new Annotation(annotation.GetType().Name[..^9])
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InapplicableConstraintException"/> caused by a
        ///   <see cref="Check.SignednessAttribute">signedness constraint annotation</see> being placed on a scalar
        ///   property corresponding to a non-numeric Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable constraint.
        /// </param>
        /// <param name="fieldType">
        ///   The data type of the Field to which <paramref name="annotation"/> was applied, accounting for any data
        ///   conversions.
        /// </param>
        public InapplicableConstraintException(Context context, Check.SignednessAttribute annotation, Type fieldType)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a Field of non-numeric type {fieldType}"),
                new Annotation(annotation.GetType().Name[..^9])
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InapplicableConstraintException"/> caused by a
        ///   <see cref="Check.IsNegativeAttribute">[Check.IsNegative] annotation</see> being placed on a scalar
        ///   property corresponding to an unsigned numeric Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable constraint.
        /// </param>
        /// <param name="fieldType">
        ///   The data type of the Field to which <paramref name="annotation"/> was applied, accounting for any data
        ///   conversions.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        public InapplicableConstraintException(Context context, Check.SignednessAttribute annotation, Type fieldType, UnsignedTag _)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a Field of unsigned numeric type {fieldType}"),
                new Annotation(annotation.GetType().Name[..^9])
              )
        {
            Debug.Assert(annotation.GetType() == typeof(Check.IsNegativeAttribute));
        }

        /// <summary>
        ///   Constructs a new <see cref="InapplicableConstraintException"/> caused by a
        ///   <see cref="Check.StringLengthAttribute">string length constraint annotation</see> being placed on a scalar
        ///   property corresponding to a non-string Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable constraint.
        /// </param>
        /// <param name="fieldType">
        ///   The data type of the Field to which <paramref name="annotation"/> was applied, accounting for any data
        ///   conversions.
        /// </param>
        public InapplicableConstraintException(Context context, Check.StringLengthAttribute annotation, Type fieldType)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a Field of non-string type {fieldType}"),
                new Annotation(annotation.GetType().Name[..^9])
              )
        {}
    }


    // Overload dispatch types
    internal readonly struct AggregateTag {}
    internal readonly struct ReferenceTag {}
    internal readonly struct RelationTag {}
    internal readonly struct UnsignedTag {}
}
