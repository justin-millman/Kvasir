using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Reconstitution;
using Kvasir.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Kvasir.Translation {
    /// <summary>
    ///   A helper class for evaluating a type's constructors for usability in Reconstitution.
    /// </summary>
    internal static class ReconstitutionHelper {
        /// <summary>
        ///   Create the <see cref="ICreator"/> for a particular type, whose data model consists of a set of Fields.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="Context"/> in which the translation of <paramref name="source"/> is occurring.
        /// </param>
        /// <param name="source">
        ///   The <see cref="Type"/> for which to make a <see cref="ICreator"/> (based on its available constructors).
        /// </param>
        /// <param name="fields">
        ///   The Fields that make up the data model of <paramref name="source"/>.
        /// </param>
        /// <param name="forNullableField">
        ///   If <see langword="false"/>, then a new non-<see langword="null"/> CLR object will be constructed even if
        ///   each of the <see cref="DBValue">database values</see> provided is <see cref="DBValue.NULL"/>. If
        ///   <see langword="false"/>, then such a set of values will result in a <see langword="null"/> object.
        /// </param>
        public static ReconstitutingCreator MakeCreator(Context context, Type source, IEnumerable<FieldGroup> fields, bool forNullableField) {
            // Strip any nullability wrapper from the source type
            source = Nullable.GetUnderlyingType(source) ?? source;

            // We need to ignore [Calculated] Fields, which have no `Creator`
            var nonCalculatedFields = fields.Where(g => g.Creator.HasValue);

            Candidate MakeCandidate(ConstructorInfo constructor) {
                var namesToFields = nonCalculatedFields.ToDictionary(g => g.ReconstitutionArgumentName.ToLower());

                static CreatorFacade CreatorFor(FieldGroup group) {
                    return new CreatorFacade(group.Creator.Unwrap(), group.Column.Unwrap(), group.Size);
                }

                var arguments = new List<CreatorFacade>();
                var mutators = nonCalculatedFields.Where(g => g.Source.CanWrite).ToDictionary(g => g.ReconstitutionArgumentName.ToLower());
                foreach (var argument in constructor.GetParameters()) {
                    var argumentName = argument.Name!.ToLower();
                    if (!namesToFields.TryGetValue(argumentName, out var match)) {
                        return Candidate.Failure(constructor);
                    }
                    else if (!AreCompatible(argument, match)) {
                        return Candidate.Failure(constructor);
                    }
                    else {
                        arguments.Add(CreatorFor(match));
                        namesToFields.Remove(argumentName);
                        mutators.Remove(argumentName);
                    }
                }

                if (arguments.Count + mutators.Count != nonCalculatedFields.Count()) {
                    return Candidate.Failure(constructor);
                }
                else {
                    return new Candidate() {
                        Constructor = constructor,
                        Arguments = arguments,
                        Mutations = mutators.Values.Select(g => new WritePropertyMutator(g.Source, CreatorFor(g))).ToList(),
                        IsViable = true
                    };
                }
            }

            // All structs have a default constructor, but that constructor does not show up in reflection. We want to
            // include it, but only for structs that don't have any other user-defined constructors. Technically, the
            // presence of a user-defined constructor on a struct in C# does not inhibit the implicit default
            // constructor; I hate that, so we're going to do our own thing.
            var constructors = source.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (source.IsValueType && constructors.IsEmpty()) {
                var defaultCtor = new SyntheticConstructorInfo(source);
                constructors = new ConstructorInfo[] { defaultCtor };
            }

            // Make one candidate per constructor; not all will be viable
            var candidates = constructors.Select(c => MakeCandidate(c));
            var viables = candidates.Where(c => c.IsViable).Order().Reverse().ToList();     // `Order` is ascending

            // Error checking
            if (candidates.Count(c => c.IsAnnotated) > 1) {
                var count = candidates.Count(c => c.IsAnnotated);
                throw new ReconstitutionNotPossibleException(context, new ReconstituteThroughAttribute(), count);
            }
            else if (candidates.Any(c => c.IsAnnotated && !c.IsViable)) {
                throw new ReconstitutionNotPossibleException(context, new ReconstituteThroughAttribute());
            }
            else if (viables.Count == 0) {
                throw new ReconstitutionNotPossibleException(context);
            }
            else if (candidates.Count(c => c.CompareTo(viables[0]) == 0) > 1) {
                var count = candidates.Count(c => c.CompareTo(viables[0]) == 0);
                throw new ReconstitutionNotPossibleException(context, count);
            }

            // No errors encountered
            return viables[0].MakeCreator(forNullableField);
        }

        /// <summary>
        ///   Determines if a constructor parameter is a viable match for a property during Reconstitution.
        /// </summary>
        /// <param name="parameter">
        ///   The constructor parameter.
        /// </param>
        /// <param name="argument">
        ///   The <see cref="FieldGroup"/> representing the <see langword="property"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the types of <paramref name="parameter"/> and <paramref name="argument"/> are
        ///   same, or if the type of <paramref name="parameter"/> is the nullable version of
        ///   <paramref name="argument"/>; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool AreCompatible(ParameterInfo parameter, FieldGroup argument) {
            var paramNullable = parameter is SyntheticParameterInfo || new NullabilityInfoContext().Create(parameter).ReadState == NullabilityState.Nullable;
            var argNullable = argument.IsNativelyNullable;

            // If the argument is nullable but the parameter is not, then it's an automatic failure: even if the types
            // are all the same, the nullability doesn't properly match.
            if (argNullable && !paramNullable) {
                return false;
            }

            // We've passed the nullability check, so all we have to verify now is that the types are correct. We have
            // to account for `Nullable<T>`, which is different than `T`.
            var paramType = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;
            var argType = Nullable.GetUnderlyingType(argument.Source.PropertyType) ?? argument.Source.PropertyType;

            // Localizations are implicitly convertible to their key type, and we expect that constructors will accept
            // the key rather than a full Localization.
            if (paramType != argType && argument is LocalizationKeyFieldGroup lfg) {
                argType = Nullable.GetUnderlyingType(lfg.KeyType) ?? lfg.KeyType;
            }
            return paramType == argType;
        }


        private readonly struct Candidate : IComparable<Candidate> {
            public ConstructorInfo Constructor { get; init; }
            public bool IsAnnotated => Constructor.HasAttribute<ReconstituteThroughAttribute>();
            public List<CreatorFacade> Arguments { get; init; }
            public List<WritePropertyMutator> Mutations { get; init; }
            public bool IsViable { get; init; }

            public static Candidate Failure(ConstructorInfo constructor) {
                return new Candidate() {
                    Constructor = constructor,
                    Arguments = new List<CreatorFacade>(),
                    Mutations = new List<WritePropertyMutator>(),
                    IsViable = false
                };
            }
            public int CompareTo(Candidate rhs) {
                var lhsPoints = (IsAnnotated ? 1000000 : 0) + (Constructor.IsPublic ? 10000 : 0) + Arguments.Count;
                var rhsPoints = (rhs.IsAnnotated ? 1000000 : 0) + (rhs.Constructor.IsPublic ? 10000 : 0) + rhs.Arguments.Count;
                return lhsPoints.CompareTo(rhsPoints);
            }
            public ReconstitutingCreator MakeCreator(bool forNullableField) {
                bool allowAllNulls = !forNullableField;

                // There are two situations where the `Constructor` may be Synthetic: either we're dealing with an
                // element type of a Relation, or we're dealing with the default constructor for a struct. In the former
                // case, we know (by invariant) that there are no mutations; in the latter case, we know (by definition)
                // that there are no arguments. It's not the cleanest, but that's what we'll use as a differentiator.
                if (Constructor is SyntheticConstructorInfo && Arguments.IsEmpty()) {
                    var ctor = new DefaultStructCreator(Mutations[0].SourceType, allowAllNulls);
                    return new ReconstitutingCreator(ctor, Mutations);
                }
                else {
                    var ctor = new ConstructingCreator(Constructor, Arguments, allowAllNulls);
                    return new ReconstitutingCreator(ctor, Mutations);
                }
            }
        }
    }
}
