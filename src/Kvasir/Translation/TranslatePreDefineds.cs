using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation {
    internal sealed partial class Translator {
        /// <summary>
        ///   Determines if an Entity type is a Pre-Defined Entity type.
        /// </summary>
        /// <param name="entity">
        ///   The Entity type.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="entity"/> is a Pre-Defined Entity type; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        private static bool IsPreDefined(Type entity) {
            Debug.Assert(entity is not null);
            return entity.HasAttribute<PreDefinedAttribute>();
        }

        /// <summary>
        ///   Identifies the pre-defined instances of a Pre-Defined Entity type.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> is being translated.
        /// </param>
        /// <param name="source">
        ///   The Pre-Defined Entity type.
        /// </param>
        /// <exception cref="NotEnoughInstancesException">
        ///   if <paramref name="source"/> has fewer than 2 pre-defined instances that are part of the data model.
        /// </exception>
        /// <exception cref="InvalidPreDefinedInstanceException">
        ///   if one of the pre-defined instances of <paramref name="source"/> that is included in the data model is
        ///   either non-public or writeable.
        /// </exception>
        /// <exception cref="InapplicableAnnotationException">
        ///   if any of the pre-defined instances of <paramref name="source"/> are annotated with an annotation other
        ///   than <c>[IncludeInModel]</c> or <c>[CodeOnly]</c>.
        /// </exception>
        private static IEnumerable<object> GetPreDefinedInstances(Context context, Type source) {
            var statics = source.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var possibleInstances = statics.Where(p => p.PropertyType == source);

            List<object> instances = new List<object>();
            foreach (var candidate in possibleInstances) {
                using var propGuard = context.Push(candidate);

                var userInclude = candidate.HasAttribute<IncludeInModelAttribute>();
                var userExclude = candidate.HasAttribute<CodeOnlyAttribute>();
                var publiclyWriteable = candidate.CanWrite && candidate.SetMethod!.IsPublic;

                if (userExclude) {
                    continue;
                }

                // It is an error for a pre-defined instance property to be non-public or writeable. This is because
                // such properties are only useful if they are accessible to users and immutable.
                if ((!candidate.CanRead || !candidate.GetMethod!.IsPublic) && userInclude) {
                    var annotation = candidate.GetCustomAttribute<IncludeInModelAttribute>()!;
                    throw new InvalidPreDefinedInstanceException(context, annotation);
                }
                else if (publiclyWriteable) {
                    throw new InvalidPreDefinedInstanceException(context);
                }
                else if (!candidate.GetMethod!.IsPublic) {
                    continue;
                }

                // Only [IncludeInModel], [NonNullable], and [CodeOnly] annotations are allowed on a pre-defined
                // instance. The first two are redundant (though the first is only allowed on already-public properties)
                // while the latter one causes a short-circuiting above. Any other annotation is an error.
                var kvasirAnnotations = candidate.GetCustomAttributes().Where(a => a.GetType().Assembly == typeof(Translator).Assembly);
                var allowedAnnotations = new Type[] { typeof(IncludeInModelAttribute), typeof(NonNullableAttribute) };
                foreach (var annotation in kvasirAnnotations) {
                    if (!allowedAnnotations.Contains(annotation.GetType())) {
                        throw new InapplicableAnnotationException(context, annotation.GetType());
                    }
                }

                // The property is a valid pre-defined instance.
                var instance = candidate.GetValue(null);
                if (instance is not null) {
                    instances.Add(instance);
                }
            }

            if (instances.Count < 2) {
                throw new NotEnoughInstancesException(context, 2, instances.Count);
            }
            return instances;
        }

        /// <summary>
        ///   Produces the Reconstitution Plan for a Pre-Defined Entity type.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which <paramref name="source"/> is being translated.
        /// </param>
        /// <param name="table">
        ///   The <see cref="ITable">Principal Table</see> for <paramref name="source"/>.
        /// </param>
        /// <param name="matcher">
        ///   The <see cref="KeyMatcher"/> with which to identify pre-defined instances of <paramref name="source"/>
        ///   from rows of database values.
        /// </param>
        /// <param name="source">
        ///   The Pre-Defined Entity type.
        /// </param>
        /// <exception cref="InvalidEntityTypeException">
        ///   if <paramref name="source"/> has any public constructors.
        /// </exception>
        /// <exception cref="ReconstitutionNotPossibleException">
        ///   if any of the constructors of <paramref name="source"/> is annotated with <c>[ReconstitueThrough]</c>.
        /// </exception>
        private static DataReconstitutionPlan MakePreDefinedReconstitutionPlan
        (Context context, ITable table, KeyMatcher matcher, Type source) {
            var constructors = source.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var constructor in constructors) {
                if (constructor.IsPublic) {
                    throw new InvalidEntityTypeException(context, new PreDefinedTag());
                }
                else if (constructor.HasAttribute<ReconstituteThroughAttribute>()) {
                    var annotation = constructor.GetCustomAttribute<ReconstituteThroughAttribute>()!;
                    throw new ReconstitutionNotPossibleException(context, annotation, new PreDefinedTag());
                }
            }

            var creator = new PreDefinedCreator(table, matcher);
            var reconstitutor = new ReconstitutingCreator(creator, Enumerable.Empty<IMutator>());
            return new DataReconstitutionPlan(reconstitutor);
        }
    }
}
