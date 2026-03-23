using Cybele.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   A utility for tracking where Localizations are encountered, for handling delayed translation.
    /// </summary>
    internal sealed class LocalizationTracker {
        /// <summary>
        ///   The <see cref="PropertyInfo">property</see> where the Localization was encountered.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        ///   Constructs a new empty <see cref="RelationTracker"/>.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="Property"/>.
        /// </param>
        public LocalizationTracker(PropertyInfo source) {
            Debug.Assert(source is not null);
            Debug.Assert(Translator.IsLocalizationType(source.PropertyType));

            Property = source;
            decontextualizedAccessPath_ = new List<string>() { source.Name.Split('.')[^1] };
        }

        /// <summary>
        ///   Constructs a new <see cref="LocalizationTracker"/> that is identical to another but with an additional
        ///   property prepended to its <see cref="AsContextOn(Type)">contextualization</see>.
        /// </summary>
        /// <param name="source">
        ///   The source <see cref="RelationTracker"/>.
        /// </param>
        /// <param name="upstreamProperty">
        ///   The additional property.
        /// </param>
        private LocalizationTracker(LocalizationTracker source, PropertyInfo upstreamProperty) {
            Debug.Assert(source is not null);
            Debug.Assert(upstreamProperty is not null);

            Property = source.Property;

            var decontextualized = new List<string>(source.decontextualizedAccessPath_);
            decontextualized.Insert(0, upstreamProperty.Name.Split('.')[^1]);
            decontextualizedAccessPath_ = decontextualized;
        }

        /// <summary>
        ///   Prepends a property to the <see cref="AsContextOn(Type)">contextualization</see>.
        /// </summary>
        /// <param name="property">
        ///   The <see cref="PropertyInfo">property</see> to prepend to the
        ///   <see cref="AsContextOn(Type)">contextualization</see>.
        /// </param>
        /// <returns>
        ///   A new <see cref="LocalizationTracker"/> identical to the current one, but with <paramref name="property"/>
        ///   prepended to the <see cref="AsContextOn(Type)">contextualization</see>.
        /// </returns>
        public LocalizationTracker Extend(PropertyInfo property) {
            Debug.Assert(property is not null);
            return new LocalizationTracker(this, property);
        }       

        /// <summary>
        ///   Creates a <see cref="Context"/> reflecting the access path to the <see cref="LocalizationTracker"/>.
        /// </summary>
        /// <param name="source">
        ///   The starting Entity Type.
        /// </param>
        /// <returns>
        ///   A <see cref="Context"/> reflecting the translation state of the Localization represented by the
        ///   <see cref="LocalizationTracker"/> when starting from <paramref name="source"/>.
        /// </returns>
        public Context AsContextOn(Type source) {
            var context = new Context(source);

            Type current = source;
            foreach (var propertyName in decontextualizedAccessPath_) {
                if (current != source) {
                    context.Push(current);
                }

                var property = current.GetPropertyNamed(propertyName).Unwrap();
                context.Push(property);
                current = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            }
            context.Push(Property.PropertyType);
            return context;
        }


        private readonly IReadOnlyList<string> decontextualizedAccessPath_;
    }
}