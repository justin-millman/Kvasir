using Cybele.Extensions;
using Kvasir.Annotations;
using Kvasir.Relations;
using Kvasir.Schema;
using System;
using System.Reflection;

namespace Kvasir.Translation {
    /// <summary>
    ///   A collection of extension methods for categorizing various CLR components during translation.
    /// </summary>
    internal static class Categorization {
        /// <summary>
        ///   Categorizes a <see cref="Type"/>.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="Type"/> on which this extension method was invoked.
        /// </param>
        /// <returns>
        ///   The <see cref="TypeCategory"/> of <paramref name="self"/>.
        /// </returns>
        public static TypeCategory TranslationCategory(this Type self) {
            self = Nullable.GetUnderlyingType(self) ?? self;

            if (self == typeof(IRelation)) {
                return TypeCategory.IRelation;
            }
            else if (self.IsInstanceOf(typeof(IRelation))) {
                // `IRelation` is an instance of itself, so this must come later
                return TypeCategory.Relation;
            }
            else if (self.IsGenericTypeDefinition) {
                return TypeCategory.OpenGeneric;
            }
            else if (self.IsGenericType) {
                // Relation containers (e.g. `RelationList`) are closed generics, so this has to come later
                return TypeCategory.ClosedGeneric;
            }
            else if (self == typeof(object)) {
                return TypeCategory.Object;
            }
            else if (self.IsArray) {
                return TypeCategory.Array;
            }
            else if (self.IsPointer) {
                return TypeCategory.Pointer;
            }
            else if (self.IsSealed && self.IsAbstract) {
                return TypeCategory.StaticClass;
            }
            else if (self.IsInterface) {
                // `IRelaton` is an interface, so this has to come later
                return TypeCategory.Interface;
            }
            else if (self == typeof(ValueType) || self == typeof(Enum)) {
                // `ValueType` and `Enum` are abstract, so this has to come earlier
                return TypeCategory.UniversalBase;
            }
            else if (self.IsAbstract) {
                // Interfaces are abstract, so this has to come later
                return TypeCategory.AbstractClass;
            }
            else if (self.IsEnum) {
                return TypeCategory.Enumeration;
            }
            else if (DBType.IsSupported(self)) {
                return TypeCategory.Supported;
            }
            else if (self.IsInstanceOf(typeof(Delegate))) {
                return TypeCategory.Delegate;
            }
            else if (self.IsValueType) {
                return TypeCategory.Struct;
            }
            else {
                return TypeCategory.Class;
            }
        }

        /// <summary>
        ///   Categories a <see cref="PropertyInfo">property</see>.
        /// </summary>
        /// <param name="self">
        ///   The <see cref="PropertyInfo">property</see> on which the extension method was invoked.
        /// </param>
        /// <returns>
        ///   The <see cref="PropertyCategory"/> of <paramref name="self"/>.
        /// </returns>
        public static PropertyCategory TranslationCategory(this PropertyInfo self) {
            var userInclude = self.HasAttribute<IncludeInModelAttribute>();
            var userExclude = self.HasAttribute<CodeOnlyAttribute>();

            var getter = self.GetMethod;        // will be null for write-only properties
            var isIndexer = self.GetIndexParameters().Length > 0;
            var isInherited = getter?.IsInherited() ?? false;
            var systemInclude = getter is not null && !isIndexer && !isInherited && getter.IsPublic && !getter.IsStatic;
            var systemExclude = !systemInclude;

            if (userInclude && userExclude) {
                return PropertyCategory.Ambiguous;
            }
            else if (userExclude || (!userInclude && systemExclude)) {
                return PropertyCategory.CodeOnly;
            }
            else if (getter is null) {
                return PropertyCategory.WriteOnly;
            }
            else if (isIndexer) {
                return PropertyCategory.Indexer;
            }
            else {
                return PropertyCategory.InDataModel;
            }
        }
    }

    /// <summary>
    ///   A "smart enum" representing the broad categorization of a CLR <see cref="Type"/>.
    /// </summary>
    internal readonly struct TypeCategory : IEquatable<TypeCategory> {
        public static TypeCategory AbstractClass { get; } = new TypeCategory("an abstract class");
        public static TypeCategory Array { get; } = new TypeCategory("an array (even of an otherwise supported type)");
        public static TypeCategory Class { get; } = new TypeCategory("a class or a record class");
        public static TypeCategory ClosedGeneric { get; } = new TypeCategory("a closed generic type");
        public static TypeCategory Delegate { get; } = new TypeCategory("a delegate");
        public static TypeCategory Enumeration { get; } = new TypeCategory("an enumeration type");
        public static TypeCategory Interface { get; } = new TypeCategory("an interface");
        public static TypeCategory IRelation { get; } = new TypeCategory("the `IRelation` interface");
        public static TypeCategory Object { get; set; } = new TypeCategory("`object` (or `dynamic`)");
        public static TypeCategory OpenGeneric { get; set; } = new TypeCategory("an open generic type");
        public static TypeCategory Pointer { get; } = new TypeCategory("a pointer type");
        public static TypeCategory Relation { get; } = new TypeCategory("an implementation of the `IRelation` interface");
        public static TypeCategory Supported { get; } = new TypeCategory("a primitive type, `string`, `DateTime`, or `Guid`");
        public static TypeCategory StaticClass { get; } = new TypeCategory("a static class");
        public static TypeCategory Struct { get; } = new TypeCategory("a struct or a record struct");
        public static TypeCategory UniversalBase { get; } = new TypeCategory("a universal base class");


        private TypeCategory(string description) { description_ = description; }
        public override string ToString() { return description_; }
        public bool Equals(TypeCategory rhs) { return description_ == rhs.description_; }
        private readonly string description_;
    }

    /// <summary>
    ///   A "smart enum" representing the broad categorization of a CLR <see cref="PropertyInfo">property</see>.
    /// </summary>
    internal readonly struct PropertyCategory : IEquatable<PropertyCategory> {
        public static PropertyCategory Ambiguous { get; } = new PropertyCategory("a property annotated both [IncludeInModel] and [CodeOnly]");
        public static PropertyCategory CodeOnly { get; } = new PropertyCategory("a property excluded from the data model");
        public static PropertyCategory InDataModel { get; } = new PropertyCategory("a property in the data model");
        public static PropertyCategory Indexer { get; } = new PropertyCategory("an indexer");
        public static PropertyCategory WriteOnly { get; } = new PropertyCategory("a write-only property");


        private PropertyCategory(string description) { description_ = description; }
        public override string ToString() { return description_;  }
        public bool Equals(PropertyCategory rhs) { return description_ == rhs.description_; }
        private readonly string description_;
    }
}
