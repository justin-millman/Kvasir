using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Exceptions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Ctxt = Kvasir.Translation.PropertyTranslationContext;

namespace Kvasir.Translation {
    /// <summary>
    ///   A strongly typed "pair" indicating the "context" in which a user-supplied annotation is translated.
    /// </summary>
    /// <param name="Property">
    ///   The property being translated. This is the property on which an annotation has been applied.
    /// </param>
    /// <param name="Path">
    ///   The nested path to which the annotation applies. The empty string indicates that it applies exactly to
    ///   <paramref name="Property"/>. A value of <see langword="null"/> is always invalid and should only appear when
    ///   detecting an <see cref="Error.InvalidPath(Ctxt, Attribute)">invalid <c>Path</c></see>.
    /// </param>
    internal readonly record struct PropertyTranslationContext(PropertyInfo Property, string? Path);

    /// <summary>
    ///   A collection of static methods for systematically generating <see cref="KvasirException">translation
    ///   exceptions</see> with contextualized, actionable error messages.
    /// </summary>
    internal static class Error {
        //////////////////////// Property-Scope Errors ////////////////////////

        /// <summary>
        ///   Create a KvasirException describing a property whose type is not supported by Kvasir.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the unsupported property type was detected.
        /// </param>
        /// <param name="reason">
        ///   The reason that the <see cref="PropertyInfo.PropertyType">type</see> of the property is not supported.
        /// </param>
        /// <param name="annotated">
        ///   <see langword="true"/> if the property was annotated with an <see cref="IncludeInModelAttribute"/>;
        ///   otherwise, false.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException UnsupportedType(Ctxt ctxt, string reason, bool annotated) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path == "");
            Debug.Assert(reason is not null && reason != "");

            var location = $"property '{ctxt.Property.Name}' of type {ctxt.Property.ReflectedType!.Name}";
            var problem = $"property has unsupported type {ctxt.Property.PropertyType.Name} ({reason})";

            if (annotated) {
                var annotations = new Type[] { typeof(IncludeInModelAttribute) };
                return MakeError(ctxt, annotations, problem);
            }
            else {
                return new KvasirException(
                    "Error Performing Translation:\n" +
                    $"  • Location: {location}\n" +
                    $"  • Problem: {problem}"
                );
            }
        }
        
        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing an invalid <c>Path</c> property on an annotation.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the invalid <c>Path</c> was detected.
        /// </param>
        /// <param name="annotation">
        ///   The annotation with the invalid <c>Path</c>.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException InvalidPath(Ctxt ctxt, Attribute annotation) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(annotation is not null);
            var annotations = new Type[] { annotation.GetType() };

