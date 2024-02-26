using Cybele.Extensions;
using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kvasir.Translation2 {
    /// <summary>
    ///   A trace of the Translation process.
    /// </summary>
    /// <remarks>
    ///   This type is intended to be used as a resource handle, being extended at the beginning of a particular scope
    ///   and then unwounded automatically at the end of the scope. Its primary purpose is to detect impermissible
    ///   reference cycles and to enable contextualized error messages.
    /// </remarks>
    internal sealed class Context {
        /// <summary>
        ///   The depth of the <see cref="Context"/>.
        /// </summary>
        /// <remarks>
        ///   A depth of <c>0</c> indicates that the <see cref="Context"/> represents translation of the initial type or
        ///   a property defined directly on the initial type. A depth of <c>N > 0</c> indicates that the
        ///   <see cref="Context"/> represents translation of a type <c>N</c> levels nested within the initial type or
        ///   a property defined on such a nested type.
        /// </remarks>
        public int Depth => history_.Count;

        /// <summary>
        ///   Whether or not the <see cref="Context"/> refers to the translation of an Entity, rather than an Aggregate.
        /// </summary>
        public bool TranslatingEntity { get; private set; }

        /// <summary>
        ///   Constructs a new <see cref="Context"/>.
        /// </summary>
        /// <param name="initial">
        ///   The initial <see cref="Type"/> whose translation will be tracked by the new <see cref="Context"/>.
        /// </param>
        public Context(Type initial) {
            Debug.Assert(initial is not null);

            frozen_ = false;
            initialType_ = initial;
            history_ = new List<PropertyInfo>();
            current_ = Option.None<PropertyInfo>();
            TranslatingEntity = true;
        }

        /// <summary>
        ///   Constructs a frozen (i.e. immutable) copy of another <see cref="Context"/>.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="Context"/>.
        /// </param>
        private Context(Context source) {
            Debug.Assert(source is not null);

            frozen_ = true;
            initialType_ = source.initialType_;
            history_ = new List<PropertyInfo>(source.history_);
            current_ = source.current_;
            TranslatingEntity = source.TranslatingEntity;
        }

        /// <summary>
        ///   Creates a frozen (i.e. immutable) copy of this <see cref="Context"/>.
        /// </summary>
        /// <returns>
        ///   A deep copy of this <see cref="Context"/> that cannot be pushed onto or popped from.
        /// </returns>
        public Context Freeze() {
            return new Context(this);
        }

        /// <summary>
        ///   Indicates that translation is proceeding on to a new <see cref="Type"/>.
        /// </summary>
        /// <param name="next">
        ///   The new <see cref="Type"/> that is now being translated.
        /// </param>
        /// <returns>
        ///   A resource handle whose disposal will unwind the <see cref="Context"/> by one stage, namely the stage
        ///   corresponding to the translation of <paramref name="next"/>.
        /// </returns>
        /// <exception cref="ReferenceCycleException">
        ///   if <paramref name="next"/> is already in the backtrace of the <see cref="Context"/>, either as the initial
        ///   translated <see cref="Type"/> or one that was <see cref="Push(Type)">pushed on</see>.
        /// </exception>
        public IDisposable Push(Type next) {
            Debug.Assert(!frozen_);
            Debug.Assert(next is not null);
            Debug.Assert(current_.Exists(p => (Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType) == next));

            if (next == initialType_ || history_.Any(p => p.PropertyType == next)) {
                history_.Add(current_.Unwrap());
                current_ = Option.None<PropertyInfo>();
                throw new ReferenceCycleException(this);
            }
            else {
                TranslatingEntity = next.IsClass;
                history_.Add(current_.Unwrap());
                current_ = Option.None<PropertyInfo>();
                return new ContextHandle() { Context = this };
            }
        }

        /// <summary>
        ///   Indicates that translation is proceeding on to a new <see cref="PropertyInfo">property</see>.
        /// </summary>
        /// <param name="next">
        ///   The new <see cref="PropertyInfo">property</see> that is now being translated.
        /// </param>
        /// <returns>
        ///   A resource handle whose disposal will unwind the <see cref="Context"/> by one stage, namely the stage
        ///   corresponding to the translation of <paramref name="next"/>.
        /// </returns>
        public IDisposable Push(PropertyInfo next) {
            Debug.Assert(!frozen_);
            Debug.Assert(next is not null);
            Debug.Assert(!current_.HasValue);

            current_ = Option.Some(next);
            return new ContextHandle() { Context = this };
        }

        /// <summary>
        ///   Pop the most recent stage of the <see cref="Context"/>.
        /// </summary>
        /// <remarks>
        ///   Unwinding a Context happens for one of two reasons: either we have finished translating a Type, or we have
        ///   finished translating a Property. In the former case, we are necessarily returning to the translation of a
        ///   Property, namely the Property whose own type is the Type we just finished translating. In the latter case,
        ///   we are necessarily returning to the translation of a Type, namely the Type that owns the Property we just
        ///   finished translating.
        /// </remarks>
        private void Pop() {
            Debug.Assert(!frozen_);

            if (current_.HasValue) {
                // Just finished translating a Property
                current_ = Option.None<PropertyInfo>();
            }
            else if (!history_.IsEmpty()) {
                // Just finished translating a Type
                current_ = Option.Some(history_[^1]);
                history_.RemoveAt(history_.Count - 1);
                TranslatingEntity = current_.Exists(p => p.ReflectedType!.IsClass);
            }
            else {
                // The `else` branch would only be hit when unwinding the first stage of the Context, which we should
                // not ever be doing. We only get Handles by calling `Push`, which we don't do when initializing the
                // Context. As such, there should not be a Handle to unwind at that point in the stack.
                Debug.Assert(false);
            }
        }

        /// <summary>
        ///   Produces a human-readable string representation of the <see cref="Context"/>.
        /// </summary>
        /// <returns>
        ///   A representation of the <see cref="Context"/>, describing the initial <see cref="Type"/> and all
        ///   subsequent stages.
        /// </returns>
        public sealed override string ToString() {
            static string Link(PropertyInfo property) {
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                return $"{type.DisplayName()} (from \"{NameOf(property)}\")";
            }
            var chain = string.Join(" → ", history_.Select(p => Link(p)));

            if (current_.HasValue) {
                if (history_.IsEmpty()) {
                    return initialType_.DisplayName() + " → " + NameOf(current_.Unwrap());
                }
                else {
                    return initialType_.DisplayName() + " → " + chain + " → " + NameOf(current_.Unwrap());
                }
            }
            else {
                if (history_.IsEmpty()) {
                    return initialType_.DisplayName();
                }
                else {
                    return initialType_.DisplayName() + " → " + chain;
                }
            }
        }

        /// <summary>
        ///   Produces the native name of a property, accounting for explicit interface implementations and indexers.
        /// </summary>
        /// <param name="property">
        ///   The property.
        /// </param>
        /// <returns>
        ///   The name of <paramref name="property"/>.
        /// </returns>
        private static string NameOf(PropertyInfo property) {
            if (property.GetIndexParameters().IsEmpty()) {
                return property.Name.Split('.')[^1];
            }
            else {
                return "this[]";
            }
        }


        // This is the type that actually serves as the resource handle. We need a type whose lifetime is separate from
        // that of the Context object; we could have the Push API clone and return a new Context instance, but that's
        // a lot clunkier. This type is essentially unusable outside of a `using` statement, since it has no publicly
        // accessible content other than the property initializer, which is only visible to the Context.
        private readonly struct ContextHandle : IDisposable {
            public Context Context { private get; init; }

            void IDisposable.Dispose() {
                Context.Pop();
            }
        }


        private readonly bool frozen_;
        private readonly Type initialType_;
        private readonly List<PropertyInfo> history_;
        private Option<PropertyInfo> current_;
    }
}
