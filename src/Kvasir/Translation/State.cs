using Kvasir.Extraction;
using System;
using System.Collections.Generic;

namespace Kvasir.Translation {
    internal readonly record struct TranslationState(
        FieldsListing Fields,
        RelationsListing Relations,
        ExtractorsListing Extractors
    );

    internal readonly record struct MutableTranslationState(
        Dictionary<string, FieldDescriptor> Fields,
        Dictionary<string, IRelationDescriptor> Relations,
        Dictionary<Guid, IMultiExtractor> Extractors
    );
}