            if (ctxt.Path is null) {
                return MakeError(ctxt with { Path = "" }, annotations, "path is null");
            }
            else if (ctxt.Path == "") {
                return MakeError(ctxt, annotations, "path is required when placed on non-scalar");
            }
            else {
                return MakeError(ctxt, annotations, $"path \"{ctxt.Path}\" does not exist (or refers to a non-scalar)");
            }
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing a property that was annotated to be included in the data
        ///   model but cannot be.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the invalid inclusion was detected.
        /// </param>
        /// <param name="reason">
        ///   An explanation as to <i>why</i> the property of <paramref name="ctxt"/> cannot be included in the data
        ///   model.
        /// </param>
        public static KvasirException CannotIncludeInModel(Ctxt ctxt, string reason) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path == "");
            Debug.Assert(reason is not null && reason != "");

            var annotations = new Type[] { typeof(IncludeInModelAttribute) };
            return MakeError(ctxt, annotations, reason);
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing an annotation that imparts an invalid name on some
        ///   schematic component at property scope.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the invalid name was detected.
        /// </param>
        /// <param name="annotation">
        ///   The annotation with the invalid name.
        /// </param>
        /// <param name="name">
        ///   The invalid name.
        /// </param>
        /// <param name="reason">
        ///   An explanation as to why <paramref name="name"/> is invalid. If <paramref name="name"/> is
        ///   <see langword="null"/> or is the empty string, the reason should be omitted (as it will be synthesized
        ///   automatically).
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException InvalidName(Ctxt ctxt, Attribute annotation, string? name, string reason = "") {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path is not null);
            Debug.Assert(annotation is not null);
            Debug.Assert(reason is not null);
            var annotations = new Type[] { annotation.GetType() };

            if (name is null) {
                Debug.Assert(reason == "");
                return MakeError(ctxt, annotations, "name is null");
            }
            else if (name == "") {
                Debug.Assert(reason == "");
                var msg = $"name {name.ForDisplay()} is invalid (it is the empty string)";
                return MakeError(ctxt, annotations, msg);
            }
            else {
                var msg = $"name {name.ForDisplay()} is invalid ({reason})";
                return MakeError(ctxt, annotations, msg);
            }
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing two or more annotations of the same type being applied
        ///   to the same (possibly nested) property.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the duplicated annotation was detected.
        /// </param>
        /// <param name="annotation">
        ///   One of the duplicated annotations; it does not matter which one.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException DuplicateAnnotation(Ctxt ctxt, Attribute annotation) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path is not null);
            Debug.Assert(annotation is not null);

            var annotations = new Type[] { annotation.GetType() };
            return MakeError(ctxt, annotations, "annotation is duplicated");
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing two mutually exclusive annotations that have been
        ///   applied to the same (possibly nested) property.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the mutually exclusive annotations were detected.
        /// </param>
        /// <param name="first">
        ///   The first of the two mutually exclusive annotations; the relative order does not matter.
        /// </param>
        /// <param name="second">
        ///   The second of the two mutually exclusive annotations; the relative order does not matter.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException MutuallyExclusive(Ctxt ctxt, Attribute first, Attribute second) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path is not null);
            Debug.Assert(first is not null);
            Debug.Assert(second is not null);

            var annotations = new Type[] { first.GetType(), second.GetType() };
            return MakeError(ctxt, annotations, "annotations are mutually exclusive");
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing a user-provided value that does not meet the type or
        ///   semantic requirements of the annotation to which it was provided.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the invalid user-provided value was detected.
        /// </param>
        /// <param name="annotation">
        ///   The annotation carrying the invalid user-provided value.
        /// </param>
        /// <param name="value">
        ///   The invalid user-provided value.
        /// </param>
        /// <param name="reason">
        ///   An explanation as to <i>why</i> <paramref name="value"/> is invalid.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException BadValue(Ctxt ctxt, Attribute annotation, object value, string reason) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path is not null);
            Debug.Assert(annotation is not null);
            Debug.Assert(value is not null);
            Debug.Assert(reason is not null && reason != "");

            var annotations = new Type[] { annotation.GetType() };
            var msg = value == DBNull.Value ?
                $"user-provided value {value.ForDisplay()} is invalid ({reason})" :
                $"user-provided value {value.ForDisplay()} of type {value.GetType().Name} is invalid ({reason})";
            return MakeError(ctxt, annotations, msg);
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing a constraint annotation that cannot be applied to the
        ///   Field on which it is placed.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the inapplicable constraint was detected.
        /// </param>
        /// <param name="annotation">
        ///   The annotation of the inapplicable constraint.
        /// </param>
        /// <param name="reason">
        ///   An explanation as to <i>why</i> <paramref name="annotation"/> is inapplicable.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException InapplicableConstraint(Ctxt ctxt, Attribute annotation, string reason) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path is not null);
            Debug.Assert(annotation is not null);
            Debug.Assert(reason is not null && reason != "");

            var annotations = new Type[] { annotation.GetType() };
            var msg = $"constraint is inapplicable ({reason})";
            return MakeError(ctxt, annotations, msg);
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing a constraint annotation that cannot be satisfied.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the unsatisfiable constraint was detected.
        /// </param>
        /// <param name="annotation">
        ///   The annotation of the unsatisfiable constraint.
        /// </param>
        /// <param name="reason">
        ///   An explanation as to <i>why</i> <paramref name="annotation"/> is unsatisfiable.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException UnsatisfiableConstraint(Ctxt ctxt, Attribute annotation, string reason) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path is not null);
            Debug.Assert(annotation is not null);
            Debug.Assert(reason is not null && reason != "");

            var annotations = new Type[] { annotation.GetType() };
            var msg = $"constraint is unsatisfiable ({reason})";
            return MakeError(ctxt, annotations, msg);
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing two or more constraint annotations that are conflicting.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the unsatisfiable constraint was detected.
        /// </param>
        /// <param name="reason">
        ///   An explanation as to the nature of the conflicting constraints.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException ConstraintsInConflict(Ctxt ctxt, string reason) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path is not null);
            Debug.Assert(reason is not null && reason != "");

            var annotations = new Type[] { typeof(ConstraintsSentinel) };
            var msg = $"conflicting constraints ({reason})";
            return MakeError(ctxt, annotations, msg);
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing a default value that does not pass the constraints
        ///   applied its Field.
        /// </summary>
        /// <remarks>
        ///   To indicate that a default value is invalid because it is of the wrong type, is impermissibly
        ///   <see langword="null"/>, etc. (i.e. any form of invalidity not connected to a constraint), use the
        ///   <see cref="BadValue(Ctxt, Attribute, object?, string)"/> function.
        /// </remarks>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which it was identified that the default value does not pass one or
        ///   more applied constraints. This should generally be the context of the translation of the constraints
        ///   rather than that of the translation of the default value.
        /// </param>
        /// <param name="defaultValue">
        ///   The default value.
        /// </param>
        /// <param name="reason">
        ///   An explanation of how the <paramref name="defaultValue"/> fails one or more constraints.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException DefaultFailsConstraints(Ctxt ctxt, object defaultValue, string reason) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path is not null);
            Debug.Assert(defaultValue is not null);
            Debug.Assert(reason is not null && reason != "");

            var annotations = new Type[] { typeof(ConstraintsSentinel) };
            var msg = $"default value {defaultValue.ForDisplay()} does not satisfy constraints ({reason})";
            return MakeError(ctxt, annotations, msg);
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing an ambiguous nullability deduction and/or specification.
        /// </summary>
        /// <param name = "ctxt" >
        ///   The <see cref="Ctxt">context</see> in which it was identified that the nullability of one or more Fields
        ///   is ambiguous.This should generally be the context of the translation of the outer Aggregate or Reference.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException AmbiguousNullability(Ctxt ctxt) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path == "");
            
            var annotations = new Type[] { typeof(NullableAttribute) };
            var msg = "nullability of Aggregate is ambiguous because all nested Fields are already nullable";
            return MakeError(ctxt, annotations, msg);
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing an arbitrary property-scope error.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the error was detected.
        /// </param>
        /// <param name="annotation">
        ///   The error-inducing annotation.
        /// </param>
        /// <param name="reason">
        ///   An explanation of the error.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException UserError(Ctxt ctxt, Attribute annotation, string reason) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path is not null);
            Debug.Assert(annotation is not null);
            Debug.Assert(reason is not null && reason != "");

            var annotations = new Type[] { annotation.GetType() };
            return MakeError(ctxt, annotations, reason);
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> with a contextualized error message describing a failed translation
        ///   of a single property.
        /// </summary>
        /// <param name="ctxt">
        ///   The <see cref="Ctxt">context</see> in which the translation error occurred.
        /// </param>
        /// <param name="annotations">
        ///   The type(s) of the annotation(s) involved that caused the translation error.
        /// </param>
        /// <param name="reason">
        ///   The explanation of the translation error.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/>.
        /// </returns>
        private static KvasirException MakeError(Ctxt ctxt, Type[] annotations, string reason) {
            Debug.Assert(ctxt.Property is not null);
            Debug.Assert(ctxt.Path is not null);
            Debug.Assert(!annotations.IsEmpty() && annotations.None(a => a is null));
            Debug.Assert(reason is not null && reason != "");

            var location = $"property '{ctxt.Property.Name}' of type {ctxt.Property.ReflectedType!.Name}";
            var what = string.Join(", ", annotations.Select(a => DisplayName(a)));

            if (ctxt.Path == "") {
                return new KvasirException(
                    "Error Performing Translation:\n" +
                    $"  • Location: {location}\n" +
                    $"  • Annotation(s): {what}\n" +
                    $"  • Problem: {reason}"
                );
            }
            else {
                return new KvasirException(
                    "Error Performing Translation:\n" +
                    $"  • Location: {location}\n" +
                    $"  • Annotation(s): {what}\n" +
                    $"  • Applied To: nested property @ \"{ctxt.Path}\"\n" +
                    $"  • Problem: {reason}"
                );
            }
        }

        //////////////////////// Type-Scope Errors ////////////////////////

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing an annotation that imparts an invalid name on some
        ///   schematic component at type scope.
        /// </summary>
        /// <param name="entity">
        ///   The <see cref="Type">Entity type</see> on which the invalid name was detected.
        /// </param>
        /// <param name="annotation">
        ///   The annotation with the invalid name.
        /// </param>
        /// <param name="name">
        ///   The invalid name.
        /// </param>
        /// <param name="reason">
        ///   An explanation as to why <paramref name="name"/> is invalid. If <paramref name="name"/> is
        ///   <see langword="null"/> or is the empty string, the reason should be omitted (as it will be synthesized
        ///   automatically).
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException InvalidName(Type entity, Attribute annotation, string? name, string reason = "") {
            Debug.Assert(entity is not null);
            Debug.Assert(annotation is not null);
            Debug.Assert(reason is not null);
            var annotations = new Type[] { annotation.GetType() };

            if (name is null) {
                Debug.Assert(reason == "");
                return MakeError(entity, annotations, "name is null");
            }
            else if (name == "") {
                Debug.Assert(reason == "");
                var msg = $"name {name.ForDisplay()} is invalid (it is the empty string)";
                return MakeError(entity, annotations, msg);
            }
            else {
                var msg = $"name {name.ForDisplay()} is invalid ({reason})";
                return MakeError(entity, annotations, msg);
            }
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing an arbitrary type-scope error caused during synthesis.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="Type"/> on which the translation error occurred.
        /// </param>
        /// <param name="reason">
        ///   An explanation of the error.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException UserError(Type type, string reason) {
            Debug.Assert(type is not null);
            Debug.Assert(reason is not null && reason != "");

            return MakeError(type, Array.Empty<Type>(), reason);
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> describing an arbitrary type-scope error caused by a specific
        ///   annotation.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="Type"/> on which the translation error occurred.
        /// </param>
        /// <param name="annotation">
        ///   The error-inducing annotation.
        /// </param>
        /// <param name="reason">
        ///   An explanation of the error.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/> with a contextualized error message.
        /// </returns>
        public static KvasirException UserError(Type type, Attribute annotation, string reason) {
            Debug.Assert(type is not null);
            Debug.Assert(annotation is not null);
            Debug.Assert(reason is not null && reason != "");

            var annotations = new Type[] { annotation.GetType() };
            return MakeError(type, annotations, reason);
        }

        /// <summary>
        ///   Create a <see cref="KvasirException"/> with a contextualized error message describing a failed translation
        ///   of a single type.
        /// </summary>
        /// <param name="type">
        ///   The <see cref="Type"/> on which the translation error occurred.
        /// </param>
        /// <param name="annotations">
        ///   The type(s) of the annotation(s) involved that caused the translation error. This may be empty, indicating
        ///   that the error occurred during a synthesis operation rather than as a result of one or more annotations in
        ///   particular.
        /// </param>
        /// <param name="reason">
        ///   The explanation of the translation error.
        /// </param>
        /// <returns>
        ///   A <see cref="KvasirException"/>.
        /// </returns>
        private static KvasirException MakeError(Type type, Type[] annotations, string reason) {
            Debug.Assert(type is not null);
            Debug.Assert(annotations is not null);
            Debug.Assert(annotations.None(a => a is null));
            Debug.Assert(reason is not null && reason != "");

            if (annotations.IsEmpty()) {
                return new KvasirException(
                    "Error Performing Translation:\n" +
                    $"  • Location: type {type.Name}\n" +
                    $"  • Problem: {reason}"
                );
            }
            else {
                var what = string.Join(", ", annotations.Select(a => DisplayName(a)));
                return new KvasirException(
                    "Error Performing Translation:\n" +
                    $"  • Location: type {type.Name}\n" +
                    $"  • Annotation(s): {what}\n" +
                    $"  • Problem: {reason}"
                );
            }
        }

        //////////////////////// Utilities ////////////////////////
        
        /// <summary>
        ///   Get the display name of an <see cref="Attribute"/> type, which is the type's name without the obligatory
        ///   <c>Attribute</c> suffix and without any namespace qualification.
        /// </summary>
        /// <param name="annotationType">
        ///   The <see cref="Attribute"/> type.
        /// </param>
        /// <returns>
        ///   Exactly <c>annotationType.Name[0..^9]</c>, enclosed in hard brackets.
        /// </returns>
        private static string DisplayName(Type annotationType) {
            Debug.Assert(annotationType is not null);

            if (annotationType == typeof(ConstraintsSentinel)) {
                return "one or more [Check.xxx] constraints";
            }
            else {
                var name = annotationType.Name[0..^9];
                bool isCheck = annotationType.FullName!.Contains("Check+");
                return $"[{(isCheck ? "Check." : "") + name}]";
            }
        }

        /// <summary>
        ///   A sentinel type specifically for indicating that a constraint arose due to the confluence of one or more
        ///   otherwise unidentified constraint annotations.
        /// </summary>
        private sealed class ConstraintsSentinel {};
    }
}