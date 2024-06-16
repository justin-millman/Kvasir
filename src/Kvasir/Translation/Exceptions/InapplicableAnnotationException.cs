using Kvasir.Annotations;
using System;
using System.Diagnostics;

namespace Kvasir.Translation {
    /// <summary>
    ///   An exception that is raised when an annotation is placed on, or applies to, a property for which it is
    ///   inapplicable.
    /// </summary>
    internal sealed class InapplicableAnnotationException : TranslationException {
        /// <summary>
        ///   Constructs a new <see cref="InapplicableAnnotationException"/> caused by a non-constraint annotation being
        ///   placed on an Aggregate property, a Reference property, or a Relation property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable annotation was encountered.
        /// </param>
        /// <param name="annotationType">
        ///   The type of the inapplicable annotation.
        /// </param>
        /// <param name="propertyType">
        ///   The type of the property to which the inapplicable annotation was applied.
        /// </param>
        /// <param name="kind">
        ///   The kind of property on which the annotation was erroneously placed.
        /// </param>
        public InapplicableAnnotationException(Context context, Type annotationType, Type propertyType, MultiKind kind)
            : base(
                new Location(context.ToString()),
                new Problem($"the annotation cannot be applied to a property of {kind} type {propertyType.DisplayName()}"),
                new Annotation(Display.AnnotationDisplayName(annotationType))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InapplicableAnnotationException"/> caused by a constraint annotation being
        ///   placed on or applying to, an Aggregate property, a Reference property, or a Relation property.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable constraint annotation was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable constraint annotation.
        /// </param>
        /// <param name="propertyType">
        ///   The type of the property to which <paramref name="annotation"/> was applied.
        /// </param>
        /// <param name="kind">
        ///   The kind of property on which the annotation was erroneously placed.
        /// </param>
        public InapplicableAnnotationException(Context context, INestableAnnotation annotation, Type propertyType, MultiKind kind)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a property of {kind} type {propertyType.DisplayName()}"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InapplicableAnnotationException"/> caused by a
        ///   <see cref="Check.ComparisonAttribute">comparison constraint annotation</see> being placed on, or applying
        ///   to, a scalar property corresponding to a non-orderable Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable comparison constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable comparison constraint.
        /// </param>
        /// <param name="fieldType">
        ///   The data type of the Field to which <paramref name="annotation"/> was applied, accounting for any data
        ///   conversions.
        /// </param>
        public InapplicableAnnotationException(Context context, Check.ComparisonAttribute annotation, Type fieldType)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a Field of non-orderable type {fieldType.DisplayName()}"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InapplicableAnnotationException"/> caused by a
        ///   <see cref="Check.SignednessAttribute">signedness constraint annotation</see> being placed on, or applying
        ///   to, a scalar property corresponding to a non-numeric Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable signedness constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable signedness constraint.
        /// </param>
        /// <param name="fieldType">
        ///   The data type of the Field to which <paramref name="annotation"/> was applied, accounting for any data
        ///   conversions.
        /// </param>
        public InapplicableAnnotationException(Context context, Check.SignednessAttribute annotation, Type fieldType)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a Field of non-numeric type {fieldType.DisplayName()}"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}

        /// <summary>
        ///   Constructs a new <see cref="InapplicableAnnotationException"/> caused by a
        ///   <see cref="Check.IsNegativeAttribute">[Check.IsNegative] annotation</see> being placed on, or applying to,
        ///   a scalar property corresponding to an unsigned numeric Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable <c>[Check.IsNegative]</c> constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable <c>[Check.IsNegative]</c> constraint.
        /// </param>
        /// <param name="fieldType">
        ///   The data type of the Field to which <paramref name="annotation"/> was applied, accounting for any data
        ///   conversions.
        /// </param>
        /// <param name="_">
        ///   <i>overload discriminator</i>
        /// </param>
        public InapplicableAnnotationException(Context context, Check.SignednessAttribute annotation, Type fieldType, UnsignedTag _)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a Field of unsigned numeric type {fieldType.DisplayName()}"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {
            Debug.Assert(annotation.GetType() == typeof(Check.IsNegativeAttribute));
        }

        /// <summary>
        ///   Constructs a new <see cref="InapplicableAnnotationException"/> caused by a
        ///   <see cref="Check.StringLengthAttribute">string length constraint annotation</see> being placed on, or
        ///   applying to, a scalar property corresponding to a non-string Field.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the inapplicable string length constraint was encountered.
        /// </param>
        /// <param name="annotation">
        ///   The inapplicable string length constraint.
        /// </param>
        /// <param name="fieldType">
        ///   The data type of the Field to which <paramref name="annotation"/> was applied, accounting for any data
        ///   conversions.
        /// </param>
        public InapplicableAnnotationException(Context context, Check.StringLengthAttribute annotation, Type fieldType)
            : base(
                new Location(context.ToString()),
                new Path(annotation.Path),
                new Problem($"the annotation cannot be applied to a Field of non-string type {fieldType.DisplayName()}"),
                new Annotation(Display.AnnotationDisplayName(annotation.GetType()))
              )
        {}
    }


    // Discrimination types
    internal enum MultiKind { Aggregate, Reference, Relation }
    internal readonly struct UnsignedTag {}
}
